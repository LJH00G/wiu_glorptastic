
using System.Collections.Generic;
using UnityEngine;
using Utility.String;


namespace Game.TextMarkup
{
    public enum TEXT_MARKUP_TYPE : byte
    {
        COMMAND,
        EFFECT
    }

    public class TextMarkupOperation
    {
        enum VALIDATION_STATE : byte
        {
            GENERAL,

            CHECK_MARKUP_NAME,
            CHECK_MARKUP_PARAM,
            CHECK_MARKUP_PARAM_VALUE,
            CHECK_MARKUP_PARAM_PENDING
        }

        public enum CHECK_TYPE : byte
        {
            ALL,
            ONLY_COMMAND,
            ONLY_EFFECT
        }

        static public string ErrorMsg { get; private set; }

        static public bool CheckMarkup(string textStr, CHECK_TYPE checkType = CHECK_TYPE.ALL)
        {
            VALIDATION_STATE state = VALIDATION_STATE.GENERAL;
            VALIDATION_STATE prevState = state;

            Stack<string> effectStack = new();
            HashSet<string> usedParams = new();

            string perStateString = "";

            bool hasContinue = false;
            if (checkType == CHECK_TYPE.ONLY_EFFECT)
                hasContinue = true;
            TEXT_MARKUP_TYPE markupType = TEXT_MARKUP_TYPE.COMMAND;

            bool checkingMarkup = false;
            bool isCheckingEndMarkup = false;
            string markupString = "";

            string markupName = "";

            string markupParam = "";

            string markupParamValue = "";
            bool checkingParamValue = false;

            bool isCheckClosingMarkup = false;

            string errorText = "";
            bool formatError = false;
            bool checkTypeError = false;


            foreach (var char_ in textStr)
            {

                if (formatError || checkTypeError)
                {
                    errorText += char_;

                    if (errorText.Length >= 16) break;
                    else continue;
                }

                perStateString += char_;

                if (checkingMarkup)
                    markupString += char_;

                switch (state)
                {
                    case VALIDATION_STATE.GENERAL:

                        if (char_ == '>')
                        {
                            errorText += char_;
                            formatError = true;
                        }
                        if (char_ == '<')
                        {
                            state = VALIDATION_STATE.CHECK_MARKUP_NAME;
                            checkingMarkup = true;
                            isCheckingEndMarkup = false;
                        }
                        break;


                    case VALIDATION_STATE.CHECK_MARKUP_NAME:

                        errorText += char_;

                        if (isCheckClosingMarkup)
                        {
                            isCheckClosingMarkup = false;

                            if (char_ != '>')
                                formatError = true;
                            else if (!CheckMarkupFormat())
                                formatError = true;

                            ClearAndSetStateToGeneral();
                            break;
                        }

                        if (perStateString == "</")
                        {
                            isCheckingEndMarkup = true;
                            break;
                        }

                        if (char_ == ' ')
                        {
                            if (!CheckMarkupFormat())
                                formatError = true;

                            state = VALIDATION_STATE.CHECK_MARKUP_PARAM;
                            break;
                        }

                        if (char_ == '>')
                        {
                            if (!CheckMarkupFormat())
                                formatError = true;

                            if (markupType == TEXT_MARKUP_TYPE.EFFECT)
                            {
                                if (!isCheckingEndMarkup)
                                    effectStack.Push(markupName);
                                else if (effectStack.Count == 0)
                                    formatError = true;
                                else if (effectStack.Peek() == markupName)
                                    effectStack.Pop();
                                else
                                    formatError = true;
                            }
                            else
                                formatError = true;

                            ClearAndSetStateToGeneral();
                            break;
                        }

                        if (char_ == '/')
                        {
                            isCheckClosingMarkup = true;
                            break;
                        }

                        markupName += char_;
                        break;


                    case VALIDATION_STATE.CHECK_MARKUP_PARAM:

                        errorText += char_;

                        if (char_ == '=')
                        {
                            state = VALIDATION_STATE.CHECK_MARKUP_PARAM_VALUE;
                            break;
                        }

                        if (!char.IsLetter(char_))
                        {
                            formatError = true;
                            break;
                        }

                        markupParam += char_;
                        break;


                    case VALIDATION_STATE.CHECK_MARKUP_PARAM_VALUE:

                        errorText += char_;

                        if (!checkingParamValue)
                        {
                            if (perStateString == "=\"")
                                checkingParamValue = true;
                            break;
                        }

                        if (char_ == '"')
                        {
                            checkingParamValue = false;

                            if (!CheckMarkupParamValueFormat())
                                formatError = true;

                            if (!usedParams.Add(markupParam))
                                formatError = true;

                            markupParamValue = "";
                            markupParam = "";
                            state = VALIDATION_STATE.CHECK_MARKUP_PARAM_PENDING;
                            break;
                        }

                        markupParamValue += char_;
                        break;


                    case VALIDATION_STATE.CHECK_MARKUP_PARAM_PENDING:

                        if (isCheckClosingMarkup)
                        {
                            isCheckClosingMarkup = false;

                            if (char_ != '>')
                                formatError = true;
                            else if (!CheckMarkupFormat())
                                formatError = true;

                            ClearAndSetStateToGeneral();
                            break;
                        }

                        if (char_ == '/')
                        {
                            isCheckClosingMarkup = true;
                            break;
                        }

                        if (char_ == ' ')
                        {
                            if (!CheckMarkupFormat())
                                formatError = true;

                            state = VALIDATION_STATE.CHECK_MARKUP_PARAM;
                            break;
                        }

                        if (char_ == '>')
                        {
                            if (!CheckMarkupFormat())
                                formatError = true;

                            if (markupType == TEXT_MARKUP_TYPE.EFFECT)
                                effectStack.Push(markupName);
                            else
                                formatError = true;

                            if (isCheckingEndMarkup)
                                formatError = true;

                            ClearAndSetStateToGeneral();
                            break;
                        }

                        formatError = true;
                        break;
                }


                if (prevState != state)
                {
                    prevState = state;
                    perStateString = "";
                    perStateString += char_;

                    if (checkingMarkup)
                        markupString += char_;
                }

            }

            if (checkTypeError)
            {
                if (checkType == CHECK_TYPE.ONLY_COMMAND)
                    ErrorMsg = $"must not contain effect markup, found at around \"...{errorText}...\"";
                else
                    ErrorMsg = $"must not contain command markup, found at around \"...{errorText}...\"";
                return false;
            }

            if (formatError || state != VALIDATION_STATE.GENERAL)
            {
                Debug.Log($"formatError: {formatError}, state is general: {state != VALIDATION_STATE.GENERAL} ");
                ErrorMsg = $"markup contains format error at around \"...{errorText}...\"";
                return false;
            }

            if (!hasContinue)
            {
                ErrorMsg = "must contain a <continue/> markup";
                return false;
            }

            if (effectStack.Count != 0)
            {
                ErrorMsg = "must close all effect markups";
                return false;
            }

            return true;

            void ClearAndSetStateToGeneral()
            {
                markupName = errorText = "";
                state = VALIDATION_STATE.GENERAL;
                usedParams.Clear();
                checkingMarkup = false;
            }

            bool CheckMarkupFormat()
            {
                switch (markupName)
                {
                    case "br":
                    case "wait":
                    case "interval":
                    case "input":
                    case "sfx":
                    case "unmark":
                    case "continue":
                        markupType = TEXT_MARKUP_TYPE.COMMAND;

                        if (checkType == CHECK_TYPE.ONLY_EFFECT)
                            checkTypeError = true;

                        if (markupName == "continue")
                            hasContinue = true;

                        return true;

                    case "speech":
                    case "color":
                    case "offset":
                    case "size":
                    case "shake":
                    case "oscillate":
                    case "rainbow":
                        markupType = TEXT_MARKUP_TYPE.EFFECT;

                        if (checkType == CHECK_TYPE.ONLY_COMMAND)
                            checkTypeError = true;

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
                        return markupParam == "time" && StringOperation.TryParseFloat(markupParamValue, out _);
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


        static public List<TextMarkupCommand> ProccessMarkup(ref string textStr)
        {
            List<TextMarkupCommand> commandList;


        }
    }



    /*
     * markup definition:
     * 
     * | command markups
     * <br/> // new line
     * <wait time="1"/> // wait for amount of time
     * <interval time="0.2f"/> // time between printing each character
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