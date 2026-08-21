using UnityEngine;
using Game.Inventory;

public class ShopUI : MonoBehaviour
{
    [Header("Source")]
    [SerializeField] ShopBaseController shopController;

    [Header("Slots (must be exactly 9)")]
    [SerializeField] ShopSlotUI[] slots = new ShopSlotUI[9];

    [Header("Confirm Panel")]
    [SerializeField] ShopPurchaseConfirmUI confirmUI;

    [Header("Sell")]
    [SerializeField] InventoryUI inventoryUI;

    void Awake()
    {
        foreach (var slot in slots)
            slot.OnSlotClicked += HandleSlotClicked;
    }

    void OnEnable()
    {
        PopulateSlots();
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
            if (i < tradeTable.Count)
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
        confirmUI.Show(tradeIndex, tradeTable[tradeIndex]);
    }

    // hook this up to the shop's Sell button OnClick
    public void OpenSellInventory()
    {
        if (inventoryUI)
        {
            inventoryUI.Show(sellModeEnabled: true);
        }
    }
}
