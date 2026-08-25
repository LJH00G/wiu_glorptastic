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

    public void SwitchScene(SceneSwitchEventContext context)
    {
        Debug.Log($"trying to scene switch to {context}", this);
        Time.timeScale = 0f;

        playMusicEventChannel.Raise(context.playMusicContext);

        delayedCallbackEventChannel.Raise(new DelayedCallbackEventContext(
            () =>
        {
            Scene oldScene = SceneManager.GetActiveScene();

            void OnSceneLoaded(Scene scene, LoadSceneMode mode)
            {
                if (scene.name != context.loadScene)
                {
                    return;
                }

                SceneManager.sceneLoaded -= OnSceneLoaded;

                SceneManager.SetActiveScene(scene);
                Time.timeScale = 1f;

                if (context.setting != SCENE_SETTING.LOAD_ADDITIVE)
                {
                    Debug.Log($"unloading old scene: {oldScene}");
                    SceneManager.UnloadSceneAsync(oldScene);

                } else
                {
                    Debug.Log($"unloading skipped, specified no unloading");
                }
            }

            
            if(context.setting != SCENE_SETTING.UNLOAD)
            {
                SceneManager.sceneLoaded += OnSceneLoaded;

                Camera main = Camera.main;
                if (main)
                {
                    main.enabled = false;
                    main.tag = "Untagged";
                }
                SceneManager.LoadSceneAsync(context.loadScene, LoadSceneMode.Additive);
                OverworldDisableManager.DisableAllObjects(oldScene, context.ignoreableObjs);
                OverworldDisableManager.EnableAllObjects(SceneManager.GetSceneByName(context.loadScene));
            }
            else
            {
                Scene scene = SceneManager.GetSceneByName(context.loadScene);
                OverworldDisableManager.EnableAllObjects(scene);
                SceneManager.SetActiveScene(scene);
                SceneManager.UnloadSceneAsync(oldScene);


                GameObject[] rootList = scene.GetRootGameObjects();
                foreach(GameObject root in rootList)
                {
                    Camera cam = root.GetComponent<Camera>();
                    if (cam)
                    {
                        cam.tag = "MainCamera";
                        cam.enabled = true;
                    }
                        
                }
            }


            
            
        },
            context.delay,
            false
        ));

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
