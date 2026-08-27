using Game;
using Game.SO.Data.TextMarkup.Dialogue;
using Game.SO.EventChannel;
using Game.SO.EventChannel.Context;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class TestController : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current[Key.Backquote].wasPressedThisFrame)
            foreach (var entry in GameManager.CurrentUserData.Flags.dict)
            {
                Debug.Log(entry);
            }
    }
}
