using Game;
using Game.Inventory;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        GameManager.SetGameState(GAME_STATE.OVERWORLD);
        GameManager.SetOverWorldState(OVERWORLD_STATE.GENERAL);
    }

#if UNITY_EDITOR

    private void OnValidate()
    {
        Awake();
    }
#endif
}
