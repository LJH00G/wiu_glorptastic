using Game.SO.EventChannel.Context;
using System;
using UnityEngine;

namespace Game.SO.EventChannel.Context
{
    [Serializable]
    public struct PlayMusicEventContext
    {
        public AudioClip music;
        public bool playOrPause;
        public bool restartIfPlay;
        public float delayToPlay;
        public bool fadeThis;
        public float fadeThis_Time;
        public bool turnOffOther;
        public bool fadeOffOther;
        public float fadeOffOther_Time;
        public bool fadeForceSetVolume;

        public PlayMusicEventContext(AudioClip music, bool playOrPause = true, bool restartIfPlay = false, float delayToPlay = 0, bool fadeThis = false, float fadeThis_Time = 0, bool turnOffOther = false, bool fadeOffOther = false, float fadeOffOther_Time = 0, bool fadeForceSetVolume = false)
        {
            this.music = music;
            this.playOrPause = playOrPause;
            this.restartIfPlay = restartIfPlay;
            this.delayToPlay = delayToPlay;
            this.fadeThis = fadeThis;
            this.fadeThis_Time = fadeThis_Time;
            this.turnOffOther = turnOffOther;
            this.fadeOffOther = fadeOffOther;
            this.fadeOffOther_Time = fadeOffOther_Time;
            this.fadeForceSetVolume = fadeForceSetVolume;
        }

        public override string ToString()
        {
            return $"PlayMusicEventContext: music({music}), playOrPause({playOrPause}), restartIfPlay({restartIfPlay}), delayToPlay({delayToPlay}) ";
        }
    }
}

namespace Game.SO.EventChannel.Derived
{
    [CreateAssetMenu(fileName = "PlayMusicEvent_Channel", menuName = "Scriptable Objects/EventChannel/PlayMusicEventChannelSO")]
    public class PlayMusicEventChannelSO : EventChannelSO<PlayMusicEventContext>
    {

    }
}