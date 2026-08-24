using Game.SO.EventChannel.Context;
using Game.SO.EventChannel;
using UnityEngine;
using UnityEngine.SceneManagement;

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

                if (context.unloadOldScene)
                {
                    Debug.Log($"unloading old scene: {oldScene}");
                    SceneManager.UnloadSceneAsync(oldScene);
                } else
                {
                    Debug.Log($"unloading skipped, specified no unloading");
                }
            }

            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.LoadSceneAsync(context.loadScene, LoadSceneMode.Additive);
            Camera.main.tag = "Untagged";
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
