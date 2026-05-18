using System.Collections.Generic;
using UnityEngine;

public class PanelDialogueOptions : MonoBehaviour
{
    public List<ButtonDialogueOption> buttonList = new();
    [SerializeField] private byte buttonsCount;
    [SerializeField] private GameObject buttonDialogueOptionPrefab;
    public void SetButtons(List<string> ids)
    {
        buttonsCount = (byte)ids.Count;
        for (int i = 0; i < buttonList.Count; i++)
        {
            buttonList[i].gameObject.SetActive(true);
            buttonList[i].dialogueSequenceId = ids[i];
        }
        for (int i = buttonList.Count; i < buttonsCount; i++)
        {
            ButtonDialogueOption nb = Instantiate(buttonDialogueOptionPrefab, transform).GetComponent<ButtonDialogueOption>();
            buttonList.Add(nb);
        }
        for (int i = buttonsCount; i < buttonList.Count; i++)
        {
            buttonList[i].gameObject.SetActive(false);
        }
    }
    public void flushButtonsNotRead()
    {
        foreach (ButtonDialogueOption butt in buttonList)
        {
            butt.setRead(false);
        }
    }
    public bool SelectFirst()
    {
        bool ret = buttonList.Count > 0;
        if (ret)
        {
            buttonList[0].Button.Select();
        }
        return ret;
    }
    public void SetDialogueOptionRead(string identifier)
    {
        bool match(ButtonDialogueOption b)
        {
            return b.dialogueSequenceId.Equals(identifier);
        }
        int ind = buttonList.FindIndex(match);
        if (ind >= 0)
        {
            buttonList[ind].setRead(true);
        }
    }
}
