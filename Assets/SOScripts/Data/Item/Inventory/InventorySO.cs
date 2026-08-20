using System;
using System.Collections.Generic;
using UnityEngine;
using Game.SO.Data.Item;

namespace Game.SO.Data.Inventory
{
    [CreateAssetMenu(fileName = "Inventory_Data", menuName = "Scriptable Objects/Data/InventorySO")]
    public class InventorySO : ScriptableObject
    {
        [SerializeField] List<ItemStack> itemList = new();

        public IReadOnlyList<ItemStack> ItemList => itemList;

        public event Action OnChanged;
        public void NotifyChanged() => OnChanged?.Invoke();

        public void Debug_AddStack(ItemSO item, int count)
        {
            itemList.Add(new ItemStack { item = item, count = count });
            NotifyChanged();
        }

        public void Debug_RemoveStack(ItemSO item)
        {
            itemList.RemoveAll(stack => stack.item == item);
            NotifyChanged();
        }
    }
}
