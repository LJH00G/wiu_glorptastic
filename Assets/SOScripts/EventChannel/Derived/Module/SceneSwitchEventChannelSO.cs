using Game.SO.EventChannel.Context;
using System;
using UnityEngine;
using System.Collections.Generic;

namespace Game.SO.EventChannel.Context
{

    public enum SCENE_SWITCH_SETTING
    {
        LOAD_ADDITIVE,
        LOAD_SEQUENTIALLY,
        UNLOAD
    }

    public enum SCENE_SWITCH_PAUSE
    {
        NONE,
        PAUSE_AT_START,
        PAUSE_DURING_LOAD
    }

    [Serializable]
    public class SceneSwitchEventContext
    {
        public SCENE_SWITCH_SETTING setting;
        public string scene;

        public float delay;
        public PlayMusicEventContext playMusicContext;

        public SCENE_SWITCH_PAUSE timePause;
        public bool setSceneAsMain;

        /// <summary>
        /// List that ignores GameObjects in old scene
        /// </summary>
        public List<GameObject> ignoreableObjs;


        public SceneSwitchEventContext(SCENE_SWITCH_SETTING setting, string scene)
        {
            this.setting = setting;
            this.scene = scene;

            delay = 0;
            playMusicContext = PlayMusicEventContext.InstantSilent;

            timePause = SCENE_SWITCH_PAUSE.PAUSE_DURING_LOAD;
            setSceneAsMain = false;

            ignoreableObjs = new();
            
        }

        public SceneSwitchEventContext(SCENE_SWITCH_SETTING setting, string scene, float delay, PlayMusicEventContext playMusicContext)
        {
            this.setting = setting;
            this.scene = scene;

            this.delay = delay;
            this.playMusicContext = playMusicContext;

            timePause = SCENE_SWITCH_PAUSE.PAUSE_DURING_LOAD;
            setSceneAsMain = false;

            ignoreableObjs = new();
        }

        public SceneSwitchEventContext(SCENE_SWITCH_SETTING setting, string scene, float delay, PlayMusicEventContext playMusicContext, SCENE_SWITCH_PAUSE timePause, bool setSceneAsMain)
        {
            this.setting = setting;
            this.scene = scene;

            this.delay = delay;
            this.playMusicContext = playMusicContext;

            this.timePause = timePause;
            this.setSceneAsMain = setSceneAsMain;

            ignoreableObjs = new();
        }

        public SceneSwitchEventContext(SCENE_SWITCH_SETTING setting, string scene, float delay, PlayMusicEventContext playMusicContext, SCENE_SWITCH_PAUSE timePause, bool setSceneAsMain, List<GameObject> ignoreableObjs)
        {
            this.setting = setting;
            this.scene = scene;

            this.delay = delay;
            this.playMusicContext = playMusicContext;

            this.timePause = timePause;
            this.setSceneAsMain = setSceneAsMain;

            this.ignoreableObjs = ignoreableObjs;
        }

        public override string ToString()
        {
            return $"SceneSwitchEventContext: setting({setting}), scene({scene}), delay({delay}) ";
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