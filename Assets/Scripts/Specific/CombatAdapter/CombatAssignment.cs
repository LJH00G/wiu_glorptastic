using UnityEngine;
using System.Collections.Generic;
using System;
using Game.SO.EventChannel.Context;

namespace Game.Combat.Integration
{
    public class CombatAssigmment : MonoBehaviour
    {

        void Start()
        {
            Debug.Log($"trying to load data from tunnel");
            LoadDataAssignment();
        }

        [SerializeField] private List<Transform> transformList;
        [SerializeField] private CombatManager combatManager;
        [SerializeField] private CombatDataTunnelSO dataTunnel;

        [SerializeField] private CombatEndEventChannelSO combatEnd;
        public void LoadDataAssignment()
        {
            combatManager.enemyAnchors = transformList.ToArray();

            combatManager.enemyEncounter = dataTunnel.enemyEncounterData.dataList.ToArray();
            combatManager.playerLoadout = dataTunnel.playerLoadout;
            combatManager.partnerLoadout = dataTunnel.partnerLoadout;

            Debug.Log($"trying set up battle");
            combatManager.SetupBattle(
                dataTunnel.playerSprite,
                dataTunnel.partnerSprite,
                dataTunnel.enemySprites
            );
        }

        public void WipeDataAssignment(bool won)
        {
            dataTunnel.WipeCall();
            CombatEndEventContextSO context = new(won);
            combatEnd.Raise(context);
        }


    }
}


