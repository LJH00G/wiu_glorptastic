using Game.SO.Data.TextMarkup.Dialogue;
using Game.SO.EventChannel;
using UnityEngine;
using UnityEngine.InputSystem;

public class TestController : MonoBehaviour
{
    [Header("Event Broadcasting Channel")]
    [SerializeField] EventChannelSO dialogueInputEventChannel;
    [SerializeField] EventChannelSO skipDialogueScrollingEventChannel;
    [SerializeField] DialogueConversationEventChannelSO dialogueConversationEventChannel;

    [Header("Text Convo")]
    [SerializeField] DialogueConversationSO testConvo1;
    [SerializeField] DialogueConversationSO testConvo2;
    [SerializeField] DialogueConversationSO testConvo3;
    [SerializeField] DialogueConversationSO testConvo4;

    // Update is called once per frame
    void Update()
    {
        
        if (InputSystem.actions["NextDialogue"].triggered)
            dialogueInputEventChannel.Raise();
        if (InputSystem.actions["SkipTextScroll"].triggered)
            skipDialogueScrollingEventChannel.Raise();


        if (Keyboard.current[Key.Digit1].wasPressedThisFrame)
            dialogueConversationEventChannel.Raise(testConvo1);
        if (Keyboard.current[Key.Digit2].wasPressedThisFrame)
            dialogueConversationEventChannel.Raise(testConvo2);
        if (Keyboard.current[Key.Digit3].wasPressedThisFrame)
            dialogueConversationEventChannel.Raise(testConvo3);
        if (Keyboard.current[Key.Digit4].wasPressedThisFrame)
            dialogueConversationEventChannel.Raise(testConvo4);

    }
}
