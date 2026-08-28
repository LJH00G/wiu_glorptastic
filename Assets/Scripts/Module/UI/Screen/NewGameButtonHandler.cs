
using UnityEngine;
using Game.SO.EventChannel;

public class NewGameButtonHandler : MonoBehaviour
{
    [SerializeField] IntEventChannelSO newSaveEventChannel;


    //public static UIButtonHandler Instance { get; private set; }

    //private void Awake()
    //{
    //    if (Instance != null && Instance != this)
    //    {
    //        Destroy(gameObject);
    //        return;
    //    }

    //    Instance = this;

    //    DontDestroyOnLoad(gameObject);
    //}

    public void NewGame()
    {
        if (!SaveManager.HasSave(0))
            newSaveEventChannel.Raise(0);
        else if (!SaveManager.HasSave(1))
            newSaveEventChannel.Raise(1);
        else
            newSaveEventChannel.Raise(2);
    }

    //public void SceneOverworld()
    //{
    //    SceneManager.LoadScene("SceneOverworld");
    //}
    //public void SceneSampleSwap()
    //{
    //    SceneManager.LoadScene("SceneSampleSwap");
    //}

    //public void OptionScene()
    //{
    //    SceneManager.LoadScene("OptionScene");
    //}
    //


    //
}
