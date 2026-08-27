using UnityEngine;
using Game.SO.EventChannel;
using Game.SO.Data.Shop;



namespace Game.SO.ActionFn
{

    public class ShopPresetLoadEventChannelActionSO : EventChannelActionSO<ShopOpenEventChannelSO, ShopPresetSO>

    {
        [SerializeField] ShopOpenEventChannelSO eventChannel;
        [SerializeField] ShopPresetSO context;

        public override void Invoke()
        {
            eventChannel.Raise(context);
        }
    }

    
}