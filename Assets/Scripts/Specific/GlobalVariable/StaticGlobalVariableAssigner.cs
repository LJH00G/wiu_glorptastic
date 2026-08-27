using Game.Combat;
using Game.SO.Behaviour.EntityOverworld;
using Game.SO.EventChannel;
using UnityEngine;


namespace Game.GlobalVariable
{
    [DefaultExecutionOrder(-99999)]
    public class StaticGlobalVariableAssigner : MonoBehaviour
    {
        [SerializeField] PlayerLoadoutSO playerLoadout;
        [SerializeField] FollowObjectOverworldBehaviourSO followerBehaviour;
        [SerializeField] ToastEventChannelSO toastEventChannel;
        [SerializeField] GenerateParticleEventChannelSO generateParticleEventChannel;
        [SerializeField] PlaySFXEventChannelSO playSFXEventChannel;
        [SerializeField] PlayMusicEventChannelSO playMusicEventChannel;

        private void Awake()
        {
            StaticGlobalVariable.PlayerLoadout = playerLoadout;
            StaticGlobalVariable.FollowerBehaviour = followerBehaviour;
            StaticGlobalVariable.ToastEventChannel = toastEventChannel;
            StaticGlobalVariable.GenerateParticleEventChannel = generateParticleEventChannel;
            StaticGlobalVariable.PlaySFXEventChannel = playSFXEventChannel;
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
        static public ToastEventChannelSO ToastEventChannel { get; set; }
        static public PlaySFXEventChannelSO PlaySFXEventChannel { get; set; }
        static public GenerateParticleEventChannelSO GenerateParticleEventChannel { get; set; }
        static public PlayMusicEventChannelSO PlayMusicEventChannel { set; get; }
    }
}