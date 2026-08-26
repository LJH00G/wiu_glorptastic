using UnityEngine;
using UnityEngine.InputSystem;
using Game;

public class TempSaveTestKey : MonoBehaviour
{
    static TempSaveTestKey instance;

    void Awake()
    {
        if (instance && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        if (Keyboard.current == null)
            return;

        if (Keyboard.current.f5Key.wasPressedThisFrame)
        {
            GameManager.CurrentUserData.SetCheckpoint("test_checkpoint");
            SaveManager.Save(SaveManager.FromUserData(GameManager.CurrentUserData));
        }
    }
}