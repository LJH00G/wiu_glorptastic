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

        void Awake()
        {
            weaponSlot.OnSlotClicked += item =>
            {
                if (item)
                {
                    inventoryManager.UnequipWeapon();
                }
            };

            for (int i = 0; i < accessorySlots.Length; i++)
            {
                int slotIndex = i;
                accessorySlots[i].OnSlotClicked += item =>
                {
                    if (item)
                    {
                        inventoryManager.UnequipAccessory(slotIndex);
                    }
                };
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
    }
}