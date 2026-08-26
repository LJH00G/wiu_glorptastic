using Game.Inventory;
using Game.SO.EventChannel;
using Game.SO.EventChannel.Context;
using System;
using UnityEngine;



namespace Game.SO.EventChannel.Context
{
    [Serializable]
    public abstract class ToastEventContext
    {
        public Color color;
        public abstract string GetToastMessage();
    }

    [Serializable]
    public class MessageToastEventContext : ToastEventContext
    {
        public string message;
        public bool isError;
        public Sprite sprite;

        public MessageToastEventContext(string message, bool isError = false, Sprite sprite = null)
        {
            this.message = message;
            color = isError ? Color.red : Color.white;
            this.sprite = sprite;
        }

        public override string GetToastMessage()
        {
            return message;
        }
    }

    [Serializable]
    public class ItemStackToastEventContext : ToastEventContext
    {
        public ItemStack itemStack;
        public bool recievedOrlost;

        public ItemStackToastEventContext(ItemStack itemStack, bool recievedOrlost = true)
        {
            this.itemStack = itemStack;
            this.recievedOrlost = recievedOrlost;
            color = Color.white;
        }

        public override string GetToastMessage()
        {
            return
                $"you " +
                (recievedOrlost ? "<color value=\"#00FF00FF\">recieved</color>" : "<color value=\"#FF0000FF\">lost</color>") +
                $" {itemStack.item.Name} x{itemStack.count}";
        }
    }

    [Serializable]
    public class ShellToastEventContext : ToastEventContext
    {
        public int amount;
        public bool recievedOrlost;

        public ShellToastEventContext(int amount, bool recievedOrlost = true)
        {
            this.amount = amount;
            this.recievedOrlost = recievedOrlost;
            color = Color.softYellow;
        }

        public override string GetToastMessage()
        {
            return
                $"you " +
                (recievedOrlost ? "<color value=\"#00FF00FF\">recieved</color>" : "<color value=\"#FF0000FF\">lost</color>") +
                $" {amount} shell" + (amount == 0 ? "" : "s");
        }
    }
}

[CreateAssetMenu(fileName = "ToastEvent_Channel", menuName = "Scriptable Objects/EventChannel/Specific/ToastEventChannelSO")]
public class ToastEventChannelSO : EventChannelSO<ToastEventContext>
{
    
}
