using System;
using TMPro;
using UnityEngine;

namespace Game.TextMarkup
{
    public abstract class TextMarkupEffect
    {
        public abstract void ApplyEffect(ref CharacterVertex[] vertices);
    }


    [Serializable]
    public class SpeechTextMarkupEffect : TextMarkupEffect
    {
        public AudioClip speechSFX;

        public override void ApplyEffect(ref CharacterVertex[] vertices)
        {
            throw new System.NotImplementedException();
        }
    }


    [Serializable]
    public class ColorTextMarkupEffect : TextMarkupEffect
    {
        public Color color = Color.white;
        public Color fadeColor = Color.white;

        public override void ApplyEffect(ref CharacterVertex[] vertices)
        {
            throw new System.NotImplementedException();
        }

    }


    [Serializable]
    public class RainbowTextMarkupEffect : TextMarkupEffect
    {
        public float speed = 1;
        public float offset = 0.1f;

        public override void ApplyEffect(ref CharacterVertex[] vertices)
        {
            throw new System.NotImplementedException();
        }

    }


    [Serializable]
    public class OffsetTextMarkupEffect : TextMarkupEffect
    {
        public Vector2 offset = Vector2.zero;

        public override void ApplyEffect(ref CharacterVertex[] vertices)
        {
            throw new System.NotImplementedException();
        }

    }


    [Serializable]
    public class SizeTextMarkupEffect : TextMarkupEffect
    {
        public Vector2 size = Vector2.one;

        public override void ApplyEffect(ref CharacterVertex[] vertices)
        {
            throw new System.NotImplementedException();
        }

    }


    [Serializable]
    public class ShakeTextMarkupEffect : TextMarkupEffect
    {
        public float maxNormalTime = 1;
        public float persistTime = 1;
        public Vector2 offsetRange = Vector2.one;

        public override void ApplyEffect(ref CharacterVertex[] vertices)
        {
            throw new System.NotImplementedException();
        }

    }


    [Serializable]
    public class OscillateTextMarkupEffect : TextMarkupEffect
    {
        public Vector2 Strength = Vector2.one;
        public float offset = 0.1f;

        public override void ApplyEffect(ref CharacterVertex[] vertices)
        {
            throw new System.NotImplementedException();
        }

    }
}