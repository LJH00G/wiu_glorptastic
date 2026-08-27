using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Game;

public class PlayerHUD : MonoBehaviour
{
    [Header("Visibility")]
    [SerializeField] GameObject hudRoot;       // the visual HUD content, separate from this script's GameObject

    [Header("Text")]
    [SerializeField] TMP_Text hpText;
    [SerializeField] TMP_Text curseText;
    [SerializeField] TMP_Text shellText;

    [Header("Sliders")]
    [SerializeField] Slider hpSlider;
    [SerializeField] Slider curseSlider;

    void Update()
    {
        bool showHUD = GameManager.GameState == GAME_STATE.OVERWORLD;


        if (hudRoot)
        {
            hudRoot.SetActive(showHUD);
        }
        if (!showHUD)
        {
            return; 
        }
        UserData userData = GameManager.CurrentUserData;
        if (userData == null)
        {
            return;
        }
        var battle = userData.PlayerBattleData;
        int shells = userData.Inventory.ShellCurrency;

        if (hpText)
        {
            hpText.text = $"{battle.CurrentHP} / {battle.MaxHP}";
        }
        if (curseText)
        {
            curseText.text = $"{battle.CurrentCurse} / {battle.MaxCurse}";
        }
        if (shellText)
        {
            shellText.text = shells.ToString();
        }
        if (hpSlider)
        {
            hpSlider.maxValue = battle.MaxHP;
            hpSlider.value = battle.CurrentHP;
        }

        if (curseSlider)
        {
            curseSlider.maxValue = battle.MaxCurse;
            curseSlider.value = battle.CurrentCurse;
        }
    }
}