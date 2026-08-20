using Game;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using Game.SO.EventChannel;
using Game.SO.EventChannel.Context;

public class UIButtonHandler : MonoBehaviour
{
    [SerializeField] SceneSwitchEventChannelSO thingalingus;
    [SerializeField] SceneSwitchEventContext grongelator;


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

    public void SwitchScene()
    {
        thingalingus.Raise(grongelator);
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
