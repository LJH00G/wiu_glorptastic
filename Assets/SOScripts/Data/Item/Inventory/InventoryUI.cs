using System.Collections.Generic;
using UnityEngine;
using Game.SO.Data.Item;
using Game.SO.EventChannel;

namespace Game.Inventory
{
    public class InventoryUI : MonoBehaviour
    {
        [Header("Event Listening Channel")]
        [SerializeField] EventChannelSO openSellableInventoryEventChannel;

        [Header("List (goes inside your ScrollRect's Content)")]
        [SerializeField] Transform contentParent;
        [SerializeField] InventoryItemUI itemUIPrefab;

        [Header("Detail Panel")]
        [SerializeField] ItemDetailUI itemDetailUI;

        [Header("Panel (optional)")]
        [SerializeField] GameObject panelRoot;
        bool sellMode;

        readonly List<InventoryItemUI> spawned = new();

        public static InventoryUI Instance { get; private set; }

        public bool IsOpen => panelRoot ? panelRoot.activeSelf : gameObject.activeSelf;

        void Awake()
        {
            if (Instance && Instance != this)
            {
                Debug.LogWarning("InventoryUI.Awake() | more than one InventoryUI exists in the scene, Instance will point at the most recently loaded one");
            }
            Instance = this;
        }

        void OnEnable()
        {
            openSellableInventoryEventChannel.Subscribe(ShowSellableInventory);
            InventoryManager.OnInventoryChanged.Subscribe(Refresh, 0);
            Refresh();
        }

        void OnDisable()
        {
            openSellableInventoryEventChannel.Unsubscribe(ShowSellableInventory);
            InventoryManager.OnInventoryChanged.Unsubscribe(Refresh);
        }

        public void ShowSellableInventory()
        {
            sellMode = true;

            if (panelRoot)
            {
                panelRoot.SetActive(true);
            }
            Refresh();
        }

        public void Show()
        {
            sellMode = false;

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
            var stacks = InventoryManager.GetItemList();

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