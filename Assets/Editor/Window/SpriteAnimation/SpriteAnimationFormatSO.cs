using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SpriteAnimationFormat_EditorSpriteAnimationFormat", menuName = "Scriptable Objects/Editor/SpriteAnimation/SpriteAnimationFormatSO")]
public class SpriteAnimationFormatSO : ScriptableObject
{
    [Serializable]
    public struct KeyDefinition
    {
        public int spriteIndex;
        public int frames;
    }

    [field: SerializeField]
    public string AnimationSuffix { get; private set; }
    [field: SerializeField]
    public float FrameRate { get; private set; } = 30;
    [field: SerializeField]
    public bool Loop { get; private set; }
    [field: SerializeField]
    public List<KeyDefinition> KeyDefinitions { get; private set; } = new();
    [field: SerializeField, Tooltip("whether there is any entries determines whether the generator will generate repetitively")]
    public List<string> RepetitionSuffixes { get; private set; } = new();
    [field: SerializeField]
    int RepetitionSpriteIndexGapSpacing { get; set; }
    [field: SerializeField, DisplayOnly, Tooltip("repeated generation will use sprites offset with this amount, offset is determined by the highest index used in KeyDefinitions + RepetitionSpriteIndexGapSpacing")]
    public int RepetitionSpriteIndexActualOffset { get; private set; }



    public int TotalKeyFrameCount()
    {
        int total = 0;
        foreach (var key in KeyDefinitions)
            total += key.frames;
        return total;
    }

    public int TotalSpritesRequired()
    {
        int suffixCount = RepetitionSuffixes.Count;
        if (suffixCount == 0)
            return RepetitionSpriteIndexActualOffset;
        else
            return RepetitionSpriteIndexActualOffset * suffixCount;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (FrameRate <= 0)
            Debug.LogError("FrameRate must be more than 0", this);


        int highestSpriteIndex = 0;

        for (int i = 0; i < KeyDefinitions.Count; i++)
        {
            var keyDefinition = KeyDefinitions[i];
            if (keyDefinition.frames <= 0)
                Debug.LogError($"KeyDefinitions[{i}].frames must be more than 0", this);

            if (keyDefinition.spriteIndex > highestSpriteIndex)
                highestSpriteIndex = keyDefinition.spriteIndex;
        }

        RepetitionSpriteIndexActualOffset = highestSpriteIndex + 1 + RepetitionSpriteIndexGapSpacing;
    }
#endif
}
