using System;
using System.Collections.Generic;
using UnityEngine;
using Utility.String;


namespace Game.SO.Data.Dialogue
{

    [CreateAssetMenu(fileName = "DialogueSpeaker_Data", menuName = "Scriptable Objects/Data/Dialogue/DialogueSpeakerSO")]
    public class DialogueSpeakerSO : ScriptableObject
    {

        [field: SerializeField]
        public string Name { get; private set; }
        [field: SerializeField]
        public SpeechDialogueTextEffect DefaultSpeechSFX { get; private set; }
        [field: SerializeField]
        public ColorDialogueTextEffect DefaultColor { get; private set; }


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
            enum VALIDATION_STATE
            {
                GENERAL,
                
                CHECK_MARKUP_NAME,
                CHECK_MARKUP_PARAM,
                CHECK_MARKUP_PARAM_VALUE,
                CHECK_MARKUP_PARAM_PENDING,
                
                FORMAT_ERROR
            }
            
            VALIDATION_STATE state = VALIDATION_STATE.GENERAL;
            VALIDATION_STATE prevState = state;
            string perStateString = "";

            Stack<string> effectStack = new();
            for (int i = 0; i < Dialogues.Length; i++)
            {
                var dialogue = Dialogues[i];

                effectStack.Clear();


                bool hasContinue = false;
                bool markupIsCommandOrEffect = true;

                bool checkingMarkup = false;
                bool markupString = "";

                string markupName = "";
                bool checkMarkupName = false;
                bool tryCheckEndMarkup = false;
                bool checkEndMarkup = false;

                string markupParam = "";
                bool checkMarkupParam = false;

                string markupParamValue = "";
                bool checkMarkupParamValue = false;
                bool checkMarkupParamValue_firstQuote = false;

                bool checkMarkupParam_pending = false;

                string formatErrorText = "";
                bool formatError = false;


                foreach (var char_ in dialogue.text)
                {

                    if (prevState != state)
                    {
                        prevState = state;
                        perStateString = "";
                    }

                    perStateString += char_;

                    if (checkingMarkup)
                        markupString += char_;

                    switch (state)
                    {
                        case VALIDATION_STATE.GENERAL:


                        case VALIDATION_STATE.CHECK_MARKUP_NAME:


                        case VALIDATION_STATE.CHECK_MARKUP_PARAM:



                        case VALIDATION_STATE.CHECK_MARKUP_PARAM_VALUE:


                        case VALIDATION_STATE.CHECK_MARKUP_PARAM_PENDING:


                        case VALIDATION_STATE.FORMAT_ERROR:
                    }



                    // push char_ to formatErrorText so it can be printed
                    if (formatError)
                    {
                        formatErrorText += char_;

                        if (formatErrorText.Length >= 16) break;
                        else continue;
                    }

                    // markup name checking
                    if (checkMarkupName)
                    {
                        formatErrorText += char_;


                        if (tryCheckEndMarkup)
                        {
                            tryCheckEndMarkup = false;
                            if (char_ == '/')
                                checkEndMarkup = true;
                        }


                            if (char_ == ' ' || char_ == '/' || char_ == '>')
                        {
                            checkMarkupName = false;
                            if (!CheckMarkupFormat())
                                formatError = true;
                            else if (char_ == ' ')
                                checkMarkupParam = true;
                            else if (char_ == '/' && markupIsCommandOrEffect) // is a command markup without wrong ending syntax
                                markupName = formatErrorText = "";
                            else if (char_ == '>' && !markupIsCommandOrEffect) // is a effect markup without wrong ending syntax
                            {
                                effectStack.Push(markupName);
                                markupName = formatErrorText = "";
                                tryCheckEndMarkup = false;
                            }
                            else
                                formatError = true;

                            continue;
                        }

                        markupName += char_;
                        continue;
                    }

                    // markup param checking
                    if (checkMarkupParam)
                    {
                        formatErrorText += char_;

                        if (char_ == '=')
                        {
                            checkMarkupParam = false;
                            checkMarkupParamValue = true;

                            continue;
                        }

                        markupParam += char_;
                        continue;
                    }

                    // markup param value checking
                    if (checkMarkupParamValue)
                    {
                        formatErrorText += char_;

                        if (char_ == '"')
                        {
                            if (!checkMarkupParamValue_firstQuote)
                            {
                                checkMarkupParamValue_firstQuote = true;
                                continue;
                            }
                            else
                            {
                                checkMarkupParamValue = checkMarkupParamValue_firstQuote = false;
                                checkMarkupParam_pending = true;

                                if (!CheckMarkupParamValueFormat())
                                    formatError = true;

                                markupParamValue = "";
                                markupParam = "";
                                continue;
                            }
                        }

                        markupParamValue += char_;
                        continue;
                    }

                    // pending whether or not to check for markup param
                    if (checkMarkupParam_pending)
                    {
                        checkMarkupParam_pending = false;

                        if (char_ == ' ' || char_ == '/' || char_ == '>')
                        {
                            if (char_ == ' ')
                                checkMarkupParam = true;
                            else if (char_ == '/' && markupIsCommandOrEffect) // is a command markup without wrong ending syntax
                                markupName = formatErrorText = "";
                            else if (char_ == '>' && !markupIsCommandOrEffect) // is a effect markup without wrong ending syntax
                            {
                                effectStack.Push(markupName);
                                markupName = formatErrorText = "";
                                checkingMarkup = tryCheckEndMarkup = false;
                            }
                            else
                                formatError = true;

                            continue;
                        }
                        else
                            formatError = true;
                    }


                    if (char_ == '>')
                    {
                        if (checkingMarkup)
                        {
                            tryCheckEndMarkup = checkingMarkup = false;
                            markupName = "";
                        }
                        else
                        {
                            formatErrorText += char_;
                            formatError = true;
                        }
                    }

                    if (char_ == '<')
                    {
                        checkMarkupName = true;
                        tryCheckEndMarkup = true;
                        checkingMarkup = true;
                    }
                }


                if (state != VALIDATION_STATE.GENERAL)
                {
                    Debug.LogError($"DialogueSO.OnValidate() | Dialogues[{i}] markup contains format error at around \"...{formatErrorText}...\"", this);
                    return;
                }

                if (!hasContinue)
                {
                    Debug.LogError($"DialogueSO.OnValidate() | Dialogues[{i}] must contain a <continue/> markup", this);
                    return;
                }


                bool CheckMarkupFormat()
                {
                    // check if is command markup
                    markupIsCommandOrEffect = true;
                    switch (markupName)
                    {
                        case "br":
                        case "wait":
                        case "interval":
                        case "input":
                        case "sfx":
                        case "unmark":
                            return true;
                        case "continue":
                            hasContinue = true;
                            return true;
                    }

                    // check if is effect markup
                    markupIsCommandOrEffect = false;
                    switch (markupName)
                    {
                        case "speech":
                        case "color":
                        case "offset":
                        case "size":
                        case "shake":
                        case "oscillate":
                        case "rainbow":
                            return true;
                        default:
                            return false;
                    }
                }

                bool CheckMarkupParamValueFormat()
                {
                    switch (markupName)
                    {
                        case "br":
                        case "continue":
                        case "input":
                        default:
                            return false;
                        case "wait":
                            return
                                markupParam == "time" && StringOperation.TryParseFloat(markupParamValue, out _);
                        case "interval":
                            return markupParam == "value" && StringOperation.TryParseFloat(markupParamValue, out _);
                        case "sfx":
                            return markupParam == "name";
                        case "unmark":
                            return markupParam == "text";
                        case "speech":
                            return markupParam == "name";
                        case "color":
                            return
                                (markupParam == "value" && StringOperation.TryParseHexColor(markupParamValue, out _)) ||
                                (markupParam == "fadeValue" && StringOperation.TryParseHexColor(markupParamValue, out _));
                        case "offset":
                            return markupParam == "value" && StringOperation.TryParseVector2(markupParamValue, out _);
                        case "size":
                            return markupParam == "value" && StringOperation.TryParseVector2(markupParamValue, out _);
                        case "shake":
                            return
                                (markupParam == "maxNormalTime" && StringOperation.TryParseFloat(markupParamValue, out _)) ||
                                (markupParam == "persistTime" && StringOperation.TryParseFloat(markupParamValue, out _)) ||
                                (markupParam == "offsetRange" && StringOperation.TryParseVector2(markupParamValue, out _));
                        case "oscillate":
                            return
                                (markupParam == "strength" && StringOperation.TryParseVector2(markupParamValue, out _)) ||
                                (markupParam == "offset" && StringOperation.TryParseFloat(markupParamValue, out _));
                        case "rainbow":
                            return
                                (markupParam == "speed" && StringOperation.TryParseFloat(markupParamValue, out _)) ||
                                (markupParam == "offset" && StringOperation.TryParseFloat(markupParamValue, out _));
                    }
                }


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