using System.Collections;
using UnityEngine;
using Game;
using Game.TPManager;
using Game.SO.EventChannel;
using Game.SO.EventChannel.Context;

public class SaveFlowController : MonoBehaviour
{
    [Header("Databases")]
    [SerializeField] ItemDatabaseSO itemDatabase;
    [SerializeField] BuddyDatabaseSO buddyDatabase;

    [Header("Event Broadcasting Channel")]
    [SerializeField] SceneSwitchEventChannelSO sceneSwitchEventChannel;

    [Header("New Game Defaults")]
    [SerializeField] string newGameSceneName;
    [SerializeField] string newGameCheckpointID;

    [Header("Respawn Teleport")]
    [SerializeField] float respawnFadeTime = 0.5f;

    [Header("Screen Fade")]
    [SerializeField] ScreenFader screenFader;
    [SerializeField] float sceneTransitionFadeOutTime = 0.5f;

    [Header("Safety")]
    [SerializeField] float sceneSwitchTimeoutSeconds = 15f;

    static SaveFlowController instance;

    void Awake()
    {
        if (instance && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public bool SlotHasSave(int slotIndex) => SaveManager.HasSave(slotIndex);

    [ContextMenu("Log Save Path")]
    void LogSavePath()
    {
        Debug.Log(Application.persistentDataPath);
    }

    public void StartNewSave(int slotIndex)
    {
        UserData fresh = new UserData();
        fresh.SetSaveSlotIndex(slotIndex);

        GameManager.SetUserData(fresh);
        SaveManager.CurrentSlot = slotIndex;

        Debug.Log($"SaveFlowController.StartNewSave() | started new save in slot {slotIndex}");

        StartCoroutine(LoadSceneAndRespawn(newGameSceneName, newGameCheckpointID));
    }

    public void LoadMostRecentSave()
    {
        Time.timeScale = 1f;
        LoadSave(SaveManager.CurrentSlot);
    }

    public void LoadSave(int slotIndex)
    {
        if (!SaveManager.HasSave(slotIndex))
        {
            Debug.LogWarning($"SaveFlowController.LoadSave() | no save found in slot {slotIndex}");
            return;
        }

        SaveData data = SaveManager.Load(slotIndex);
        SaveManager.ApplyToUserData(data, GameManager.CurrentUserData, itemDatabase, buddyDatabase);
        SaveManager.CurrentSlot = slotIndex;

        Debug.Log($"SaveFlowController.LoadSave() | loaded slot {slotIndex}, checkpoint: {data.lastCheckpointID}, scene: {data.lastSceneName}");

        StartCoroutine(LoadSceneAndRespawn(data.lastSceneName, data.lastCheckpointID));
    }

    IEnumerator LoadSceneAndRespawn(string sceneName, string checkpointID)
    {
        if (screenFader)
        {
            screenFader.FadeIn(1);

            yield return new WaitForSeconds(sceneTransitionFadeOutTime);
        }
        var context = new SceneSwitchEventContext(
            SCENE_SWITCH_SETTING.LOAD_SEQUENTIALLY,
            sceneName,
            0f,
            PlayMusicEventContext.InstantSilent,
            SCENE_SWITCH_PAUSE.NONE,
            true
        );
        sceneSwitchEventChannel.Raise(context);

        float elapsed = 0f;
        while (GameplaySceneTracker.CurrentGameplayScene != sceneName)
        {
            elapsed += Time.unscaledDeltaTime;
            if (elapsed >= sceneSwitchTimeoutSeconds)
            {
                Debug.LogError($"SaveFlowController.LoadSceneAndRespawn() | timed out waiting for scene switch to '{sceneName}'");
                if (screenFader) screenFader.FadeIn(1);
                {
                    yield break;
                }
            }
            yield return null;
        }

        yield return null;

        if (!GameManager.Player)
        {
            Debug.LogWarning("SaveFlowController.LoadSceneAndRespawn() | scene loaded but GameManager.Player is still null, cannot respawn player");
        }
        else
        {
            bool foundCheckpoint = false;
            var checkpoints = FindObjectsOfType<SaveCheckpointTrigger>();

            foreach (var checkpoint in checkpoints)
            {
                if (checkpoint.CheckpointID != checkpointID)
                    continue;

                foundCheckpoint = true;
                TPManager tpManager = FindObjectOfType<TPManager>();

                if (tpManager)
                {
                    TPDefinition respawnDefinition = new TPDefinition
                    {
                        position = checkpoint.transform.position,
                        time = respawnFadeTime
                    };

                    yield return tpManager.Teleport(respawnDefinition);
                    Debug.Log($"SaveFlowController.LoadSceneAndRespawn() | teleported to checkpoint {checkpointID} via TPManager");
                }
                else
                {
                    Debug.LogWarning("SaveFlowController.LoadSceneAndRespawn() | no TPManager found in scene, falling back to instant position set");
                    GameManager.Player.transform.position = checkpoint.transform.position;
                }

                break;
            }

            if (!foundCheckpoint)
            {
                Debug.LogWarning($"SaveFlowController.LoadSceneAndRespawn() | no checkpoint found matching ID '{checkpointID}' in scene '{sceneName}'");
            }
        }

        if (screenFader)
        {

            screenFader.FadeOut(1);

        }
    }
}