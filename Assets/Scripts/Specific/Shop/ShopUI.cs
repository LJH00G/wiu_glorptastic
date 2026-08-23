using UnityEngine;

public class ShopUI : MonoBehaviour
{
    [Header("Source")]
    [SerializeField] ShopBaseController shopController;

    [Header("Slots (must be exactly 9)")]
    [SerializeField] ShopSlotUI[] slots = new ShopSlotUI[9];

    [Header("Confirm Panel")]
    [SerializeField] ShopPurchaseConfirmUI confirmUI;

    [Header("Panel (optional - leave empty if this UI is always visible)")]
    [SerializeField] GameObject panelRoot;

    public static ShopUI Instance { get; private set; }

    public bool IsOpen => panelRoot ? panelRoot.activeSelf : gameObject.activeSelf;

    void Awake()
    {
        if (Instance && Instance != this)
        {
            Debug.LogWarning("ShopUI.Awake() | more than one ShopUI exists in the scene, Instance will point at the most recently loaded one");
        }
        Instance = this;

        foreach (var slot in slots)
        {
            slot.OnSlotClicked += HandleSlotClicked;
        }
    }

    void OnEnable()
    {
        PopulateSlots();
    }

    public void Show()
    {
        if (panelRoot)
        {
            panelRoot.SetActive(true);
        }
        PopulateSlots();
    }

    public void Hide()
    {
        if (panelRoot)
        {
            panelRoot.SetActive(false);
        }
    }

    void PopulateSlots()
    {
        var tradeTable = shopController.Preset.TradeTable;

        if (tradeTable.Count != slots.Length)
        {
            Debug.LogWarning($"ShopUI.PopulateSlots() | expected exactly {slots.Length} trades in the preset, found {tradeTable.Count}");
        }
        for (int i = 0; i < slots.Length; i++)
        {
            bool hasTrade = i < tradeTable.Count;
            slots[i].gameObject.SetActive(hasTrade);

            if (hasTrade)
            {
                slots[i].SetTrade(i, tradeTable[i]);
            }
        }
    }

    void HandleSlotClicked(int tradeIndex)
    {
        var tradeTable = shopController.Preset.TradeTable;
        if (tradeIndex < 0 || tradeIndex >= tradeTable.Count)
        {
            return;
        }
        confirmUI.Show(tradeTable[tradeIndex], () => shopController.TryMakeDeal(tradeIndex));
    }
}