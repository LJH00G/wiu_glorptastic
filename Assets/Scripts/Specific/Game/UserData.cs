
using Game;
using Game.Combat.Integration;
using Game.Inventory;
using Game.SO.Data.Buddy;
using System;
using UnityEngine;
using Utility.VisualizableDictionary;


[Serializable]
public class UserData
{
    [field: Header("Save System")]
    [field: SerializeField, DisplayOnly]
    public double PlayTime { get; set; }
    [field: SerializeField, DisplayOnly]
    public int SaveSlotIndex { get; private set; }


    [field: Header("Buddy")]
    [field: SerializeField]
    public BuddyDataSO CurrentEquipedBuddy { get; private set; }


    [field: Header("Battle")]
    [field: SerializeField]
    public PlayerBattleData PlayerBattleData { get; set; } = new();


    [field: Header("Inventory")]
    [field: SerializeField]
    public Inventory Inventory { get; set; } = new();


    [field: Header("Flags")]
    [field: SerializeField]
    public VisualizableDict<string, bool> Flags { get; set; } = new();


    [field: Header("Statistics")]
    [field: SerializeField]
    public VisualizableDict<string, int> Statistics { get; set; } = new();


    public void SetCurrentBuddy(BuddyDataSO buddyData)
    {
        if (!buddyData)
            return;

        CurrentEquipedBuddy = buddyData;
        GameManager.Follower.GetComponent<EntityOverworldController>().SetBehaviour(CurrentEquipedBuddy.OverworldBehaviour);
    }

    public UserData Clone(int saveSlotIndex)
    {
        UserData cloned = new();
        cloned.PlayTime = PlayTime;
        cloned.SaveSlotIndex = saveSlotIndex;

        cloned.CurrentEquipedBuddy = CurrentEquipedBuddy;

        cloned.PlayerBattleData = new(PlayerBattleData);

        cloned.Inventory = new(Inventory);
        
        cloned.Flags.dict = new(Flags.dict);
        
        cloned.Statistics.dict = new(Statistics.dict);
        
        return cloned;
    }


#if UNITY_EDITOR //this stuff only exists in editor

    public void OnUpdate_IfUnityEditor()
    {
        Flags.InverseValidate();
        Statistics.InverseValidate();
    }

    public void OnValidate() 
    {
        Inventory.OnValidate();
        Flags.OnValidate();
        Statistics.OnValidate();
    }

#endif
}
