using Game.SO.EventChannel.Context;
using System;
using UnityEngine;

namespace Game.TextMarkup
{
    public abstract class TextMarkupCommand
    {
        public abstract void TriggerCommand(TextMarkupTypeWriter writer);
    }


    [Serializable]
    public class BrTextMarkupCommand : TextMarkupCommand
    {
        public override void TriggerCommand(TextMarkupTypeWriter writer)
        {
            throw new NotImplementedException();
        }
    }


    [Serializable]
    public class WaitTextMarkupCommand : TextMarkupCommand
    {
        public float time = 1;

        public override void TriggerCommand(TextMarkupTypeWriter writer)
        {
            writer.WaitTime = time;
        }

    }


    [Serializable]
    public class IntervalTextMarkupCommand : TextMarkupCommand
    {
        static public readonly float DEFAULT_INTERVAL = 0.075f;

        public float time = DEFAULT_INTERVAL;

        public override void TriggerCommand(TextMarkupTypeWriter writer)
        {
            writer.PrintInterval = time;
        }

    }


    [Serializable]
    public class EndTextMarkupCommand : TextMarkupCommand
    {

        public override void TriggerCommand(TextMarkupTypeWriter writer)
        {
            Debug.Log("triggered end command");
            writer.ReachedEnd = true;
        }

    }


    [Serializable]
    public class InputTextMarkupCommand : TextMarkupCommand
    {

        public override void TriggerCommand(TextMarkupTypeWriter writer)
        {
            writer.WaitForInput = true;
        }

    }


    [Serializable]
    public class SFXTextMarkupCommand : TextMarkupCommand
    {
        public AudioClip sfx;

        public override void TriggerCommand(TextMarkupTypeWriter writer)
        {
            if (sfx)
                writer.SFXEventChannel.Raise(new PlaySFXEventContext(sfx));
        }

    }


    [Serializable]
    public class UnmarkTextMarkupCommand : TextMarkupCommand
    {
        public string text = "";

        public override void TriggerCommand(TextMarkupTypeWriter writer)
        {
            throw new NotImplementedException();
        }

    }
}