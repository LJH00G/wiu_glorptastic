using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Game.SO.EventChannel;

public class SaveSlotPanel : MonoBehaviour
{
    [SerializeField] IntEventChannelSO startNewSaveEventChannel;
    [SerializeField] IntEventChannelSO loadSaveEventChannel;

    [Header("Slot")]
    [SerializeField] int slotIndex;

    [Header("UI References")]
    [SerializeField] TMP_Text statusText;
    [SerializeField] Button loadButton;
    [SerializeField] Button newGameButton;

    ConfirmationPopup confirmationPopup;

    void Awake()
    {
        loadButton.onClick.AddListener(OnLoadClicked);
        newGameButton.onClick.AddListener(OnNewGameClicked);
    }

    public void Setup(ConfirmationPopup confirmationPopup)
    {
        this.confirmationPopup = confirmationPopup;
        Refresh();
    }

    public void Refresh()
    {
        bool hasSave = SaveManager.HasSave(slotIndex);
        loadButton.interactable = hasSave;

        if (!hasSave)
        {
            //statusText.text = "Empty Slot";
            return;
        }

        SaveData data = SaveManager.Load(slotIndex);
        //statusText.text = $"Play Time: {FormatPlayTime(data.playTime)}";
    }

    void OnLoadClicked()
    {
        loadSaveEventChannel.Raise(slotIndex);
    }

    void OnNewGameClicked()
    {
        if (SaveManager.HasSave(slotIndex))
        {
            confirmationPopup.Show($"Slot {slotIndex + 1} already has a save. Start a new game and overwrite it?", () => startNewSaveEventChannel.Raise(slotIndex));
        }
        else
        {
            startNewSaveEventChannel.Raise(slotIndex);
        }
    }

    string FormatPlayTime(double seconds)
    {
        var t = System.TimeSpan.FromSeconds(seconds);
        return $"{(int)t.TotalHours:00}:{t.Minutes:00}:{t.Seconds:00}";
    }
}
