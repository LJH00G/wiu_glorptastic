using Game.SO.Data.Shop;
using Game.SO.EventChannel;
using UnityEngine;

public class ShopController : MonoBehaviour
{
    [Header("Event Broadcasting Channel")]
    [SerializeField] ShopPurchaseEventChannelSO shopPurchaseEventChannel;

    [Header("ShopPreset")]
    [SerializeField] ShopPresetSO preset;


    public void TryMakeDeal(int index)
    {
        if (index < 0 || index >= preset.SellTable.Count)
            return;

        shopPurchaseEventChannel.Raise(preset.SellTable[index]);

    }


}
