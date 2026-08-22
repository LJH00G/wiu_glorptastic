using Game;
using Game.Combat;
using Game.SO.Data.Item.Sellable.Battle;
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

        var inventory = GameManager.CurrentUserData.Inventory;

        player.inventory = inventory.ItemList;
        player.equippedWeapon = inventory.EquipedWeapon;
        player.equippedArmor = inventory.EquipedArmour;
        player.equippedAccessories= inventory.EquipedAccessoryList.ToList();

        tunnelDataTunnel.enemyEncounterData = data;
        tunnelDataTunnel.playerLoadout = player;

        PlayMusicEventContext music = new PlayMusicEventContext();
        SceneSwitchEventContext context = new("Combat Scene", 2, music);

        onSwitch.Raise(context);
    }
}
