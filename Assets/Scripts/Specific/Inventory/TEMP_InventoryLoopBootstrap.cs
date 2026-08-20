using UnityEngine;
using Game;

namespace Game.Inventory
{
    /// <summary>
    /// TEMPORARY stand-in for the real save/load flow. creates a fresh, empty UserData
    /// and links it into GameManager + InventoryManager on scene start, so the
    /// inventory UI loop (InventoryManager -> InventoryUI -> InventoryItemUI) can be
    /// tested end-to-end in a single scene before the player/shop systems exist.
    /// <br/><br/>
    /// DELETE THIS once real save/load exists and calls GameManager.SetUserData() +
    /// InventoryManager.ManageInventory() itself.
    /// </summary>
    [DefaultExecutionOrder(-9999)] // runs before InventoryUI/InventoryManager's default-order Awake/OnEnable
    public class TEMP_InventoryLoopBootstrap : MonoBehaviour
    {
        [Header("Target")]
        [SerializeField] InventoryManager inventoryManager;

        void Awake()
        {
            UserData userData = new UserData
            {
                Inventory = new Inventory()
            };

            GameManager.SetUserData(userData);
            inventoryManager.ManageInventory(userData.Inventory);

            Debug.Log("TEMP_InventoryLoopBootstrap.Awake() | created a throwaway UserData and linked it into GameManager + InventoryManager");
        }
    }
}
