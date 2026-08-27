using Game.SO.Data.Item;
using Game.SO.EventChannel.Context;
using System;
using System.Collections.Generic;
using UnityEngine;


namespace Game.Combat.Integration
{
    public class CombatAssigmment : MonoBehaviour
    {

        void Start()
        {
            Debug.Log($"trying to load data from tunnel");
            LoadDataAssignment();
        }

        [SerializeField] private List<Transform> transformList;
        [SerializeField] private CombatManager combatManager;
        [SerializeField] private CombatDataTunnelSO dataTunnel;

        [SerializeField] private CombatEndEventChannelSO combatEnd;
        public void LoadDataAssignment()
        {
            var EnemyDataList = dataTunnel.enemyEncounterData.dataList;

            combatManager.enemyAnchors = transformList.ToArray();

            combatManager.enemyEncounter = EnemyDataList.ToArray();
            combatManager.playerLoadout = dataTunnel.playerLoadout;
            combatManager.partnerLoadout = dataTunnel.partnerLoadout;

            Sprite[] enemySprites = new Sprite [EnemyDataList.Count];
            for (int i = 0; i < EnemyDataList.Count; i++)
                enemySprites[i] = EnemyDataList[i].sprite;

            Debug.Log($"trying set up battle");
            combatManager.SetupBattle(
                dataTunnel.playerLoadout.sprite,
                dataTunnel.partnerLoadout ? dataTunnel.partnerLoadout.sprite : null,
                enemySprites
            );
        }

        public void WipeDataAssignment(CombatState state, int hp, int curse)
        {

            List<LootData> loot = null;
            if (state == CombatState.BATTLE_WON)
            {
                List<LootTableSO> lootPool = ObtainLootPool(dataTunnel);
                if(lootPool.Count > 0)
                    loot = LootCalculation(lootPool);
            }
            

            dataTunnel.WipeCall();
            CombatEndEventContextSO context = new(state, loot, hp, curse);
            combatEnd.Raise(context);
        }

        static public List<LootData> LootCalculation(List<LootTableSO> lootChanceData)
        {
            List<LootData> loot = new();
            
            foreach (LootTableSO lootTable in lootChanceData)
            {
                Dictionary<int, LootData> lootWeightTable = new Dictionary<int, LootData>();
                int index = 0;

                if (lootTable == null || lootTable.itemList == null || lootTable.itemList.Count == 0)
                    continue;

                foreach (LootChanceData chanceData in lootTable.itemList)
                {
                    int amt = chanceData.odds;

                    for (int i = 0; i < amt; i++)
                    {
                        index++;
                        LootData lootData = new LootData(chanceData.amount, chanceData.item);
                        lootWeightTable.Add(index, lootData);
                    }

                }

                for (int i = 0; i < lootTable.rolls; i++)
                {

                    int roll = UnityEngine.Random.Range(1, lootWeightTable.Count + 1);

                    if (lootWeightTable.TryGetValue(roll, out LootData item))
                        loot.Add(item);
                }

            }

            return loot;
        }

        public List<LootTableSO> ObtainLootPool(CombatDataTunnelSO tunnelData)
        {
            List<LootTableSO> lootPool = new();
            foreach(EnemyDataSO lootTable in tunnelData.enemyEncounterData.dataList)
            {
                lootPool.Add(lootTable.enemyLootTable);
            }
            lootPool.Add(tunnelData.enemyEncounterData.encounterLootTable);

            return lootPool;
        }

    }
}


