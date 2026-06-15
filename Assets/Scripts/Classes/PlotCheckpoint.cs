using System;
using System.Collections.Generic;

[System.Serializable]
public struct PlotCheckpoint : IComparable<PlotCheckpoint>, IComparer<PlotCheckpoint>
{
    public string id;
    public bool isAdditive;
    public byte checkpointField;
    public PlotCheckpoint(int val, string id, bool ad)
    {
        this.id = id;
        checkpointField = (byte)val;
        isAdditive = ad;
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
