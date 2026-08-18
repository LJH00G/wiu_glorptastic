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
            throw new System.NotImplementedException();
        }
    }


    [Serializable]
    public class WaitTextMarkupCommand : TextMarkupCommand
    {
        public float time = 1;

        public override void TriggerCommand(TextMarkupTypeWriter writer)
        {
            throw new System.NotImplementedException();
        }

    }


    [Serializable]
    public class IntervalTextMarkupCommand : TextMarkupCommand
    {
        public float time = 0.2f;

        public override void TriggerCommand(TextMarkupTypeWriter writer)
        {
            throw new System.NotImplementedException();
        }

    }


    [Serializable]
    public class ContinueTextMarkupCommand : TextMarkupCommand
    {

        public override void TriggerCommand(TextMarkupTypeWriter writer)
        {
            throw new System.NotImplementedException();
        }

    }


    [Serializable]
    public class InputTextMarkupCommand : TextMarkupCommand
    {

        public override void TriggerCommand(TextMarkupTypeWriter writer)
        {
            throw new System.NotImplementedException();
        }

    }


    [Serializable]
    public class SFXTextMarkupCommand : TextMarkupCommand
    {
        public AudioClip sfx;

        public override void TriggerCommand(TextMarkupTypeWriter writer)
        {
            throw new System.NotImplementedException();
        }

    }


    [Serializable]
    public class UnmarkTextMarkupCommand : TextMarkupCommand
    {
        public string text = "";

        public override void TriggerCommand(TextMarkupTypeWriter writer)
        {
            throw new System.NotImplementedException();
        }

    }
}