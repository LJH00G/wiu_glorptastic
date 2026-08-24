using Game;
using Game.Combat;
using Game.GlobalVariable;
using Game.Inventory;
using Game.OverworldDisableManager;
using Game.SO.Data.Item;
using Game.SO.EventChannel;
using Game.SO.EventChannel.Context;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CombatInitialiser : MonoBehaviour
{

    [SerializeField] InitializeCombatEventChannelSO initializeConbatEventChannel;
    [SerializeField] CombatEndEventChannelSO combatEndEventChannel;
    [SerializeField] CombatDataTunnelSO tunnelDataTunnel;
    [SerializeField] SceneSwitchEventChannelSO onSwitch;
    [SerializeField] Camera overworldCamera;

    GameObject enemyInitiated;
    Scene scene;

    public void StartCombat(EnemyEncounterDataSO data)
    {
        PlayerLoadoutSO player = StaticGlobalVariable.PlayerLoadout;
        PartnerLoadoutSO partner = GameManager.CurrentUserData.CurrentEquipedBuddy.Loadout;

        player.inventory = InventoryManager.GetItemList();
        player.equippedGem = InventoryManager.TryGetItemInList(out CurseGemItemSO gem) ? gem : null;
        player.equippedWeapon = InventoryManager.GetEquipedWeapon();
        player.equippedArmor = InventoryManager.GetEquipedArmour();
        player.equippedAccessories = InventoryManager.GetEquipedAccessories().ToList();

        tunnelDataTunnel.enemyEncounterData = data;
        tunnelDataTunnel.playerLoadout = player;
        tunnelDataTunnel.partnerLoadout = partner;

        enemyInitiated = data.enemy;
        scene = SceneManager.GetActiveScene();
        GameManager.SetAllCanMove(false);

        List<GameObject> list = new List<GameObject>();
        list.Add(this.gameObject);
        PlayMusicEventContext music = PlayMusicEventContext.FadeAllOut_2s;
        SceneSwitchEventContext context = new("Combat Scene", 2, music, false, list);
        GameManager.SetGameState(GAME_STATE.BATTLE);
        onSwitch.Raise(context);
    }

    public void EndCombat(CombatEndEventContextSO context)
    {
        if (context.won && enemyInitiated)
            Destroy(enemyInitiated);

        enemyInitiated = null;

        
        
        OverworldDisableManager.EnableAllObjects(scene);
        SceneManager.SetActiveScene(scene);
        SceneManager.UnloadSceneAsync("Combat Scene");

        overworldCamera.tag = "MainCamera";
        overworldCamera.enabled = true;
        GameManager.SetAllCanMove(true);
        GameManager.SetGameState(GAME_STATE.OVERWORLD);
    }

    public void AddLootToInventory(CombatEndEventContextSO context)
    {

    }

    private void OnEnable()
    {
        initializeConbatEventChannel.Subscribe(StartCombat);
        combatEndEventChannel.Subscribe(EndCombat);
    }

    private void OnDisable()
    {
        initializeConbatEventChannel.Unsubscribe(StartCombat);
        combatEndEventChannel.Unsubscribe(EndCombat);
    }
}
