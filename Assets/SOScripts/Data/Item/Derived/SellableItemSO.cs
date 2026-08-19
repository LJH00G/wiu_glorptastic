using UnityEngine;


namespace Game.SO.Data.Item.Sellable
{
    public abstract class SellableItemSO : ItemSO
    {
        [field: SerializeField]
        public int SellValue { get; private set; }
    }
}