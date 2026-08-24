using Game.Combat;
using System.Collections.Generic;
using UnityEngine;


namespace Game.SO.Data.Item.Sellable
{
    [CreateAssetMenu(fileName = "ConsumableItem_Data", menuName = "Scriptable Objects/Data/Item/Sellable/ConsumableItemSO")]
    public class ConsumableItemSO : SellableItemSO
    {

        [field: SerializeField] 
        public List<EffectEntry> Effects { get; private set; } = new();
        [field: SerializeField] 
        public CombatTargetType TargetType { get; private set; } = CombatTargetType.SELF;
        [field: SerializeField] public bool ConsumeOnUse { get; private set; } = true;

    }
}