using UnityEngine;

namespace Game.SO.Data.Item.Sellable.Battle
{
    [CreateAssetMenu(fileName = "AccessoryItem_Data", menuName = "Scriptable Objects/Data/Item/Sellable/Battle/AccessoryItemSO")]
    public class AccessoryItemSO : BattleItemSO
    {
        public float masteryWindowWidthMultiplier = 1f;
        public int hpRegenPerTurn = 0;
        public int csRegenPerTurn = 0;
    }
}