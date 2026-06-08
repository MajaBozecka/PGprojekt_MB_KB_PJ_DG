using System;
using System.Collections.Generic;
using UnityEngine.Identifiers;

[System.Serializable]
public struct PlotCheckpoint : IComparable<PlotCheckpoint>, IComparer<PlotCheckpoint>
{
    public string id;
    public byte checkpointField;
    public PlotCheckpoint(int val, string id)
    {
        this.id = id;
        checkpointField = (byte)val;
    }
    public int Compare(PlotCheckpoint x, PlotCheckpoint y)
    {
        return string.Compare(x.id, y.id);
    }
    public int CompareTo(PlotCheckpoint other)
    {
        return Compare(this, other);
    }
}
