using Game;
using Game.Combat;
using Game.GlobalVariable;
using Game.Inventory;
using Game.SO.Data.Item;
using Game.SO.Data.Item.Sellable;
using Game.SO.EventChannel;
using Game.SO.EventChannel.Context;
using System.Collections;
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
        Debug.Log(GameManager.CurrentUserData.CurrentEquipedBuddy, this);

        PlayerLoadoutSO player = StaticGlobalVariable.PlayerLoadout;
        PartnerLoadoutSO partner = null;
        if (GameManager.CurrentUserData.CurrentEquipedBuddy)
        {
            partner = GameManager.CurrentUserData.CurrentEquipedBuddy.Loadout;
        }


        InventoryManager.TryGetItemInList<ConsumableItemSO>(out player.inventory);
        player.equippedGem = InventoryManager.TryGetItemInList(out CurseGemItemSO gem) ? gem : null;
        player.equippedWeapon = InventoryManager.GetEquipedWeapon();
        player.equippedArmor = InventoryManager.GetEquipedArmour();
        player.equippedAccessories = InventoryManager.GetEquipedAccessories().ToList();

        tunnelDataTunnel.enemyEncounterData = data;
        tunnelDataTunnel.playerLoadout = player;

        if(partner)
            tunnelDataTunnel.partnerLoadout = partner;

        enemyInitiated = data.overworldPresetationObject;
        scene = SceneManager.GetActiveScene();
        GameManager.SetAllCanMove(false);

        List<GameObject> list = new()
        {
            gameObject
        };


        GameManager.SetGameState(GAME_STATE.BATTLE);
        onSwitch.Raise(new(
            SCENE_SWITCH_SETTING.LOAD_ADDITIVE,
            "Combat Scene",
            1,
            PlayMusicEventContext.FadeAllOut_1s,
            SCENE_SWITCH_PAUSE.PAUSE_DURING_LOAD,
            true,
            list
            ));
    }

    public void EndCombat(CombatEndEventContextSO context)
    {
        if (context.state == CombatState.BATTLE_WON && enemyInitiated)
        {
            enemyInitiated.SetActive(false);
            AddLootToInventory(context);
        } else
        if(context.state == CombatState.BATTLE_LOST)
        {
            List<GameObject> list = new();
            onSwitch.Raise(new(SCENE_SWITCH_SETTING.LOAD_SEQUENTIALLY, "DeathScene", 0, PlayMusicEventContext.FadeAllOut_1s, SCENE_SWITCH_PAUSE.PAUSE_DURING_LOAD, true, list));
        }
            

      
        onSwitch.Raise(new(
            SCENE_SWITCH_SETTING.UNLOAD,
            scene.name,
            2,
            PlayMusicEventContext.FadeAllOut_2s,
            SCENE_SWITCH_PAUSE.PAUSE_AT_START,
            true
            ));

        GameManager.SetAllCanMove(true);
        GameManager.SetGameState(GAME_STATE.OVERWORLD);

        if (context.state == CombatState.FLED && enemyInitiated)
        {
            Debug.Log("Freeze Started");
            StartCoroutine(PauseEnemy(enemyInitiated));
        }

        enemyInitiated = null;
    }

    public void AddLootToInventory(CombatEndEventContextSO context)
    {
        foreach(LootData loot in context.lootCollected)
        {
            InventoryManager.AddItem(loot.item, (uint)loot.count);
        }
    }

    IEnumerator PauseEnemy(GameObject enemy)
    {
        var controller = enemy.GetComponent<EntityOverworldController>();
        controller.SetFrozen(true);

        yield return new WaitForSeconds(5);

        Debug.Log("Freeze Ended");
        controller.SetFrozen(false);
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
