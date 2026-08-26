using System.Collections.Generic;
using UnityEngine;
using Game.SO.Data.Buddy;

[CreateAssetMenu(fileName = "BuddyDatabase", menuName = "Scriptable Objects/Data/BuddyDatabaseSO")]
public class BuddyDatabaseSO : ScriptableObject
{
    [SerializeField] List<BuddyDataSO> allBuddies = new();

    public BuddyDataSO GetByID(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return null;
        }
        return allBuddies.Find(buddy => buddy.name == id);
    }
}
