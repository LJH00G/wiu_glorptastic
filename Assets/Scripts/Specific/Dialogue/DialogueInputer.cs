using Game;
using Game.SO.EventChannel;
using UnityEngine;
using UnityEngine.InputSystem;

public class DialogueInputer : MonoBehaviour
{
    [SerializeField] EventChannelSO dialogueInputEventChannel;
    [SerializeField] EventChannelSO skipDialogueScrollingEventChannel;

    private void Update()
    {
        if (GameManager.GameState == GAME_STATE.OVERWORLD && GameManager.OverworldState == OVERWORLD_STATE.GENERAL)
        {
            if (InputSystem.actions["NextDialogue"].triggered)
                dialogueInputEventChannel.Raise();
            if (InputSystem.actions["SkipTextScroll"].triggered)
                skipDialogueScrollingEventChannel.Raise();
        }
        
    }
}
