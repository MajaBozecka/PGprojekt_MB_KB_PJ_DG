using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class PanelDialogueSequenceController : MonoBehaviour
{
    public GameObject proceedIcon;
    public GameObject panel
    {
        get
        {
            return gameObject;
        }
    }

    [SerializeField]
    private TMP_Text dialogueText;

    [Header("Miejsca na postacie (Scena)")]
    [SerializeField]
    private Image[] characterSlots;

    private Dictionary<string, Image> activeCharacters = new Dictionary<string, Image>();

    public string textToShowInDialogueField;
    public void fillDialogueField(int n)
    {
        if(n<0)
        {
            dialogueText.text = textToShowInDialogueField;
        }
        else
        {
            dialogueText.text = textToShowInDialogueField[..(n)];
        }
    }
    public void SpeakerCustomization(Speaker s, string speakerMod)
    {
        dialogueText.fontSize = s.fontSize;
        dialogueText.fontStyle = s.styles;
        dialogueText.color = s.color;

        if (s != null)
        {
            if (speakerMod == "Exit" || speakerMod == "Hide")
            {
                if (activeCharacters.ContainsKey(s.speakerId))
                {
                    activeCharacters[s.speakerId].gameObject.SetActive(false);
                    activeCharacters.Remove(s.speakerId);
                }
                return;
            }
                
            Sprite foundSprite = s.GetSpriteByMod(speakerMod);
            if (foundSprite != null)
            {
                if (activeCharacters.ContainsKey(s.speakerId))
                {
                    activeCharacters[s.speakerId].sprite = foundSprite;
                }
                else
                {
                    Image freeSlot = GetFreeSlot();
                    if(freeSlot != null)
                    {
                        freeSlot.gameObject.SetActive(true);
                        activeCharacters.Add(s.speakerId, freeSlot);
                    }
                    else
                    {
                        Debug.LogWarning("Brak wolnych miejsc na scenie");
                    }
                }
                   
            }
        }
    }
    private Image GetFreeSlot()
    {
        foreach (Image slot in characterSlots)
        {
            if (!slot.gameObject.activeSelf) return slot;
        }
        return null;
    }
    public void ClearStage()
    {
        foreach (Image slot in characterSlots)
        {
            slot.gameObject.SetActive(false);
        }
        activeCharacters.Clear();
    }
}
