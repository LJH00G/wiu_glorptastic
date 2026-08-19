using Game.SO.EventChannel.Context;
using Game.SO.EventChannel;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class SceneSwitchController : MonoBehaviour
{
    [Header("Event Listening Channels")]
    [SerializeField] SceneSwitchEventChannelSO sceneSwitchEventChannel;

    [Header("Event Broadcasting Channel")]
    [SerializeField] DelayedCallbackEventChannelSO delayedCallbackEventChannel;
    [SerializeField] PlayMusicEventChannelSO playMusicEventChannel;

    public void SwitchScene(SceneSwitchEventContext context)
    {
        Time.timeScale = 0f;

        playMusicEventChannel.Raise(context.playMusicContext);

        delayedCallbackEventChannel.Raise(new DelayedCallbackEventContext(
            () =>
        {
            Scene oldScene = SceneManager.GetActiveScene();

            void OnSceneLoaded(Scene scene, LoadSceneMode mode)
            {
                if (scene.name != context.loadScene)
                    return;

                SceneManager.sceneLoaded -= OnSceneLoaded;

                SceneManager.SetActiveScene(scene);
                Time.timeScale = 1f;

                SceneManager.UnloadSceneAsync(oldScene);
            }

            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.LoadSceneAsync(context.loadScene, LoadSceneMode.Additive);
            Camera.main.tag = "Untagged";


            Debug.Log(SceneManager.GetActiveScene());
            Scene oldScene = SceneManager.GetActiveScene();
            SceneManager.UnloadSceneAsync(oldScene);
        },
            context.delay
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
