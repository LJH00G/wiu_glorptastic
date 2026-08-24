
using Game.SO.Data.TextMarkup.Dialogue;
using Game.SO.EventChannel;
using UnityEngine;


namespace Game.Interactable.TriggerHandler.Mass
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class DialogueTriggerHandler : MassTriggerHandler<DialogueConversationSO>
    {
        [Header("Event Broadcasting Channel")]
        [SerializeField] DialogueConversationEventChannelSO dialogueConversationEventChannel;

        protected override void TriggerTriggerable(ref DialogueConversationSO triggerable)
        {
            dialogueConversationEventChannel.Raise(triggerable);
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();

            if (!dialogueConversationEventChannel)
                Debug.LogError("dialogueConversationEventChannel must be filled in", this);
        }
#endif
    }
}