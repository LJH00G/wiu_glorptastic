
using Utility.DictionaryEntry;
using UnityEngine;
using System.Collections.Generic;


namespace Game.SO.Data.TextMarkup
{

    [CreateAssetMenu(fileName = "TextMarkupAudioPreset_Data", menuName = "Scriptable Objects/Data/TextMarkup/TextMarkupAudioPresetSO")]
    public class TextMarkupAudioPresetSO : ScriptableObject
    {
        [SerializeField] List<DictEntry<string, AudioClip>> textMarkupAudioPresets;

        public Dictionary<string, AudioClip> TextMarkupAudioPresets { get; private set; } = new();


#if UNITY_EDITOR

        private void OnValidate()
        {
            TextMarkupAudioPresets.Clear();
            foreach (var preset in textMarkupAudioPresets)
            {
                TextMarkupAudioPresets.Add(preset.key, preset.value);
            }
        }
#endif

    }
}