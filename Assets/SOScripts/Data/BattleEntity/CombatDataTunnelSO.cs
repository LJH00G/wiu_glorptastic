using UnityEngine;
using Game.Combat;


[CreateAssetMenu(fileName = "CombatDataTunnelSO", menuName = "Scriptable Objects/CombatDataTunnelSO")]
public class CombatDataTunnelSO : ScriptableObject
{
    public PlayerLoadoutSO playerLoadout;
    public PartnerLoadoutSO partnerLoadout;
    public EnemyEncounterDataSO enemyEncounterData;

    public void WipeCall()
    {
        Destroy(playerLoadout);
        Destroy(partnerLoadout);
        enemyEncounterData = null;

    }
}
