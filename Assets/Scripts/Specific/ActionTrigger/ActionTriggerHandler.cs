
using Game.SO.ActionFn;
using UnityEngine;


namespace Game.TriggerHandler.MassTriggerHandler
{

    [RequireComponent(typeof(BoxCollider2D))]
    public class ActionTriggerHandler : MassTriggerHandler<ActionSO>
    {
        protected override void TriggerTriggerable(ref ActionSO triggerable)
        {
            triggerable.Invoke();
        }
    }
}