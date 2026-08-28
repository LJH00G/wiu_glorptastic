using Game;
using Game.SO.Data.Game;
using Game.SO.EventChannel;
using Game.SO.EventChannel.Context;
using Game.TPManager;
using System.Collections;
using UnityEngine;

public class SaveFlowController : MonoBehaviour
{
    [Header("Databases")]
    [SerializeField] ItemDatabaseSO itemDatabase;
    [SerializeField] BuddyDatabaseSO buddyDatabase;
    [SerializeField] UserDataSO defaultUser;

    [Header("Event Listening Channel")]
    [SerializeField] IntEventChannelSO loadSaveEventChannel;
    [SerializeField] IntEventChannelSO startNewSaveEventChannel;
    [SerializeField] EventChannelSO loadMostRecentSaveEventChannel;
    [SerializeField] StringEventChannelSO gameplaySceneChangedEventChannel;

    [Header("Event Broadcasting Channel")]
    [SerializeField] SceneSwitchEventChannelSO sceneSwitchEventChannel;
    [SerializeField] EventChannelSO onNewSaveloadedEventChannel;

    [Header("New Game Defaults")]
    [SerializeField] string newGameSceneName;
    [SerializeField] string newGameCheckpointID;

    [Header("Respawn Teleport")]
    [SerializeField] float respawnFadeTime = 0.5f;

    [Header("Safety")]
    [SerializeField] float sceneSwitchTimeoutSeconds = 2f;
    [SerializeField] bool isLoadingSave;
    [SerializeField] string loadingSaveID;

    public void StartNewSave(int slotIndex)
    {
        if (isLoadingSave)
        {
            Debug.Log("cannot start new save when is currently laoding a save");
            return;
        }

        UserData fresh = defaultUser.UserData.Clone(slotIndex);

        GameManager.SetUserData(fresh);
        onNewSaveloadedEventChannel.Raise();
        SaveManager.CurrentSlot = slotIndex;

        Debug.Log($"SaveFlowController.StartNewSave() | started new save in slot {slotIndex}");

        isLoadingSave = true;
        loadingSaveID = newGameCheckpointID;
        LoadSceneAndRespawn(newGameSceneName);
    }

    public void LoadMostRecentSave()
    {
        if (isLoadingSave)
        {
            Debug.Log("cannot load most recent save when is currently laoding a save");
            return;
        }

        Time.timeScale = 1f;
        LoadSave(SaveManager.CurrentSlot);
    }

    public void LoadSave(int slotIndex)
    {
        if (isLoadingSave)
        {
            Debug.Log("cannot load save when is currently laoding a save");
            return;
        }

        if (!SaveManager.HasSave(slotIndex))
        {
            Debug.LogWarning($"SaveFlowController.LoadSave() | no save found in slot {slotIndex}");
            return;
        }

        SaveData data = SaveManager.Load(slotIndex);
        SaveManager.ApplyToUserData(data, GameManager.CurrentUserData, itemDatabase, buddyDatabase);
        onNewSaveloadedEventChannel.Raise();
        SaveManager.CurrentSlot = slotIndex;

        Debug.Log($"SaveFlowController.LoadSave() | loaded slot {slotIndex}, checkpoint: {data.lastCheckpointID}, scene: {data.lastSceneName}");

        isLoadingSave = true;
        loadingSaveID = data.lastCheckpointID;
        LoadSceneAndRespawn(data.lastSceneName);
    }

    void LoadSceneAndRespawn(string sceneName)
    {
        var context = new SceneSwitchEventContext(
            SCENE_SWITCH_SETTING.LOAD_SEQUENTIALLY,
            sceneName,
            respawnFadeTime,
            PlayMusicEventContext.FadeAllOut_1s,
            SCENE_SWITCH_PAUSE.PAUSE_DURING_LOAD,
            true
        );
        sceneSwitchEventChannel.Raise(context);
    }

    void CheckSceneSwitchedIsLoadingSave(string sceneName)
    {
        StartCoroutine(TryRespawnPlayer(sceneName));
    }

    IEnumerator TryRespawnPlayer(string sceneName)
    {
        float elapsed = 0f;
        while (!GameManager.Player)
        {
            elapsed += Time.deltaTime;
            if (elapsed >= 0.25)
            {
                Debug.LogWarning("SaveFlowController.TryRespawnPlayer() | scene loaded but GameManager.Player is still null, cannot respawn player");
                break;
            }
            yield return null;
        }

        {
            bool foundCheckpoint = false;
            var checkpoints = FindObjectsByType<SaveCheckpointTrigger>(FindObjectsSortMode.None);

            foreach (var checkpoint in checkpoints)
            {
                if (checkpoint.CheckpointID != loadingSaveID)
                    continue;

                foundCheckpoint = true;
                TPManager tpManager = FindFirstObjectByType<TPManager>();

                if (tpManager)
                {
                    TPDefinition respawnDefinition = new TPDefinition
                    {
                        position = checkpoint.transform.position,
                        time = respawnFadeTime
                    };

                    yield return tpManager.Teleport(respawnDefinition);
                    Debug.Log($"SaveFlowController.LoadSceneAndRespawn() | teleported to checkpoint {loadingSaveID} via TPManager");
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
                Debug.LogWarning($"SaveFlowController.LoadSceneAndRespawn() | no checkpoint found matching ID '{loadingSaveID}' in scene '{sceneName}'");
            }
        }


        isLoadingSave = false;
        yield break;
    }


    private void OnEnable()
    {
        loadSaveEventChannel.Subscribe(LoadSave);
        startNewSaveEventChannel.Subscribe(StartNewSave);
        loadMostRecentSaveEventChannel.Subscribe(LoadMostRecentSave);
        gameplaySceneChangedEventChannel.Subscribe(CheckSceneSwitchedIsLoadingSave);
    }

    private void OnDisable()
    {
        loadSaveEventChannel.Unsubscribe(LoadSave);
        startNewSaveEventChannel.Unsubscribe(StartNewSave);
        loadMostRecentSaveEventChannel.Unsubscribe(LoadMostRecentSave);
        gameplaySceneChangedEventChannel.Unsubscribe(CheckSceneSwitchedIsLoadingSave);
    }
}
