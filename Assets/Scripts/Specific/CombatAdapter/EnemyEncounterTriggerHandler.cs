
using UnityEngine;

namespace Game.Interactable.TriggerHandler.Single
{
    public class EnemyEncounterTriggerHandler : SingleTriggerHandler<EnemyEncounterDataSO>
    {
        [SerializeField] InitializeCombatEventChannelSO initializeCombatEventChannel;

        protected override void TriggerType(ref EnemyEncounterDataSO type)
        {
            type.overworldPresetationObject = transform.parent.gameObject;
            initializeCombatEventChannel.Raise(type);
        }
    }
}