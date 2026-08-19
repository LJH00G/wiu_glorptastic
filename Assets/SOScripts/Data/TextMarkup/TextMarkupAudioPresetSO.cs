
using Utility.VisualDictionary;
using UnityEngine;
using System.Collections.Generic;


namespace Game.SO.Data.TextMarkup
{

    [CreateAssetMenu(fileName = "TextMarkupAudioPreset_Data", menuName = "Scriptable Objects/Data/TextMarkup/TextMarkupAudioPresetSO")]
    public class TextMarkupAudioPresetSO : ScriptableObject
    {
        [field: SerializeField]
        public VisualDict<string, AudioClip> Presets { get; private set; } = new();


#if UNITY_EDITOR

        private void OnValidate()
        {
            Presets.OnValidate();
        }
#endif

    }
}