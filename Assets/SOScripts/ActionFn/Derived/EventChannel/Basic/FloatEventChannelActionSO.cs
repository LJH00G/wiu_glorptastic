using Game.SO.EventChannel;
using UnityEngine;

namespace Game.SO.ActionFn.EventChannel
{
    [CreateAssetMenu(fileName = "FloatEventChannel_Act", menuName = "Scriptable Objects/ActionFn/EventChannel/Basic/FloatEventChannelActionSO")]
    public class FloatEventChannelActionSO : EventChannelActionSO<FloatEventChannelSO, float>
    {

    }
}