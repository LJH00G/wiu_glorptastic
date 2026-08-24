
using Game.SO.Data.Item;
using Game.SO.Data.Item.Sellable;
using Game.SO.Data.Item.Sellable.Battle;
using Game.SO.Data.Shop;
using System;
using System.Collections.Generic;
using UnityEngine;
using Game.CSEvent;


namespace Game.Inventory
{
    static public class InventoryManager
    {
        //static public ToastNotifEventChannel toastNotifEventChannel;

        static public Inventory ManagedInventory { get; private set; }

        static public PriorityEventCS OnInventoryChanged { get; set; } = new(TryRefreshPlayerBattleData, -128);


        static void TryRefreshPlayerBattleData()
        {
            if (ManagedInventory != GameManager.CurrentUserData.Inventory)
                return;

            GameManager.CurrentUserData.PlayerBattleData.Refresh();
        }

        static public bool TryShopPurchase(ref ShopTrade trade)
        {
            ref Shopable cost = ref trade.cost;


            // check can buy
            bool canBuy = true;

            if (cost.useShell && cost.shell > ManagedInventory.ShellCurrency)
                canBuy = false;

            foreach (var stack in cost.itemStacks)
            {
                if (!HasItemInList(stack.item, out uint amount) || amount < stack.count)
                {
                    canBuy = false;
                    break;
                }
            }

            if (!canBuy)
            {
                // toast notif event
                return false;
            }


            // remove resource
            if (cost.useShell)
            {
                DeductShell(cost.shell);
            }
            foreach (var stack in cost.itemStacks)
            {
                RemoveItem(stack.item, stack.count);
            }

            // add resource
            ref Shopable product = ref trade.product;
            if (product.useShell)
            {
                RecieveShell(product.shell);
            }
            foreach (var stack in product.itemStacks)
            {
                AddItem(stack.item, stack.count);
            }

            // toast notif event

            return true;
        }


        static public void AddItem(ItemSO item, uint amount = 1)
        {
            var itemList = ManagedInventory.ItemList;

            for (int i = 0; i < itemList.Count; i++)
            {
                var itemStack = itemList[i];

                if (itemStack.item == item)
                {
                    itemStack.count += amount;
                    itemList[i] = itemStack;
                    OnInventoryChanged.Raise();
                    return;
                }
            }

            itemList.Add(new(item, amount));
            OnInventoryChanged.Raise();
        }

        static public bool TryGetItemInList<T_ItemSO>(out T_ItemSO item)
            where T_ItemSO : ItemSO
        {
            var itemList = ManagedInventory.ItemList;
            for (int i = 0; i < itemList.Count; i++)
            {
                if (itemList[i].item is T_ItemSO item_T)
                {
                    item = item_T;
                    return true;
                }
            }

            item = null;
            return false;
        }

        static public bool HasItemInList<T_ItemSO>(out uint amount)
            where T_ItemSO : ItemSO
        {
            amount = 0;

            var itemList = ManagedInventory.ItemList;
            for (int i = 0; i < itemList.Count; i++)
            {
                if (itemList[i].item is T_ItemSO)
                {
                    amount += itemList[i].count;
                }
            }

            return amount != 0;
        }

        static public bool HasItemInList(ItemSO item, out uint amount)
        {
            amount = 0;

            var itemList = ManagedInventory.ItemList;
            for (int i = 0; i < itemList.Count; i++)
            {
                if (itemList[i].item == item)
                {
                    amount = itemList[i].count;
                    break;
                }
            }

            return amount != 0;
        }

        static public bool TryGetItemInEquiped<T_ItemSO>(out T_ItemSO item)
            where T_ItemSO : ItemSO
        {
            item = null;

            if (ManagedInventory.EquipedWeapon is T_ItemSO item_weapon)
            {
                item = item_weapon;
            }
            else if (ManagedInventory.EquipedArmour is T_ItemSO item_armour)
            {
                item = item_armour;
            }
            else
            {
                foreach (var accessory in ManagedInventory.EquipedAccessoryList)
                {
                    if (accessory is T_ItemSO item_accessory)
                    {
                        item = item_accessory;
                        break;
                    }
                }
            }

            return item != null;
        }

        static public bool HasItemInEquiped<T_ItemSO>(out uint amount)
            where T_ItemSO : ItemSO
        {
            amount = 0;

            if (ManagedInventory.EquipedWeapon is T_ItemSO)
            {
                amount++;
            }

            if (ManagedInventory.EquipedArmour is T_ItemSO)
            {
                amount++;
            }

            foreach (var accessory in ManagedInventory.EquipedAccessoryList)
                if (accessory is T_ItemSO)
                    amount++;

            return amount != 0;
        }

        static public bool HasItemInEquiped(ItemSO item, out uint amount)
        {
            amount = 0;

            if (ManagedInventory.EquipedWeapon == item)
            {
                amount++;
            }
            else
            if (ManagedInventory.EquipedArmour == item)
            {
                amount++;
            }
            else
            {
                foreach (var accessory in ManagedInventory.EquipedAccessoryList)
                    if (accessory == item)
                        amount++;
            }
            return amount != 0;
        }

        static public bool HasItem<T_ItemSO>(out uint amount)
            where T_ItemSO : ItemSO
        {
            HasItemInList<T_ItemSO>(out uint a1);
            HasItemInEquiped<T_ItemSO>(out uint a2);
            amount = a1 + a2;

            return amount != 0;
        }

        static public bool HasItem(ItemSO item, out uint amount)
        {
            HasItemInList(item, out uint a1);
            HasItemInEquiped(item, out uint a2);
            amount = a1 + a2;

            return amount != 0;
        }


        static public void RemoveItem(ItemSO item, uint amount)
        {
            var itemList = ManagedInventory.ItemList;

            for (int i = 0; i < itemList.Count; i++)
            {
                if (itemList[i].item == item)
                {
                    var itemStack = itemList[i];

                    if (amount > itemStack.count)
                    {
                        itemList.RemoveAt(i);
                        Debug.LogWarning($"InventoryManager.RemoveItem() | tried to remove more items than whats in the inventory");
                    }
                    else if (amount == itemStack.count)
                    {
                        itemList.RemoveAt(i);
                    }
                    else
                    {
                        itemStack.count -= amount;
                        itemList[i] = itemStack;
                    }
                    OnInventoryChanged.Raise();
                    return;
                }
            }

            throw new System.InvalidOperationException($"InventoryManager.RemoveItem() | no item of {item} exists in the inventory to be removed");
        }


        static public List<ItemStack> GetItemList()
        {
            return new(ManagedInventory.ItemList);
        }


        static public void DeductShell(int amount)
        {
            ManagedInventory.ShellCurrency -= amount;
            OnInventoryChanged.Raise();
        }

        static public void RecieveShell(int amount)
        {
            ManagedInventory.ShellCurrency += amount;
            OnInventoryChanged.Raise();
        }

        static public void SetShell(int amount)
        {
            ManagedInventory.ShellCurrency = amount;
            OnInventoryChanged.Raise();
        }


        static public BattleItemSO[] GetEquipedBattleItems()
        {
            BattleItemSO[] battleItemList = new BattleItemSO[Inventory.MAX_ACCESSORYIES + 1];
            battleItemList[0] = ManagedInventory.EquipedWeapon;

            for (int i = 0; i < ManagedInventory.EquipedAccessoryList.Length; i++)
            {
                battleItemList[i + 1] = ManagedInventory.EquipedAccessoryList[i];
            }

            return battleItemList;
        }


        static public WeaponItemSO GetEquipedWeapon()
        {
            return ManagedInventory.EquipedWeapon;
        }


        static public ArmourItemSO GetEquipedArmour()
        {
            return ManagedInventory.EquipedArmour;
        }


        static public AccessoryItemSO[] GetEquipedAccessories()
        {
            AccessoryItemSO[] list = new AccessoryItemSO[Inventory.MAX_ACCESSORYIES];
            ManagedInventory.EquipedAccessoryList.CopyTo(list, 0);
            return list;
        }

        static public bool EquipItem(BattleItemSO item)
        {
            if (item is WeaponItemSO weapon)
            {
                return EquipWeapon(weapon);
            }
            if (item is AccessoryItemSO accessory)
            {
                return EquipAccessory(accessory);
            }
            return false;
        }

        static public bool EquipWeapon(WeaponItemSO weapon)
        {
            if (!weapon || !HasItemInList(weapon, out _))
                return false;


            Inventory inv = ManagedInventory;

            WeaponItemSO previous = inv.EquipedWeapon;

            RemoveItem(weapon, 1);
            inv.EquipedWeapon = weapon;

            if (previous)
            {
                AddItem(previous, 1);
            }

            OnInventoryChanged.Raise();
            return true;
        }

        static public void UnequipWeapon()
        {
            Inventory inv = ManagedInventory;

            if (!inv.EquipedWeapon)
                return;

            AddItem(inv.EquipedWeapon, 1);
            inv.EquipedWeapon = null;

            OnInventoryChanged.Raise();
        }

        static public bool EquipAccessory(AccessoryItemSO accessory, int slotIndex = -1)
        {
            if (!accessory || !HasItemInList(accessory, out _))
                return false;

            var slots = ManagedInventory.EquipedAccessoryList;

            if (slotIndex < 0)
            {
                slotIndex = Array.IndexOf(slots, null);
                if (slotIndex < 0)
                {
                    return false;
                }
            }

            AccessoryItemSO previous = slots[slotIndex];

            RemoveItem(accessory, 1);
            slots[slotIndex] = accessory;

            if (previous)
            {
                AddItem(previous, 1);
            }

            OnInventoryChanged.Raise();
            return true;
        }

        static public bool HasFreeAccessorySlot()
        {
            return Array.IndexOf(inventory.EquipedAccessoryList, null) >= 0;
        }

        static public void UnequipAccessory(int slotIndex)
        {
            var slots = ManagedInventory.EquipedAccessoryList;
            if (slotIndex < 0 || slotIndex >= slots.Length || !slots[slotIndex])
            {
                return;
            }

            AddItem(slots[slotIndex], 1);
            slots[slotIndex] = null;

            OnInventoryChanged.Raise();
        }

        static public bool EquipArmour(ArmourItemSO armour)
        {
            if (!armour || !HasItemInList(armour, out _))
                return false;

            Inventory inv = ManagedInventory;

            ArmourItemSO previous = inv.EquipedArmour;

            RemoveItem(armour, 1);
            inv.EquipedArmour = armour;

            if (previous)
            {
                AddItem(previous, 1);
            }

            OnInventoryChanged.Raise();
            return true;
        }

        static public void UnequipArmour()
        {
            Inventory inv = ManagedInventory;

            if (!inv.EquipedArmour)
            {
                return;
            }
            AddItem(inv.EquipedArmour, 1);
            inv.EquipedArmour = null;

            OnInventoryChanged.Raise();
        }

        public bool SellItem(SellableItemSO item, uint amount = 1)
        {
            if (!item || amount == 0 || !HasItemInList(item, out uint available) || available < amount)
            {
                return false;
            }
            RemoveItem(item, amount);
            RecieveShell(item.SellValue * (int)amount);

            return true;
        }

        static public void ManageInventory(Inventory inv)
        {
            if (inv != null)
                ManagedInventory = inv;
        }



        private void OnEnable()
        {
            shopPurchaseEventChannel.Subscribe(HandleShopPurchase);
        }

        private void OnDisable()
        {
            shopPurchaseEventChannel.Unsubscribe(HandleShopPurchase);
        }

    }
}