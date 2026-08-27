using UnityEngine;
using UnityEngine.InputSystem;

public class EscMenuToggle : MonoBehaviour
{
    [SerializeField] GameObject canvas;
    void Start()
    {
    }
    void Update()
    {
        if (Keyboard.current == null)
        {
            return;
        }
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            canvas.SetActive(!canvas.activeSelf);
        }
    }
}