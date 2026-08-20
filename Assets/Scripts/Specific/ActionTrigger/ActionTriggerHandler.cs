
using Game.SO.ActionFn;
using System;
using UnityEngine;


namespace Game.TriggerHandler
{

    [Serializable]
    public class ActionTriggerList : TriggerableList<ActionSO>
    {
        protected override void Trigger(ActionSO triggerable)
        {
            triggerable.Invoke();
        }
    }


    [RequireComponent(typeof(BoxCollider2D))]
    public class ActionTriggerHandler : TriggerHandler<ActionTriggerList, ActionSO>
    {
        
    }
}