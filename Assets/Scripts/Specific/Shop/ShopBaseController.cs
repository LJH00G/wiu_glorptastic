using Game.Inventory;
using Game.SO.Data.Shop;
using UnityEngine;

public class ShopBaseController : MonoBehaviour
{
    [Header("ShopPreset")]
    [field: SerializeField]
    public ShopPresetSO Preset { get; private set; }


    public void TryMakeDeal(int index)
    {
        if (index < 0 || index >= Preset.TradeTable.Length)
            return;

        InventoryManager.HandleShopPurchase(ref Preset.TradeTable[index]);
    }


}
