using Game.SO.EventChannel.Context;
using System;
using UnityEngine;
using Utility.Math;

namespace Game.TextMarkup
{
    public abstract class TextMarkupEffect
    {
        public abstract void Update(float dt);
        public abstract void ApplyEffect(TextMarkupTypeWriter writer, ref CharacterVertex[] vertices);
        public abstract TextMarkupEffect Clone();
    }


    [Serializable]
    public class SpeechTextMarkupEffect : TextMarkupEffect
    {
        public AudioClip speechSFX;

        [SerializeField, DisplayOnly] bool played = false;

        public override void Update(float dt)
        {
            
        }
        public override void ApplyEffect(TextMarkupTypeWriter writer, ref CharacterVertex[] vertices)
        {
            if (played)
                return;

            played = true;

            if (speechSFX && !writer.WasSkipTextScrolling && writer.PrintInterval != 0)
                writer.speechSFXEventChannel.Raise(new PlaySFXEventContext(speechSFX));
        }

        public override TextMarkupEffect Clone()
        {
            SpeechTextMarkupEffect cloned = new();
            cloned.speechSFX = speechSFX;
            return cloned;
        }
    }


    [Serializable]
    public class ColorTextMarkupEffect : TextMarkupEffect
    {
        public Color color = Color.white;
        public Color fadeColor = Color.white;

        public override void Update(float dt)
        {

        }
        public override void ApplyEffect(TextMarkupTypeWriter writer, ref CharacterVertex[] vertices)
        {
            for (int i = 0; i < vertices.Length; i++)
            {
                var vertex = vertices[i];

                if (i == 1 || i == 2)
                    vertex.color = color;
                else
                    vertex.color = fadeColor;

                vertices[i] = vertex;
            }
        }

        public override TextMarkupEffect Clone()
        {
            ColorTextMarkupEffect cloned = new();
            cloned.color = color;
            cloned.fadeColor = fadeColor;
            return cloned;
        }
    }


    [Serializable]
    public class OffsetTextMarkupEffect : TextMarkupEffect
    {
        public Vector2 offset = Vector2.zero;

        public override void Update(float dt)
        {

        }
        public override void ApplyEffect(TextMarkupTypeWriter writer, ref CharacterVertex[] vertices)
        {
            for (int i = 0; i < vertices.Length; i++)
            {
                var vertex = vertices[i];
                vertex.position += offset;
                vertices[i] = vertex;
            }
        }

        public override TextMarkupEffect Clone()
        {
            OffsetTextMarkupEffect cloned = new();
            cloned.offset = offset;
            return cloned;
        }
    }


    [Serializable]
    public class SizeTextMarkupEffect : TextMarkupEffect
    {
        public Vector2 size = Vector2.one;

        public override void Update(float dt)
        {

        }
        public override void ApplyEffect(TextMarkupTypeWriter writer, ref CharacterVertex[] vertices)
        {
            Vector2 center = Vector2.zero;

            for (int i = 0; i < vertices.Length; i++)
                center += vertices[i].position;
            center *= 0.25f;

            for (int i = 0; i < vertices.Length; i++)
            {
                var vertex = vertices[i];

                vertex.position =
                    center +
                    Vector2.Scale(vertex.position - center, size);

                vertices[i] = vertex;
            }
        }

        public override TextMarkupEffect Clone()
        {
            SizeTextMarkupEffect cloned = new();
            cloned.size = size;
            return cloned;
        }
    }


    [Serializable]
    public class ShakeTextMarkupEffect : TextMarkupEffect
    {
        public float maxNormalTime = 1;
        public float persistTime = 0.05f;
        public Vector2 offsetRange = Vector2.one;

        [SerializeField, DisplayOnly] float timer = 0;
        [SerializeField, DisplayOnly] bool hasShake = true;
        [SerializeField, DisplayOnly] Vector2 offset = Vector2.zero;

        public override void Update(float dt)
        {
            timer -= dt;
        }
        public override void ApplyEffect(TextMarkupTypeWriter writer, ref CharacterVertex[] vertices)
        {

            if (timer <= 0)
            {
                hasShake = !hasShake;

                if (hasShake)
                {
                    timer += persistTime;
                    offset =new Vector2(
                        UnityEngine.Random.Range(-offsetRange.x, offsetRange.x),
                        UnityEngine.Random.Range(-offsetRange.y, offsetRange.y)
                        );
                }
                else
                    timer += UnityEngine.Random.Range(maxNormalTime * 0.1f, maxNormalTime);
            }

            if (hasShake)
            {
                for (int i = 0; i < vertices.Length; i++)
                {
                    var vertex = vertices[i];
                    vertex.position += offset;
                    vertices[i] = vertex;
                }
            }

        }

        public override TextMarkupEffect Clone()
        {
            ShakeTextMarkupEffect cloned = new();
            cloned.maxNormalTime = maxNormalTime;
            cloned.persistTime = persistTime;
            cloned.offsetRange = offsetRange;
            return cloned;
        }
    }


    public abstract class OffsetableTextMarkupEffect : TextMarkupEffect
    {
        public float offset = 0.1f;
    }


    [Serializable]
    public class RainbowTextMarkupEffect : OffsetableTextMarkupEffect
    {
        public float speed = 0.25f;

        [SerializeField, DisplayOnly] float timer = 0;

        public override void Update(float dt)
        {
            timer += dt * speed;
        }
        public override void ApplyEffect(TextMarkupTypeWriter writer, ref CharacterVertex[] vertices)
        {
            Color32 color = Color.HSVToRGB(
                Mathf.Repeat(timer + offset, 1f),
                1f,
                1f
            );

            for (int i = 0; i < vertices.Length; i++)
            {
                var vertex = vertices[i];
                vertex.color = color;
                vertices[i] = vertex;
            }
        }

        public override TextMarkupEffect Clone()
        {
            RainbowTextMarkupEffect cloned = new();
            cloned.speed = speed;
            cloned.offset = offset;
            return cloned;
        }
    }


    [Serializable]
    public class OscillateTextMarkupEffect : OffsetableTextMarkupEffect
    {
        public Vector2 frequency = Vector2.one * 4;
        public Vector2 amplitude = Vector2.one;
        public Vector2 oscillateOffset = new Vector2(0.5f, 0);

        [SerializeField, DisplayOnly] float timer = 0;

        public override void Update(float dt)
        {
            timer += dt;
        }
        public override void ApplyEffect(TextMarkupTypeWriter writer, ref CharacterVertex[] vertices)
        {
            Vector2 positionOffset = new Vector2(
                Mathf.Sin((timer + offset) * frequency.x + oscillateOffset.x * Math_C.PI) * amplitude.x,
                Mathf.Sin((timer + offset) * frequency.y + oscillateOffset.y * Math_C.PI) * amplitude.y
                );

            for (int i = 0; i < vertices.Length; i++)
            {
                var vertex = vertices[i];
                vertex.position += positionOffset;
                vertices[i] = vertex;
            }
        }

        public override TextMarkupEffect Clone()
        {
            OscillateTextMarkupEffect cloned = new();
            cloned.frequency = frequency;
            cloned.amplitude = amplitude;
            cloned.oscillateOffset = oscillateOffset;
            cloned.offset = offset;
            return cloned;
        }
    }

}