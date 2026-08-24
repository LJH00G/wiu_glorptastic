using Game.Combat;
using Game.SO.Behaviour.EntityOverworld;
using UnityEngine;

namespace Game.SO.Data.Buddy
{
    [CreateAssetMenu(fileName = "BuddyData_Data", menuName = "Scriptable Objects/Data/Buddy/BuddyDataSO")]
    public class BuddyDataSO : ScriptableObject
    {
        [field: SerializeField]
        public PartnerLoadoutSO Loadout { get; private set; }

        [field: SerializeField]
        public FollowObjectOverworldBehaviourSO OverworldBehaviour { get; private set; }
    }
}