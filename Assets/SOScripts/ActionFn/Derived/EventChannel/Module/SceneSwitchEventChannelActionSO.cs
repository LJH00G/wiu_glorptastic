
using Game.SO.EventChannel.Context;
using Game.SO.EventChannel;
using UnityEngine;

namespace Game.SO.ActionFn.Derived.EventChannel
{
    [CreateAssetMenu(fileName = "SceneSwitchEventChannel_Act", menuName = "Scriptable Objects/ActionFn/EventChannel/Module/SceneSwitchEventChannelActionSO")]
    public class SceneSwitchEventChannelActionSO : EventChannelActionSO<SceneSwitchEventChannelSO, SceneSwitchEventContext>
    {
    }
}