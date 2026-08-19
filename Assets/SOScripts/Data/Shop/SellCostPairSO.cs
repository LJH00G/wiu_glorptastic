
using Game.Inventory;
using System;
using System.Collections.Generic;
using UnityEngine;


namespace Game.SO.Data.Shop
{
    [CreateAssetMenu(fileName = "SellCostPair_Data", menuName = "Scriptable Objects/Data/Shop/SellCostPairSO")]
    public class SellCostPairSO : ScriptableObject
    {
        [Serializable]
        public struct Shopable
        {
            public List<ItemStack> itemStacks;
            public bool useShell;
            public int shell;
        }

        [field: SerializeField]
        public Shopable Sell { get; private set; }
        [field: SerializeField]
        public Shopable Cost { get; private set; }
    }
}
