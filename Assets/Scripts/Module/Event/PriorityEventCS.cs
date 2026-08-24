using System;
using System.Collections.Generic;


namespace Game.CSEvent
{
    public struct PriorityListener<T>
    {
        public Action<T> method;
        public sbyte priority;

        public PriorityListener(Action<T> method, sbyte priority)
        {
            this.method = method;
            this.priority = priority;
        }
    }

    public struct PriorityListener
    {
        public Action method;
        public sbyte priority;

        public PriorityListener(Action method, sbyte priority)
        {
            this.method = method;
            this.priority = priority;
        }
    }


    /// <summary>
    /// callbacks raised will be sorted by priority in descending order
    /// </summary>
    public class PriorityEventCS<T>
    {

        List<PriorityListener<T>> listeners = new();

        void Sort()
        {
            listeners.Sort( // comparison lambda take in 2 listeners, then return a int, depending on the sign, it determines which listener comes first
                (a, b) => b.priority.CompareTo(a.priority) // ComapreTo returns a int which has its possitive sign indecating b is larger than a, negative sign is opposite, and 0 means both are the same
                );
        }
        public void Subscribe(Action<T> method, sbyte priority)
        {
            listeners.Add(new PriorityListener<T>(method, priority));
            Sort();
        }
        public void Unsubscribe(Action<T> method)
        {
            listeners.RemoveAll(
                listener => listener.method == method
                );
            Sort();
        }
        public void Raise(T args)
        {
            if (Lock)
                return;

            foreach (var listener in listeners)
                listener.method.Invoke(args);
        }

        /// <summary>
        /// Whether this event is locked from <see cref="Raise"/>
        /// </summary>
        public bool Lock { get; set; }

        public PriorityEventCS() { }
        public PriorityEventCS(Action<T> method, sbyte priority)
        {
            listeners.Add(new PriorityListener<T>(method, priority));
        }
        public PriorityEventCS(PriorityListener<T>[] startingListeners)
        {
            foreach (var listener in startingListeners)
                listeners.Add(listener);

            Sort();
        }
    }

    public class PriorityEventCS
    {

        List<PriorityListener> listeners = new();
        void Sort()
        {
            listeners.Sort( // comparison lambda take in 2 listeners, then return a int, depending on the sign, it determines which listener comes first
                (a, b) => b.priority.CompareTo(a.priority) // ComapreTo returns a int which has its possitive sign indecating b is larger than a, negative sign is opposite, and 0 means both are the same
                );
        }
        public void Subscribe(Action method, sbyte priority)
        {
            listeners.Add(new PriorityListener(method, priority));
            Sort();
        }
        public void Unsubscribe(Action method)
        {
            listeners.RemoveAll(
                listener => listener.method == method
                );
            Sort();
        }
        public void Raise()
        {
            if (Lock)
                return;

            foreach (var listener in listeners)
                listener.method.Invoke();
        }

        /// <summary>
        /// Whether this event is locked from <see cref="Raise"/>
        /// </summary>
        public bool Lock { get; set; }

        public PriorityEventCS() { }
        public PriorityEventCS(Action method, sbyte priority)
        {
            listeners.Add(new PriorityListener(method, priority));
        }
        public PriorityEventCS(PriorityListener[] startingListeners)
        {
            foreach (var listener in startingListeners)
                listeners.Add(listener);

            Sort();
        }
    }
}