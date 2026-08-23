
using UnityEngine;
using System.Collections.Generic;
using System;

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
        public void LoadDataAssignment()
        {
            combatManager.enemyAnchors = transformList.ToArray();

            combatManager.enemyEncounter = dataTunnel.enemyEncounterData.dataList.ToArray();
            combatManager.playerLoadout = dataTunnel.playerLoadout;
            combatManager.partnerLoadout = dataTunnel.partnerLoadout;

            Debug.Log($"trying set up battle");
            combatManager.SetupBattle();
        }

        public void WipeDataAssignment()
        {
            dataTunnel.WipeCall();
        }


    }
}

    

    
