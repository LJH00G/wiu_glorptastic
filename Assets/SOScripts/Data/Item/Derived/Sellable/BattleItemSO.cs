using UnityEngine;
using Game.Combat;


namespace Game.SO.Data.Item.Sellable.Battle
{
    public abstract class BattleItemSO : SellableItemSO
    {
        [field: SerializeField]
        public AbilitySO[] curseAbilityList { get; private set; }
        [field: SerializeField]
        public int ExtraMaxHP { get; private set; }
        [field: SerializeField]
        public int ExtraMaxCurse { get; private set; }
        [field: SerializeField]
        public int ExtraDamage { get; private set; }
        [field: SerializeField]
        public int ExtraDefence { get; private set; }
    }
}