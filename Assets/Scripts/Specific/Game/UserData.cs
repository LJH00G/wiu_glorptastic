
using Game.Inventory;
using System;
using UnityEngine;
using Utility.VisualizableDictionary;


[Serializable]
public class UserData
{

    [field: Header("Inventory")]
    [field: SerializeField]
    public Inventory Inventory { get; set; }


    [field: Header("Flags")]
    [field: SerializeField]
    public VisualizableDict<string, bool> Flags { get; set; }


    [field: Header("Statistics")]
    [field: SerializeField]
    public VisualizableDict<string, int> Statistics { get; set; }


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
