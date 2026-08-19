using Game.SO.Data.Shop;
using UnityEngine;


//namespace Game.SO.EventChannel.Context
//{
//    [Serializable]
//    public struct ShopPurchaseEventContext
//    {
//        SellCostPairSO SellCostPair;

//        public ShopPurchaseEventContext(Action method, float delay, bool dtScaledOrUnscaled = true, bool addOrRemove = true)
//        {
//            this.method = method;
//            this.delay = delay;
//            this.dtScaledOrUnscaled = dtScaledOrUnscaled;
//            this.addOrRemove = addOrRemove;
//        }

//        public override string ToString()
//        {
//            return $"ShopPurchaseEventContext: method({method}), delay({delay}), dtScaledOrUnscaled({dtScaledOrUnscaled}), addOrRemove({addOrRemove}) ";
//        }
//    }
//}

namespace Game.SO.EventChannel
{
    [CreateAssetMenu(fileName = "ShopPurchaseEvent_Channel", menuName = "Scriptable Objects/EventChannel/Inventory/ShopPurchaseEventChannelSO")]
    public class ShopPurchaseEventChannelSO : EventChannelSO<ShopTrade>
    {

    }
}