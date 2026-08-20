using System;
using Game.SO.Data.Item;

namespace Game.SO.Data.Inventory
{
    [Serializable]
    public struct ItemStack
    {
        public ItemSO item;
        public int count;
    }
}
