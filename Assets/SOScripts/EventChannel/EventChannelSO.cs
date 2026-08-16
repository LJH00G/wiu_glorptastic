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

    public abstract class EventChannelSO : ScriptableObject
    {
        Action listeners = null;
        public void Subscribe(Action method) => listeners += method;
        public void Unsubscribe(Action method) => listeners -= method;
        public void Raise() { if (!Lock) listeners?.Invoke(); }
        public bool Lock { get; set; } = false;
    }

    public abstract class StructReturnChannelSO<Return, T> : ScriptableObject
        where Return : struct
    {
        Func<T, Return> listener = null;
        public void Subscribe(Func<T, Return> method) => listener = method;
        public Return? Raise(T args) { if (!Lock) return listener?.Invoke(args); return null; }
        public bool Lock { get; set; } = false;
    }

    public abstract class StructReturnChannelSO<Return> : ScriptableObject
        where Return : struct
    {
        Func<Return> listener = null;
        public void Subscribe(Func<Return> method) => listener = method;
        public Return? Raise() { if (!Lock) return listener?.Invoke(); return null; }
        public bool Lock { get; set; } = false;
    }

    public abstract class ClassReturnChannelSO<Return> : ScriptableObject
        where Return : class
    {
        Func<Return> listener = null;
        public void Subscribe(Func<Return> method) => listener = method;
        public Return Raise() { if (!Lock) return listener?.Invoke(); return null; }
        public bool Lock { get; set; } = false;
    }
}