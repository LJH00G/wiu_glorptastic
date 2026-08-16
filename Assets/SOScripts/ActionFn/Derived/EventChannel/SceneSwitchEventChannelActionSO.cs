
using Game.SO.EventChannel.Context;
using Game.SO.EventChannel.Derived;
using UnityEngine;

namespace Game.SO.ActionFn.Derived
{
    [CreateAssetMenu(fileName = "SceneSwitchEventChannel_Act", menuName = "Scriptable Objects/ActionFn/EventChannel/SceneSwitchEventChannelActionSO")]
    public class SceneSwitchEventChannelActionSO : EventChannelActionSO<SceneSwitchEventChannelSO, SceneSwitchEventContext>
    {
    }
}