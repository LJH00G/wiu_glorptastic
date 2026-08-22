using Game.Interactable.SingleTriggerHandler;
using UnityEngine;

public class EnemyEncounterTriggerHandler : SingleTriggerHandler<EnemyEncounterDataSO>
{
    [SerializeField] CombatInitialiser combatInitialiser;

    protected override void TriggerTriggerable(ref EnemyEncounterDataSO triggerable)
    {
        combatInitialiser.StartCombat(triggerable);
    }
}
