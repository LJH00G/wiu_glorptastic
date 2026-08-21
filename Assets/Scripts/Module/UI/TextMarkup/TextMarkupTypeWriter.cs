using Game.SO.Data.TextMarkup;
using Game.SO.EventChannel;
using Game.TextMarkup;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class TextMarkupTypeWriter : MonoBehaviour
{
    [Header("Event Broadcasting Channel")]
    public PlaySFXEventChannelSO SFXEventChannel;
    public PlaySFXEventChannelSO speechSFXEventChannel;

    [Header("Preset Data")]
    [SerializeField] TextMarkupAudioPresetSO speechPresets;
    [SerializeField] TextMarkupAudioPresetSO sfxPresets;


    [field: Header("Info")]
    [field: SerializeField, DisplayOnly]
    public bool WaitForInput { get; set; }
    [field: SerializeField, DisplayOnly]
    public bool ReachedEnd { get; set; }
    [field: SerializeField, DisplayOnly]
    public float PrintInterval { get; set; }
    [SerializeField, DisplayOnly] float printTimer;
    [SerializeField, DisplayOnly] int currentCharacterIndex;
    [field: SerializeField, DisplayOnly]
    public bool Waiting { get; private set; }
    [field: SerializeField, DisplayOnly]
    public float WaitTime { get; set; } = 0;
    [SerializeField, DisplayOnly] float waitTimer;
    [SerializeField, DisplayOnly] bool canTypeWrite;
    [field: SerializeField, DisplayOnly]
    public bool SkipTextScrolling { get; set; }
    [field: SerializeField, DisplayOnly]
    public bool WasSkipTextScrolling { get; private set; }


    [Header("Dont Touch")]
    [SerializeField]
    List<TextMarkupEffect> defaultEffectStack = new();
    [SerializeField]
    List<TextMarkupEffect> effectStack = new();
    [SerializeField]
    List<CharacterData> charDataList = new();
    [SerializeField]
    List<TextMarkupOperation.IndexedCommand> commandList;


    TextMeshProUGUI tmpText;
    CharacterVertex[] workingVertices = new CharacterVertex[4];


    public void SetDefaultEffect(List<TextMarkupEffect> stack)
    {
        defaultEffectStack = stack;
    }

    public void ResetTypeWriting()
    {
        ReachedEnd = WaitForInput = Waiting = SkipTextScrolling = false;
        WaitTime = 0;
        PrintInterval = IntervalTextMarkupCommand.DEFAULT_INTERVAL;
        printTimer = 0f;
        currentCharacterIndex = -1;
        effectStack.Clear();

        tmpText.text = "";
        canTypeWrite = false;
    }

    public void StartNewTypeWriting(string text)
    {
        ResetTypeWriting();
        canTypeWrite = true;

        TextMarkupOperation.SFXPresets = sfxPresets;
        TextMarkupOperation.SpeechPresets = speechPresets;

        TextMarkupOperation.ProccessMarkup(ref text, out commandList, out List<TextMarkupOperation.IndexedEffect> effectList, out List<int> effectPopList);


        tmpText.text = text;


        tmpText.ForceMeshUpdate();
        TMP_TextInfo textInfo = tmpText.textInfo;
        
        charDataList.Clear();
        charDataList.Capacity = textInfo.characterCount;


        for (int i = 0; i < textInfo.characterCount; i++)
        {
            // handle effect pushing and poping
            for (int j = 0; j < effectList.Count;)
            {
                if (effectList[j].index != i)
                {
                    j++;
                    continue;
                }

                effectStack.Add(effectList[j].effect);
                effectList.RemoveAt(j);
            }

            for (int j = 0; j < effectPopList.Count;)
            {
                if (effectPopList[j] != i)
                {
                    j++;
                    continue;
                }

                effectStack.RemoveAt(effectStack.Count - 1);
                effectPopList.RemoveAt(j);
            }



            TMP_CharacterInfo charInfo = textInfo.characterInfo[i];

            if (!charInfo.isVisible) // for characters that dont show (space, \n, etc)
            {
                charDataList.Add(new CharacterData(false));
                continue;
            }

            int materialIndex = charInfo.materialReferenceIndex;
            int vertexIndex = charInfo.vertexIndex;

            ref readonly TMP_MeshInfo meshinfo = ref textInfo.meshInfo[materialIndex];

            // build vertex list
            CharacterVertex[] vertices = new CharacterVertex[4];
            for (int j = 0; j < vertices.Length; j++)
                vertices[j].Set(
                    meshinfo.vertices[vertexIndex + j],
                    meshinfo.colors32[vertexIndex + j]
                );



            // build effect list
            List<TextMarkupEffect> effects = new();
            foreach (var effect in defaultEffectStack)
                effects.Add(effect.Clone());
            foreach (var effect in effectStack)
                effects.Add(effect.Clone());

            
            bool foundSpeech = false;
            for (int j = effects.Count - 1; j >= 0; j--)
            {
                var effect = effects[j];

                // remove speech effect that are covered by the latest speech effect
                if (effect is SpeechTextMarkupEffect)
                {
                    if (foundSpeech)
                    {
                        effects.RemoveAt(j);
                        continue;
                    }

                    foundSpeech = true;
                }

                // make offset work
                if (effect is OffsetableTextMarkupEffect effect_offsetable)
                {
                    effect_offsetable.offset *= i;
                }

            }

            charDataList.Add(new CharacterData(materialIndex, vertexIndex, vertices, effects));
        }


        // skip type writing of first frame of characters that are within print interval of 0
        int startRange = 0, endRange = charDataList.Count;
        bool foundZeroInterval = false;

        for (int i = 0; i < commandList.Count; i++)
        {
            if (commandList[i].command is not IntervalTextMarkupCommand interval)
                continue;

            if (!foundZeroInterval)
            {
                if (interval.time == 0)
                {
                    startRange = commandList[i].index;
                    foundZeroInterval = true;
                }
            }
            else
            {
                if (interval.time != 0)
                {
                    endRange = commandList[i].index;
                    break;
                }
            }
        }

        if (foundZeroInterval) 
        {
            for (int i = startRange; i < endRange; i++)
            {
                var charData = charDataList[i];
                charData.show = true;
                charDataList[i] = charData;
            }
        }

    }


    private void Awake()
    {
        tmpText = GetComponent<TextMeshProUGUI>();
    }


    private void Update()
    {
        if (!canTypeWrite)
            return;

        float dt = Time.deltaTime;

        if (WaitTime > 0)
        {
            Waiting = true;
            waitTimer += dt;

            if (waitTimer >= WaitTime)
            {
                Waiting = false;
                waitTimer = WaitTime = 0;
            }
        }

        // text scrolling for printing and commands
        if ((SkipTextScrolling && !WaitForInput) || (!WaitForInput && !ReachedEnd && !Waiting))
        {
            printTimer += dt;

            bool breakCommand = false;

            while (
                SkipTextScrolling ?
                    !ReachedEnd :
                    (PrintInterval <= 0 ?
                        !ReachedEnd :
                        printTimer >= PrintInterval)
                )
            {
                currentCharacterIndex++;

                for (int i = 0; i < commandList.Count;)
                {
                    if (commandList[i].index != currentCharacterIndex)
                    {
                        i++;
                        continue;
                    }

                    commandList[i].command.TriggerCommand(this);
                    commandList.RemoveAt(i);

                    if (SkipTextScrolling)
                        WasSkipTextScrolling = true;
                    else
                        WasSkipTextScrolling = false;

                    if (WaitForInput || ReachedEnd || WaitTime > 0)
                    {
                        if (WaitForInput || ReachedEnd)
                            SkipTextScrolling = false;

                        currentCharacterIndex--;
                        breakCommand = true;
                        break;
                    }
                }

                if (breakCommand)
                    break;

                if (PrintInterval > 0)
                    printTimer -= PrintInterval;

                if (currentCharacterIndex < charDataList.Count)
                {
                    var charData = charDataList[currentCharacterIndex];
                    charData.show = true;
                    charDataList[currentCharacterIndex] = charData;
                }
            }
        }
        else
            printTimer = PrintInterval;

        // animation
        TMP_TextInfo textInfo = tmpText.textInfo;

        for (int i = 0; i < charDataList.Count; i++)
        {
            var charData = charDataList[i];

            if (!charData.isVisible)
                continue;

            charData.originalVertices.CopyTo(workingVertices, 0);

            foreach (var effect in charData.effects)
                effect.Update(dt);

            if (!charData.show)
            {
                for (int j = 0; j < workingVertices.Length; j++)
                {
                    var vertex = workingVertices[j];
                    vertex.color.a = 0;
                    workingVertices[j] = vertex;
                }
            }
            else
                foreach (var effect in charData.effects)
                    effect.ApplyEffect(this, ref workingVertices);

            // push changed vertex data to text mesh
            ref TMP_MeshInfo meshinfo = ref textInfo.meshInfo[charData.materialIndex];
            for (int j = 0; j < workingVertices.Length; j++)
            {
                var vertex = workingVertices[j];
                meshinfo.vertices[charData.vertexIndex + j] = vertex.position;
                meshinfo.colors32[charData.vertexIndex + j] = vertex.color;
            }

            charDataList[i] = charData;
        }

        tmpText.UpdateVertexData(TMP_VertexDataUpdateFlags.Vertices | TMP_VertexDataUpdateFlags.Colors32);

    }


}
