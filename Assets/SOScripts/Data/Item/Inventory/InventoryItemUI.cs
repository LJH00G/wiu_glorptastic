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
        [SerializeField] TMP_Text titleText;
        [SerializeField] TMP_Text descriptionText;
        [SerializeField] TMP_Text quantityText;
        [SerializeField] Button detailsButton;

        static readonly Dictionary<Texture2D, Sprite> spriteCache = new();

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
                icon.sprite = TextureToSprite(item.Texture);
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
}