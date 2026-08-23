using Game.Interactable.SingleTriggerHandler;
using UnityEngine;

public class EnemyEncounterTriggerHandler : SingleTriggerHandler<EnemyEncounterDataSO>
{
    [SerializeField] InitializeCombatEventChannelSO initializeCombatEventChannel;

    protected override void TriggerTriggerable(ref EnemyEncounterDataSO triggerable)
    {
        initializeCombatEventChannel.Raise(triggerable);
    }
}
