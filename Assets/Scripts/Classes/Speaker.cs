using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Identifiers;
[System.Serializable]
public class Speaker : IComparable<string>, IComparer<Speaker>
{
    public string speakerId;
    public string speakerName;
    public float fontSize;
    public FontStyles styles;
    public Color color;
    public float timePerCharacterTalking;
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

}
