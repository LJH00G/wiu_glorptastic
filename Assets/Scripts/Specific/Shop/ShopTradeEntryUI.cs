using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Game.SO.Data.Item;

public class ShopTradeEntryUI : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] Image icon;
    [SerializeField] TMP_Text nameText;
    [SerializeField] TMP_Text countText;

    [Header("Shell Display")]
    [SerializeField] Sprite shellIcon;

    static readonly Dictionary<Texture2D, Sprite> spriteCache = new();

    public void SetItem(ItemSO item, uint count)
    {
        if (icon)
        {
            icon.enabled = item != null;
            icon.sprite = item ? TextureToSprite(item.Texture) : null;
        }

        if (nameText)
        {
            nameText.text = item ? item.Name : "???";
        }
        if (countText)
        {
            countText.text = count > 1 ? $"x{count}" : string.Empty;
        }
    }

    public void SetShell(int amount)
    {
        if (icon)
        {
            icon.enabled = shellIcon != null;
            icon.sprite = shellIcon;
        }

        if (nameText)
        {
            nameText.text = "Shell";
        }
        if (countText)
        {
            countText.text = amount.ToString();
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
