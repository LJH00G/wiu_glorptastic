using Game.SO.EventChannel.Context;
using System;
using UnityEngine;
using Utility.Math;

namespace Game.TextMarkup
{
    public abstract class TextMarkupEffect
    {
        public abstract void ApplyEffect(TextMarkupTypeWriter writer, ref CharacterVertex[] vertices, float dt);
    }


    [Serializable]
    public class SpeechTextMarkupEffect : TextMarkupEffect
    {
        public AudioClip speechSFX;

        [SerializeField, DisplayOnly] bool played = false;

        public override void ApplyEffect(TextMarkupTypeWriter writer, ref CharacterVertex[] vertices, float dt)
        {
            if (played)
                return;

            played = true;
            writer.speechSFXEventChannel.Raise(new PlaySFXEventContext(speechSFX));
        }
    }


    [Serializable]
    public class ColorTextMarkupEffect : TextMarkupEffect
    {
        public Color color = Color.white;
        public Color fadeColor = Color.white;

        public override void ApplyEffect(TextMarkupTypeWriter writer, ref CharacterVertex[] vertices, float dt)
        {
            for (int i = 0; i < vertices.Length; i++)
            {
                var vertex = vertices[i];
                vertex.color = i < 2 ? fadeColor : color;
                vertices[i] = vertex;
            }
        }

    }


    [Serializable]
    public class RainbowTextMarkupEffect : TextMarkupEffect
    {
        public float speed = 1;
        public float offset = 0.1f;

        [SerializeField, DisplayOnly] float timer = 0;

        public override void ApplyEffect(TextMarkupTypeWriter writer, ref CharacterVertex[] vertices, float dt)
        {
            timer += dt * speed;

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

    }


    [Serializable]
    public class OffsetTextMarkupEffect : TextMarkupEffect
    {
        public Vector2 offset = Vector2.zero;

        public override void ApplyEffect(TextMarkupTypeWriter writer, ref CharacterVertex[] vertices, float dt)
        {
            for (int i = 0; i < vertices.Length; i++)
            {
                var vertex = vertices[i];
                vertex.position += offset;
                vertices[i] = vertex;
            }
        }

    }


    [Serializable]
    public class SizeTextMarkupEffect : TextMarkupEffect
    {
        public Vector2 size = Vector2.one;

        public override void ApplyEffect(TextMarkupTypeWriter writer, ref CharacterVertex[] vertices, float dt)
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

    }


    [Serializable]
    public class ShakeTextMarkupEffect : TextMarkupEffect
    {
        public float maxNormalTime = 1;
        public float persistTime = 1;
        public Vector2 offsetRange = Vector2.one;

        [SerializeField, DisplayOnly] float timer = 0;
        [SerializeField, DisplayOnly] bool hasShake = true;
        [SerializeField, DisplayOnly] Vector2 offset = Vector2.zero;

        public override void ApplyEffect(TextMarkupTypeWriter writer, ref CharacterVertex[] vertices, float dt)
        {
            timer -= dt;

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
                    timer += maxNormalTime;
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

    }


    [Serializable]
    public class OscillateTextMarkupEffect : TextMarkupEffect
    {
        public Vector2 frequency = Vector2.one;
        public Vector2 amplitude = Vector2.one;
        public Vector2 oscillateOffset = Vector2.zero;
        public float offset = 0.1f;

        [SerializeField, DisplayOnly] float timer = 0;

        public override void ApplyEffect(TextMarkupTypeWriter writer, ref CharacterVertex[] vertices, float dt)
        {
            timer += dt;

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

    }
}