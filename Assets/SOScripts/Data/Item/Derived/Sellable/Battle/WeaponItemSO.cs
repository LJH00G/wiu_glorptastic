using Game.Combat;
using UnityEngine;

namespace Game.SO.Data.Item.Sellable.Battle
{
    [CreateAssetMenu(fileName = "WeaponItem_Data", menuName = "Scriptable Objects/Data/Item/Sellable/Battle/WeaponItemSO")]
    public class WeaponItemSO : BattleItemSO
    {
        [field: SerializeField]
        public AbilitySO[] curseAbilityList { get; private set; }
        [field: SerializeField]
        public int Dmage {  get; private set; }

    }
}