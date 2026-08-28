using Game.SO.EventChannel.Context;
using Game.SO.EventChannel;
using UnityEngine;
using UnityEngine.SceneManagement;
using Game.OverworldDisableManager;

public class SceneSwitchController : MonoBehaviour
{
    [Header("Event Listening Channels")]
    [SerializeField] SceneSwitchEventChannelSO sceneSwitchEventChannel;

    [Header("Event Broadcasting Channel")]
    [SerializeField] DelayedCallbackEventChannelSO delayedCallbackEventChannel;
    [SerializeField] PlayMusicEventChannelSO playMusicEventChannel;
    [SerializeField] StringEventChannelSO gameplaySceneChangedEventChannel;

    [SerializeField] FadeEventChannelSO fadeEvent;

    public void SwitchScene(SceneSwitchEventContext context)
    {
        Debug.Log($"trying to scene switch to {context}", this);

        FadeEventChannelContext fadeContext = new FadeEventChannelContext(true, context.delay);
        fadeEvent.Raise(fadeContext);

        if (context.timePause == SCENE_SWITCH_PAUSE.PAUSE_AT_START)
        {
            Time.timeScale = 0f;
        }
        playMusicEventChannel.Raise(context.playMusicContext);

        if (context.delay <= 0)
        {
            PerformSceneSwitch();
        }
        else
        {
            delayedCallbackEventChannel.Raise(new DelayedCallbackEventContext(
                PerformSceneSwitch,
                context.delay,
                false
            ));
        }

        void PerformSceneSwitch()
        {
            Scene oldScene = SceneManager.GetActiveScene();

            if (context.setting == SCENE_SWITCH_SETTING.UNLOAD)
            {
                Debug.Log($"unloading old scene: {oldScene.name}, context.setting: {context.setting}");
                SceneManager.sceneUnloaded += OnSceneUnloaded;
                SceneManager.UnloadSceneAsync(oldScene);

                Time.timeScale = 1f;

                if (!context.setSceneAsMain)
                {
                    return;
                }
                Scene scene = SceneManager.GetSceneByName(context.scene);
                OverworldDisableManager.EnableAllObjects(scene);
                SceneManager.SetActiveScene(scene);
                gameplaySceneChangedEventChannel.Raise(scene.name);

                GameObject[] rootList = scene.GetRootGameObjects();
                foreach (GameObject root in rootList)
                {
                    Camera cam = root.GetComponent<Camera>();
                    if (cam)
                    {
                        cam.tag = "MainCamera";
                        cam.enabled = true;
                    }
                }

                fadeContext.isFade = false;
                fadeEvent.Raise(fadeContext);
            }
            else
            {
                SceneManager.sceneLoaded += OnSceneLoaded;

                Debug.Log($"loading scene: {context.scene}, context.setting: {context.setting}");
                SceneManager.LoadSceneAsync(context.scene, LoadSceneMode.Additive);

                if (context.timePause == SCENE_SWITCH_PAUSE.PAUSE_DURING_LOAD)
                {
                    Time.timeScale = 0f;
                }
                if (!context.setSceneAsMain)
                {
                    return;
                }

                Camera main = Camera.main;
                if (main)
                {
                    main.enabled = false;
                    main.tag = "Untagged";
                }

                OverworldDisableManager.DisableAllObjects(oldScene, context.ignoreableObjs);

                
            }

            void OnSceneLoaded(Scene scene, LoadSceneMode mode)
            {
                if (scene.name != context.scene)
                {
                    return;
                }

                fadeContext.isFade = false;
                fadeEvent.Raise(fadeContext);

                SceneManager.sceneLoaded -= OnSceneLoaded;

                Debug.Log($"loaded scene: {context.scene}, context.setting: {context.setting}");

                if (context.setSceneAsMain)
                {
                    SceneManager.SetActiveScene(scene);
                    gameplaySceneChangedEventChannel.Raise(scene.name);
                }

                Time.timeScale = 1f;

                if (context.setting == SCENE_SWITCH_SETTING.LOAD_ADDITIVE)
                {
                    Debug.Log($"unloading skipped, specified no unloading");
                }
                else
                {
                    Debug.Log($"unloading old scene: {oldScene.name}, context.setting: {context.setting}");
                    SceneManager.sceneUnloaded += OnSceneUnloaded;
                    SceneManager.UnloadSceneAsync(oldScene);
                }
            }

            void OnSceneUnloaded(Scene scene)
            {
                if (scene.name != oldScene.name)
                {
                    return;
                }

                SceneManager.sceneUnloaded -= OnSceneUnloaded;
                Debug.Log($"unloaded old scene {oldScene.name}");
            }
        }
    }

    private void OnEnable()
    {
        sceneSwitchEventChannel.Subscribe(SwitchScene);
        
    }

    private void OnDisable()
    {
        sceneSwitchEventChannel.Unsubscribe(SwitchScene);
    }
}
