using System.Collections.Generic;
using UnityEngine;
using Game.SO.Data.Item;

namespace Game.Inventory
{
    public class InventoryUI : MonoBehaviour
    {
        [Header("Source")]
        [SerializeField] InventoryManager inventoryManager;

        [Header("List (goes inside your ScrollRect's Content)")]
        [SerializeField] Transform contentParent;
        [SerializeField] InventoryItemUI itemUIPrefab;

        [Header("Detail Panel")]
        [SerializeField] ItemDetailUI itemDetailUI;

        [Header("Panel (optional)")]
        [SerializeField] GameObject panelRoot;
        bool sellMode;


        readonly List<InventoryItemUI> spawned = new();

        void OnEnable()
        {
            inventoryManager.OnInventoryChanged += Refresh;
            Refresh();
        }

        void OnDisable()
        {
            inventoryManager.OnInventoryChanged -= Refresh;
        }

        public void Show(bool sellModeEnabled = false)
        {
            sellMode = sellModeEnabled;

            if (panelRoot)
            {
                panelRoot.SetActive(true);
            }
            Refresh();
        }

        public void Hide()
        {
            if (panelRoot)
            {
                panelRoot.SetActive(false);
            }
        }

        void Refresh()
        {
            var stacks = inventoryManager.GetItemList();

            while (spawned.Count < stacks.Count)
            {
                InventoryItemUI row = Instantiate(itemUIPrefab);
                row.transform.SetParent(contentParent, false);
                row.OnDetailsRequested += HandleDetailsRequested;
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

        void HandleDetailsRequested(ItemSO item)
        {
            if (item && itemDetailUI)
            {
                itemDetailUI.Show(item, sellMode);
            }
        }
    }
}