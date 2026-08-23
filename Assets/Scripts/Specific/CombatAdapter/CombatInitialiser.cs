using Game;
using Game.Combat;
using Game.GlobalVariable;
using Game.Inventory;
using Game.SO.Data.Item;
using Game.SO.EventChannel;
using Game.SO.EventChannel.Context;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CombatInitialiser : MonoBehaviour
{

    [SerializeField] InitializeCombatEventChannelSO initializeConbatEventChannel;
    [SerializeField] CombatDataTunnelSO tunnelDataTunnel;
    [SerializeField] SceneSwitchEventChannelSO onSwitch;


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

        PlayMusicEventContext music = PlayMusicEventContext.FadeAllOut_2s;
        SceneSwitchEventContext context = new("Combat Scene", 2, music);

        onSwitch.Raise(context);
    }

    private void OnEnable()
    {
        initializeConbatEventChannel.Subscribe(StartCombat);
    }

    private void OnDisable()
    {
        initializeConbatEventChannel.Unsubscribe(StartCombat);
    }
}
