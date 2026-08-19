
using Game.SO.Data.TextMarkup;
using System;
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

        [Serializable]
        public struct IndexedCommand
        {
            public int index;
            public TextMarkupCommand command;

            public IndexedCommand(int index, TextMarkupCommand command)
            {
                this.index = index;
                this.command = command;
            }
        }

        [Serializable]
        public struct IndexedEffect
        {
            public int index;
            public int endIndex;
            public TextMarkupEffect effect;

            public IndexedEffect(int index, TextMarkupEffect effect)
            {
                this.index = index;
                endIndex = this.index;
                this.effect = effect;
            }
        }



        static public string ErrorMsg { get; private set; }


        static public TextMarkupAudioPresetSO SpeechPresets { get; set; }
        static public TextMarkupAudioPresetSO SFXPresets { get; set; }


        static public bool CheckMarkup(string textStr, CHECK_TYPE checkType = CHECK_TYPE.ALL)
        {
            VALIDATION_STATE state = VALIDATION_STATE.GENERAL;
            VALIDATION_STATE prevState = state;

            Stack<string> effectStack = new();
            HashSet<string> usedParams = new();

            string perStateString = "";

            bool hasEndCommand = false;
            if (checkType == CHECK_TYPE.ONLY_EFFECT)
                hasEndCommand = true;
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
                ErrorMsg = $"markup contains format error at around \"...{errorText}...\"";
                return false;
            }

            if (!hasEndCommand)
            {
                ErrorMsg = "must contain a <end/> markup";
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
                    case "end":
                        markupType = TEXT_MARKUP_TYPE.COMMAND;

                        if (checkType == CHECK_TYPE.ONLY_EFFECT)
                            checkTypeError = true;

                        if (markupName == "end")
                            hasEndCommand = true;

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
                    case "end":
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
                            (markupParam == "frequency" && StringOperation.TryParseVector2(markupParamValue, out _)) ||
                            (markupParam == "amplitude" && StringOperation.TryParseVector2(markupParamValue, out _)) ||
                            (markupParam == "oscillateOffset" && StringOperation.TryParseVector2(markupParamValue, out _)) ||
                            (markupParam == "offset" && StringOperation.TryParseFloat(markupParamValue, out _));
                    case "rainbow":
                        return
                            (markupParam == "speed" && StringOperation.TryParseFloat(markupParamValue, out _)) ||
                            (markupParam == "offset" && StringOperation.TryParseFloat(markupParamValue, out _));
                }
            }
        }


        static public void ProccessMarkup(ref string textStr, out List<IndexedCommand> commandList, out List<IndexedEffect> effectList, out List<int> effectPopList)
        {
            commandList = new();
            commandList.Capacity = 8;
            effectList = new();
            effectList.Capacity = 8;
            effectPopList = new();
            effectPopList.Capacity = 8;


            string textStrCopy = textStr;
            textStr = "";
            int charatcerIndexOfMarkup = 0;

            VALIDATION_STATE state = VALIDATION_STATE.GENERAL;
            VALIDATION_STATE prevState = state;

            string perStateString = "";

            bool checkingMarkup = false;
            bool isCheckingEndMarkup = false;
            string markupString = "";

            string markupName = "";

            string markupParam = "";

            string markupParamValue = "";
            bool checkingParamValue = false;

            bool isCheckClosingMarkup = false;


            foreach (var char_ in textStrCopy)
            {
                perStateString += char_;

                if (checkingMarkup)
                    markupString += char_;

                switch (state)
                {
                    case VALIDATION_STATE.GENERAL:

                        if (char_ == '<')
                        {
                            state = VALIDATION_STATE.CHECK_MARKUP_NAME;
                            checkingMarkup = true;
                            isCheckingEndMarkup = false;

                            charatcerIndexOfMarkup = textStr.Length;

                            break;
                        }
                        else
                            textStr += char_;
                        
                        break;


                    case VALIDATION_STATE.CHECK_MARKUP_NAME:

                        if (isCheckClosingMarkup) // for commands
                        {
                            isCheckClosingMarkup = false;

                            AddToList(commandList, effectList, ref textStr);

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
                            AddToList(commandList, effectList, ref textStr);

                            state = VALIDATION_STATE.CHECK_MARKUP_PARAM;
                            break;
                        }

                        if (char_ == '>')
                        {

                            if (!isCheckingEndMarkup)
                                AddToList(commandList, effectList, ref textStr);
                            else
                                effectPopList.Add(charatcerIndexOfMarkup);

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


                        if (char_ == '=')
                        {
                            state = VALIDATION_STATE.CHECK_MARKUP_PARAM_VALUE;
                            break;
                        }

                        markupParam += char_;
                        break;


                    case VALIDATION_STATE.CHECK_MARKUP_PARAM_VALUE:

                        if (!checkingParamValue)
                        {
                            if (perStateString == "=\"")
                                checkingParamValue = true;
                            break;
                        }

                        if (char_ == '"')
                        {
                            checkingParamValue = false;

                            SetMarkupValue(commandList, effectList, ref textStr);

                            markupParamValue = "";
                            markupParam = "";
                            state = VALIDATION_STATE.CHECK_MARKUP_PARAM_PENDING;
                            break;
                        }

                        markupParamValue += char_;
                        break;


                    case VALIDATION_STATE.CHECK_MARKUP_PARAM_PENDING:

                        if (isCheckClosingMarkup) // for commands
                        {
                            isCheckClosingMarkup = false;
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
                            state = VALIDATION_STATE.CHECK_MARKUP_PARAM;
                            break;
                        }

                        if (char_ == '>')
                        {
                            ClearAndSetStateToGeneral();
                            break;
                        }

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


            void ClearAndSetStateToGeneral()
            {
                markupName = "";
                state = VALIDATION_STATE.GENERAL;
                checkingMarkup = false;
            }

            void AddToList(List<IndexedCommand> commandList, List<IndexedEffect> effectList, ref string textStr)
            {

                switch (markupName)
                {
                    case "br":
                        //commandList.Add(new IndexedCommand(charatcerIndexOfMarkup , new BrTextMarkupCommand()));
                        textStr += '\n';
                        return;
                    case "wait":
                        commandList.Add(new IndexedCommand(charatcerIndexOfMarkup , new WaitTextMarkupCommand()));
                        return;
                    case "interval":
                        commandList.Add(new IndexedCommand(charatcerIndexOfMarkup , new IntervalTextMarkupCommand()));
                        return;
                    case "input":
                        commandList.Add(new IndexedCommand(charatcerIndexOfMarkup , new InputTextMarkupCommand()));
                        return;
                    case "sfx":
                        commandList.Add(new IndexedCommand(charatcerIndexOfMarkup , new SFXTextMarkupCommand()));
                        return;
                    case "unmark":
                        //commandList.Add(new IndexedCommand(charatcerIndexOfMarkup , new UnmarkTextMarkupCommand()));
                        return;
                    case "end":
                        commandList.Add(new IndexedCommand(charatcerIndexOfMarkup , new EndTextMarkupCommand()));
                        return;

                    case "speech":
                        effectList.Add(new IndexedEffect(charatcerIndexOfMarkup , new SpeechTextMarkupEffect()));
                        return;
                    case "color":
                        effectList.Add(new IndexedEffect(charatcerIndexOfMarkup , new ColorTextMarkupEffect()));
                        return;
                    case "offset":
                        effectList.Add(new IndexedEffect(charatcerIndexOfMarkup , new OffsetTextMarkupEffect()));
                        return;
                    case "size":
                        effectList.Add(new IndexedEffect(charatcerIndexOfMarkup , new SizeTextMarkupEffect()));
                        return;
                    case "shake":
                        effectList.Add(new IndexedEffect(charatcerIndexOfMarkup , new ShakeTextMarkupEffect()));
                        return;
                    case "oscillate":
                        effectList.Add(new IndexedEffect(charatcerIndexOfMarkup , new OscillateTextMarkupEffect()));
                        return;
                    case "rainbow":
                        effectList.Add(new IndexedEffect(charatcerIndexOfMarkup , new RainbowTextMarkupEffect()));
                        return;

                    default:
                        return;
                }
            }

            void SetMarkupValue(List<IndexedCommand> commandList, List<IndexedEffect> effectList, ref string textStr)
            {
                bool notCommand = true;

                if (commandList.Count > 0)
                {
                    var indexedCommand = commandList[^1];
                    notCommand = false;

                    switch (markupName)
                    {
                        case "br":
                        case "end":
                        case "input":
                            break;
                        case "wait":
                            if (indexedCommand.command is WaitTextMarkupCommand command_wait)
                                if (markupParam == "time")
                                    StringOperation.TryParseFloat(markupParamValue, out command_wait.time);
                            break;
                        case "interval":
                            if (indexedCommand.command is IntervalTextMarkupCommand command_interval)
                                if (markupParam == "time")
                                    StringOperation.TryParseFloat(markupParamValue, out command_interval.time);
                            break;
                        case "sfx":
                            if (indexedCommand.command is SFXTextMarkupCommand command_sfx)
                                if (markupParam == "name")
                                    command_sfx.sfx = SFXPresets.TextMarkupAudioPresets[markupParamValue];
                            break;
                        case "unmark":
                            if (markupParam == "text")
                                textStr += markupParamValue;
                            break;
                        default:
                            notCommand = true;
                            break;
                    }

                    commandList[^1] = indexedCommand;
                }


                if (notCommand && effectList.Count > 0)
                {
                    var indexedEffect = effectList[^1];

                    switch (markupName)
                    {
                        case "speech":
                            if (indexedEffect.effect is SpeechTextMarkupEffect effect_speech)
                                if (markupParam == "name")
                                    effect_speech.speechSFX = SpeechPresets.TextMarkupAudioPresets[markupParamValue];
                            break;
                        case "color":
                            if (indexedEffect.effect is ColorTextMarkupEffect effect_color)
                            {
                                if (markupParam == "value")
                                {
                                    StringOperation.TryParseHexColor(markupParamValue, out effect_color.color);
                                    effect_color.fadeColor = effect_color.color;
                                }
                                else if (markupParam == "fadeValue")
                                {
                                    StringOperation.TryParseHexColor(markupParamValue, out effect_color.fadeColor);
                                }
                            }
                            break;
                        case "offset":
                            if (indexedEffect.effect is OffsetTextMarkupEffect effect_offset)
                                if (markupParam == "value")
                                    StringOperation.TryParseVector2(markupParamValue, out effect_offset.offset);
                            break;
                        case "size":
                            if (indexedEffect.effect is SizeTextMarkupEffect effect_size)
                                if (markupParam == "value")
                                    StringOperation.TryParseVector2(markupParamValue, out effect_size.size);
                            break;
                        case "shake":
                            if (indexedEffect.effect is ShakeTextMarkupEffect effect_shake)
                            {
                                if (markupParam == "maxNormalTime")
                                    StringOperation.TryParseFloat(markupParamValue, out effect_shake.maxNormalTime);
                                else if (markupParam == "persistTime")
                                    StringOperation.TryParseFloat(markupParamValue, out effect_shake.persistTime);
                                else if (markupParam == "offsetRange")
                                    StringOperation.TryParseVector2(markupParamValue, out effect_shake.offsetRange);
                            }
                            break;
                        case "oscillate":
                            if (indexedEffect.effect is OscillateTextMarkupEffect effect_oscillate)
                            {
                                if (markupParam == "frequency")
                                    StringOperation.TryParseVector2(markupParamValue, out effect_oscillate.frequency);
                                else if (markupParam == "amplitude")
                                    StringOperation.TryParseVector2(markupParamValue, out effect_oscillate.amplitude);
                                else if (markupParam == "oscillateOffset")
                                    StringOperation.TryParseVector2(markupParamValue, out effect_oscillate.oscillateOffset);
                                else if (markupParam == "offset")
                                {
                                    StringOperation.TryParseFloat(markupParamValue, out effect_oscillate.offset);
                                }
                            }
                            break;
                        case "rainbow":
                            if (indexedEffect.effect is RainbowTextMarkupEffect effect_rainbow)
                            {
                                if (markupParam == "speed")
                                    StringOperation.TryParseFloat(markupParamValue, out effect_rainbow.speed);
                                else if (markupParam == "offset")
                                {
                                    StringOperation.TryParseFloat(markupParamValue, out effect_rainbow.offset);
                                }
                            }
                            break;
                    }

                    effectList[^1] = indexedEffect;
                }

            }



        }
    }



    /*
     * markup definition:
     * 
     * | command markups
     * <br/> // new line
     * <wait time="1"/> // wait for amount of time
     * <interval time="0.2"/> // time between printing each character
     * <end/> // ends text scrolling
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
     * <oscillate frequency="1,1" amplitude="1,1" oscillateOffset="0,0" offset="0.1"></oscillate> // set oscillation, strength is multiplier on respective axis, offset is how much difference between oscillation per character passed
     * <rainbow speed="1" offset="0.1"></rainbow> // set rainbow effect, speed is how fast it changes color, offset is how much difference between color per character passed (as a percentage)
     * 
     * 
     * note:
     * markup should be read right after the character before it is printed
     * 
     * 
     */
}