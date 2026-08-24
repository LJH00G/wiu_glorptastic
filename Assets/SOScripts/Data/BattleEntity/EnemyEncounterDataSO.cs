using NUnit.Framework;
using UnityEngine;
using Game.Combat;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Combat/Enemy Combat Encounter", fileName = "New Enemy Encounter")]
public class EnemyEncounterDataSO : ScriptableObject
{
    public GameObject enemy;
    public List<EnemyDataSO> dataList;
   
}
