using Game.SO.EventChannel;
using Game.SO.EventChannel.Context;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utility.Math;

public class DeathScreenAnimator : MonoBehaviour
{
    [Header("Event Broadcasting Channel")]
    [SerializeField] SceneSwitchEventChannelSO sceneSwitchEventChannel;

    [Header("Death Text")]
    [SerializeField] TextMeshProUGUI deathText;
    [SerializeField] CanvasGroup deathTextCGroup;
    [SerializeField] float deathTextAppearTime;
    float deathTextAppearTime_inv;
    [SerializeField] float deathTextAppearTimer;

    [Header("Button")]
    [SerializeField] Button loadSaveBtn;
    [SerializeField] Button sceneSwitchBtn;
    [SerializeField] CanvasGroup ButtonCGroup;
    [SerializeField] float buttonDelayTime;
    [SerializeField] float buttonDelayTimer;
    [SerializeField] float buttonAppearTime;
    float buttonAppearTime_inv;
    [SerializeField] float buttonAppearTimer;



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
        deathTextCGroup.alpha = 0;
        ButtonCGroup.alpha = 0;

        deathTextAppearTime_inv = 1 / deathTextAppearTime;
        buttonAppearTime_inv = 1 / buttonAppearTime;

    }

    private void Update()
    {
        float dt = Time.deltaTime;

        deathTextAppearTimer += dt;
        buttonDelayTimer += dt;

        if (deathTextAppearTimer < deathTextAppearTime)
        {
            float t = Math_Ease.Ease(EASE.IN_OUT_SIN, deathTextAppearTimer * deathTextAppearTime_inv);
            deathTextCGroup.alpha = t;
            deathText.color = Color.Lerp(Color.white, Color.red, t);
        }

        if (buttonDelayTimer > buttonDelayTime)
        {
            buttonAppearTimer += dt;
            if (buttonAppearTimer < buttonAppearTime)
            {
                ButtonCGroup.alpha = Math_Ease.Ease(EASE.IN_OUT_SIN, buttonAppearTimer * buttonAppearTime_inv);
            }
        }
    }


}
