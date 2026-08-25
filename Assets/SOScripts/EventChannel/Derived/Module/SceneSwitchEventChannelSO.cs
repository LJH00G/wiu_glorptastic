using Game.SO.EventChannel.Context;
using System;
using UnityEngine;
using System.Collections.Generic;

namespace Game.SO.EventChannel.Context
{

    public enum SCENE_SETTING
    {
        LOAD_ADDITIVE,
        LOAD_SEQUENTIALLY,
        UNLOAD
    }

    [Serializable]
    public class SceneSwitchEventContext
    {
        public string loadScene;
        public float delay;
        public PlayMusicEventContext playMusicContext;

        public SCENE_SETTING setting;
        

        /// <summary>
        /// List that ignores GameObjects in old scene
        /// </summary>
        public List<GameObject> ignoreableObjs;

        public SceneSwitchEventContext(string loadNewActiveScene, float delay, PlayMusicEventContext playMusicContext, SCENE_SETTING setting, List<GameObject> obj)
        {
            loadScene = loadNewActiveScene;
            this.delay = delay;
            this.playMusicContext = playMusicContext;
            this.setting = setting;
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