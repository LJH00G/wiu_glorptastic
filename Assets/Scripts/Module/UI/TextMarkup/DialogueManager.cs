using Game.SO.Data.TextMarkup.Dialogue;
using Game.SO.EventChannel;
using TMPro;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    [Header("Event Listening Channel")]
    [SerializeField] DialogueConversationEventChannelSO dialogueConversationEventChannel;

    [Header("Dialogue")]
    [SerializeField] TextMarkupTypeWriter speakerNameWriter;
    [SerializeField] TextMarkupTypeWriter textWriter;
    [SerializeField, DisplayOnly] DialogueConversationSO currentDialogueConversation;
    [SerializeField, DisplayOnly] bool playDialogue = false;

    void handleDialogueConversationEvent(DialogueConversationSO dialogueConversation)
    {
        currentDialogueConversation = dialogueConversation;
        playDialogue = true;
    }





    private void OnEnable()
    {
        dialogueConversationEventChannel.Subscribe(handleDialogueConversationEvent);
    }

    private void OnDisable()
    {
        dialogueConversationEventChannel.Unsubscribe(handleDialogueConversationEvent);
    }
}
