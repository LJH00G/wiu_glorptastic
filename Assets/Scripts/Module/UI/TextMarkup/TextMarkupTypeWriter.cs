using Game.SO.Data.TextMarkup;
using Game.SO.EventChannel;
using Game.TextMarkup;
using System.Collections.Generic;
using TMPro;
using UnityEditor.UI;
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
    [field: DisplayOnly]
    public bool WaitForInput { get; private set; }
    [field: DisplayOnly]
    public bool ReachedEnd { get; private set; }
    [SerializeField, DisplayOnly] float printInterval, printTimer;
    [SerializeField, DisplayOnly] int currentCharatcerIndex;


    [Header("Dont Touch")]
    [SerializeField]
    List<TextMarkupEffect> effectStack = new();
    [SerializeField]
    List<CharacterData> charDataList = new();
    [SerializeField]
    List<TextMarkupOperation.IndexedCommand> commandList;


    TextMeshProUGUI tmpText;



    public void SetDefaultEffect(SpeechTextMarkupEffect speechEffect, ColorTextMarkupEffect colorEffect)
    {
        effectStack.Clear();
        effectStack.Add(speechEffect);
        effectStack.Add(colorEffect);
    }


    public void StartNewTypeWriting(string text)
    {

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
            for (int j = 0; j < effectList.Count; j++)
            {
                var indexedEffect = effectList[j];

                if (indexedEffect.index == i)
                {
                    effectStack.Add(indexedEffect.effect);
                    effectList.RemoveAt(j);
                    break;
                }
            }

            for (int j = 0; j < effectPopList.Count; j++)
            {
                if (effectPopList[j] == i)
                {
                    effectStack.RemoveAt(effectStack.Count - 1);
                    effectList.RemoveAt(j);
                    break;
                }
            }

            TMP_CharacterInfo charInfo = textInfo.characterInfo[i];

            int materialIndex = charInfo.materialReferenceIndex;
            int vertexIndex = charInfo.vertexIndex;

            ref readonly TMP_MeshInfo meshinfo = ref textInfo.meshInfo[materialIndex];

            CharacterVertex[] vertices = new CharacterVertex[4];

            for (int j = 0; j < vertices.Length; j++)
                vertices[j].Set(
                    meshinfo.vertices[vertexIndex + j],
                    meshinfo.colors32[vertexIndex + j]
                    );

            charDataList.Add(new CharacterData(materialIndex, vertexIndex, vertices, effectStack));
        }

        currentCharatcerIndex = -1;
    }


    private void Awake()
    {
        tmpText = GetComponent<TextMeshProUGUI>();
    }


    private void Update()
    {
        if (ReachedEnd)
            return;

        float dt = Time.deltaTime;

        printTimer += dt;
        if (printTimer >= printInterval)
        {
            printTimer -= printInterval;
            currentCharatcerIndex++;

            var charData = charDataList[currentCharatcerIndex];
            charData.show = true;
            charDataList[currentCharatcerIndex] = charData;
        }


        // animation
        TMP_TextInfo textInfo = tmpText.textInfo;

        CharacterVertex[] vertices = new CharacterVertex[4];

        for (int i = 0; i < charDataList.Count; i++)
        {
            for (int j = 0; j < commandList.Count; j++)
            {
                var indexedCommand = commandList[j];

                if (indexedCommand.index == i)
                {
                    indexedCommand.command.TriggerCommand(this);
                    commandList.RemoveAt(j);
                    break;
                }
            }

            var charData = charDataList[i];
            charData.originalVertices.CopyTo(vertices, 0);

            if (!charData.show)
            {
                for (int j = 0; j < vertices.Length; j++)
                {
                    var vertex = vertices[j];
                    vertex.color = (Color)new Vector4(1, 1, 1, 0);
                    vertices[j] = vertex;
                }
            }
            else
                foreach (var effect in charData.effects)
                    effect.ApplyEffect(this, ref vertices, dt);

            // push changed vertices data to text mesh
            ref TMP_MeshInfo meshinfo = ref textInfo.meshInfo[charData.materialIndex];
            for (int j = 0; j < vertices.Length; j++)
            {
                var vertex = vertices[j];
                meshinfo.vertices[charData.vertexIndex + j] = vertex.position;
                meshinfo.colors32[charData.vertexIndex + j] = vertex.color;
            }

            charDataList[i] = charData;
        }

        tmpText.UpdateVertexData(TMP_VertexDataUpdateFlags.Vertices | TMP_VertexDataUpdateFlags.Colors32);

    }


}
