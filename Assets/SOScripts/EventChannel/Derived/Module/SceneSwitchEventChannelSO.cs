using Game.SO.EventChannel.Context;
using NUnit.Framework;
using System;
using UnityEngine;
using System.Collections.Generic;

namespace Game.SO.EventChannel.Context
{
    [Serializable]
    public class SceneSwitchEventContext
    {
        public string loadScene;
        public float delay;
        public PlayMusicEventContext playMusicContext;
        public bool unloadOldScene;

        /// <summary>
        /// List that ignores GameObjects in old scene
        /// </summary>
        public List<GameObject> ignoreableObjs;

        public SceneSwitchEventContext(string loadNewActiveScene, float delay, PlayMusicEventContext playMusicContext, bool unloadOldScene, List<GameObject> obj)
        {
            loadScene = loadNewActiveScene;
            this.delay = delay;
            this.playMusicContext = playMusicContext;
            this.unloadOldScene = unloadOldScene;
            this.ignoreableObjs = obj;
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