using System.Collections.Generic;
using UnityEngine;
using Game.SO.Data.Item;

[CreateAssetMenu(fileName = "ItemDatabase", menuName = "Scriptable Objects/Data/ItemDatabaseSO")]
public class ItemDatabaseSO : ScriptableObject
{
    [SerializeField] List<ItemSO> allItems = new();

    public ItemSO GetByID(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return null;
        }
        return allItems.Find(item => item.Name == id);
    }
}
