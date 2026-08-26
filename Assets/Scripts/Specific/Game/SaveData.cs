using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ItemSaveEntry
{
    public string itemID;
    public uint count;
    public ItemSaveEntry() { }//shimi
    public ItemSaveEntry(string itemID, uint count)
    {
        this.itemID = itemID;
        this.count = count;
    }
}

[Serializable]
public class FlagSaveEntry
{
    public string key;
    public bool value;

    public FlagSaveEntry() { }

    public FlagSaveEntry(string key, bool value)
    {
        this.key = key;
        this.value = value;
    }
}

[Serializable]
public class StatSaveEntry
{
    public string key;
    public int value;
    public StatSaveEntry() { }
    public StatSaveEntry(string key, int value)
    {
        this.key = key;
        this.value = value;
    }
}

[Serializable]
public class SaveData
{
    [Header("Save System")]
    public double playTime;
    public int saveSlotIndex;
    public string lastCheckpointID = "";
    public string lastSceneName = "";

    [Header("Buddy")]
    public string equipedBuddyID = "";

    [Header("Battle")]
    public int maxHP;
    public int maxCurse;
    public int currentHP;
    public int currentCurse;

    [Header("Inventory")]
    public int shellCurrency;
    public string equipedWeaponID = "";
    public List<string> equipedAccessoryIDs = new();
    public List<ItemSaveEntry> itemList = new();

    [Header("Flags & Statistics")]
    public List<FlagSaveEntry> flags = new();
    public List<StatSaveEntry> statistics = new();
}