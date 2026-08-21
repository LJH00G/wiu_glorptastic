using Game.SO.EventChannel;
using UnityEngine;

namespace Game.SO.ActionFn.EventChannel
{
    [CreateAssetMenu(fileName = "CharEventChannel_Act", menuName = "Scriptable Objects/ActionFn/EventChannel/Basic/CharEventChannelActionSO")]
    public class CharEventChannelActionSO : EventChannelActionSO<CharEventChannelSO, char>
    {

    }
}