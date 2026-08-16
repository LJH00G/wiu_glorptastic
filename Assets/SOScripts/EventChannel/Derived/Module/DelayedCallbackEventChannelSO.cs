using Game.SO.EventChannel.Context;
using System;
using UnityEngine;

namespace Game.SO.EventChannel.Context
{
    [Serializable]
    public struct DelayedCallbackEventContext
    {
        public Action method;
        public float delay;
        public bool addOrRemove;

        public DelayedCallbackEventContext(Action method, float delay, bool addOrRemove = true)
        {
            this.method = method;
            this.delay = delay;
            this.addOrRemove = addOrRemove;
        }

        public override string ToString()
        {
            return $"DelayedCallbackEventContext: method({method}), delay({delay}), addOrRemove({addOrRemove}) ";
        }
    }
}

namespace Game.SO.EventChannel.Derived
{
    [CreateAssetMenu(fileName = "DelayedCallbackEvent_Channel", menuName = "Scriptable Objects/EventChannel/DelayedCallbackEventChannelSO")]
    public class DelayedCallbackEventChannelSO : EventChannelSO<DelayedCallbackEventContext>
    {

    }
}