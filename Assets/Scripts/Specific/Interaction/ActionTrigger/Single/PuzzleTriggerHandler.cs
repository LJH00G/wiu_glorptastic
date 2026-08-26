

using Game.SO.EventChannel;
using UnityEngine;


namespace Game.Interactable.TriggerHandler.Single
{

    [RequireComponent(typeof(BoxCollider2D))]
    public class PuzzleTriggerHandler : SingleTriggerHandler<PuzzleData>
    {
        [Header("Event Broadcasting Channel")]
        [SerializeField] PuzzleStartEventChannelSO tpEventChannel;

        protected override void TriggerTriggerable(ref PuzzleData triggerable)
        {
            tpEventChannel.Raise(triggerable);
        }


#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();

            if (!tpEventChannel)
                Debug.LogError("tpEventChannel must be filled in", this);
        }
#endif
    }
}