using Game.SO.EventChannel.Context;
using Game.SO.EventChannel;

using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SFXPlayer : MonoBehaviour
{
    [Header("Event Listening Channel")]
    [SerializeField] PlaySFXEventChannelSO playSFXEventChannel;
    [SerializeField] BoolEventChannelSO canPlaySFXEventChannel;
    [SerializeField] bool canPlaySFX;

    AudioSource SFXSource;

    public void CanPlaySFX(bool value)
    {
        canPlaySFX = value;
    }

    void HandlePlaySFXEvent(PlaySFXEventContext context)
    {
        if (!canPlaySFX)
            return;

        if (!context.playOrStop)
        {
            SFXSource.Stop();
        }
        else
        {
            if (context.playOneShot)
            {
                SFXSource.PlayOneShot(context.SFX);
            }
            else
            {
                SFXSource.clip = context.SFX;
                SFXSource.Play();
            }
        }

        Debug.Log($"Handled {context}", this);
    }

    void Awake()
    {
        SFXSource = GetComponent<AudioSource>();
    }


    private void OnEnable()
    {
        playSFXEventChannel.Subscribe(HandlePlaySFXEvent);
        canPlaySFXEventChannel.Subscribe(CanPlaySFX);
    }

    private void OnDisable()
    {
        playSFXEventChannel.Unsubscribe(HandlePlaySFXEvent);
        canPlaySFXEventChannel.Unsubscribe(CanPlaySFX);
    }
}
