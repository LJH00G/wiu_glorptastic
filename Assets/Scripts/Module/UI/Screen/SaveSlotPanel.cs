using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SaveSlotPanel : MonoBehaviour
{
    [Header("Slot")]
    [SerializeField] int slotIndex;

    [Header("UI References")]
    [SerializeField] TMP_Text statusText;
    [SerializeField] Button loadButton;
    [SerializeField] Button newGameButton;

    SaveFlowController saveFlow;
    ConfirmationPopup confirmationPopup;

    void Awake()
    {
        loadButton.onClick.AddListener(OnLoadClicked);
        newGameButton.onClick.AddListener(OnNewGameClicked);
    }

    public void Setup(SaveFlowController saveFlow, ConfirmationPopup confirmationPopup)
    {
        this.saveFlow = saveFlow;
        this.confirmationPopup = confirmationPopup;
        Refresh();
    }

    public void Refresh()
    {
        bool hasSave = saveFlow.SlotHasSave(slotIndex);
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
        saveFlow.LoadSave(slotIndex);
    }

    void OnNewGameClicked()
    {
        if (saveFlow.SlotHasSave(slotIndex))
        {
            confirmationPopup.Show($"Slot {slotIndex + 1} already has a save. Start a new game and overwrite it?", () => saveFlow.StartNewSave(slotIndex));
        }
        else
        {
            saveFlow.StartNewSave(slotIndex);
        }
    }

    string FormatPlayTime(double seconds)
    {
        var t = System.TimeSpan.FromSeconds(seconds);
        return $"{(int)t.TotalHours:00}:{t.Minutes:00}:{t.Seconds:00}";
    }
}
