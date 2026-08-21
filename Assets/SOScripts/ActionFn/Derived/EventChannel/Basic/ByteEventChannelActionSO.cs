using Game.SO.EventChannel;
using UnityEngine;

namespace Game.SO.ActionFn.EventChannel
{
    [CreateAssetMenu(fileName = "ByteEventChannel_Act", menuName = "Scriptable Objects/ActionFn/EventChannel/Basic/ByteEventChannelActionSO")]
    public class ByteEventChannelActionSO : EventChannelActionSO<ByteEventChannelSO, byte>
    {

    }
}