using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System;
using Game.Combat;

namespace Game.Combat.Integration
{
    public class CombatAssigmment : MonoBehaviour
    {

        void Start()
        {
            LoadDataAssignment(dataTunnel.enemyEncounterData.dataList.Count);
        }

        [SerializeField] private List<Transform> transformList;
        [SerializeField] private CombatManager combatManager;
        [SerializeField] private CombatDataTunnelSO dataTunnel;
        public void LoadDataAssignment(int count)
        {
            Array.Clear(combatManager.enemyAnchors, 0, combatManager.enemyAnchors.Length);

            for (int i = 0; i < count; i++)
            {
                combatManager.enemyAnchors.SetValue(transformList[i], i);
            }

            combatManager.enemyEncounter = dataTunnel.enemyEncounterData.dataList.ToArray();
            combatManager.playerLoadout = dataTunnel.playerLoadout;
            combatManager.partnerLoadout = dataTunnel.partnerLoadout;

            combatManager.SetupBattle();
        }

        public void WipeDataAssignment()
        {
            dataTunnel.WipeCall();
        }


    }
}

    

    
