using UnityEngine;
using Game.SO.Data.Item;
using Game.SO.Data.Item.Sellable;
using Game.SO.Data.Item.Sellable.Battle;
using Game.SO.Data.Inventory;

namespace Game.Inventory
{
    /// <summary>
    /// pokes InventorySO directly (bypassing whatever add/remove logic the real manager
    /// ends up owning) so the display pipeline - SO -> InventoryUI -> InventoryItemUI -
    /// can be verified with one sample of every ItemSO subtype before the manager exists
    /// </summary>
    public class InventoryTester : MonoBehaviour
    {
        [Header("Target")]
        [SerializeField] InventorySO inventorySO;

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
                inventorySO.Debug_AddStack(item, 1);
        }

        [ContextMenu("Remove All Samples")]
        public void RemoveAllSamples()
        {
            foreach (var item in AllSamples)
                inventorySO.Debug_RemoveStack(item);
        }

        // hook these up to individual buttons if you want to test one item at a time
        public void AddQuestItem() => inventorySO.Debug_AddStack(sampleQuestItem, 1);
        public void RemoveQuestItem() => inventorySO.Debug_RemoveStack(sampleQuestItem);

        public void AddResourceItem() => inventorySO.Debug_AddStack(sampleResourceItem, 1);
        public void RemoveResourceItem() => inventorySO.Debug_RemoveStack(sampleResourceItem);

        public void AddConsumableItem() => inventorySO.Debug_AddStack(sampleConsumableItem, 1);
        public void RemoveConsumableItem() => inventorySO.Debug_RemoveStack(sampleConsumableItem);

        public void AddWeaponItem() => inventorySO.Debug_AddStack(sampleWeaponItem, 1);
        public void RemoveWeaponItem() => inventorySO.Debug_RemoveStack(sampleWeaponItem);

        public void AddAccessoryItem() => inventorySO.Debug_AddStack(sampleAccessoryItem, 1);
        public void RemoveAccessoryItem() => inventorySO.Debug_RemoveStack(sampleAccessoryItem);

        public void AddCurseGemItem() => inventorySO.Debug_AddStack(sampleCurseGemItem, 1);
        public void RemoveCurseGemItem() => inventorySO.Debug_RemoveStack(sampleCurseGemItem);
    }
}