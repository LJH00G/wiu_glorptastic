using System;
using TMPro;
using UnityEngine;


public abstract class DialogueTextEffect
{
    public abstract void ApplyEffect(TMP_Text text);
}


[Serializable]
public class SpeechDialogueTextEffect : DialogueTextEffect
{
    public AudioClip speechSFX;

    public override void ApplyEffect(TMP_Text text)
    {
        throw new System.NotImplementedException();
    }
}


[Serializable]
public class ColorDialogueTextEffect : DialogueTextEffect
{
    public Color color = Color.white;
    public Color fadeColor = Color.white;

    public override void ApplyEffect(TMP_Text text)
    {
        throw new System.NotImplementedException();
    }

}


[Serializable]
public class RainbowDialogueTextEffect : DialogueTextEffect
{
    public float speed = 1;
    public float offset = 0.1f;

    public override void ApplyEffect(TMP_Text text)
    {
        throw new System.NotImplementedException();
    }

}


[Serializable]
public class OffsetDialogueTextEffect : DialogueTextEffect
{
    public Vector2 offset = Vector2.zero;

    public override void ApplyEffect(TMP_Text text)
    {
        throw new System.NotImplementedException();
    }

}


[Serializable]
public class SizeDialogueTextEffect : DialogueTextEffect
{
    public Vector2 size = Vector2.one;

    public override void ApplyEffect(TMP_Text text)
    {
        throw new System.NotImplementedException();
    }

}


[Serializable]
public class ShakeDialogueTextEffect : DialogueTextEffect
{
    public float maxNormalTime = 1;
    public float persistTime = 1;
    public Vector2 offsetRange = Vector2.one;

    public override void ApplyEffect(TMP_Text text)
    {
        throw new System.NotImplementedException();
    }

}


[Serializable]
public class OscillateDialogueTextEffect : DialogueTextEffect
{
    public Vector2 Strength = Vector2.one;
    public float offset = 0.1f;

    public override void ApplyEffect(TMP_Text text)
    {
        throw new System.NotImplementedException();
    }

}
