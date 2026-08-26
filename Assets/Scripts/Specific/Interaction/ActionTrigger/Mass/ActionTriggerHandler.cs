
using Game.SO.ActionFn;
using UnityEngine;


namespace Game.Interactable.TriggerHandler.Mass
{

    [RequireComponent(typeof(BoxCollider2D))]
    public class ActionTriggerHandler : MassTriggerHandler<ActionSO>
    {
        protected override void TriggerType(ref ActionSO type)
        {
            type.Invoke();
        }
    }
}