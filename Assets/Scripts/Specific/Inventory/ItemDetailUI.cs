
using Game.SO.Data.Item;
using Game.SO.Data.Item.Sellable;
using Game.SO.Data.Item.Sellable.Battle;
using Game.SO.Data.Shop;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Inventory
{
    public class ItemDetailUI : MonoBehaviour
    {
        //[Header("Source")]
        //[SerializeField] InventoryManager inventoryManager;

        [Header("Refs")]
        [SerializeField] GameObject panelRoot;
        [SerializeField] Image icon;
        [SerializeField] TextMarkupTypeWriter titleText;
        [SerializeField] TextMarkupTypeWriter descriptionText;
        [SerializeField] TextMarkupTypeWriter statsText;
        [SerializeField] Button equipButton;
        [SerializeField] Button unequipButton;
        [SerializeField] Button closeButton;

        [SerializeField] Button sellButton;
        [SerializeField] Button dontSellButton;
        [SerializeField] ShopPurchaseConfirmUI shopConfirmUI;

        ItemSO currentItem;
        int currentAccessorySlotIndex = -1;
        bool sellMode;

        void Awake()
        {
            if (equipButton)
                equipButton.onClick.AddListener(HandleEquipClicked);
            if (unequipButton)
                unequipButton.onClick.AddListener(HandleUnequipClicked);
            if (closeButton)
                closeButton.onClick.AddListener(Hide);

            if (sellButton)
                sellButton.onClick.AddListener(HandleSellClicked);
            if (dontSellButton)
                dontSellButton.onClick.AddListener(Hide);

            InventoryManager.OnInventoryChanged.Subscribe(Refresh,0);

            Hide();
        }

        void OnDestroy()
        {
            InventoryManager.OnInventoryChanged.Unsubscribe(Refresh);
        }

        public void Show(ItemSO item, bool sellMode = false)
        {
            currentItem = item;
            this.sellMode = sellMode;

            if (panelRoot)
            {
                panelRoot.SetActive(true);
            }
            Refresh();
        }

        public void Hide()
        {
            currentItem = null;

            if (panelRoot)
            {
                panelRoot.SetActive(false);
            }
        }

        void Refresh()
        {
            if (currentItem == null || (panelRoot && !panelRoot.activeSelf))
            {
                return;
            }
            if (icon)
            {
                icon.sprite = currentItem.Sprite;
            }
            if (titleText)
            {
                titleText.StartNewTypeWriting(currentItem.Name);
            }
            if (descriptionText)
            {
                descriptionText.StartNewTypeWriting(currentItem.Description, true, 0.05f);
            }
            if (statsText)
            {
                statsText.StartNewTypeWriting(BuildStatsText(currentItem), true, 0.025f);
            }
            UpdateButtons();
        }

        void UpdateButtons()
        {
            bool isEquipped = IsCurrentlyEquipped(currentItem, out currentAccessorySlotIndex);
            bool canEquip = currentItem is BattleItemSO && !isEquipped;

            if (canEquip && currentItem is AccessoryItemSO && !InventoryManager.HasFreeAccessorySlot())
            {
                canEquip = false;
            }
            if (equipButton)
            {
                equipButton.gameObject.SetActive(!sellMode && canEquip);
            }
            if (unequipButton)
            {
                unequipButton.gameObject.SetActive(!sellMode && currentItem is BattleItemSO && isEquipped);
            }
            if (sellButton)
            {
                sellButton.gameObject.SetActive(sellMode && currentItem is SellableItemSO);
            }
            if (dontSellButton)
            {
                dontSellButton.gameObject.SetActive(sellMode);
            }
        }

        bool IsCurrentlyEquipped(ItemSO item, out int accessorySlotIndex)
        {
            accessorySlotIndex = -1;
            if (!item)
            {
                return false;
            }
            if (InventoryManager.GetEquipedWeapon() == item)
            {
                return true;
            }
            if (InventoryManager.GetEquipedArmour() == item)
            {
                return true;
            }
            var accessories = InventoryManager.GetEquipedAccessories();
            for (int i = 0; i < accessories.Length; i++)
            {
                if (accessories[i] == item)
                {
                    accessorySlotIndex = i;
                    return true;
                }
            }

            return false;
        }

        void HandleEquipClicked()
        {
            if (currentItem is BattleItemSO battleItem)
            {
                InventoryManager.EquipItem(battleItem);
            }
        }

        void HandleUnequipClicked()
        {
            if (currentItem is WeaponItemSO)
            {
                InventoryManager.UnequipWeapon();
            }
            if (currentItem is ArmourItemSO)
            {
                InventoryManager.UnequipArmour();
            }
            else if (currentItem is AccessoryItemSO && currentAccessorySlotIndex >= 0)
            {
                InventoryManager.UnequipAccessory(currentAccessorySlotIndex);
            }
        }

        void HandleSellClicked()
        {
            if (currentItem is not SellableItemSO sellable)
            {
                return;
            }

            ShopTrade sellTrade = new ShopTrade
            {
                cost = new Shopable
                {
                    itemStacks = new ItemStack[1] { new(sellable, 1) },
                    useShell = false
                },
                product = new Shopable
                {
                    itemStacks = null,
                    useShell = true,
                    shell = sellable.SellValue
                }
            };

            if (shopConfirmUI)
            {

                shopConfirmUI.Show(sellTrade, () => InventoryManager.TryShopPurchase(ref sellTrade));
                Hide();
            }
            else
            {
                InventoryManager.TryShopPurchase(ref sellTrade);
                Hide();
            }
        }

        static string BuildStatsText(ItemSO item)
        {
            StringBuilder sb = new();

            if (item is SellableItemSO sellable)//oopsie gurts this is uhhhhhhhh very glorptastic shuper magisco tism
            {
                sb.AppendLine($"Sell Value: {sellable.SellValue}");
            }
            //if (item is BattleItemSO battle)
            //{
            //    if (battle.ExtraMaxHP != 0)
            //    {
            //        sb.AppendLine($"Max HP: +{battle.ExtraMaxHP}");
            //    }
            //    if (battle.ExtraMaxCurse != 0)
            //    {
            //        sb.AppendLine($"Max Curse: +{battle.ExtraMaxCurse}");
            //    }
            //    if (battle.ExtraDamage != 0)
            //    {
            //        sb.AppendLine($"Damage: +{battle.ExtraDamage}");
            //    }
            //    if (battle.ExtraDefence != 0)
            //    {
            //        sb.AppendLine($"Defence: +{battle.ExtraDefence}");
            //    }
            //}

            switch (item)
            {
                case WeaponItemSO weapon:
                    if (weapon.curseAbilityList.Length != 0)
                    {
                        sb.AppendLine($"Ability: ");
                        for (int i = 0; i < weapon.curseAbilityList.Length; i++)
                        {
                            var ability = weapon.curseAbilityList[i];
                            sb.AppendLine($"{i}. {ability.name} | cost: {ability.curseCost}");
                        }
                    }
                    sb.AppendLine($"Weapon Damage: {weapon.Dmage}");
                    break;

                case ArmourItemSO armour:
                    sb.AppendLine($"Weapon Defence: {armour.Defence}");
                    break;

                case AccessoryItemSO accessoryItem:
                    if (accessoryItem.curseAbilityList.Length != 0)
                    {
                        sb.AppendLine($"Ability: ");
                        for (int i = 0; i < accessoryItem.curseAbilityList.Length; i++)
                        {
                            var ability = accessoryItem.curseAbilityList[i];
                            sb.AppendLine($"{i}. {ability.name} | cost: {ability.curseCost}");
                        }
                    }
                    if (accessoryItem.ExtraMaxHP != 0)
                    {
                        sb.AppendLine($"Extra Max HP: {accessoryItem.ExtraMaxHP}");
                    }
                    if (accessoryItem.ExtraMaxCurse != 0)
                    {
                        sb.AppendLine($"Extra Max Curse: {accessoryItem.ExtraMaxCurse}");
                    }
                    if (accessoryItem.ExtraDamage != 0)
                    {
                        sb.AppendLine($"Extra Damage: {accessoryItem.ExtraDamage}");
                    }
                    if (accessoryItem.ExtraDefence != 0)
                    {
                        sb.AppendLine($"Extra Defence: {accessoryItem.ExtraDefence}");
                    }
                    if (accessoryItem.MasteryWindowWidthMultiplier != 1)
                    {
                        sb.AppendLine($"Mastery Multiplier: {accessoryItem.MasteryWindowWidthMultiplier}");
                    }
                    if (accessoryItem.HPRegenPerTurn != 0)
                    {
                        sb.AppendLine($"HP Regen Per Turn: {accessoryItem.HPRegenPerTurn}");
                    }
                    if (accessoryItem.CSRegenPerTurn != 0)
                    {
                        sb.AppendLine($"CS Regen Per Turn: {accessoryItem.CSRegenPerTurn}");
                    }
                    break;

                case CurseGemItemSO curseGem:
                    if (curseGem.ExtraMaxHP != 0)
                    {
                        sb.AppendLine($"Extra Max HP: {curseGem.ExtraMaxHP}");
                    }
                    if (curseGem.ExtraMaxCurse != 0)
                    {
                        sb.AppendLine($"Extra Max Curse: {curseGem.ExtraMaxCurse}");
                    }
                    if (curseGem.ExtraDamage != 0)
                    {
                        sb.AppendLine($"Extra Damage: {curseGem.ExtraDamage}");
                    }
                    if (curseGem.ExtraDefence != 0)
                    {
                        sb.AppendLine($"Extra Defence: {curseGem.ExtraDefence}");
                    }
                    break;

                case ConsumableItemSO consumable:
                    if (consumable.Effects.Count != 0)
                    {
                        sb.AppendLine($"Effect: ");

                        for (int i = 0; i < consumable.Effects.Count; i++)
                        {
                            var effectEntry = consumable.Effects[i];

                            string effectTxt = "";
                            if (effectEntry.power != 0)
                                effectTxt = $"{effectEntry.effect}: {effectEntry.power}";
                            string statusEffectTxt = "";
                            if (effectEntry.duration != 0)
                                statusEffectTxt = $"{effectEntry.status}: {effectEntry.duration}";

                            string spliter = "";
                            if (effectTxt != "" && statusEffectTxt != "")
                                spliter = " | ";

                            sb.AppendLine($"{i}. {effectTxt}{spliter}{statusEffectTxt}");
                        }
                    }
                    sb.AppendLine($"Target: {consumable.TargetType}");
                    sb.AppendLine($"Is Single Use: {consumable.ConsumeOnUse}");
                    break;

                default:
                    break;
            }


            return sb.Length > 0 ? sb.ToString() : "No additional stats";
        }

    }
}