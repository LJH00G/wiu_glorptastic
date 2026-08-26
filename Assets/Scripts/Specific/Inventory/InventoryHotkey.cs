using Game.SO.Data.Shop;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using Utility.VisualizableDictionary;

namespace Game.Inventory
{
    public class UIHotkey : MonoBehaviour
    {
        [SerializeField] Key hotkey = Key.I;
        [SerializeField] VisualizableDict<OVERWORLD_STATE, bool> stateDictionary;
        

        void Update()
        {

            if (Keyboard.current != null && Keyboard.current[hotkey].wasPressedThisFrame)
            {
                if(CheckGameState())
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

        bool CheckGameState()
        {
            //Checks Gamestate
            if (GameManager.GameState != GAME_STATE.OVERWORLD)
            {
                Debug.Log("GameState returned false");
                return false;
            }
                

            if (stateDictionary.dict.TryGetValue(GameManager.OverworldState, out bool confirm))
            {
                Debug.Log($"Overworld returned: {GameManager.OverworldState}");
                return confirm;
            }
                
            return false;
        }

    

      


#if UNITY_EDITOR
        void OnValidate()
        {
            stateDictionary.OnValidate();
        }
#endif
    }
}
