using Game.SO.EventChannel;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsController : MonoBehaviour
{
    [SerializeField] BoolEventChannelSO canPlaySFXEventChannel;
    [SerializeField] Toggle sfxToggle;

    [SerializeField] AudioMixer mixer;
    [SerializeField] Slider masterSlider;
    [SerializeField] Slider musicSlider;
    [SerializeField] Slider sfxSlider;


    public void SetMasterVolume()
    {
        mixer.SetFloat(
            "MasterVol",
            masterSlider.value <= 0f ?
            -80f : Mathf.Log10(masterSlider.value * 0.01f) * 20f
            );
    }

    public void SetMusicVolume()
    {
        mixer.SetFloat(
            "MusicVol",
            musicSlider.value <= 0f ?
            -80f : Mathf.Log10(musicSlider.value * 0.01f) * 20f
            );
    }

    public void SetSFXVolume()
    {
        mixer.SetFloat(
            "SFXVol",
            sfxSlider.value <= 0f ?
            -80f : Mathf.Log10(sfxSlider.value * 0.01f) * 20f
            );
    }

    public void ToggleSFX()
    {
        canPlaySFXEventChannel.Raise(sfxToggle.isOn);
    }
}
