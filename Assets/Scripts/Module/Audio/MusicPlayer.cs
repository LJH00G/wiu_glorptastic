using Game.SO.EventChannel.Context;
using Game.SO.EventChannel;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using Utility.Math;

public class MusicPlayer : MonoBehaviour
{

    [Serializable]
    public struct AudioSourceInfo
    {
        /// <summary>
        /// this field must always be filled
        /// </summary>
        [SerializeField] public AudioSource source;
        [SerializeField] public bool allowSound, fade;
        [SerializeField] public float fadeTime;
        [SerializeField, DisplayOnly] public float fadeTime_inv;
        [SerializeField] public float fadeTimer;

        public AudioSourceInfo(AudioSource source)
        {
            this.source = source;
            allowSound = fade = false;
            fadeTime = fadeTime_inv = fadeTimer = 0;
        }

        public void StopAndClear()
        {
            source.Stop();
            source.clip = null;

            fade = allowSound = false;
            fadeTime = fadeTime_inv = fadeTimer = 0;
        }

        /// <param name="forceSetVolume">whether the volume is forcefully set as 0 or 1 based on fade direction</param>
        public void SetFade(float time, bool fadeInOrOut, bool forceSetVolume)
        {
            if (time <= 0)
                return;

            allowSound = fadeInOrOut;

            if (forceSetVolume) {
                source.volume = allowSound ? 0 : 1;
                fadeTimer = allowSound ? 0 : time;
            }
            else
            {
                if (fade)
                    fadeTimer = fadeTimer * fadeTime_inv * time;
                else
                    fadeTimer = allowSound ? 0 : time;
            }

            fadeTime = time;
            fadeTime_inv = 1 / fadeTime;

            fade = true;

            if (allowSound)
                source.Play();
        }

        public void Play()
        {
            fade = false;
            allowSound = true;
            fadeTimer = 0;
            fadeTime = 0;
            fadeTime_inv = 0;
            source.volume = 1;

            source.Play();
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

    uint currentCallbackID = 0;

    public void PlayMusicWithContext(PlayMusicEventContext context)
    {
        AudioSourceInfo sourceInfoWithMusic = new(null), freeSourceInfo = new(null);
        int sourceInfoWithMusic_index = -1, freeSourceInfo_index = -1;

        for (int i = 0; i < sourceInfos.Length; i++)
        {
            var sourceInfo = sourceInfos[i];
            if (!sourceInfo.source)
                continue;

            if (context.music) // set sourceInfoWithMusic and freeSourceInfo
            {
                if (sourceInfoWithMusic_index == -1 && sourceInfo.source.clip == context.music)
                {
                    sourceInfoWithMusic_index = i;
                    sourceInfoWithMusic = sourceInfo;
                }
                else if (freeSourceInfo_index == -1 && !sourceInfo.source.clip)
                {
                    freeSourceInfo_index = i;
                    freeSourceInfo = sourceInfo;
                }
            }

            if (sourceInfo.source.clip && sourceInfo.source.clip != context.music) // turn off music that are already playing
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


        if (!context.music)
        {
            Debug.Log($"MusicPlayer.HandlePlayMusicEvent() | handled {context}");
            return;
        }


        currentCallbackID++;
        uint thisCallbackID = currentCallbackID;

        if (context.playOrStop) // play music
        {

            if (sourceInfoWithMusic_index == -1 && freeSourceInfo_index == -1)
            {
                Debug.Log($"MusicPlayer.HandlePlayMusicEvent() | handled {context}, tried to play but no audio source can be used / is playing the same music");
                return;
            }

            if (context.delayToPlay > 0)
                delayedCallbackEventChannel.Raise(new DelayedCallbackEventContext(
                    TestPlayMusic,
                    context.delayToPlay
                    ));
            else
                TestPlayMusic();

            Debug.Log($"MusicPlayer.HandlePlayMusicEvent() | handled {context}");
            return;


            void TestPlayMusic()
            {
                // ignore if another music callback is scheduled
                if (thisCallbackID != currentCallbackID)
                    return;

                // the if statement at 114 made sure either sourceInfoWithMusic exist, or freeSourceInfo exist
                ref AudioSourceInfo usingSourceInfo = ref (sourceInfoWithMusic_index != -1 ?
                    ref sourceInfoWithMusic : ref freeSourceInfo);


                if (usingSourceInfo.source.isPlaying && !context.restartIfPlay)
                    return;


                usingSourceInfo.source.clip = context.music;

                if (context.fadeThis)
                    usingSourceInfo.SetFade(context.fadeThis_Time, true, context.fadeForceSetVolume);
                else
                    usingSourceInfo.Play();

                if (sourceInfoWithMusic_index != -1)
                    sourceInfos[sourceInfoWithMusic_index] = sourceInfoWithMusic;
                if (freeSourceInfo_index != -1)
                    sourceInfos[freeSourceInfo_index] = freeSourceInfo;

            }

        }
        else if (!sourceInfoWithMusic.source)
        {
            Debug.Log($"MusicPlayer.HandlePlayMusicEvent() | handled {context}, tried to pause but there isnt any audio source currently playing the same music");
            return;
        }

        // pause music
        if (context.fadeThis)
            sourceInfoWithMusic.SetFade(context.fadeThis_Time, false, context.fadeForceSetVolume);
        else
            sourceInfoWithMusic.StopAndClear();

        sourceInfos[sourceInfoWithMusic_index] = sourceInfoWithMusic;

        Debug.Log($"MusicPlayer.HandlePlayMusicEvent() | handled {context}");
    }


    void Update()
    {
        float dt = Time.unscaledDeltaTime;

        for (int i = 0; i < sourceInfos.Length; i++)
        {
            var sourceInfo = sourceInfos[i];

            if (!sourceInfo.fade)
                continue;

            if (sourceInfo.allowSound)
                sourceInfo.fadeTimer += dt;
            else
                sourceInfo.fadeTimer -= dt;

            if (sourceInfo.HasFaded())
            {
                if (!sourceInfo.allowSound)
                    sourceInfo.StopAndClear();
                else
                    sourceInfo.fade = false;

                sourceInfo.source.volume = sourceInfo.allowSound ? 1 : 0;
            }
            else
            {
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
        playMusicChannel.Subscribe(PlayMusicWithContext);
    }

    private void OnDisable()
    {
        playMusicChannel.Unsubscribe(PlayMusicWithContext);
    }


#if UNITY_EDITOR

    private void OnValidate()
    {
        HashSet<AudioSource> usedAudioSources = new();
        AudioMixerGroup usingAudioMixerGroup = null;

        for (int i = 0; i < sourceInfos.Length; ++i)
        {
            var sourceInfo = sourceInfos[i];
            if (!sourceInfo.source)
            {
                Debug.LogError($"MusicPlayer | sourceInfos[{i}] has no AudioSource assigned", this);
                continue;
            }

            if (usedAudioSources.Contains(sourceInfo.source))
            {
                Debug.LogError($"MusicPlayer | sourceInfos[{i}].source is repeated with one of the previous sources", this);
                continue;
            }

            usedAudioSources.Add(sourceInfo.source);
            sourceInfo.source.playOnAwake = false;
            sourceInfo.source.loop = true;


            if (!sourceInfo.source.outputAudioMixerGroup)
            {
                Debug.LogError($"MusicPlayer | sourceInfos[{i}].source has no output assigned", this);
                continue;
            }

            if (usingAudioMixerGroup != null && usingAudioMixerGroup != sourceInfo.source.outputAudioMixerGroup)
            {
                Debug.LogError($"MusicPlayer | sourceInfos[{i}].source.outputAudioMixerGroup must use the same output as the previous sources", this);
                continue;
            }

            usingAudioMixerGroup = sourceInfo.source.outputAudioMixerGroup;
        }
    }
#endif
}
