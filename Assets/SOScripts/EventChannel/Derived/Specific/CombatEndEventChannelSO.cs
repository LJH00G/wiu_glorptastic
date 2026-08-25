using Game.SO.EventChannel;
using Game.SO.EventChannel.Context;
using UnityEngine;
using System.Collections.Generic;
using Game.Combat;



namespace Game.SO.EventChannel.Context
{
    public class CombatEndEventContextSO
    {
        public bool won;
        public List<LootData> lootCollected;

        public CombatEndEventContextSO(bool won, List<LootData> loot)
        {
            this.won = won;
            this.lootCollected = loot;
        }
    }
}

[CreateAssetMenu(fileName = "CombatEndEvent_Channel", menuName = "Scriptable Objects/EventChannel/Specific/CombatEndEventChannelSO")]
public class CombatEndEventChannelSO : EventChannelSO<CombatEndEventContextSO>
{
    
}
