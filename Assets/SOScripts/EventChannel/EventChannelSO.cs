using System;
using UnityEngine;

namespace Game.SO.EventChannel
{
    public abstract class EventChannelSO<T> : ScriptableObject
    {
        Action<T> listeners = null;
        public void Subscribe(Action<T> method) => listeners += method;
        public void Unsubscribe(Action<T> method) => listeners -= method;
        public void Raise(T args) { if (!Lock) listeners?.Invoke(args); }
        public bool Lock { get; set; } = false;
    }

    [CreateAssetMenu(fileName = "Event_Channel", menuName = "Scriptable Objects/EventChannel/EventChannelSO")]
    public abstract class EventChannelSO : ScriptableObject
    {
        Action listeners = null;
        public void Subscribe(Action method) => listeners += method;
        public void Unsubscribe(Action method) => listeners -= method;
        public void Raise() { if (!Lock) listeners?.Invoke(); }
        public bool Lock { get; set; } = false;
    }

}