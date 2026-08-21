using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Game.SO.Data.Item;

namespace Game.Inventory
{

    public class EquipmentSlotUI : MonoBehaviour
    {
        [Header("Refs")]
        [SerializeField] Image icon;
        [SerializeField] Button clickButton;

        static readonly Dictionary<Texture2D, Sprite> spriteCache = new();

        public event Action OnUnequipRequested;

        void Awake()
        {
            if (clickButton)
            {
                clickButton.onClick.AddListener(() => OnUnequipRequested?.Invoke());
            }
        }

        public void SetItem(ItemSO item)
        {
            if (!icon)
            {
                return;
            }
            icon.enabled = item != null;
            icon.sprite = item ? TextureToSprite(item.Texture) : null;
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
