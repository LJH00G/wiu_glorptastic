using Game.SO.EventChannel.Context;
using System;
using UnityEngine;

namespace Game.SO.EventChannel.Context
{
    [Serializable]
    public class SceneSwitchEventContext
    {
        public string loadScene;
        public float delay;
        public PlayMusicEventContext playMusicContext;

        public SceneSwitchEventContext(string loadNewActiveScene, float delay, PlayMusicEventContext playMusicContext)
        {
            loadScene = loadNewActiveScene;
            this.delay = delay;
            this.playMusicContext = playMusicContext;
        }

        public override string ToString()
        {
            return $"SceneSwitchEventContext: loadScene({loadScene}), delay({delay}) ";
        }
    }
}

namespace Game.SO.EventChannel.Derived
{
    [CreateAssetMenu(fileName = "SceneSwitchEvent_Channel", menuName = "Scriptable Objects/EventChannel/SceneSwitchEventChannelSO")]
    public class SceneSwitchEventChannelSO : EventChannelSO<SceneSwitchEventContext>
    {

    }
}