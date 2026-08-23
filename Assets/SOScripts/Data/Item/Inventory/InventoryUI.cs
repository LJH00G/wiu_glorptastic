using System.Collections.Generic;
using UnityEngine;
using Game.SO.Data.Item;
using Game.SO.Data.Item.Sellable.Battle;

namespace Game.Inventory
{
    public class InventoryUI : MonoBehaviour
    {

        [Header("List (goes inside your ScrollRect's Content)")]
        [SerializeField] Transform contentParent;
        [SerializeField] InventoryItemUI itemUIPrefab;

        readonly List<InventoryItemUI> spawned = new();

        void OnEnable()
        {
            InventoryManager.OnInventoryChanged.Subscribe(Refresh);
            Refresh();
        }

        void OnDisable()
        {
            InventoryManager.OnInventoryChanged.Unsubscribe(Refresh);
        }

        void Refresh()
        {
            var stacks = InventoryManager.GetItemList();

            while (spawned.Count < stacks.Count)
            {
                InventoryItemUI row = Instantiate(itemUIPrefab);
                row.transform.SetParent(contentParent, false);
                row.OnEquipRequested += HandleEquipRequested;
                spawned.Add(row);
            }


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

        void HandleEquipRequested(ItemSO item)
        {
            if (item is BattleItemSO battleItem)
                InventoryManager.EquipItem(battleItem);
        }
    }
}