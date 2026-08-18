using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Utility.String;


namespace Game.SO.Data.Dialogue
{

    enum MARKUP_TYPE : byte
    {
        COMMAND,
        EFFECT
    }

    [CreateAssetMenu(fileName = "DialogueSpeaker_Data", menuName = "Scriptable Objects/Data/Dialogue/DialogueSpeakerSO")]
    public class DialogueSpeakerSO : ScriptableObject
    {

        [field: SerializeField]
        public string Name { get; private set; }
        [field: SerializeField]
        public SpeechTextMarkupEffect DefaultSpeechSFX { get; private set; }
        [field: SerializeField]
        public ColorTextMarkupEffect DefaultColor { get; private set; }


#if UNITY_EDITOR

        private void OnValidate()
        {

        }
#endif

    }


    [CreateAssetMenu(fileName = "Dialogue_Data", menuName = "Scriptable Objects/Data/Dialogue/DialogueSO")]
    public class DialogueSO : ScriptableObject
    {
        [Serializable]
        public struct DialoguePage
        {
            public DialogueSpeakerSO speaker;
            public string text;
        }

        [field: SerializeField]
        public DialoguePage[] Dialogues { get; private set; }


#if UNITY_EDITOR

        


        // scrap this into state machine
        private void OnValidate()
        {

            for (int i = 0; i < Dialogues.Length; i++)
            {
                

            }
        }
#endif

    }


    /*
     * markup definition:
     * 
     * | command markups
     * <br/> // new line
     * <wait time="1"/> // wait for amount of time
     * <interval value="0.5"/> // time between printing each character
     * <continue/> // page break
     * <input/> // wait for player input to continue
     * <sfx name="name"/> // play a sfx
     * <unmark text="</>"/> // whatever between " will not be checked for marking
     * 
     * | effect markups
     * <speech name="name"></speech> // change speech sound
     * <color value="color" fadeValue="color"></color> // set color, fadeValue sets te bottom vertice color
     * <offset value="1,1"></offset> // sets the offset (this is technically size multiplier)
     * <size value="1,1"></size> // sets the size (this is technically size multiplier)
     * <shake maxNormalTime="1" persistTime="0.1" offsetRange="1,1"></shake> // set shake, maxNormalTime is max time for shake to no appear, persistTime is how long the offset persist, offsetRange is how much away is shaked from original position (as a multiplier)
     * <oscillate Strength="1,1" offset="1"></oscillate> // set oscillation, Strength is multiplier on respective axis, offset is how much difference between oscillation per character passed
     * <rainbow speed="1" offset="0.1"></rainbow> // set rainbow effect, speed is how fast it changes color, offset is how much difference between color per character passed (as a percentage)
     * 
     * 
     * note:
     * markup should be read right after the character before it is printed
     * 
     * 
     */
}