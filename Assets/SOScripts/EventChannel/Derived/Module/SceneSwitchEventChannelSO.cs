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

        public bool unloadOldScene;

        public SceneSwitchEventContext(string loadNewActiveScene, float delay, PlayMusicEventContext playMusicContext, bool unloadOldScene)
        {
            loadScene = loadNewActiveScene;
            this.delay = delay;
            this.playMusicContext = playMusicContext;
            this.unloadOldScene = unloadOldScene;
        }

        public override string ToString()
        {
            return $"SceneSwitchEventContext: loadScene({loadScene}), delay({delay}) ";
        }
    }
}

namespace Game.SO.EventChannel
{
    [CreateAssetMenu(fileName = "SceneSwitchEvent_Channel", menuName = "Scriptable Objects/EventChannel/Module/SceneSwitchEventChannelSO")]
    public class SceneSwitchEventChannelSO : EventChannelSO<SceneSwitchEventContext>
    {

    }
}