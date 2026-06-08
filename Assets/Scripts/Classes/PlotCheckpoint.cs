using System.Collections.Generic;

[System.Serializable]
public struct PlotCheckpoint : IComparer<PlotCheckpoint>
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
}
