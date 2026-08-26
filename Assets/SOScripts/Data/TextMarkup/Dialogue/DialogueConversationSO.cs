using Game.SO.ActionFn;
using Game.TextMarkup;
using System;
using UnityEngine;


namespace Game.SO.Data.TextMarkup.Dialogue
{

    [CreateAssetMenu(fileName = "DialogueConversation_Data", menuName = "Scriptable Objects/Data/TextMarkup/Dialogue/DialogueConversationSO")]
    public class DialogueConversationSO : ScriptableObject
    {
        [Serializable]
        public struct DialoguePage
        {
            public DialogueSpeakerSO speaker;
            [TextArea] public string text;
            public ActionSO endAction;
        }

        [field: SerializeField]
        public DialoguePage[] Dialogues { get; private set; }


#if UNITY_EDITOR

        // scrap this into state machine
        private void OnValidate()
        {
            for (int i = 0; i < Dialogues.Length; i++)
            {
                var dialogue = Dialogues[i];

                if (!TextMarkupOperation.CheckMarkup(dialogue.text))
                    Debug.LogError($"DialogueSO.OnValidate() | error at Dialogues[{i}].text: " + TextMarkupOperation.ErrorMsg, this);

            }
        }
#endif

    }


    
}