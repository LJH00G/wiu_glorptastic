using Game.SO.EventChannel;
using Game.SO.EventChannel.Context;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeathScreenAnimator : MonoBehaviour
{
    [Header("Event Broadcasting Channel")]
    [SerializeField] SceneSwitchEventChannelSO sceneSwitchEventChannel;

    [Header("Death Text")]
    [SerializeField] TextMeshProUGUI deathText;
    [SerializeField] CanvasGroup deathTextCGroup;
    [SerializeField] float deathTextAppearTime;
    float deathTextAppearTimer;

    [Header("Button")]
    [SerializeField] Button loadSaveBtn;
    [SerializeField] Button sceneSwitchBtn;
    [SerializeField] CanvasGroup ButtonCGroup;
    [SerializeField] float buttonDelayTime;
    float buttonDelayTimer;
    [SerializeField] float buttonAppearTime;
    float buttonAppearTimer;



    public void SwitchMenu()
    {
        sceneSwitchEventChannel.Raise(new SceneSwitchEventContext(
            SCENE_SWITCH_SETTING.LOAD_SEQUENTIALLY,
            "MainMenu",
            1,
            PlayMusicEventContext.FadeAllOut_1s,
            SCENE_SWITCH_PAUSE.PAUSE_AT_START,
            true
            ));
    }


    public void LoadSave()
    {




        
    }


    private void Awake()
    {
        


    }

    private void Update()
    {
        float dt = Time.deltaTime;

        deathTextAppearTimer += dt;
        buttonDelayTimer += dt;
        buttonAppearTimer += dt;
        

        

    }


}
