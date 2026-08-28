using Game;
using Game.SO.Data.Buddy;
using Game.SO.EventChannel;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Utility.Math;

public class PartnerSwicherController : MonoBehaviour
{
    [Header("event")]
    [SerializeField] EventChannelSO onNewSaveloadedEvent;

    [Header("Btns")]
    [SerializeField] Button noOneBtn;
    [SerializeField] Image noOneImage;
    [SerializeField] BuddyDataSO noOneBuddyData;

    [SerializeField] string noahLedgeFlagKey;
    [SerializeField] string noahLedgeEquipeFlagKey;
    [SerializeField] Button noahLedgeBtn;
    [SerializeField] Image noahLedgeImage;
    [SerializeField] BuddyDataSO noahBuddyData;

    [SerializeField] string dinoMikeFlagKey;
    [SerializeField] string dinoMikeEquipeFlagKey;
    [SerializeField] Button dinoMikeBtn;
    [SerializeField] Image dinoMikeImage;
    [SerializeField] BuddyDataSO dinoBuddyData;

    [SerializeField] string piaoFuFlagKey;
    [SerializeField] string piaoFuEquipeFlagKey;
    [SerializeField] Button piaoFuBtn;
    [SerializeField] Image piaoFuImage;
    [SerializeField] BuddyDataSO piaoBuddyData;


    [field: Header("Animation")]
    [field: SerializeField]
    public bool Show { get; private set; }
    [SerializeField] float showPosY;
    [SerializeField] float hidePosY;
    [SerializeField] float animTime;
    float animTime_inv;
    [SerializeField] float animTimer;


    RectTransform rectForm;
    CanvasGroup cGroup;


    public void ShowUI(bool value)
    {
        Debug.Log($"trying to set partner switch ui {value}", this);
        Show = value;
        animTimer = Mathf.Max(animTime - animTimer, 0);
    }

    public void EquipNoOne()
    {
        GameManager.SetFlag(noahLedgeEquipeFlagKey, false);
        GameManager.SetFlag(dinoMikeEquipeFlagKey, false);
        GameManager.SetFlag(piaoFuEquipeFlagKey, false);
        GameManager.CurrentUserData.SetCurrentBuddy(noOneBuddyData);
    }

    public void EquipNoah()
    {
        if (!GameManager.CurrentUserData.Flags[noahLedgeFlagKey])
            return;

        GameManager.SetFlag(noahLedgeEquipeFlagKey, true);
        GameManager.SetFlag(dinoMikeEquipeFlagKey, false);
        GameManager.SetFlag(piaoFuEquipeFlagKey, false);
        GameManager.CurrentUserData.SetCurrentBuddy(noahBuddyData);
    }

    public void EquipDino()
    {
        if (!GameManager.CurrentUserData.Flags[dinoMikeFlagKey])
            return;

        GameManager.SetFlag(noahLedgeEquipeFlagKey, false);
        GameManager.SetFlag(dinoMikeEquipeFlagKey, true);
        GameManager.SetFlag(piaoFuEquipeFlagKey, false);
        GameManager.CurrentUserData.SetCurrentBuddy(dinoBuddyData);
    }

    public void EquipPiao()
    {
        if (!GameManager.CurrentUserData.Flags[piaoFuFlagKey])
            return;

        GameManager.SetFlag(noahLedgeEquipeFlagKey, false);
        GameManager.SetFlag(dinoMikeEquipeFlagKey, false);
        GameManager.SetFlag(piaoFuEquipeFlagKey, true);
        GameManager.CurrentUserData.SetCurrentBuddy(piaoBuddyData);
    }

    bool EquipedAnyBuddy()
    {
        return
            GameManager.CurrentUserData.Flags[noahLedgeEquipeFlagKey] ||
            GameManager.CurrentUserData.Flags[dinoMikeEquipeFlagKey] ||
            GameManager.CurrentUserData.Flags[piaoFuEquipeFlagKey];

    }


    void EnsureFlags()
    {
        GameManager.EnsureFlag(noahLedgeFlagKey);
        GameManager.EnsureFlag(noahLedgeEquipeFlagKey);

        GameManager.EnsureFlag(dinoMikeFlagKey);
        GameManager.EnsureFlag(dinoMikeEquipeFlagKey);

        GameManager.EnsureFlag(piaoFuFlagKey);
        GameManager.EnsureFlag(piaoFuEquipeFlagKey);
    }


    private void Awake()
    {
        rectForm = GetComponent<RectTransform>();
        cGroup = GetComponent<CanvasGroup>();

        Show = false;

        var rectPos = rectForm.anchoredPosition;
        rectPos.y = hidePosY;
        rectForm.anchoredPosition = rectPos;

        cGroup.alpha = 0;

        animTimer = animTime;
        animTime_inv = 1 / animTime;
    }

    private void Start()
    {
        EnsureFlags();

        Debug.Log($"showPosY: {showPosY}, hidePosY: {hidePosY}", this);
    }

    private void Update()
    {

        if (GameManager.GameState == GAME_STATE.OVERWORLD && GameManager.OverworldState == OVERWORLD_STATE.GENERAL)
        {
            if (Keyboard.current[Key.P].wasPressedThisFrame)
                ShowUI(!Show);
        }
        else if (Show)
            ShowUI(false);

        noOneImage.gameObject.SetActive(true);
        noahLedgeBtn.gameObject.SetActive(GameManager.CurrentUserData.Flags[noahLedgeFlagKey]);
        dinoMikeBtn.gameObject.SetActive(GameManager.CurrentUserData.Flags[dinoMikeFlagKey]);
        piaoFuBtn.gameObject.SetActive(GameManager.CurrentUserData.Flags[piaoFuFlagKey]);

        noOneImage.color = !EquipedAnyBuddy() ? Color.white : Color.gray;
        noahLedgeImage.color = GameManager.CurrentUserData.Flags[noahLedgeEquipeFlagKey] ? Color.white : Color.gray;
        dinoMikeImage.color = GameManager.CurrentUserData.Flags[dinoMikeEquipeFlagKey] ? Color.white : Color.gray;
        piaoFuImage.color = GameManager.CurrentUserData.Flags[piaoFuEquipeFlagKey] ? Color.white : Color.gray;

        if (animTimer < animTime)
        {
            animTimer += Time.deltaTime;
            
            var rectPos = rectForm.anchoredPosition;

            float startY = Show ? hidePosY : showPosY;
            float targetY = Show ? showPosY : hidePosY;

            float startAlpha = Show ? 0f : 1f;
            float targetAlpha = Show ? 1f : 0f;

            float t = Mathf.Clamp01(animTimer * animTime_inv);
            t = Math_Ease.Ease(EASE.IN_OUT_SIN, t);

            rectPos.y = Mathf.Lerp(startY, targetY, t);
            cGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, t);

            rectForm.anchoredPosition = rectPos;
        }


    }

    private void OnEnable()
    {
        onNewSaveloadedEvent.Subscribe(EnsureFlags);
    }

    private void OnDisable()
    {
        onNewSaveloadedEvent.Unsubscribe(EnsureFlags);
    }



#if UNITY_EDITOR

    private void OnValidate()
    {
        rectForm = GetComponent<RectTransform>();
        cGroup = GetComponent<CanvasGroup>();

        if (animTime != 0)
            animTime_inv = 1 / animTime;

        var pos = rectForm.anchoredPosition;
        pos.y = Show ? showPosY : hidePosY;
        rectForm.anchoredPosition = pos;
    }
#endif

}
