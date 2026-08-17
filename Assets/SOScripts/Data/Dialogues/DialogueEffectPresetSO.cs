
using Utility.DictionaryEntry;
using UnityEngine;
using System.Collections.Generic;


namespace Game.SO.Data.Dialogue
{

    [CreateAssetMenu(fileName = "DialogueAudioPreset_Data", menuName = "Scriptable Objects/Data/Dialogue/DialogueAudioPresetSO")]
    public class DialogueAudioPresetSO : ScriptableObject
    {
        [SerializeField] List<DictEntry<string, AudioClip>> dialogueAudioPresets;

        public Dictionary<string, AudioClip> DialogueAudioPresets { get; private set; } = new();


#if UNITY_EDITOR

        private void OnValidate()
        {
            DialogueAudioPresets.Clear();
            foreach (var preset in dialogueAudioPresets)
            {
                DialogueAudioPresets.Add(preset.key, preset.value);
            }
        }
#endif

    }
}