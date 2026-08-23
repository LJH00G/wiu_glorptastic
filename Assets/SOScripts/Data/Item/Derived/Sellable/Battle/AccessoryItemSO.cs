using Game.Combat;
using UnityEngine;

namespace Game.SO.Data.Item.Sellable.Battle
{
    [CreateAssetMenu(fileName = "AccessoryItem_Data", menuName = "Scriptable Objects/Data/Item/Sellable/Battle/AccessoryItemSO")]
    public class AccessoryItemSO : BattleItemSO
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
        [field: SerializeField]
        public float MasteryWindowWidthMultiplier { get; private set; } = 1f;
        [field: SerializeField]
        public int HPRegenPerTurn { get; private set; }
        [field: SerializeField]
        public int CSRegenPerTurn { get; private set; }
    }
}