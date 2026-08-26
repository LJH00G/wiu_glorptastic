using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using Game.SO.Data.Item;


namespace Game.Combat
{
    [CreateAssetMenu(fileName = "LootTableSO", menuName = "Combat/LootTableSO")]

    public class LootTableSO : ScriptableObject
    {
        public int rolls;
        public List<LootChanceData> itemList;
    }

    [System.Serializable]
    public class LootChanceData
    {
        public int odds;
        public int amount;
        public ItemSO item;

        public LootChanceData(int odds, int amount, ItemSO item)
        {
            this.odds = odds;
            this.amount = amount;
            this.item = item;
            
        }
    }

    public struct LootData
    {
        public int count;
        public ItemSO item;

        public LootData(int count, ItemSO item)
        {
            this.count = count;
            this.item = item;
        }
    }
}

