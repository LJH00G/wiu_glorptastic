using Game.SO.EventChannel.Context;
using System;
using UnityEngine;

namespace Game.SO.EventChannel.Context
{
    [Serializable]
    public struct PlaySFXEventContext
    {
        public AudioClip SFX;
        public bool playOrStop;
        public bool playOneShot;

        public PlaySFXEventContext(AudioClip SFX, bool playOrStop = true, bool playOneShot = true)
        {
            this.SFX = SFX;
            this.playOrStop = playOrStop;
            this.playOneShot = playOneShot;
        }

        public override string ToString()
        {
            return $"PlaySFXEventContext: SFX( {SFX} ), playOrStop( {playOrStop} ), playOneShot( {playOneShot} ) ";
        }
    }
}

namespace Game.SO.EventChannel
{
    [CreateAssetMenu(fileName = "PlaySFXEvent_Channel", menuName = "Scriptable Objects/EventChannel/Module/PlaySFXEventChannelSO")]
    public class PlaySFXEventChannelSO : EventChannelSO<PlaySFXEventContext>
    {

    }
}