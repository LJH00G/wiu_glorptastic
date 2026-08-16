using System;

namespace Game.CSEvent
{
    public class EventCS<T>
    {
        Action<T> listeners = null;
        public void Subscribe(Action<T> method) => listeners += method;
        public void Unsubscribe(Action<T> method) => listeners -= method;
        public void Raise(T args) { if (!Lock) listeners?.Invoke(args); }
        /// <summary>
        /// Whether this event is locked from <see cref="Raise"/>
        /// </summary>
        public bool Lock { get; set; }
    }

    public class EventCS
    {
        Action listeners = null;
        public void Subscribe(Action method) => listeners += method;
        public void Unsubscribe(Action method) => listeners -= method;
        public void Raise() { if (!Lock) listeners?.Invoke(); }
        /// <summary>
        /// Whether this event is locked from <see cref="Raise"/>
        /// </summary>
        public bool Lock { get; set; }
    }

}