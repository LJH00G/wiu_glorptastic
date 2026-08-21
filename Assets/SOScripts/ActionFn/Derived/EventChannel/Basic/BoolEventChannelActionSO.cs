using Game.SO.EventChannel;
using UnityEngine;

namespace Game.SO.ActionFn.EventChannel
{
    [CreateAssetMenu(fileName = "BoolEventChannel_Act", menuName = "Scriptable Objects/ActionFn/EventChannel/Basic/BoolEventChannelActionSO")]
    public class BoolEventChannelActionSO : EventChannelActionSO<BoolEventChannelSO, bool>
    {

    }
}