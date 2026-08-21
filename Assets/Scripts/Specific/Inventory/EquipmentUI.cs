using UnityEngine;

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
            weaponSlot.OnUnequipRequested += () => inventoryManager.UnequipWeapon();

            for (int i = 0; i < accessorySlots.Length; i++)
            {
                int slotIndex = i;
                accessorySlots[i].OnUnequipRequested += () => inventoryManager.UnequipAccessory(slotIndex);
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
