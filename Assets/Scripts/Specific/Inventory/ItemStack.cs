using Game.SO.Data.Item;
using System;


namespace Game.Inventory
{
    [Serializable]
    public struct ItemStack
    {
        public ItemSO item;
        public uint count;

        public ItemStack(ItemSO item, uint count = 1)
        {
            this.item = item;
            this.count = count;
        }
    }
}