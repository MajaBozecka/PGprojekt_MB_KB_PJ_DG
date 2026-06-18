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

    [Header("Miejsca na postacie (Œwiat Gry)")]
    [SerializeField]
    private SpriteRenderer[] characterSlots;

    [Header("Ustawienia Skali")]
    public float characterScale = 1.0f;

    private Dictionary<string, SpriteRenderer> activeCharacters = new Dictionary<string, SpriteRenderer>();

    public string textToShowInDialogueField;

    public void fillDialogueField(int n)
    {
        if (n < 0)
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
        if (s == null)
        {
            Debug.LogError("B³¹d: Próba wyœwietlenia dialogu dla postaci, której nie ma w DataFlowSO!");
            return;
        }

        dialogueText.fontSize = s.fontSize;
        dialogueText.fontStyle = s.styles;
        dialogueText.color = s.color;
        if (string.IsNullOrEmpty(speakerMod)) return;

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
            if (foundSprite == null) Debug.LogWarning($"UWAGA: Nie znalaz³em grafiki dla postaci '{s.speakerId}' z emocj¹ '{speakerMod}'!");
            if (foundSprite != null)
            {

                if (activeCharacters.ContainsKey(s.speakerId))
                {
                    SpriteRenderer existingSlot = activeCharacters[s.speakerId];
                    existingSlot.sprite = foundSprite;

                    existingSlot.transform.localScale = new Vector3(characterScale, characterScale, 1f);
                }
                else
                {
                    SpriteRenderer freeSlot = GetFreeSlot();
                    if (freeSlot != null)
                    {
                        freeSlot.gameObject.SetActive(true);
                        activeCharacters.Add(s.speakerId, freeSlot);

                        freeSlot.sprite = foundSprite;

                        freeSlot.transform.localScale = new Vector3(characterScale, characterScale, 1f);
                    }
                    else
                    {
                        Debug.LogWarning("Brak wolnych miejsc na scenie");
                    }
                }
            }
        }
    }

    private SpriteRenderer GetFreeSlot()
    {
        foreach (SpriteRenderer slot in characterSlots)
        {
            if (!slot.gameObject.activeSelf) return slot;
        }
        return null;
    }

    public void ClearStage()
    {
        foreach (SpriteRenderer slot in characterSlots)
        {
            slot.gameObject.SetActive(false);
        }
        activeCharacters.Clear();
    }
}
