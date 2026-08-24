using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Inventory
{
    public class InventoryHotkey : MonoBehaviour
    {
        [SerializeField] Key hotkey = Key.I;

        void Update()
        {
            if (Keyboard.current != null && Keyboard.current[hotkey].wasPressedThisFrame)
            {
                Toggle();
            }
        }
        //hello glorptastic teammates its 2am am losing my mand
        void Toggle()
        {
            if (!InventoryUI.Instance)
            {
                return;
            }
            if (InventoryUI.Instance.IsOpen)
            {
                InventoryUI.Instance.Hide();
            }
            else
            {
                InventoryUI.Instance.Show();
            }
        }
    }
}
