
using UnityEngine;

namespace Game.Interactable.TriggerHandler.Single
{
    public class EnemyEncounterTriggerHandler : SingleTriggerHandler<EnemyEncounterDataSO>
    {
        [SerializeField] InitializeCombatEventChannelSO initializeCombatEventChannel;

        protected override void TriggerTriggerable(ref EnemyEncounterDataSO triggerable)
        {
            triggerable.enemy = transform.parent.gameObject;
            initializeCombatEventChannel.Raise(triggerable);
        }
    }
}