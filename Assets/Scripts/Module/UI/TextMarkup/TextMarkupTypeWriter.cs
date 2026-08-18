using Game.TextMarkup;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class TextMarkupTypeWriter : MonoBehaviour
{

    TextMeshProUGUI tmpText;

    [field: Header("Info")]
    [field: DisplayOnly]
    public bool WaitForInput { get; private set; }
    [field: DisplayOnly]
    public bool ReachedEnd { get; private set; }

    [Header("Dont Touch")]
    [SerializeField]
    List<TextMarkupEffect> effectStack = new();
    [SerializeField]
    List<CharacterData> charDataList = new();

    public void SetDefaultEffect(SpeechTextMarkupEffect speechEffect, ColorTextMarkupEffect colorEffect)
    {
        effectStack.Clear();
        effectStack.Add(speechEffect);
        effectStack.Add(colorEffect);
    }


    public void StartNewTypeWriting(string text)
    {
        tmpText.text = text;

        tmpText.ForceMeshUpdate();
        TMP_TextInfo textInfo = tmpText.textInfo;
        
        charDataList.Clear();
        charDataList.Capacity = textInfo.characterCount;

        for (int i = 0; i < textInfo.characterCount; i++)
        {
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

    }



    private void Awake()
    {
        tmpText = GetComponent<TextMeshProUGUI>();
    }


    private void Update()
    {
        float dt = Time.deltaTime;

        TMP_TextInfo textInfo = tmpText.textInfo;

        CharacterVertex[] vertices = new CharacterVertex[4];

        for (int i = 0; i < charDataList.Count; i++)
        {
            var charData = charDataList[i];
            charData.originalVertices.CopyTo(vertices, 0);

            foreach (var effect in charData.effects)
                effect.ApplyEffect(ref vertices);

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
