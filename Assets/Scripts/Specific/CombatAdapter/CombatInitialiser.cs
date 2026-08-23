using Game;
using Game.Combat;
using Game.Inventory;
using Game.SO.EventChannel;
using Game.SO.EventChannel.Context;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CombatInitialiser : MonoBehaviour
{

    [SerializeField] List<EnemyEncounterDataSO> enemyEncounterDataTable;
    [SerializeField] CombatDataTunnelSO tunnelDataTunnel;
    [SerializeField] SceneSwitchEventChannelSO onSwitch;


    public void StartCombat(EnemyEncounterDataSO data)
    {
        PlayerLoadoutSO player = ScriptableObject.CreateInstance<PlayerLoadoutSO>();
        PartnerLoadoutSO partner = ScriptableObject.CreateInstance<PartnerLoadoutSO>();

        player.inventory = InventoryManager.GetItemList();
        player.equippedWeapon = InventoryManager.GetEquipedWeapon();
        player.equippedArmor = InventoryManager.GetEquipedArmour();
        player.equippedAccessories = InventoryManager.GetEquipedAccessories().ToList();

        tunnelDataTunnel.enemyEncounterData = data;
        tunnelDataTunnel.playerLoadout = player;

        PlayMusicEventContext music = PlayMusicEventContext.FadeAllOut_2s;
        SceneSwitchEventContext context = new("Combat Scene", 2, music);

        onSwitch.Raise(context);
    }
}
