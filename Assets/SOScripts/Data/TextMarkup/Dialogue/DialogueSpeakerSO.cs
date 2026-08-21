using Game.TextMarkup;
using System;
using UnityEngine;


namespace Game.SO.Data.TextMarkup.Dialogue
{

    [CreateAssetMenu(fileName = "DialogueSpeaker_Data", menuName = "Scriptable Objects/Data/TextMarkup/Dialogue/DialogueSpeakerSO")]
    public class DialogueSpeakerSO : ScriptableObject
    {

        [field: SerializeField]
        public string Name { get; private set; }
        [Header("Default Speech")]
        [SerializeField] TextMarkupAudioPresetSO speechSFXPreset;
        [SerializeField] string speechSFXName;
        public SpeechTextMarkupEffect DefaultSpeechSFX { get; private set; } = new();
        [field: Header("Default Color")]
        [field: SerializeField]
        public ColorTextMarkupEffect DefaultTextColor { get; private set; }


#if UNITY_EDITOR

        private void OnValidate()
        {
            if (!TextMarkupOperation.CheckMarkup(Name, TextMarkupOperation.CHECK_TYPE.ONLY_EFFECT))
                Debug.LogError($"DialogueSpeakerSO.OnValidate() | error at Name: " + TextMarkupOperation.ErrorMsg, this);

            if (speechSFXPreset && speechSFXPreset.Presets.Count > 0)
                DefaultSpeechSFX.speechSFX = speechSFXPreset.Presets[speechSFXName];
        }
#endif

    }
}