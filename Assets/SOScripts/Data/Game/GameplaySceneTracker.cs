using Game.SO.EventChannel;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class GameplaySceneTracker
{
    public static string CurrentGameplayScene { get; private set; } = "";

    public static void HandleSceneChanged(string sceneName)
    {
        Debug.Log($"GameplaySceneTracker.HandleSceneChanged() | called with '{sceneName}'");
        CurrentGameplayScene = sceneName;
    }
}