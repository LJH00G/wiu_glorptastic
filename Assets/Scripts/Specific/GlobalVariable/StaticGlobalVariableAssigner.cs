using Game.Combat;
using Game.SO.Behaviour.EntityOverworld;
using UnityEngine;


namespace Game.GlobalVariable
{
    [DefaultExecutionOrder(-99999)]
    public class StaticGlobalVariableAssigner : MonoBehaviour
    {
        [SerializeField]
        PlayerLoadoutSO playerLoadout;
        [SerializeField]
        FollowObjectOverworldBehaviourSO followerBehaviour;

        private void Awake()
        {
            StaticGlobalVariable.PlayerLoadout = playerLoadout;
            StaticGlobalVariable.FollowerBehaviour = followerBehaviour;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            Awake();
        }
#endif
    }

    static public class StaticGlobalVariable
    {
        static public PlayerLoadoutSO PlayerLoadout { get; set; }
        static public FollowObjectOverworldBehaviourSO FollowerBehaviour { get; set; }
    }
}