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
        public bool dtScaledOrUnscaled;
        public bool addOrRemove;

        public DelayedCallbackEventContext(Action method, float delay, bool dtScaledOrUnscaled = true, bool addOrRemove = true)
        {
            this.method = method;
            this.delay = delay;
            this.dtScaledOrUnscaled = dtScaledOrUnscaled;
            this.addOrRemove = addOrRemove;
        }

        public override string ToString()
        {
            return $"DelayedCallbackEventContext: method({method}), delay({delay}), dtScaledOrUnscaled({dtScaledOrUnscaled}), addOrRemove({addOrRemove}) ";
        }
    }
}

namespace Game.SO.EventChannel
{
    [CreateAssetMenu(fileName = "DelayedCallbackEvent_Channel", menuName = "Scriptable Objects/EventChannel/Module/DelayedCallbackEventChannelSO")]
    public class DelayedCallbackEventChannelSO : EventChannelSO<DelayedCallbackEventContext>
    {

    }
}