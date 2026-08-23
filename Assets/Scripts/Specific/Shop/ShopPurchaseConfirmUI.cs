using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Game.SO.Data.Shop;

public class ShopPurchaseConfirmUI : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] GameObject panelRoot;
    [SerializeField] Transform costContainer;
    [SerializeField] Transform productContainer;
    [SerializeField] ShopTradeEntryUI entryPrefab;
    [SerializeField] Button yesButton;
    [SerializeField] Button noButton;

    readonly List<ShopTradeEntryUI> costEntries = new();
    readonly List<ShopTradeEntryUI> productEntries = new();

    Action pendingConfirm;

    public static ShopPurchaseConfirmUI Instance { get; private set; }

    void Awake()
    {
        if (Instance && Instance != this)
        {
            Debug.LogWarning("ShopPurchaseConfirmUI.Awake() ++++OWO++++ more than one instance exists in this scene,glorp will point at the most recently loaded one");
        }
        Instance = this;

        if (yesButton)
        {
            yesButton.onClick.AddListener(HandleYesClicked);
        }
        if (noButton)
        {
            noButton.onClick.AddListener(Hide);
        }
        Hide();
    }

    public void Show(ShopTrade trade, Action onConfirm)
    {
        pendingConfirm = onConfirm;

        if (panelRoot)
        {
            panelRoot.SetActive(true);
        }
        PopulateSide(costContainer, costEntries, trade.cost);
        PopulateSide(productContainer, productEntries, trade.product);
    }

    public void Hide()
    {
        pendingConfirm = null;

        if (panelRoot)
        {
            panelRoot.SetActive(false);
        }
    }

    void HandleYesClicked()
    {
        pendingConfirm?.Invoke();
        Hide();
    }

    void PopulateSide(Transform container, List<ShopTradeEntryUI> pool, Shopable shopable)
    {
        int neededCount = (shopable.useShell ? 1 : 0) + (shopable.itemStacks?.Count ?? 0);

        while (pool.Count < neededCount)
        {
            ShopTradeEntryUI entry = Instantiate(entryPrefab);
            entry.transform.SetParent(container, false);
            pool.Add(entry);
        }

        // shrink the pool if needed
        while (pool.Count > neededCount)
        {
            int last = pool.Count - 1;
            Destroy(pool[last].gameObject);
            pool.RemoveAt(last);
        }

        int index = 0;

        if (shopable.useShell)
        {
            pool[index].gameObject.SetActive(true);
            pool[index].SetShell(shopable.shell);
            index++;
        }

        if (shopable.itemStacks != null)
        {
            foreach (var stack in shopable.itemStacks)
            {
                pool[index].gameObject.SetActive(true);
                pool[index].SetItem(stack.item, stack.count);
                index++;
            }
        }
    }
}