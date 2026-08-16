using System;
using UnityEngine;

namespace Game.SO.ActionFn
{
    public abstract class ActionSO<P> : ScriptableObject
    {
        public abstract void Invoke(P param);
    }
    public abstract class ActionSO : ScriptableObject
    {
        public abstract void Invoke();
    }
}