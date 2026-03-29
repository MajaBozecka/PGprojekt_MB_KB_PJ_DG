using System.Collections.Generic;

/// <summary>
/// We store all dialogue lines in the sequence
/// Some dialogue lines might be autoskip, while others need confirmation
/// </summary>
[System.Serializable]
public class DialogueSequence
{
    public List<DialogueLine> lines = new List<DialogueLine>();
}
[System.Serializable]
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
}
[System.Serializable]
public struct SubDialogueLine
{
    public float timeForSingleCharDisplay;
    public string subline;
    public SubDialogueLine(string s)
    {
        timeForSingleCharDisplay = 0.01f;
        subline = s;
    }
}