using UnityEngine;

public class TriggerSettingsMAINMENU : MonoBehaviour
{
    [SerializeField] GameObject mainMenuCanvas;
    [SerializeField] GameObject settingsCanvas;

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
}