using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Game;

public class SaveFlowController : MonoBehaviour
{
    [Header("Databases")]
    [SerializeField] ItemDatabaseSO itemDatabase;
    [SerializeField] BuddyDatabaseSO buddyDatabase;

    [Header("New Game Defaults")]
    [SerializeField] string newGameSceneName;
    [SerializeField] string newGameCheckpointID;

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

    public void StartNewSave(int slotIndex)
    {
        UserData fresh = new UserData();
        fresh.SetSaveSlotIndex(slotIndex);

        GameManager.SetUserData(fresh);
        SaveManager.CurrentSlot = slotIndex;

        Debug.Log($"SaveFlowController.StartNewSave() | started new save in slot {slotIndex}");

        StartCoroutine(LoadSceneAndRespawn(newGameSceneName, newGameCheckpointID));
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
        var op = SceneManager.LoadSceneAsync(sceneName);
        while (!op.isDone)
        {
            yield return null;
        }
        yield return null;

        if (!GameManager.Player)
        {
            Debug.LogWarning("SaveFlowController.LoadSceneAndRespawn() | scene loaded but GameManager.Player is still null, cannot position player");
            yield break;
        }

        var checkpoints = FindObjectsOfType<SaveCheckpointTrigger>();
        foreach (var checkpoint in checkpoints)
        {
            if (checkpoint.CheckpointID != checkpointID)
            {
                continue;
            }
            GameManager.Player.transform.position = checkpoint.transform.position;
            Debug.Log($"SaveFlowController.LoadSceneAndRespawn() | positioned player at checkpoint {checkpointID}");
            yield break;
        }

        Debug.LogWarning($"SaveFlowController.LoadSceneAndRespawn() | no checkpoint found matching ID '{checkpointID}' in scene '{sceneName}'");
    }
}