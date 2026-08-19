using System.Collections.Generic;
using UnityEngine;
using Game.SO.Data.Inventory;

namespace Game.Inventory
{
    public class InventoryUI : MonoBehaviour
    {
        [Header("Source")]
        [SerializeField] InventorySO inventorySO;

        [Header("List (goes inside your ScrollRect's Content)")]
        [SerializeField] Transform contentParent;
        [SerializeField] InventoryItemUI itemUIPrefab;

        readonly List<InventoryItemUI> spawned = new();

        void OnEnable()
        {
            inventorySO.OnChanged += Refresh;
            Refresh();
        }

        void OnDisable()
        {
            inventorySO.OnChanged -= Refresh;
        }

        void Refresh()
        {
            var stacks = inventorySO.ItemList;

            // grow the pool if we have more stacks than rows
            while (spawned.Count < stacks.Count)
            {
                InventoryItemUI row = Instantiate(itemUIPrefab);
                row.transform.SetParent(contentParent, false); // false = don't keep world position, reset local pos/scale relative to new parent
                spawned.Add(row);
            }

            // shrink the pool if we have fewer stacks than rows
            while (spawned.Count > stacks.Count)
            {
                int last = spawned.Count - 1;
                Destroy(spawned[last].gameObject);
                spawned.RemoveAt(last);
            }

            for (int i = 0; i < stacks.Count; i++)
            {
                spawned[i].gameObject.SetActive(true);
                spawned[i].SetData(stacks[i].item, stacks[i].count);
            }
        }
    }
}