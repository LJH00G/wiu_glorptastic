using UnityEngine;

namespace Game.Inventory
{
    public class EquipmentUI : MonoBehaviour
    {

        [Header("Slots")]
        [SerializeField] EquipmentSlotUI weaponSlot;
        [SerializeField] EquipmentSlotUI[] accessorySlots;

        void Awake()
        {
            weaponSlot.OnUnequipRequested += () => InventoryManager.UnequipWeapon();

            for (int i = 0; i < accessorySlots.Length; i++)
            {
                int slotIndex = i;
                accessorySlots[i].OnUnequipRequested += () => InventoryManager.UnequipAccessory(slotIndex);
            }
        }

        void OnEnable()
        {
            InventoryManager.OnInventoryChanged.Subscribe(Refresh);
            Refresh();
        }

        void OnDisable()
        {
            InventoryManager.OnInventoryChanged.Unsubscribe(Refresh);
        }

        void Refresh()
        {
            weaponSlot.SetItem(InventoryManager.GetEquipedWeapon());

            var accessories = InventoryManager.GetEquipedAccessories();
            for (int i = 0; i < accessorySlots.Length && i < accessories.Length; i++)
            {
                accessorySlots[i].SetItem(accessories[i]);
            }
        }
    }
}
