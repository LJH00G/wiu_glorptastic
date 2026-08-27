
using System;
using UnityEngine;
using Utility.VisualizableDictionary;
using System.Collections.Generic;
using Game.Combat;
using Game.Combat.Integration;
using Game.Inventory;


namespace Game.Interactable.TriggerHandler.Single
{

    [RequireComponent(typeof(BoxCollider2D))]
    public class ChestTriggerHandler : SingleTriggerHandler<List<LootTableSO>>
    {

        public bool openableMultipleTimes;
        protected override void TriggerType(ref List<LootTableSO> table)
        {
            List<LootData> loot = CombatAssigmment.LootCalculation(table);
            foreach (LootData lootData in loot)
            {
                InventoryManager.AddItem(lootData.item, (uint)lootData.count);
            }

            if (!openableMultipleTimes)
                gameObject.SetActive(false);
        }
        

        

        


#if UNITY_EDITOR

        protected override void OnValidate()
        {
            base.OnValidate();
        }
#endif
    }
}