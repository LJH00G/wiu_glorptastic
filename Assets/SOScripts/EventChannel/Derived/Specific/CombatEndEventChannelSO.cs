using Game.SO.EventChannel;
using Game.SO.EventChannel.Context;
using UnityEngine;
using System.Collections.Generic;
using Game.Combat;



namespace Game.SO.EventChannel.Context
{
    public class CombatEndEventContextSO
    {
        public CombatState state;
        public List<LootData> lootCollected;
        public int hp;
        public int curse;

        public CombatEndEventContextSO(CombatState state, List<LootData> loot, int hp, int curse)
        {
            this.state = state;
            this.lootCollected = loot;
            this.hp = hp;
            this.curse = curse;
        }
    }
}

[CreateAssetMenu(fileName = "CombatEndEvent_Channel", menuName = "Scriptable Objects/EventChannel/Specific/CombatEndEventChannelSO")]
public class CombatEndEventChannelSO : EventChannelSO<CombatEndEventContextSO>
{
    
}
