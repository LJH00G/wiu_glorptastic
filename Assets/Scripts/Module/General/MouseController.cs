using Utility;

using UnityEngine;
using UnityEngine.InputSystem;
using Utility.Math;

public class MouseController
{

    static public Vector2 ScreenPos { get; private set; }
    static public Vector3 WorldPos { get; private set; }

    static public Vector2 Delta { get => Mouse.current.delta.ReadValue(); }

    static public bool Locked { get => Cursor.lockState == CursorLockMode.Locked; }

    static public void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    static public void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    static public void Update()
    {
        ScreenPos = Mouse.current.position.ReadValue();
        ScreenPos = Math_Vec.Clamp(ScreenPos, Vector2.zero, new Vector2(Screen.width, Screen.height));
        if (Camera.main)
            WorldPos = Camera.main.ScreenToWorldPoint(ScreenPos);
    }
}
