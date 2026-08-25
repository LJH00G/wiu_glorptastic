
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Game.SO.Data.Item;

public class ShopTradeEntryUI : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] Image icon;
    [SerializeField] TextMarkupTypeWriter nameText;
    [SerializeField] TMP_Text countText;

    [Header("Shell Display")]
    [SerializeField] Sprite shellIcon;

    public void SetItem(ItemSO item, uint count)
    {
        if (icon)
        {
            icon.enabled = item != null;
            icon.sprite = item ? item.Sprite : null;
        }

        if (nameText)
        {
            nameText.StartNewTypeWriting(item ? item.Name : "???");
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
            nameText.StartNewTypeWriting("Shell");
        }
        if (countText)
        {
            countText.text = amount.ToString();
        }
    }

}
