using Game.SO.Data.Item;
using Game.SO.Data.Item.Sellable.Battle;
using Game.SO.Data.Shop;
using Game.SO.EventChannel;
using System;
using System.Collections.Generic;
using UnityEngine;


namespace Game.Inventory
{
    public class InventoryManager : MonoBehaviour
    {
        [Header("Event Lisening Channel")]
        [SerializeField] ShopPurchaseEventChannelSO shopPurchaseEventChannel;

        [Header("Event Broadcasting Channel")]
        //[SerializeField] StringEventChannelSO ; // toast notif event

        [Header("Inventory")]
        [SerializeField] Inventory inventory = null;

        //made it subscribe based
        public event Action OnInventoryChanged;


        public void HandleShopPurchase(ShopTrade trade)
        {
            ref Shopable cost = ref trade.cost;

            bool canBuy = true;

            if (cost.useShell && cost.shell > inventory.ShellCurrency)
            {    
                canBuy = false;
            }

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
                return;
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
        }


        public void ManageInventory(Inventory inv)
        {
            inventory = inv;
            OnInventoryChanged?.Invoke();
        }


        public void AddItem(ItemSO item, uint amount = 1)
        {
            var itemList = inventory.ItemList;

            for (int i = 0; i < itemList.Count; i++)
            {
                var itemStack = itemList[i];

                if (itemStack.item == item)
                {
                    itemStack.count += amount;
                    itemList[i] = itemStack;
                    OnInventoryChanged?.Invoke();
                    return;
                }
            }

            itemList.Add(new(item, amount));
            OnInventoryChanged?.Invoke();
        }


        public bool HasItemInList(ItemSO item, out uint amount)
        {
            amount = 0;

            var itemList = inventory.ItemList;
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

        public bool HasItemInEquiped(ItemSO item, out uint amount)
        {
            amount = 0;

            if (inventory.EquipedWeapon == item)
            {
                amount++;
            } 
            else
            {
                foreach (var accessory in inventory.EquipedAccessoryList)
                {
                    if (accessory == item)
                    {
                        amount++;
                    }
                }
            }
            return amount != 0;
        }


        public bool HasItem(ItemSO item, out uint amount)
        {
            HasItemInList(item, out uint a1);
            HasItemInEquiped(item, out uint a2);
            amount = a1 + a2;

            return amount != 0;
        }


        public void RemoveItem(ItemSO item, uint amount)
        {
            var itemList = inventory.ItemList;

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
                    OnInventoryChanged?.Invoke();
                    return;
                }
            }

            throw new System.InvalidOperationException($"InventoryManager.RemoveItem() | no item of {item} exists in the inventory to be removed");
        }


        public List<ItemStack> GetItemList()
        {
            return new(inventory.ItemList);
        }


        public void DeductShell(int amount)
        {
            inventory.ShellCurrency -= amount;
            OnInventoryChanged?.Invoke();
        }

        public void RecieveShell(int amount)
        {
            inventory.ShellCurrency += amount;
            OnInventoryChanged?.Invoke();
        }

        public void SetShell(int amount)
        {
            inventory.ShellCurrency = amount;
            OnInventoryChanged?.Invoke();
        }


        public BattleItemSO[] GetEquipedBattleItems()
        {
            BattleItemSO[] battleItemList = new BattleItemSO[Inventory.MAX_ACCESSORYIES + 1];
            battleItemList[0] = inventory.EquipedWeapon;

            for (int i = 0; i < inventory.EquipedAccessoryList.Length; i++)
            {
                battleItemList[i + 1] = inventory.EquipedAccessoryList[i];
            }

            return battleItemList;
        }


        public WeaponItemSO GetEquipedWeapon()
        {
            return inventory.EquipedWeapon;
        }


        public AccessoryItemSO[] GetEquipedAccessories()
        {
            AccessoryItemSO[] list = new AccessoryItemSO[Inventory.MAX_ACCESSORYIES];
            inventory.EquipedAccessoryList.CopyTo(list, 0);
            return list;
        }





        //private void OnEnable()
        //{
        //    shopPurchaseEventChannel.Subscribe(HandleShopPurchase);
        //}

        //private void OnDisable()
        //{
        //    shopPurchaseEventChannel.Unsubscribe(HandleShopPurchase);
        //}

    }
}