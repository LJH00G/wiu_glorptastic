using UnityEngine;
using Game.SO.Data.Buddy;
using Game;

public class DespawnOnBuddyRecruited : MonoBehaviour
{
    [SerializeField] BuddyDataSO buddyData;

    void Update()
    {
        if (GameManager.CurrentUserData.CurrentEquipedBuddy != buddyData)
            return;

        if (GameManager.Follower)
            GameManager.Follower.transform.position = transform.position;

        gameObject.SetActive(false);
    }
}
