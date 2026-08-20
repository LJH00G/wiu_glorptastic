using UnityEngine;
using Game.SO.Data.Item;

namespace Game.Inventory
{
    public class EquipmentUI : MonoBehaviour
    {
        [Header("Source")]
        [SerializeField] InventoryManager inventoryManager;

        [Header("Slots")]
        [SerializeField] EquipmentSlotUI weaponSlot;
        [SerializeField] EquipmentSlotUI[] accessorySlots;

        [Header("Detail Panel")]
        [SerializeField] ItemDetailUI itemDetailUI;

        void Awake()
        {
            weaponSlot.OnSlotClicked += HandleSlotClicked;

            foreach (var slot in accessorySlots)
            {
                slot.OnSlotClicked += HandleSlotClicked;
            }
        }

        void OnEnable()
        {
            inventoryManager.OnInventoryChanged += Refresh;
            Refresh();
        }

        void OnDisable()
        {
            inventoryManager.OnInventoryChanged -= Refresh;
        }

        void Refresh()
        {
            weaponSlot.SetItem(inventoryManager.GetEquipedWeapon());

            var accessories = inventoryManager.GetEquipedAccessories();
            for (int i = 0; i < accessorySlots.Length && i < accessories.Length; i++)
            {
                accessorySlots[i].SetItem(accessories[i]);
            }
        }

        void HandleSlotClicked(ItemSO item)
        {
            if (item && itemDetailUI)
            {
                itemDetailUI.Show(item);
            }
        }
    }
}