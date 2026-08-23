using Game;
using Game.Inventory;
using UnityEngine;

[DefaultExecutionOrder(-99999)]
public class GameIniter : MonoBehaviour
{
    [SerializeField] GameObject player;
    [SerializeField] GameObject follower;

    private void Awake()
    {
        GameManager.SetPlayer(player);
        GameManager.SetFollower(follower);
        InventoryManager.ManageInventory(GameManager.CurrentUserData.Inventory);
    }

#if UNITY_EDITOR

    private void OnValidate()
    {
        Awake();
    }
#endif
}
