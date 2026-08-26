using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ConfirmationPopup : MonoBehaviour
{
    [SerializeField] GameObject panelRoot;
    [SerializeField] TMP_Text messageText;
    [SerializeField] Button yesButton;
    [SerializeField] Button noButton;

    Action onConfirm;

    void Awake()
    {
        yesButton.onClick.AddListener(HandleYes);
        noButton.onClick.AddListener(HandleNo);
        panelRoot.SetActive(false);
    }

    public void Show(string message, Action onConfirm)
    {
        messageText.text = message;
        this.onConfirm = onConfirm;
        panelRoot.SetActive(true);
    }

    void HandleYes()
    {
        panelRoot.SetActive(false);
        var callback = onConfirm;
        onConfirm = null;
        callback?.Invoke();
    }

    void HandleNo()
    {
        panelRoot.SetActive(false);
        onConfirm = null;
    }
}
