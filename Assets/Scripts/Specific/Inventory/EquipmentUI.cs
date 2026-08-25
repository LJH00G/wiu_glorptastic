using UnityEngine;
using Game.SO.Data.Item;

namespace Game.Inventory
{
    public class EquipmentUI : MonoBehaviour
    {

        [Header("Slots")]
        [SerializeField] EquipmentSlotUI weaponSlot;
        [SerializeField] EquipmentSlotUI armourSlot;
        [SerializeField] EquipmentSlotUI[] accessorySlots;

        void Awake()
        {
            weaponSlot.OnSlotClicked += item =>
            {
                if (item)
                {
                    InventoryManager.UnequipWeapon();
                }
            };

            armourSlot.OnSlotClicked += item =>
            {
                if (item)
                {
                    InventoryManager.UnequipArmour();
                }
            };

            for (int i = 0; i < accessorySlots.Length; i++)
            {
                int slotIndex = i;
                accessorySlots[i].OnSlotClicked += item =>
                {
                    if (item)
                    {
                        InventoryManager.UnequipAccessory(slotIndex);
                    }
                };
            }
        }

        void OnEnable()
        {
            InventoryManager.OnInventoryChanged.Subscribe(Refresh, 0);
            Refresh();
        }

        void OnDisable()
        {
            InventoryManager.OnInventoryChanged.Unsubscribe(Refresh);
        }

        void Refresh()
        {
            weaponSlot.SetItem(InventoryManager.GetEquipedWeapon());
            armourSlot.SetItem(InventoryManager.GetEquipedArmour());

            var accessories = InventoryManager.GetEquipedAccessories();
            for (int i = 0; i < accessorySlots.Length && i < accessories.Length; i++)
            {
                accessorySlots[i].SetItem(accessories[i]);
            }
        }
    }
}