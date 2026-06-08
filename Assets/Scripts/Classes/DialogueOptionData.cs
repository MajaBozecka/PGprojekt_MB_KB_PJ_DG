using System.Collections.Generic;
[System.Serializable]
public class DialogueOptionData
{
    public string identifier;
    public bool read;
    public string buttonText;
    public List<PlotCheckpoint> requirements;
    public List<PlotCheckpoint> updatedCheckpointValues;
}
