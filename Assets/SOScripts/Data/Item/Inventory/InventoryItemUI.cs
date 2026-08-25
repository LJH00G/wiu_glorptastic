using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Game.SO.Data.Item;

namespace Game.Inventory
{
    public class InventoryItemUI : MonoBehaviour
    {
        [Header("Refs")]
        [SerializeField] Image icon;
        [SerializeField] TextMarkupTypeWriter titleText;
        [SerializeField] TMP_Text descriptionText;
        [SerializeField] TMP_Text quantityText;
        [SerializeField] Button detailsButton;

        public ItemSO BoundItem { get; private set; }
        public event Action<ItemSO> OnDetailsRequested;
        void Awake()
        {
            if (detailsButton)
            {
                detailsButton.onClick.AddListener(() => OnDetailsRequested?.Invoke(BoundItem));
            }
        }

        public void SetData(ItemSO item, uint quantity)
        {
            BoundItem = item;

            if (icon)
            {
                icon.sprite = item.Sprite;
            }
            if (titleText)
            {
                titleText.StartNewTypeWriting(item.Name);
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