
using UnityEngine;
using Utility.VisualizableDictionary;


namespace Game.SO.Data.TextMarkup
{

    [CreateAssetMenu(fileName = "TextMarkupAudioPreset_Data", menuName = "Scriptable Objects/Data/TextMarkup/TextMarkupAudioPresetSO")]
    public class TextMarkupAudioPresetSO : ScriptableObject
    {
        [field: SerializeField]
        public VisualizableDict<string, AudioClip> Presets { get; private set; } = new();


#if UNITY_EDITOR

        private void OnValidate()
        {
            Presets.OnValidate();
        }
#endif

    }
}