using Game.SO.Data.Shop;
using System.Collections.Generic;
using UnityEngine;


namespace Game.SO.Data.Shop
{
    [CreateAssetMenu(fileName = "ShopPreset_Data", menuName = "Scriptable Objects/Data/Shop/ShopPresetSO")]
    public class ShopPresetSO : ScriptableObject
    {
        [field: SerializeField]
        public List<SellCostPairSO> SellTable { get; private set; }
    }
}
