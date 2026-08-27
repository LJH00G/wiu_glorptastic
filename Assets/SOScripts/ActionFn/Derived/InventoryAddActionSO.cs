using System;
using UnityEngine;
using Game.SO.Data.Item;
using Game.Inventory;

namespace Game.SO.ActionFn
{
    [CreateAssetMenu(fileName = "InventoryAction", menuName = "Scriptable Objects/ActionFn/InventoryAction")]
    public class InventoryAddActionSO : ActionSO
    {
        public ItemSO item;
        public uint amt;
        public override void Invoke()
        {
            InventoryManager.AddItem(item, amt);
        }
    }
    
}