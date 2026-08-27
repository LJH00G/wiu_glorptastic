using System;
using System.IO;
using UnityEngine;
using Game.Inventory;
using Game.SO.Data.Item;
using Game.SO.Data.Item.Sellable.Battle;
using Game.SO.Data.Buddy;

public static class SaveManager
{
    /// <summary>which slot new saves/loads default to when no slot is explicitly picked (e.g. by a save-select menu)</summary>
    public static int CurrentSlot = 0;

    static string GetSavePath(int slotIndex) => Path.Combine(Application.persistentDataPath, $"save_{slotIndex}.json");

    public static void Save(SaveData data)
    {
        string path = GetSavePath(data.saveSlotIndex);
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(path, json);
        Debug.Log($"SaveManager.Save() | wrote slot {data.saveSlotIndex} to {path}");
    }

    public static SaveData Load(int slotIndex)
    {
        string path = GetSavePath(slotIndex);

        if (!File.Exists(path))
        {
            Debug.Log($"SaveManager.Load() | no save found for slot {slotIndex}, returning fresh SaveData");
            return new SaveData { saveSlotIndex = slotIndex };
        }

        try
        {
            string json = File.ReadAllText(path);
            return JsonUtility.FromJson<SaveData>(json);
        }
        catch (Exception e)
        {
            Debug.LogError($"SaveManager.Load() | failed to load slot {slotIndex}: {e}");
            return new SaveData { saveSlotIndex = slotIndex };
        }
    }

    public static bool HasSave(int slotIndex) => File.Exists(GetSavePath(slotIndex));


    public static SaveData FromUserData(UserData userData)
    {
        var data = new SaveData
        {
            playTime = userData.PlayTime,
            saveSlotIndex = userData.SaveSlotIndex,
            lastCheckpointID = userData.LastCheckpointID,
            lastSceneName = userData.LastSceneName,

            equipedBuddyID = userData.CurrentEquipedBuddy ? userData.CurrentEquipedBuddy.name : "",

            maxHP = userData.PlayerBattleData.MaxHP,
            maxCurse = userData.PlayerBattleData.MaxCurse,
            currentHP = userData.PlayerBattleData.CurrentHP,
            currentCurse = userData.PlayerBattleData.CurrentCurse,

            shellCurrency = userData.Inventory.ShellCurrency,
            equipedWeaponID = userData.Inventory.EquipedWeapon ? userData.Inventory.EquipedWeapon.Name : ""
        };

        foreach (var accessory in userData.Inventory.EquipedAccessoryList)
        {
            data.equipedAccessoryIDs.Add(accessory ? accessory.Name : "");
        }
        foreach (var stack in userData.Inventory.ItemList)
        {
            data.itemList.Add(new ItemSaveEntry(stack.item ? stack.item.Name : "", stack.count));
        }
        foreach (var kvp in userData.Flags.dict)
        {
            data.flags.Add(new FlagSaveEntry(kvp.Key, kvp.Value));
        }
        foreach (var kvp in userData.Statistics.dict)
        {
            data.statistics.Add(new StatSaveEntry(kvp.Key, kvp.Value));
        }
        return data;
    }


    public static void ApplyToUserData(SaveData data, UserData userData, ItemDatabaseSO itemDatabase, BuddyDatabaseSO buddyDatabase)
    {
        userData.PlayTime = data.playTime;
        userData.SetSaveSlotIndex(data.saveSlotIndex);
        userData.SetCheckpoint(data.lastCheckpointID, data.lastSceneName);

        if (!string.IsNullOrEmpty(data.equipedBuddyID))
        {
            userData.SetCurrentBuddyData(buddyDatabase.GetByID(data.equipedBuddyID));
        }
        userData.PlayerBattleData.SetFromSave(data.maxHP, data.maxCurse, data.currentHP, data.currentCurse);

        userData.Inventory.ShellCurrency = data.shellCurrency;
        userData.Inventory.EquipedWeapon = itemDatabase.GetByID(data.equipedWeaponID) as WeaponItemSO;

        for (int i = 0; i < userData.Inventory.EquipedAccessoryList.Length && i < data.equipedAccessoryIDs.Count; i++)
        {
            userData.Inventory.EquipedAccessoryList[i] = itemDatabase.GetByID(data.equipedAccessoryIDs[i]) as AccessoryItemSO;
        }
        userData.Inventory.ItemList.Clear();
        foreach (var entry in data.itemList)
        {
            var item = itemDatabase.GetByID(entry.itemID);
            if (item)
            {
                userData.Inventory.ItemList.Add(new ItemStack(item, entry.count));
            }
        }

        userData.Flags.dict.Clear();
        foreach (var entry in data.flags)
        {
            userData.Flags.dict[entry.key] = entry.value;
        }
        userData.Statistics.dict.Clear();
        foreach (var entry in data.statistics)
        {
            userData.Statistics.dict[entry.key] = entry.value;
        }
    }
}