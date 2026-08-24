using UnityEngine;
using Game.Combat;


[CreateAssetMenu(fileName = "CombatDataTunnelSO", menuName = "Scriptable Objects/CombatDataTunnelSO")]
public class CombatDataTunnelSO : ScriptableObject
{
    public PlayerLoadoutSO playerLoadout;
    public PartnerLoadoutSO partnerLoadout;
    public EnemyEncounterDataSO enemyEncounterData;

    public Sprite playerSprite;
    public Sprite partnerSprite;
    public Sprite[] enemySprites;

    public void WipeCall()
    {
        enemyEncounterData = null;

    }
}