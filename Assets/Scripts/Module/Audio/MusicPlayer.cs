using Game.SO.EventChannel.Context;
using Game.SO.EventChannel.Derived;
using System;
using UnityEngine;
using Utility.Math;

public class MusicPlayer : MonoBehaviour
{
    [Serializable]
    public struct AudioSourceInfo
    {
        [SerializeField] public AudioSource source;
        [SerializeField] public bool allowSound, fade;
        [SerializeField] public float fadeTime, fadeTime_inv, fadeTimer;

        public AudioSourceInfo(AudioSource source)
        {
            this.source = source;
            allowSound = fade = false;
            fadeTime = fadeTime_inv = fadeTimer = 0;
        }

        public void StopAndClear()
        {
            if (source)
                source.Pause();

            source.clip = null;
            fade = allowSound = false;
            fadeTime = fadeTime_inv = fadeTimer = 0;
        }

        public void SetFade(float time, bool fadeInOrOut, bool forceSetVolume)
        {
            fade = true;
            allowSound = fadeInOrOut;
            fadeTime = time;
            fadeTime_inv = 1 / fadeTime;

            Debug.Log($"forceSetVolume: {forceSetVolume}");
            if (forceSetVolume)
            {
                source.volume = allowSound ? 0 : 1;
            }
        }

        public bool HasFaded()
        {
            return allowSound ?
                fadeTimer >= fadeTime :
                fadeTimer <= 0;
        }
    }


    [Header("Event Listening Channel")]
    [SerializeField] PlayMusicEventChannelSO playMusicChannel;

    [Header("Event Broadcasting Channel")]
    [SerializeField] DelayedCallbackEventChannelSO delayedCallbackEventChannel;

    [Header("Music")]
    [SerializeField] AudioSourceInfo[] sourceInfos;

    void HandlePlayMusicEvent(PlayMusicEventContext context)
    {
        AudioSourceInfo sourceInfoWithMusic = new(null), freeSourceInfo = new(null);
        int? sourceInfoWithMusic_index = null, freeSourceInfo_index = null;

        for (int i = 0; i < sourceInfos.Length; i++)
        {
            var sourceInfo = sourceInfos[i];

            if (context.music)
            {
                if (!sourceInfoWithMusic_index.HasValue && sourceInfo.source.clip == context.music)
                {
                    sourceInfoWithMusic_index = i;
                    sourceInfoWithMusic = sourceInfo;
                }
                else if (!freeSourceInfo_index.HasValue && !sourceInfo.source.clip)
                {
                    freeSourceInfo_index = i;
                    freeSourceInfo = sourceInfo;
                }
            }

            if (sourceInfo.source.clip && sourceInfo.source.clip != context.music)
            {
                if (context.turnOffOther)
                {
                    sourceInfo.StopAndClear();

                }
                else if (context.fadeOffOther)
                {
                    sourceInfo.SetFade(context.fadeOffOther_Time, false, context.fadeForceSetVolume);
                }

                sourceInfos[i] = sourceInfo;
            }
        }

        if (context.music)
        {
            if (context.playOrPause) // play music
            {

                void TestPlayMusic()
                {
                    ref AudioSourceInfo usingSourceInfo = ref sourceInfoWithMusic;
                    if (!usingSourceInfo.source)
                        usingSourceInfo = ref freeSourceInfo;

                    if (context.restartIfPlay || !usingSourceInfo.source.isPlaying)
                    {
                        usingSourceInfo.source.clip = context.music;
                        usingSourceInfo.source.Play();

                        usingSourceInfo.SetFade(context.fadeThis_Time, true, context.fadeForceSetVolume);

                        if (sourceInfoWithMusic_index.HasValue)
                            sourceInfos[sourceInfoWithMusic_index.Value] = sourceInfoWithMusic;
                        if (freeSourceInfo_index.HasValue)
                            sourceInfos[freeSourceInfo_index.Value] = freeSourceInfo;
                    }
                }


                if (sourceInfoWithMusic.source || freeSourceInfo_index.HasValue)
                {
                    if (context.delayToPlay >= 0)
                        delayedCallbackEventChannel.Raise(new DelayedCallbackEventContext(
                            TestPlayMusic,
                            context.delayToPlay
                            ));
                    else
                        TestPlayMusic();
                }

            }
            else // pause music
            {

                if (sourceInfoWithMusic.source)
                {
                    if (context.fadeThis)
                    {
                        sourceInfoWithMusic.SetFade(context.fadeThis_Time, false, context.fadeForceSetVolume);
                    }
                    else
                    {
                        sourceInfoWithMusic.StopAndClear();
                    }

                    sourceInfos[sourceInfoWithMusic_index.Value] = sourceInfoWithMusic;
                }
            }
        }

        Debug.Log($"MusicPlayer.HandlePlayMusicEvent() | handled {context}");
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float dt = Time.unscaledDeltaTime;

        for (int i = 0; i < sourceInfos.Length; i++)
        {
            var sourceInfo = sourceInfos[i];

            if (sourceInfo.fadeTime == 0)
                continue;
            if (sourceInfo.HasFaded()) {
                if (!sourceInfo.allowSound)
                    sourceInfo.StopAndClear();
            }
            else
            {
                if (sourceInfo.allowSound)
                    sourceInfo.fadeTimer += dt;
                else
                    sourceInfo.fadeTimer -= dt;

                sourceInfo.source.volume = Mathf.Lerp(
                    0,
                    1,
                    Math_Ease.Ease(
                        EASE.IN_QUAD,
                        sourceInfo.fadeTimer * sourceInfo.fadeTime_inv)
                    );
            }


            sourceInfos[i] = sourceInfo;
        }

    }

    private void OnEnable()
    {
        playMusicChannel.Subscribe(HandlePlayMusicEvent);
    }

    private void OnDisable()
    {
        playMusicChannel.Unsubscribe(HandlePlayMusicEvent);
    }
}
