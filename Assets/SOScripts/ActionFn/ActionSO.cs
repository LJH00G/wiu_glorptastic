using System;
using UnityEngine;

namespace Game.SO.ActionFn
{
    public abstract class ActionSO<T> : ScriptableObject
    {
        public abstract void Invoke(T param);
    }
    public abstract class ActionSO : ScriptableObject
    {
        public abstract void Invoke();
    }
}