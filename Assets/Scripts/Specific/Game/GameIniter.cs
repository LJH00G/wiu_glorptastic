using Game;
using Game.Inventory;
using UnityEngine;
using System.Collections.Generic;
using Game.SO.EventChannel.Context;
using Game.GlobalVariable;

[DefaultExecutionOrder(-99999)]
public class GameIniter : MonoBehaviour
{
    [SerializeField] GameObject player;
    [SerializeField] GameObject follower;
    [SerializeField] List<string> Flags;

    [SerializeField] PlayMusicEventContext playMusicEventContext;

    private void Awake()
    {
        Debug.Log($"game initer awaked in scene {gameObject.scene.name}", this);

        GameManager.SetPlayer(player);
        GameManager.SetFollower(follower);

        InventoryManager.ManageInventory(GameManager.CurrentUserData.Inventory);

        GameManager.SetGameState(GAME_STATE.OVERWORLD);
        GameManager.SetOverWorldState(OVERWORLD_STATE.GENERAL);
        foreach(string flagName in Flags)
            GameManager.EnsureFlag(flagName);
    }

    private void Start()
    {
        Debug.Log($"game initer started in scene {gameObject.scene.name}", this);

        StaticGlobalVariable.PlayMusicEventChannel.Raise(playMusicEventContext);
        if (GameManager.CurrentUserData.CurrentEquipedBuddy)
            GameManager.CurrentUserData.SetCurrentBuddy(GameManager.CurrentUserData.CurrentEquipedBuddy);
    }

#if UNITY_EDITOR

    private void OnValidate()
    {
        Awake();
    }
#endif
}
