using UnityEngine;

public class SaveSlotMenuController : MonoBehaviour
{
    [SerializeField] SaveFlowController saveFlowController;
    [SerializeField] ConfirmationPopup confirmationPopup;
    [SerializeField] SaveSlotPanel[] slotPanels; // exactly 3, one per slot GameObject

    void OnEnable()
    {
        foreach (var panel in slotPanels)
            panel.Setup(saveFlowController, confirmationPopup);
    }
}
