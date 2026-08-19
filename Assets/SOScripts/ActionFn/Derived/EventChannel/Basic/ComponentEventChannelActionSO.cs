using Game.SO.EventChannel;
using UnityEngine;

namespace Game.SO.ActionFn.EventChannel
{
    [CreateAssetMenu(fileName = "ComponentEventChannel_Act", menuName = "Scriptable Objects/ActionFn/EventChannel/Basic/ComponentEventChannelActionSO")]
    public class ComponentEventChannelActionSO : EventChannelActionSO<ComponentEventChannelSO, Component>
    {

    }
}