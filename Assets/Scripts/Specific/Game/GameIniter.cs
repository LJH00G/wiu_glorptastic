using Game;
using Game.Inventory;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
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
        GameManager.SetPlayer(player);
        GameManager.SetFollower(follower);

        if (GameManager.CurrentUserData.CurrentEquipedBuddy)
            GameManager.CurrentUserData.SetCurrentBuddy(GameManager.CurrentUserData.CurrentEquipedBuddy);

        InventoryManager.ManageInventory(GameManager.CurrentUserData.Inventory);


        GameManager.SetGameState(GAME_STATE.OVERWORLD);
        GameManager.SetOverWorldState(OVERWORLD_STATE.GENERAL);
        foreach(string flagName in Flags)
            GameManager.EnsureFlag(flagName);

        GameManager.CurrentUserData.PlayerBattleData.Refresh();
        GameManager.CurrentUserData.PlayerBattleData.CurrentHP = GameManager.CurrentUserData.PlayerBattleData.MaxHP;
        GameManager.CurrentUserData.PlayerBattleData.CurrentCurse = GameManager.CurrentUserData.PlayerBattleData.MaxCurse;
    }

    private void Start()
    {
        StaticGlobalVariable.PlayMusicEventChannel.Raise(playMusicEventContext);
    }

#if UNITY_EDITOR

    private void OnValidate()
    {
        Awake();
    }
#endif
}
