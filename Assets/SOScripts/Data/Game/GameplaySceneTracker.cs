using Game.SO.EventChannel;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class GameplaySceneTracker
{
    public static string CurrentGameplayScene { get; private set; } = "";

    public static void HandleSceneChanged(string sceneName)
    {
        CurrentGameplayScene = sceneName;
    }
}

public class GameplaySceneTrackerListener : MonoBehaviour
{
    [Header("Event Listening Channel")]
    [SerializeField] StringEventChannelSO gameplaySceneChangedEventChannel;

    static GameplaySceneTrackerListener instance;

    void Awake()
    {
        GameplaySceneTracker.HandleSceneChanged(SceneManager.GetActiveScene().name);

        if (instance && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void OnEnable()
    {
        gameplaySceneChangedEventChannel.Subscribe(GameplaySceneTracker.HandleSceneChanged);
    }

    void OnDisable()
    {
        gameplaySceneChangedEventChannel.Unsubscribe(GameplaySceneTracker.HandleSceneChanged);
    }
}