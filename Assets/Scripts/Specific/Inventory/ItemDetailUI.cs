using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Game.SO.Data.Item;
using Game.SO.Data.Item.Sellable;
using Game.SO.Data.Item.Sellable.Battle;

namespace Game.Inventory
{
    public class ItemDetailUI : MonoBehaviour
    {
        [Header("Source")]
        [SerializeField] InventoryManager inventoryManager;

        [Header("Refs")]
        [SerializeField] GameObject panelRoot;
        [SerializeField] Image icon;
        [SerializeField] TMP_Text titleText;
        [SerializeField] TMP_Text descriptionText;
        [SerializeField] TMP_Text statsText;
        [SerializeField] Button equipButton;
        [SerializeField] Button unequipButton;
        [SerializeField] Button closeButton;

        static readonly Dictionary<Texture2D, Sprite> spriteCache = new();

        ItemSO currentItem;
        int currentAccessorySlotIndex = -1;

        void Awake()
        {
            if (equipButton)
                equipButton.onClick.AddListener(HandleEquipClicked);
            if (unequipButton)
                unequipButton.onClick.AddListener(HandleUnequipClicked);
            if (closeButton)
                closeButton.onClick.AddListener(Hide);

            inventoryManager.OnInventoryChanged += Refresh;

            Hide();
        }

        void OnDestroy()
        {
            inventoryManager.OnInventoryChanged -= Refresh;
        }
        public void Show(ItemSO item)
        {
            currentItem = item;

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
                icon.sprite = TextureToSprite(currentItem.Texture);
            }
            if (titleText)
            {
                titleText.text = currentItem.Name;
            }
            if (descriptionText)
            {
                descriptionText.text = currentItem.Description;
            }
            if (statsText)
            {
                statsText.text = BuildStatsText(currentItem);
            }
            UpdateButtons();
        }

        void UpdateButtons()
        {
            bool isEquipped = IsCurrentlyEquipped(currentItem, out currentAccessorySlotIndex);
            bool isEquippable = currentItem is BattleItemSO;

            if (equipButton)
            {
                equipButton.gameObject.SetActive(isEquippable && !isEquipped);
            }
            if (unequipButton)
            {
                unequipButton.gameObject.SetActive(isEquippable && isEquipped);
            }
        }

        bool IsCurrentlyEquipped(ItemSO item, out int accessorySlotIndex)
        {
            accessorySlotIndex = -1;
            if (!item)
            {
                return false;
            }
            if (inventoryManager.GetEquipedWeapon() == item)
            {
                return true;
            }
            var accessories = inventoryManager.GetEquipedAccessories();
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
                inventoryManager.EquipItem(battleItem);
            }
        }

        void HandleUnequipClicked()
        {
            if (currentItem is WeaponItemSO)
            {
                inventoryManager.UnequipWeapon();
            }
            else if (currentItem is AccessoryItemSO && currentAccessorySlotIndex >= 0)
            {
                inventoryManager.UnequipAccessory(currentAccessorySlotIndex);
            }
        }

        static string BuildStatsText(ItemSO item)
        {
            StringBuilder sb = new();

            if (item is SellableItemSO sellable)
            {
                sb.AppendLine($"Sell Value: {sellable.SellValue}");
            }

            if (item is BattleItemSO battle)
            {
                if (battle.ExtraMaxHP != 0) sb.AppendLine($"Max HP: +{battle.ExtraMaxHP}");
                if (battle.ExtraMaxCurse != 0) sb.AppendLine($"Max Curse: +{battle.ExtraMaxCurse}");
                if (battle.ExtraDamage != 0) sb.AppendLine($"Damage: +{battle.ExtraDamage}");
                if (battle.ExtraDefence != 0) sb.AppendLine($"Defence: +{battle.ExtraDefence}");
            }

            if (item is WeaponItemSO weapon)
            {
                sb.AppendLine($"Weapon Damage: {weapon.Dmage}");
            }

            if (item is CurseGemItemSO curseGem)
            {
                if (curseGem.ExtraMaxHP != 0) sb.AppendLine($"Max HP: +{curseGem.ExtraMaxHP}");
                if (curseGem.ExtraMaxCurse != 0) sb.AppendLine($"Max Curse: +{curseGem.ExtraMaxCurse}");
                if (curseGem.ExtraDamage != 0) sb.AppendLine($"Damage: +{curseGem.ExtraDamage}");
                if (curseGem.ExtraDefence != 0) sb.AppendLine($"Defence: +{curseGem.ExtraDefence}");
            }

            if (item is ConsumableItemSO consumable)
            {
                sb.AppendLine($"Effect: {consumable.Effect} ({consumable.Value})");
            }
            return sb.Length > 0 ? sb.ToString() : "No additional stats";
        }

        static Sprite TextureToSprite(Texture2D texture)
        {
            if (!texture)
            {
                return null;
            }
            if (!spriteCache.TryGetValue(texture, out Sprite sprite))
            {
                sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                spriteCache[texture] = sprite;
            }

            return sprite;
        }
    }
}
