using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Game.SO.Data.Item;

namespace Game.Inventory
{
    /// <summary>
    /// runtime debug panel - pick an item from a dropdown, type a quantity, hit add/remove.
    /// lets you test any item + any amount without needing per-item buttons or recompiling
    /// </summary>
    public class DebugInventoryPanel : MonoBehaviour
    {

        [Header("Items Available To Debug With")]
        [SerializeField] ItemSO[] debugItemPool;

        [Header("UI Refs")]
        [SerializeField] TMP_Dropdown itemDropdown;
        [SerializeField] TMP_InputField quantityInput;

        void OnEnable()
        {
            PopulateDropdown();
        }

        void PopulateDropdown()
        {
            if (!itemDropdown)
            {
                return;
            }
            itemDropdown.ClearOptions();

            List<string> names = new();
            foreach (var item in debugItemPool)
            {
                names.Add(item ? item.Name : "<missing>");
            }
            itemDropdown.AddOptions(names);
        }

        ItemSO GetSelectedItem()
        {
            if (debugItemPool == null || debugItemPool.Length == 0)
            {
                return null;
            }
            int index = Mathf.Clamp(itemDropdown ? itemDropdown.value : 0, 0, debugItemPool.Length - 1);
            return debugItemPool[index];
        }

        uint GetQuantity()
        {
            if (quantityInput && uint.TryParse(quantityInput.text, out uint value) && value > 0)
            {
                return value;
            }
            return 1;
        }


        public void AddFromUI()
        {
            ItemSO item = GetSelectedItem();
            if (!item)
            {
                return;
            }
            InventoryManager.AddItem(item, GetQuantity());
        }

        public void RemoveFromUI()
        {
            ItemSO item = GetSelectedItem();
            if (!item)
            {
                return;
            }
            if (!InventoryManager.HasItemInList(item, out uint available))
            {
                return;
            }
            uint amount = GetQuantity();
            InventoryManager.RemoveItem(item, amount > available ? available : amount);
        }
    }
}
