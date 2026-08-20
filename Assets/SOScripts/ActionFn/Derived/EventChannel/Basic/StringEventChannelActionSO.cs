using Game.SO.EventChannel;
using UnityEngine;

namespace Game.SO.ActionFn.EventChannel
{
    [CreateAssetMenu(fileName = "StringEventChannel_Act", menuName = "Scriptable Objects/ActionFn/EventChannel/Basic/StringEventChannelActionSO")]
    public class StringEventChannelActionSO : EventChannelActionSO<StringEventChannelSO, string>
    {

    }
}