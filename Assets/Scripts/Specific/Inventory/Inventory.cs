
using Game.SO.Data.Item.Sellable.Battle;
using System;
using System.Collections.Generic;
using UnityEngine;


namespace Game.Inventory
{
    [Serializable]
    public class Inventory
    {
        [field: SerializeField]
        public List<ItemStack> ItemList { get; set; } = new();
        [field: SerializeField]
        public int ShellCurrency { get; set; } = 0;
        [field: Header("Equiped")]
        [field: SerializeField]
        public WeaponItemSO EquipedWeapon { get; set; } = null;
        public const uint MAX_ACCESSORYIES = 3;
        [field: SerializeField]
        public AccessoryItemSO[] EquipedAccessoryList { get; set; } = new AccessoryItemSO[MAX_ACCESSORYIES];


#if UNITY_EDITOR
        public void OnValidate()
        {
            if (EquipedAccessoryList.Length != MAX_ACCESSORYIES)
                EquipedAccessoryList = new AccessoryItemSO[MAX_ACCESSORYIES];
        }
#endif
    }
}