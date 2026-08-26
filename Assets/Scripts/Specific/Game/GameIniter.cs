using Game;
using Game.Inventory;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

[DefaultExecutionOrder(-99999)]
public class GameIniter : MonoBehaviour
{
    [SerializeField] GameObject player;
    [SerializeField] GameObject follower;
    [SerializeField] List<string> Flags;
    private void Awake()
    {
        GameManager.SetPlayer(player);
        GameManager.SetFollower(follower);
        if (GameManager.CurrentUserData.CurrentEquipedBuddy)
            GameManager.CurrentUserData.SetCurrentBuddy(GameManager.CurrentUserData.CurrentEquipedBuddy);
        InventoryManager.ManageInventory(GameManager.CurrentUserData.Inventory);
        GameManager.SetGameState(GAME_STATE.OVERWORLD);
        GameManager.SetOverWorldState(OVERWORLD_STATE.GENERAL);
        foreach(string flagName in Flags)
            GameManager.EnsureFlag(flagName);
    }

#if UNITY_EDITOR

    private void OnValidate()
    {
        Awake();
    }
#endif
}
