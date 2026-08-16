using Game.SO.EventChannel.Context;
using System;
using UnityEngine;

namespace Game.SO.EventChannel.Context
{
    [Serializable]
    public struct PlayMusicEventContext
    {
        public AudioClip music;
        public bool playOrStop;
        public bool restartIfPlay;
        public float delayToPlay;
        /// <summary>whether should this music fade when play / stop</summary>
        public bool fadeThis;
        public float fadeThis_Time;
        /// <summary>turn off other music</summary>
        public bool turnOffOther;
        /// <summary>cannot fade off if <see cref="turnOffOther"/> is true</summary>
        public bool fadeOffOther;
        public float fadeOffOther_Time;
        /// <summary>whether the volume is forcefully set as 0 or 1 based on fade direction</summary>
        public bool fadeForceSetVolume;

        public PlayMusicEventContext(AudioClip music, bool playOrStop = true, bool restartIfPlay = false, float delayToPlay = 0, bool fadeThis = false, float fadeThis_Time = 0, bool turnOffOther = false, bool fadeOffOther = false, float fadeOffOther_Time = 0, bool fadeForceSetVolume = false)
        {
            this.music = music;
            this.playOrStop = playOrStop;
            this.restartIfPlay = restartIfPlay;
            this.delayToPlay = delayToPlay;
            this.fadeThis = fadeThis;
            this.fadeThis_Time = fadeThis_Time;
            this.turnOffOther = turnOffOther;
            this.fadeOffOther = fadeOffOther;
            this.fadeOffOther_Time = fadeOffOther_Time;
            this.fadeForceSetVolume = fadeForceSetVolume;
        }


        static public PlayMusicEventContext InstantSilent { get; } = new PlayMusicEventContext(null, false, false, 0, false, 0, true);
        static public PlayMusicEventContext FadeAllOut_dot5s { get; } = new PlayMusicEventContext(null, false, false, 0, false, 0, false, true, 0.5f);
        static public PlayMusicEventContext FadeAllOut_1s { get; } = new PlayMusicEventContext(null, false, false, 0, false, 0, false, true, 1f);
        static public PlayMusicEventContext FadeAllOut_2s { get; } = new PlayMusicEventContext(null, false, false, 0, false, 0, false, true, 2f);
        static public PlayMusicEventContext FadeAllOut_3s { get; } = new PlayMusicEventContext(null, false, false, 0, false, 0, false, true, 3f);



        public override string ToString()
        {
            return $"PlayMusicEventContext: music({music}), playOrStop({playOrStop}), restartIfPlay({restartIfPlay}), delayToPlay({delayToPlay}) ";
        }
    }
}

namespace Game.SO.EventChannel
{
    [CreateAssetMenu(fileName = "PlayMusicEvent_Channel", menuName = "Scriptable Objects/EventChannel/Module/PlayMusicEventChannelSO")]
    public class PlayMusicEventChannelSO : EventChannelSO<PlayMusicEventContext>
    {

    }
}