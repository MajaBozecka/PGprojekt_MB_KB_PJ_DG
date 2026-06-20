using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Identifiers;

[System.Serializable]
public struct SpeakerSprite
{
    public string modName;
    public Sprite portrait;
}
[System.Serializable]
public class Speaker : IComparable<string>, IComparer<Speaker>
{
    public string speakerId;
    public string speakerName;
    public float fontSize;
    public FontStyles styles;
    public Color color;
    public float timePerCharacterTalking;
    public List<SpeakerSprite> sprites = new List<SpeakerSprite>();
    [Header("Wygl¹d UI")]
    public Sprite customTextboxGraphic;
    public int CompareTo(string other)
    {
        if (other == null)
            return -1;
        return String.Compare(speakerId, other);
    }

    public int Compare(Speaker x, Speaker y)
    {
        return x.CompareTo(y.speakerId);
    }

    public Sprite GetSpriteByMod(string mod)
    {
        if (string.IsNullOrEmpty(mod)) mod = "Neutral"; // Domyœlna emocja

        foreach (var sprite in sprites)
        {
            if (sprite.modName == mod) return sprite.portrait;
        }
        return null;
    }

}
