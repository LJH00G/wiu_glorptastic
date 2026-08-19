using System;
using System.Collections.Generic;
using UnityEngine;
using Game.SO.Data.Item;

namespace Game.SO.Data.Inventory
{
    /// <summary>
    /// stub standing in for the teammate's inventory SO - just needs to expose the
    /// same shape (a readable list of <see cref="ItemStack"/> + a change event) for
    /// <see cref="InventoryUI"/> to work. swap this file out once the real one exists,
    /// or rename this SO's fields/type to match theirs.
    /// </summary>
    [CreateAssetMenu(fileName = "Inventory_Data", menuName = "Scriptable Objects/Data/InventorySO")]
    public class InventorySO : ScriptableObject
    {
        [SerializeField] List<ItemStack> itemList = new();

        public IReadOnlyList<ItemStack> ItemList => itemList;

        /// <summary>
        /// raised whenever itemList changes, so InventoryUI knows to refresh.
        /// whoever owns add/remove logic should call NotifyChanged() after mutating itemList
        /// </summary>
        public event Action OnChanged;
        public void NotifyChanged() => OnChanged?.Invoke();

        // --- debug-only helpers, for testing the UI before the real manager exists ---
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
