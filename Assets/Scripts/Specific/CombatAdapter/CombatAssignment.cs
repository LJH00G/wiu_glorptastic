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
            var EnemyDataList = dataTunnel.enemyEncounterData.dataList;

            combatManager.enemyAnchors = transformList.ToArray();

            combatManager.enemyEncounter = EnemyDataList.ToArray();
            combatManager.playerLoadout = dataTunnel.playerLoadout;
            combatManager.partnerLoadout = dataTunnel.partnerLoadout;

            Sprite[] enemySprites = new Sprite [EnemyDataList.Count];
            for (int i = 0; i < EnemyDataList.Count; i++)
                enemySprites[i] = EnemyDataList[i].sprite;

            Debug.Log($"trying set up battle");
            combatManager.SetupBattle(
                dataTunnel.playerLoadout.sprite,
                dataTunnel.partnerLoadout ? dataTunnel.partnerLoadout.sprite : null,
                enemySprites
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


