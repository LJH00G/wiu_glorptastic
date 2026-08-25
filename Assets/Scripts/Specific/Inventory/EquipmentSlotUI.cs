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

        public ItemSO BoundItem { get; private set; }
        public event Action<ItemSO> OnSlotClicked;

        void Awake()
        {
            if (clickButton)
            {
                clickButton.onClick.AddListener(() => OnSlotClicked?.Invoke(BoundItem));
            }
        }

        public void SetItem(ItemSO item)
        {
            BoundItem = item;

            if (!icon)
            {
                return;
            }
            icon.enabled = item != null;
            icon.sprite = item ? item.Sprite : null;
        }
    }
}