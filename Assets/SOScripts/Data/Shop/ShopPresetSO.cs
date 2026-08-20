using Game.Inventory;
using System;
using System.Collections.Generic;
using UnityEngine;


namespace Game.SO.Data.Shop
{
    [Serializable]
    public struct Shopable
    {
        public List<ItemStack> itemStacks;
        public bool useShell;
        public int shell;
    }

    [Serializable]
    public struct ShopTrade
    {
        public Shopable product;
        public Shopable cost;
    }

    [CreateAssetMenu(fileName = "ShopPreset_Data", menuName = "Scriptable Objects/Data/Shop/ShopPresetSO")]
    public class ShopPresetSO : ScriptableObject
    {
        [field: SerializeField]
        public List<ShopTrade> TradeTable { get; private set; }
    }
}
