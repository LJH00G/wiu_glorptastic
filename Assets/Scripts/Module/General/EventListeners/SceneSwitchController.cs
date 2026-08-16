using Game.SO.EventChannel.Context;
using Game.SO.EventChannel.Derived;
using Game.SO.EventChannel.Derived.Basic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitchController : MonoBehaviour
{
    [Header("Event Listening Channels")]
    [SerializeField] SceneSwitchEventChannelSO sceneSwitchEventChannel;

    [Header("Event Broadcasting Channel")]
    [SerializeField] DelayedCallbackEventChannelSO delayedCallbackEventChannel;
    [SerializeField] PlayMusicEventChannelSO playMusicEventChannel;

    void HandleSceneSwitchEvent(SceneSwitchEventContext context)
    {
        Time.timeScale = 0f;

        playMusicEventChannel.Raise(context.playMusicContext);

        delayedCallbackEventChannel.Raise(new DelayedCallbackEventContext(
            () =>
        {
            Scene oldScene = SceneManager.GetActiveScene();
            SceneManager.UnloadSceneAsync(oldScene);

            void OnSceneLoaded(Scene scene, LoadSceneMode mode)
            {
                if (scene.name != context.loadScene)
                    return;

                SceneManager.sceneLoaded -= OnSceneLoaded;

                SceneManager.SetActiveScene(scene);
                Time.timeScale = 1f;
            }

            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.LoadSceneAsync(context.loadScene, LoadSceneMode.Additive);
            Camera.main.tag = "Untagged";
        },
            context.delay
        ));

    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable()
    {
        sceneSwitchEventChannel.Subscribe(HandleSceneSwitchEvent);
    }

    private void OnDisable()
    {
        sceneSwitchEventChannel.Unsubscribe(HandleSceneSwitchEvent);
    }
}
