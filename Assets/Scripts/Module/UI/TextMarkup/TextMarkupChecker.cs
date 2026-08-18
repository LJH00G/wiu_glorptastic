using Game.SO.Data.Dialogue;
using System.Collections.Generic;
using UnityEngine;
using Utility.String;

namespace Game.TextMarkup
{
    public class TextMarkupChecker
    {
        enum VALIDATION_STATE : byte
        {
            GENERAL,

            CHECK_MARKUP_NAME,
            CHECK_MARKUP_PARAM,
            CHECK_MARKUP_PARAM_VALUE,
            CHECK_MARKUP_PARAM_PENDING
        }



        static public bool CheckAllMarkup(string textStr, GameObject owner)
        {
            VALIDATION_STATE state = VALIDATION_STATE.GENERAL;
            VALIDATION_STATE prevState = state;

            Stack<string> effectStack = new();
            HashSet<string> usedParams = new();

            string perStateString = "";

            bool hasContinue = false;
            MARKUP_TYPE markupType = MARKUP_TYPE.COMMAND;

            bool checkingMarkup = false;
            bool isCheckingEndParkup = false;
            string markupString = "";

            string markupName = "";

            string markupParam = "";

            string markupParamValue = "";
            bool checkingParamValue = false;

            string formatErrorText = "";
            bool formatError = false;


            foreach (var char_ in textStr)
            {

                if (formatError)
                {
                    formatErrorText += char_;

                    if (formatErrorText.Length >= 16) break;
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
                            formatErrorText += char_;
                            formatError = true;
                        }
                        if (char_ == '<')
                        {
                            state = VALIDATION_STATE.CHECK_MARKUP_NAME;
                            checkingMarkup = true;
                            isCheckingEndParkup = false;
                        }
                        break;


                    case VALIDATION_STATE.CHECK_MARKUP_NAME:

                        formatErrorText += char_;

                        if (perStateString == "</")
                        {
                            isCheckingEndParkup = true;
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

                            if (markupType == MARKUP_TYPE.EFFECT)
                            {
                                if (!isCheckingEndParkup)
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

                        if (perStateString.Length >= 3 && perStateString[^3..] == "/>")
                        {
                            markupName = markupName[..^1];

                            if (!CheckMarkupFormat())
                                formatError = true;

                            ClearAndSetStateToGeneral();
                            break;
                        }

                        markupName += char_;
                        break;


                    case VALIDATION_STATE.CHECK_MARKUP_PARAM:

                        formatErrorText += char_;

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

                        formatErrorText += char_;

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

                            if (markupType == MARKUP_TYPE.EFFECT)
                                effectStack.Push(markupName);
                            else
                                formatError = true;

                            if (isCheckingEndParkup)
                                formatError = true;

                            ClearAndSetStateToGeneral();
                            break;
                        }

                        if (perStateString.Length >= 3 && perStateString[^3..] == "/>")
                        {

                            if (!CheckMarkupFormat())
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


            if (state != VALIDATION_STATE.GENERAL)
            {
                Debug.LogError($"DialogueSO.OnValidate() | Dialogues[{i}] markup contains format error at around \"...{formatErrorText}...\"", this);
                return false;
            }

            if (!hasContinue)
            {
                Debug.LogError($"DialogueSO.OnValidate() | Dialogues[{i}] must contain a <continue/> markup", this);
                return false;
            }

            if (effectStack.Count != 0)
            {
                Debug.LogError($"DialogueSO.OnValidate() | Dialogues[{i}] must close all effect markups", this);
                return false;
            }

            return true;

            void ClearAndSetStateToGeneral()
            {
                markupName = formatErrorText = "";
                state = VALIDATION_STATE.GENERAL;
                usedParams.Clear();
                checkingMarkup = false;
            }

            bool CheckMarkupFormat()
            {
                // check if is command markup
                markupType = MARKUP_TYPE.COMMAND;
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
                markupType = MARKUP_TYPE.EFFECT;
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
}