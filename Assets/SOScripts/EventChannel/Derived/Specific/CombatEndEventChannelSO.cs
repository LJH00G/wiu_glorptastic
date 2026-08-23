using Game.SO.EventChannel;
using Game.SO.EventChannel.Context;
using UnityEngine;




namespace Game.SO.EventChannel.Context
{
    public class CombatEndEventContextSO
    {
        public bool won;

        public CombatEndEventContextSO(bool won)
        {
            this.won = won;
        }
    }
}

[CreateAssetMenu(fileName = "CombatEndEvent_Channel", menuName = "Scriptable Objects/EventChannel/Specific/CombatEndEventChannelSO")]
public class CombatEndEventChannelSO : EventChannelSO<CombatEndEventContextSO>
{
    
}
