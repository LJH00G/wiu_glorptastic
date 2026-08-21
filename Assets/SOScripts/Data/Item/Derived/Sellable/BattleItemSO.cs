using Game.SO.Data.CurseAbility;
using UnityEngine;


namespace Game.SO.Data.Item.Sellable.Battle
{
    public abstract class BattleItemSO : SellableItemSO
    {
        [field: SerializeField]
        public CurseAbilitySO[] curseAbilityList { get; private set; }
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