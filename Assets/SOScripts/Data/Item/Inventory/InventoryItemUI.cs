using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Game.SO.Data.Item;
using Game.SO.Data.Item.Sellable.Battle;
///add a new system that allows inventory to be reused during the shop section when the player is prompted whether they want to sell anything, inventory should pop back open, disable the underlying shop layer and let player
///choose what items in their inventory they want to sell, then it uses the inventory description space to display the shop trading stuff like the cost you gain from trading and the amount players want to trade for it etc.
namespace Game.Inventory
{
    public class InventoryItemUI : MonoBehaviour
    {
        [Header("Refs")]
        [SerializeField] Image icon;
        [SerializeField] TMP_Text titleText;
        [SerializeField] TMP_Text descriptionText;
        [SerializeField] TMP_Text quantityText;

        [SerializeField] Button equipButton;

        static readonly Dictionary<Texture2D, Sprite> spriteCache = new();

        public ItemSO BoundItem { get; private set; }

        public event Action<ItemSO> OnEquipRequested;
        void Awake()
        {
            if (equipButton)
            {
                equipButton.onClick.AddListener(() => OnEquipRequested?.Invoke(BoundItem));
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
            if (equipButton)
            {
                equipButton.gameObject.SetActive(item is BattleItemSO);
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