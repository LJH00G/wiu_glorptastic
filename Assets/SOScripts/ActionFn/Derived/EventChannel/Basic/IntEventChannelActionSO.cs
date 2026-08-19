using Game.SO.EventChannel;
using UnityEngine;

namespace Game.SO.ActionFn.EventChannel
{
    [CreateAssetMenu(fileName = "IntEventChannel_Act", menuName = "Scriptable Objects/ActionFn/EventChannel/Basic/IntEventChannelActionSO")]
    public class IntEventChannelActionSO : EventChannelActionSO<IntEventChannelSO, int>
    {

    }
}