using Game.SO.Data.Shop;
using Game.SO.EventChannel;
using UnityEngine;

public class ShopBaseController : MonoBehaviour
{
    [Header("Event Broadcasting Channel")]
    [SerializeField] ShopPurchaseEventChannelSO shopPurchaseEventChannel;

    [Header("ShopPreset")]
    [field: SerializeField]
    public ShopPresetSO Preset { get; private set; }


    public void TryMakeDeal(int index)
    {
        if (index < 0 || index >= Preset.TradeTable.Count)
            return;

        shopPurchaseEventChannel.Raise(Preset.TradeTable[index]);

    }


}
