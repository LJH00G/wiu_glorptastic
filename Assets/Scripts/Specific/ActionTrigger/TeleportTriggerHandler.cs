

using Game.SO.EventChannel;
using UnityEngine;


namespace Game.TriggerHandler.SingleTriggerHandler
{

    [RequireComponent(typeof(BoxCollider2D))]
    public class TeleportTriggerHandler : SingleTriggerHandler<string>
    {
        [Header("Event Broadcasting Channel")]
        [SerializeField] StringEventChannelSO tpEventChannel;

        protected override void TriggerTriggerable(ref string triggerable)
        {
            tpEventChannel.Raise(triggerable);
        }


#if UNITY_EDITOR
        protected override void OnValidate_Editor()
        {
            if (!tpEventChannel)
                Debug.LogError("tpEventChannel must be filled in", this);
        }
#endif
    }
}