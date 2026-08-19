
using System;
using UnityEngine;
using Utility.VisualDictionary;
using Game.Inventory;


[Serializable]
public class UserData
{

    [field: Header("Inventory")]
    [field: SerializeField]
    public Inventory Inventory { get; set; }


    [field: Header("Flags")]
    [field: SerializeField]
    public VisualDict<string, bool> Flags { get; set; }


    [field: Header("Statistics")]
    [field: SerializeField]
    public VisualDict<string, int> Statistics { get; set; }


#if UNITY_EDITOR

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
