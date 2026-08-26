using UnityEngine;

public class TriggerSettingsMAINMENU : MonoBehaviour
{
    [SerializeField] GameObject mainMenuCanvas;
    [SerializeField] GameObject settingsCanvas;
    [SerializeField] GameObject saveMenuPanel;

    public void OpenSettings()
    {
        mainMenuCanvas.SetActive(false);
        settingsCanvas.SetActive(true);
    }
    public void CloseSettings()
    {
        settingsCanvas.SetActive(false);
        mainMenuCanvas.SetActive(true);
    }
    public void ShowSaveMenu()
    {
        mainMenuCanvas.SetActive(false);
        saveMenuPanel.SetActive(true);
    }
    public void HideSaveMenu()
    {
        saveMenuPanel.SetActive(false);
        mainMenuCanvas.SetActive(true);
    }
}