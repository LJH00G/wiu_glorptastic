using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Game.SO.Data.Item;

namespace Game.Inventory
{
    public class InventoryItemUI : MonoBehaviour
    {
        [Header("Refs")]
        [SerializeField] RawImage icon;
        [SerializeField] TMP_Text titleText;
        [SerializeField] TMP_Text descriptionText;
        [SerializeField] TMP_Text quantityText;

        public ItemSO BoundItem { get; private set; }

        public void SetData(ItemSO item, int quantity)
        {
            BoundItem = item;

            if (icon)
            {
                icon.texture = item.Texture;
            }
            if (titleText)
            {
                titleText.text = item.Name;
            }
            if (descriptionText)
            {
                descriptionText.text = item.Description;
            }
            if (quantityText)
            {
                quantityText.text = quantity > 1 ? $"x{quantity}" : string.Empty;
            }
        }
    }
}
