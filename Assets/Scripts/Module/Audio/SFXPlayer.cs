using Game.SO.EventChannel.Context;
using Game.SO.EventChannel;

using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SFXPlayer : MonoBehaviour
{
    [Header("Event Listening Channel")]
    [SerializeField] PlaySFXEventChannelSO playSFXEventChannel;

    AudioSource SFXSource;

    void HandlePlaySFXEvent(PlaySFXEventContext context)
    {
        if (!context.playOrStop)
            SFXSource.Stop();
        else
        {
            if (context.playOneShot)
                SFXSource.PlayOneShot(context.SFX);
            else
            {
                SFXSource.clip = context.SFX;
                SFXSource.Play();
            }
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        SFXSource = GetComponent<AudioSource>();
    }


    private void OnEnable()
    {
        playSFXEventChannel.Subscribe(HandlePlaySFXEvent);
    }

    private void OnDisable()
    {
        playSFXEventChannel.Unsubscribe(HandlePlaySFXEvent);
    }
}
