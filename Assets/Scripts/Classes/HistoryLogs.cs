using System;
using System.Collections.Generic;

[Serializable]
/// <summary>
/// Class that stores dialogue sequence ids of sequences that were read, keeping their read order.
/// The field 'progrssInLastRead' is meant to show which line we reached last time.
/// </summary>
public class HistoryLogs
{
    public List<string> dialogueSequenceIdentifiersList = new();
    public int progressInLastRead = -1;
}