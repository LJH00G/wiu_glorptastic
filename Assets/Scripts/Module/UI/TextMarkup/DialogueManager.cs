using Game;
using Game.SO.Data.TextMarkup.Dialogue;
using Game.SO.EventChannel;
using Game.TextMarkup;
using System.Collections.Generic;
using UnityEngine;
using Utility.Math;

public class DialogueManager : MonoBehaviour
{
    [Header("Event Listening Channel")]
    [SerializeField] DialogueConversationEventChannelSO dialogueConversationEventChannel;
    [SerializeField] EventChannelSO dialogueInputEventChannel;
    [SerializeField] EventChannelSO skipDialogueScrollingEventChannel;

    [Header("Dialogue")]
    [SerializeField] TextMarkupTypeWriter speakerNameWriter;
    [SerializeField] TextMarkupTypeWriter textWriter;
    [SerializeField, DisplayOnly] DialogueConversationSO currentConversation;
    [SerializeField, DisplayOnly] int dialogueIndex;
    [SerializeField, DisplayOnly] bool tryTriggerFirstDialogue;
    [SerializeField, DisplayOnly] bool hasOngoingConversation;

    [field: Header("Text Box")]
    [field: SerializeField]
    public bool Show { get; private set; }
    [SerializeField] float showPosY;
    [SerializeField] float hidePosY;
    [SerializeField] float animTime;
    float animTime_inv;
    [SerializeField] float animTimer;


    RectTransform rectForm;
    CanvasGroup cGroup;


    public void SetNewConversation(DialogueConversationSO dialogueConversation)
    {
        if (hasOngoingConversation)
            return;

        currentConversation = dialogueConversation;
        hasOngoingConversation = true;
        Show = true;
        animTimer = Mathf.Max(animTime - animTimer, 0);
        dialogueIndex = -1;
        tryTriggerFirstDialogue = true;

        GameManager.SetPlayerCanMove(false);
        GameManager.SetCanInteract(false);

        textWriter.ResetTypeWriting();
        speakerNameWriter.StartNewTypeWriting("<interval time=\"0\"/>" + currentConversation.Dialogues[0].speaker.Name + "<end/>");
    }


    public void TriggerDialogueInput()
    {
        textWriter.WaitForInput = false;
    }


    public void SkipDialogueScrolling()
    {
        textWriter.SkipTextScrolling = true;
    }


    bool TryAdvanceNextDialogue()
    {
        if (dialogueIndex >= 0)
        {
            var prevDialogueEndAction = currentConversation.Dialogues[dialogueIndex].endAction;
            if (prevDialogueEndAction)
                prevDialogueEndAction.Invoke();
        }


        dialogueIndex++;

        if (dialogueIndex >= currentConversation.Dialogues.Length)
            return false;

        var thisDialogue = currentConversation.Dialogues[dialogueIndex];

        speakerNameWriter.StartNewTypeWriting("<interval time=\"0\"/>" + thisDialogue.speaker.Name + "<end/>");

        
        List<TextMarkupEffect> defaultEffects = new()
        {
            thisDialogue.speaker.DefaultSpeechSFX,
            thisDialogue.speaker.DefaultTextColor
        };
        textWriter.SetDefaultEffect(defaultEffects);
        textWriter.StartNewTypeWriting(thisDialogue.text);
        return true;
    }


    void OnDialogueEnd()
    {
        hasOngoingConversation = false;
        Show = false;
        animTimer = Mathf.Max(animTime - animTimer, 0);

        GameManager.SetPlayerCanMove(true);
        GameManager.SetCanInteract(true);
    }


    private void Awake()
    {
        rectForm = GetComponent<RectTransform>();
        cGroup = GetComponent<CanvasGroup>();

        Show = false;

        var rectPos = rectForm.anchoredPosition;
        rectPos.y = hidePosY;
        rectForm.anchoredPosition = rectPos;

        cGroup.alpha = 0;

        animTimer = animTime;
    }


    private void Update()
    {

        if (animTimer < animTime)
        {
            animTimer += Time.deltaTime;

            var rectPos = rectForm.anchoredPosition;
            rectPos.y = Mathf.Lerp(
                !Show ? showPosY : hidePosY,
                Show ? showPosY : hidePosY,
                Math_Ease.Ease(EASE.IN_OUT_SIN, animTimer * animTime_inv)
                );
            rectForm.anchoredPosition = rectPos;

            cGroup.alpha = Mathf.Lerp(
                !Show ? 1 : 0,
                Show ? 1 : 0,
                Math_Ease.Ease(EASE.IN_OUT_SIN, animTimer * animTime_inv)
                );
        }
        else if (Show && tryTriggerFirstDialogue)
        {
            tryTriggerFirstDialogue = false;
            TryAdvanceNextDialogue();
        }


        if (!hasOngoingConversation)
            return;

        if (textWriter.ReachedEnd)
            if (!TryAdvanceNextDialogue())
                OnDialogueEnd();
    }


    private void OnEnable()
    {
        dialogueConversationEventChannel.Subscribe(SetNewConversation);
        dialogueInputEventChannel.Subscribe(TriggerDialogueInput);
        skipDialogueScrollingEventChannel.Subscribe(SkipDialogueScrolling);
    }

    private void OnDisable()
    {
        dialogueConversationEventChannel.Unsubscribe(SetNewConversation);
        dialogueInputEventChannel.Unsubscribe(TriggerDialogueInput);
        skipDialogueScrollingEventChannel.Unsubscribe(SkipDialogueScrolling);
    }



#if UNITY_EDITOR

    private void OnValidate()
    {
        rectForm = GetComponent<RectTransform>();
        cGroup = GetComponent<CanvasGroup>();

        if (animTime != 0)
            animTime_inv = 1 / animTime;

        var pos = rectForm.anchoredPosition;
        pos.y = Show ? showPosY : hidePosY;
        rectForm.anchoredPosition = pos;
    }
#endif

}
