using UnityEngine;


namespace Game.SO.Data.Item
{
    [CreateAssetMenu(fileName = "CurseGemItem_Data", menuName = "Scriptable Objects/Data/Item/CurseGemItemSO")]
    public class CurseGemItemSO : ItemSO
    {
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