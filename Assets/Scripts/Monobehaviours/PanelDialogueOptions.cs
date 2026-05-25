using System;
using System.Collections.Generic;
using UnityEngine;

public class PanelDialogueOptions : MonoBehaviour
{
    public List<ButtonDialogueOption> buttonList = new();
    [SerializeField] private byte buttonsCount;
    [SerializeField] private GameObject buttonDialogueOptionPrefab;
    [SerializeField] private Transform ContentTransform;
    public void SetButtons(List<DialogueOptionData> dod, Action<string> onClickHandler)
    {
        buttonsCount = (byte)dod.Count;
        for (int i = buttonList.Count; i < buttonsCount; i++)
        {
            ButtonDialogueOption nb = Instantiate(buttonDialogueOptionPrefab, ContentTransform).GetComponent<ButtonDialogueOption>();
            buttonList.Add(nb);
        }
        for (int i = 0; i < buttonsCount; i++)
        {
            ButtonDialogueOption b = buttonList[i];
            b.gameObject.SetActive(true);
            b.dialogueSequenceId = dod[i].identifier;
            b.setText(dod[i].buttonText);
            b.onClick = onClickHandler;
        }
        for (int i = buttonsCount; i < buttonList.Count; i++)
        {
            buttonList[i].gameObject.SetActive(false);
        }
    }
    public void flushButtonsNotRead()///////////////////////////////////////////////////////////
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
