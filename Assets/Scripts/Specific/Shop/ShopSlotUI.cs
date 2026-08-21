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

    static readonly Dictionary<Texture2D, Sprite> spriteCache = new();

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

        ItemSO displayItem = (trade.product.itemStacks != null && trade.product.itemStacks.Count > 0) ? trade.product.itemStacks[0].item : null;

        if (icon)
        {
            icon.enabled = displayItem != null;
            icon.sprite = displayItem ? TextureToSprite(displayItem.Texture) : null;
        }
    }

    static Sprite TextureToSprite(Texture2D texture)
    {
        if (!texture)
        {
            return null;
        }
        if (!spriteCache.TryGetValue(texture, out Sprite sprite))
        {
            sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            spriteCache[texture] = sprite;
        }

        return sprite;
    }
}
