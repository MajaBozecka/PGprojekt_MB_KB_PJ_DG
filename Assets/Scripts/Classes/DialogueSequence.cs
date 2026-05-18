using System;
using System.Collections.Generic;

/// <summary>
/// We store all dialogue lines in the sequence
/// Some dialogue lines might be autoskip, while others need confirmation
/// </summary>
[Serializable]
public class DialogueSequence : IComparable<DialogueSequence>, IComparer<DialogueSequence>
{
    public List<DialogueLine> lines = new List<DialogueLine>();
    public string identifier;
    public bool runnedAlready = false;
    public override int GetHashCode()
    {
        return identifier.GetHashCode();
    }

    public int CompareTo(DialogueSequence other)
    {
        if (other == null)
            return -1;
        return String.Compare(identifier, other.identifier);
    }

    public int Compare(DialogueSequence x, DialogueSequence y)
    {
        return x.CompareTo(y);
    }

    public DialogueSequence() { }
    public DialogueSequence(string id) { identifier = id; }

    public void CopyTo(DialogueSequence target)
    {
        if (target is null) return;
        target.lines.Clear();
        target.lines.AddRange(lines);
        target.identifier = identifier;
        target.runnedAlready = runnedAlready;
    }

/*    public override bool Equals(object other)
    {
        if (other is DialogueSequence t)
        {
            return t.identifier == this.identifier;
        }
        return false;
    }*/
}
[Serializable]
/// <summary>
/// Idividual dialogue lines can be partial for dramatic pause
/// </summary>
public class DialogueLine
{
    /// <summary>
    /// We store the timeForSingleCharDisplay it takes for the line to be said and how long the line is.
    /// We can insert empty strings, this way we will insert a pause, that we can detect.
    /// </summary>
    public List<SubDialogueLine> subLines = new List<SubDialogueLine>();
    /// <summary>
    /// how long should the text remain seen after it is finished
    /// negative values can be used to claim it's meant to be confirmed
    /// </summary>
    public float lingering;
    public string speakerID;
    public string dumpWholeLine()
    {
        string ret = "";
        foreach (SubDialogueLine subLine in subLines)
        {
            ret += subLine.subline;
        }
        return ret;
    }
}
[Serializable]
public struct SubDialogueLine
{
    public float timeForSingleCharDisplay;
    public string subline;
    public SubDialogueLine(string s)
    {
        timeForSingleCharDisplay = -1;
        subline = s;
    }
}
