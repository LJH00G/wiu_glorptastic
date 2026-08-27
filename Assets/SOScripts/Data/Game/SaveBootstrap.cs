//using UnityEngine;
//using Game;

//[DefaultExecutionOrder(-9999)]
//public class SaveBootstrap : MonoBehaviour
//{
//    [Header("Databases")]
//    [SerializeField] ItemDatabaseSO itemDatabase;
//    [SerializeField] BuddyDatabaseSO buddyDatabase;

//    void Awake()
//    {
//        if (!SaveManager.HasSave())
//        {
//            Debug.Log("SaveBootstrap.Awake() | no save found, starting fresh");
//            return;
//        }

//        SaveData data = SaveManager.Load();
//        SaveManager.ApplyToUserData(data, GameManager.CurrentUserData, itemDatabase, buddyDatabase);

//        Debug.Log($"SaveBootstrap.Awake() | loaded save, checkpoint: {data.lastCheckpointID}, scene: {data.lastSceneName}");
//    }
//}