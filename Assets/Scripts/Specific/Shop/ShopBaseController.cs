using Game.GlobalVariable;
using Game.Inventory;
using Game.SO.Data.Shop;
using Game.SO.EventChannel.Context;
using UnityEngine;

public class ShopBaseController : MonoBehaviour
{
    [Header("ShopPreset")]
    [field: SerializeField]
    public ShopPresetSO Preset { get; private set; }


    public bool TryMakeDeal(int index)
    {
        if (index < 0 || index >= Preset.TradeTable.Length)
            return false;

        return InventoryManager.TryShopPurchase(ref Preset.TradeTable[index]);
    }

    public void SetPreset(ShopPresetSO preset)
    {
        Preset = preset;
    }


}
