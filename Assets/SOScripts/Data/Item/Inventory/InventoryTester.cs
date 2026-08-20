using UnityEngine;
using Game.SO.Data.Item;
using Game.SO.Data.Item.Sellable;
using Game.SO.Data.Item.Sellable.Battle;

namespace Game.Inventory
{
    /// <summary>
    /// exercises InventoryManager's real Add/RemoveItem with one sample of every ItemSO
    /// subtype, so the manager -> InventoryUI -> InventoryItemUI pipeline can be verified
    /// </summary>
    public class InventoryTester : MonoBehaviour
    {
        [Header("Target")]
        [SerializeField] InventoryManager inventoryManager;

        [Header("One Sample Per Item Type")]
        [SerializeField] QuestItemSO sampleQuestItem;
        [SerializeField] ResourceSO sampleResourceItem;
        [SerializeField] ConsumableItemSO sampleConsumableItem;
        [SerializeField] WeaponItemSO sampleWeaponItem;
        [SerializeField] AccessoryItemSO sampleAccessoryItem;
        [SerializeField] CurseGemItemSO sampleCurseGemItem;

        ItemSO[] AllSamples => new ItemSO[]
        {
            sampleQuestItem,
            sampleResourceItem,
            sampleConsumableItem,
            sampleWeaponItem,
            sampleAccessoryItem,
            sampleCurseGemItem
        };

        [ContextMenu("Add All Samples")]
        public void AddAllSamples()
        {
            foreach (var item in AllSamples)
            {
                inventoryManager.AddItem(item, 1);
            }
        }

        [ContextMenu("Remove All Samples")]
        public void RemoveAllSamples()
        {
            foreach (var item in AllSamples)
                RemoveSafely(item);
        }
        public void AddQuestItem() => inventoryManager.AddItem(sampleQuestItem, 1);
        public void RemoveQuestItem() => RemoveSafely(sampleQuestItem);
        public void AddResourceItem() => inventoryManager.AddItem(sampleResourceItem, 1);
        public void RemoveResourceItem() => RemoveSafely(sampleResourceItem);
        public void AddConsumableItem() => inventoryManager.AddItem(sampleConsumableItem, 1);
        public void RemoveConsumableItem() => RemoveSafely(sampleConsumableItem);
        public void AddWeaponItem() => inventoryManager.AddItem(sampleWeaponItem, 1);
        public void RemoveWeaponItem() => RemoveSafely(sampleWeaponItem);
        public void AddAccessoryItem() => inventoryManager.AddItem(sampleAccessoryItem, 1);
        public void RemoveAccessoryItem() => RemoveSafely(sampleAccessoryItem);
        public void AddCurseGemItem() => inventoryManager.AddItem(sampleCurseGemItem, 1);
        public void RemoveCurseGemItem() => RemoveSafely(sampleCurseGemItem);

        void RemoveSafely(ItemSO item)
        {
            if (inventoryManager.HasItemInList(item, out _))
            {
                inventoryManager.RemoveItem(item, 1);
            }
        }
    }
}