using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Game.SO.Data.Item;
using Game.SO.Data.Shop;

public class ShopSlotUI : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] Image icon;
    [SerializeField] Button button;

    public int TradeIndex { get; private set; } = -1;
    public event Action<int> OnSlotClicked;

    void Awake()
    {
        if (button)
        {
            button.onClick.AddListener(() => OnSlotClicked?.Invoke(TradeIndex));
        }
    }

    public void SetTrade(int index, ShopTrade trade)
    {
        TradeIndex = index;

        ItemSO displayItem = (trade.product.itemStacks != null && trade.product.itemStacks.Length > 0) ? trade.product.itemStacks[0].item : null;

        if (icon)
        {
            icon.enabled = displayItem != null;
            icon.sprite = displayItem ? displayItem.Sprite : null;
        }
    }

}
