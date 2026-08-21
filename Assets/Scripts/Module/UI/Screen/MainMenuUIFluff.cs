using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MainMenuUIFluff : MonoBehaviour
{
    [SerializeField] private GameObject[] buttons;
    [SerializeField] private float rotationStep = 45f;
    [SerializeField] private float rotationSpeed = 5f;

    private int currentIndex = 0;
    private float targetAngle;

    void Start()
    {
        ShowOnly(currentIndex);
    }

    void Update()
    {
        if (Keyboard.current[Key.Q].wasPressedThisFrame)
        {
            Cycle(-1);
        }
        if (Keyboard.current[Key.E].wasPressedThisFrame)
        {
            Cycle(1);
        }
        if (Keyboard.current[Key.Space].wasPressedThisFrame)
        {
            TriggerCurrent();
        }
        float z = transform.eulerAngles.z;
        transform.rotation = Quaternion.Euler(0f, 0f, Mathf.LerpAngle(z, targetAngle, Time.deltaTime * rotationSpeed));
    }

    void Cycle(int direction)
    {
        currentIndex = (currentIndex + direction + buttons.Length) % buttons.Length;
        targetAngle += direction * rotationStep;
        ShowOnly(currentIndex);
    }

    void TriggerCurrent()
    {
        var button = buttons[currentIndex].GetComponent<Button>();
        if (button != null && button.interactable)
        {
            button.onClick.Invoke();
        }
    }

    void ShowOnly(int index)
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].SetActive(i == index);

        }
    }
}