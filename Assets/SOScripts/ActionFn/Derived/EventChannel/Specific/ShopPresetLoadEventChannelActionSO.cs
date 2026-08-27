using UnityEngine;
using Game.SO.Data.Shop;

namespace Game.SO.ActionFn
{
    [CreateAssetMenu(fileName = "ShopPresetLoadEventChannel_Act", menuName = "Scriptable Objects/ActionFn/EventChannel/ShopPresetLoadEventChannelActionSO")]
    public class ShopPresetLoadEventChannelActionSO : EventChannelActionSO<ShopOpenEventChannelSO, ShopPresetSO>
    {

    }

    
}