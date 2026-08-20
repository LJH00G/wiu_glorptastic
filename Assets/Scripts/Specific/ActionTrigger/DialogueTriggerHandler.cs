
using Game.SO.Data.TextMarkup.Dialogue;
using Game.SO.EventChannel;
using System;
using UnityEngine;


namespace Game.TriggerHandler
{

    [Serializable]
    public class DialogueTriggerList : TriggerableList<DialogueConversationSO>
    {
        [HideInInspector] public DialogueConversationEventChannelSO eventChannel;
        protected override void Trigger(DialogueConversationSO triggerable)
        {
            eventChannel.Raise(triggerable);
        }
    }


    [RequireComponent(typeof(BoxCollider2D))]
    public class DialogueTriggerHandler : TriggerHandler<DialogueTriggerList, DialogueConversationSO>
    {
        [Header("Event Broadcasting Channel")]
        [SerializeField] DialogueConversationEventChannelSO dialogueConversationEventChannel;
        public override void InitTriggerList(DialogueTriggerList triggerlist)
        {
            if (!dialogueConversationEventChannel)
                Debug.Log("DialogueTriggerHandler.InitTriggerList() | dialogueConversationEventChannel must be filled in", this);

            triggerlist.eventChannel = dialogueConversationEventChannel;
        }
    }
}