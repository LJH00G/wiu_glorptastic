using UnityEngine;

namespace Game.SO.Data.Item.Sellable.Battle
{
    [CreateAssetMenu(fileName = "ArmourItem_Data", menuName = "Scriptable Objects/Data/Item/Sellable/Battle/ArmourItemSO")]
    public class ArmourItemSO : BattleItemSO
    {

        [field: SerializeField]
        public int Dmage {  get; private set; }

    }
}