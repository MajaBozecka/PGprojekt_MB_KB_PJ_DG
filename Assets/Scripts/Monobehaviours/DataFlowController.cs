using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DataFlowController : MonoBehaviour
{
    public DataFlowSO data;
    public CanvasController canvasCtrl;
    public LocationManager locationManager;
    [SerializeField] SpriteRenderer background;
    public ObjectWithDialogueInteraction testedObjectWithDialogueInteraction;

    [SerializeField]
    private bool pause = false;
    private bool historyLook = false;
    private bool confirmNextLine;

    [Header("Interfejs Dialogu")]
    public GameObject dialogueCanvas;

    public static System.Action<string> OnSequenceEnded;
    public static System.Action OnSequenceStarted;
    public static System.Action OnConversationFinished;

    [Header("System Audio")]
    public AudioSource sfxSource;
    public AudioClip clickSound;
    public AudioClip hoverSound;

    public AudioSource musicSource;
    public AudioClip backgroundMusic;

    private bool SKIP
    {
        get { return data.skipping; }
        set { data.skipping = value; }
    }

    private float tillDialogueLineSkippable
    {
        get { return data.defaultTimeTillTextSkippable; }
    }

    private float tillSubDialogueLineSkippable
    {
        get { return data.defaultTimeTillSubTextSkippable; }
    }

    void Start()
    {
        PlayBackgroundMusic();
        InputAction skip = InputSystem.actions.FindAction("SKIP");
        skip.performed += OnSkipPerformed;
        skip.canceled += OnSkipCanceled;
        InputAction confirm = InputSystem.actions.FindAction("Submit");
        confirm.canceled += OnSubmitCancel;
        InputAction history = InputSystem.actions.FindAction("History");
        history.started += OnHistoryLookUp;

        data.LoadFromJSON();

        for (int i = 0; i < data.listPlotCheckpoints.Count; i++)
        {
            PlotCheckpoint tempCheckpoint = data.listPlotCheckpoints[i];
            tempCheckpoint.checkpointField = 0;
            data.listPlotCheckpoints[i] = tempCheckpoint;
        }

        canvasCtrl.dialogueHistoryRewrite();

        foreach (var item in data.dialogueSequenceHashSet)
        {
            item.runnedAlready = false;
        }
    }

    private void OnDestroy()
    {
        InputAction skip = InputSystem.actions.FindAction("SKIP");
        skip.performed -= OnSkipPerformed;
        skip.canceled -= OnSkipCanceled;
        InputAction confirm = InputSystem.actions.FindAction("Submit");
        confirm.canceled -= OnSubmitCancel;
        InputAction history = InputSystem.actions.FindAction("History");
        history.started -= OnHistoryLookUp;
    }

    void Update()
    {
        SkippingVisibility();
    }

    private void OnSkipPerformed(InputAction.CallbackContext context)
    {
        switch (canvasCtrl.UIMode)
        {
            case EUIMode.DIALOGUE:
                {
                    SKIP = true;
                    break;
                }
        }
    }

    private void OnSkipCanceled(InputAction.CallbackContext context)
    {
        SKIP = false;
    }

    private void OnSubmitCancel(InputAction.CallbackContext context)
    {
        switch (canvasCtrl.UIMode)
        {
            case EUIMode.BUTTONS:
                {
                    canvasCtrl.setSelect();
                    break;
                }
            case EUIMode.DIALOGUE:
                {
                    confirmNextLine = true;
                    break;
                }
        }
        SKIP = false;
    }

    private void OnHistoryLookUp(InputAction.CallbackContext context)
    {
        historyLook = !historyLook;
        pauseGameTime();
        canvasCtrl.lookUpHistory(historyLook);
    }

    private void pauseGameTime()
    {
        if (historyLook || pause)
        {
            Time.timeScale = 0;
        }
        else
        {
            if (!pause && !historyLook)
            {
                Time.timeScale = 1;
            }
        }
    }

    private void SkippingVisibility()
    {
        if (SKIP != canvasCtrl.isSkippingIconVisible)
        {
            canvasCtrl.setSkippingIconVisibility(SKIP);
        }
    }

    private int populateCanvasWithButtons()
    {
        canvasCtrl.setDialogueOptions(testedObjectWithDialogueInteraction.listOfDialogueOptions, StartDialogueSequence);
        return canvasCtrl.updateDialogueOptions(data.listPlotCheckpoints);
    }


    public void InteractWithObject(ObjectWithDialogueInteraction interactedObject)
    {
        PlayClickSound();
        testedObjectWithDialogueInteraction = interactedObject;
        DialogueOptionData validOption = interactedObject.getDOD;

        if (validOption != null)
        {
            StartDialogueSequence(validOption);
        }
    }

    public void StartDialogueSequence(DialogueOptionData DOD)
    {
        if (UnityEngine.EventSystems.EventSystem.current != null)
        {
            UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
        }

        OnSequenceStarted?.Invoke();
        if (locationManager != null) locationManager.SetDialogueState(true);
        StopAllCoroutines();
        if (dialogueCanvas != null) dialogueCanvas.SetActive(true);
        canvasCtrl.UIMode = EUIMode.DIALOGUE;
        canvasCtrl.SetDialogueOptionRead(DOD);
        StartCoroutine(DialogueFlow(DOD));
    }

    IEnumerator DialogueFlow(DialogueOptionData DOD)
    {
        DialogueSequence TestedDialogueSequence = data.getDialogueSequence(DOD.identifier);

        if (TestedDialogueSequence is not null)
        {
            foreach (DialogueLine fullDialogueLine in TestedDialogueSequence.lines)
            {
                if (locationManager != null && !string.IsNullOrEmpty(fullDialogueLine.changeLocationId))
                {
                    locationManager.ChangeLocation(fullDialogueLine.changeLocationId, false, false);
                }

                float timeLineIterator = 0;
                int compoundLength = 0;

                Speaker currentSpeaker = data.getSpeaker(fullDialogueLine.speakerID);
                canvasCtrl.setDialogueSequence(fullDialogueLine.dumpWholeLine(), currentSpeaker, fullDialogueLine.speakerMod);
                canvasCtrl.showDialogueText(0);
                canvasCtrl.setProceedIconVisibility(false);

                foreach (SubDialogueLine partLine in fullDialogueLine.subLines)
                {
                    float timePartLineIterator = 0;
                    int partLineDisplayedLength = 0;
                    int newLength;
                    do
                    {
                        timeLineIterator += Time.deltaTime;
                        timePartLineIterator += Time.deltaTime;
                        float f_timeForSingleCharDisplay = data.getTimeForSingleCharDisplayCorrected(fullDialogueLine, partLine);
                        newLength = f_timeForSingleCharDisplay <= 0 ? partLine.subline.Length : (int)(timePartLineIterator / f_timeForSingleCharDisplay);
                        bool updateDisplay = partLineDisplayedLength != newLength;
                        partLineDisplayedLength = newLength;

                        if (updateDisplay && (partLineDisplayedLength > 0 && partLineDisplayedLength <= partLine.subline.Length))
                        {
                            if (sfxSource != null && currentSpeaker != null && currentSpeaker.characterVoiceBlip != null)
                            {

                                if (!sfxSource.isPlaying)
                                {
                                    sfxSource.PlayOneShot(currentSpeaker.characterVoiceBlip);
                                }
                            }

                            canvasCtrl.showDialogueText(compoundLength + partLineDisplayedLength);
                        }
                        yield return null;
                    } while (!(canSkipNow(timePartLineIterator, tillSubDialogueLineSkippable) || isItTimeForNextSubLine(partLine, partLineDisplayedLength)));

                    if (sfxSource != null && sfxSource.isPlaying)
                    {
                        sfxSource.Stop();
                    }

                    compoundLength += partLine.subline.Length;
                    confirmNextLine = false;
                }

                float lingerIterator = 0;
                canvasCtrl.showDialogueText(-1);
                UpdateDialogueHistory(TestedDialogueSequence, fullDialogueLine);

                while (!(canSkipNow(timeLineIterator, tillDialogueLineSkippable) || isItTimeForNextLine(fullDialogueLine, lingerIterator)))
                {
                    timeLineIterator += Time.deltaTime;
                    lingerIterator += Time.deltaTime;
                    canvasCtrl.setProceedIconVisibility(true);
                    yield return null;
                }
                confirmNextLine = false;
            }
            Debug.Log("<color=cyan>[DIALOG]</color> Czy dialog byl juz wczesniej odczytany? runnedAlready = " + TestedDialogueSequence.runnedAlready);
            if (!TestedDialogueSequence.runnedAlready)
            {
                sbyte check = data.pc_CheckpointForDialogueDepth.checkpointField;
                updatePlotCheckpoints();
                sbyte check2 = data.pc_CheckpointForDialogueDepth.checkpointField;
                if (!((check != 0 && check2 == 0) || (check==0 && check2 == 1)))
                    TestedDialogueSequence.runnedAlready = true;
            }

            OnSequenceEnded?.Invoke(TestedDialogueSequence.identifier);

            if (locationManager != null) locationManager.SetDialogueState(false);

            if (testedObjectWithDialogueInteraction != null &&
                testedObjectWithDialogueInteraction.hasChoicesMenu &&
                testedObjectWithDialogueInteraction.listOfDialogueOptions != null && populateCanvasWithButtons() > 0 &&
                data.pc_CheckpointForDialogueDepth.checkpointField > 0)
            {
                canvasCtrl.UIMode = EUIMode.BUTTONS;
            }
            else
            {
                testedObjectWithDialogueInteraction = null;
                if (dialogueCanvas != null) dialogueCanvas.SetActive(false);
                canvasCtrl.UIMode = EUIMode.NOTHING;
                OnConversationFinished?.Invoke();
            }

            SKIP = false;
        }

        bool isItTimeForNextSubLine(SubDialogueLine partLine, int partLineDisplayedLength)
        {
            return confirmNextLine || partLineDisplayedLength > partLine.subline.Length;
        }

        bool isItTimeForNextLine(DialogueLine line, float lingerIterator)
        {
            return confirmNextLine || (line.lingering >= 0 && lingerIterator >= line.lingering);
        }

        bool canSkipNow(float timeLineIterator, float tillWhat)
        {
            return SKIP && timeLineIterator > tillWhat;
        }

        void updatePlotCheckpoints()
        {
            if (DOD.updatedCheckpointValues == null) return;
            foreach (PlotCheckpoint check in DOD.updatedCheckpointValues)
            {
                int indexOfMatchedCheckpointFromControl = data.listPlotCheckpoints.BinarySearch(check);

                if (indexOfMatchedCheckpointFromControl < 0)
                {
                    data.listPlotCheckpoints.Add(check);
                    data.listPlotCheckpoints.Sort();
                    continue;
                }

                if (data.listPlotCheckpoints[indexOfMatchedCheckpointFromControl].isAdditive)
                {
                    PlotCheckpoint tempDOD = new PlotCheckpoint(data.listPlotCheckpoints[indexOfMatchedCheckpointFromControl].checkpointField + check.checkpointField, check.id, data.listPlotCheckpoints[indexOfMatchedCheckpointFromControl].isAdditive);
                    data.listPlotCheckpoints[indexOfMatchedCheckpointFromControl] = tempDOD;
                }
                else
                {
                    if (data.listPlotCheckpoints[indexOfMatchedCheckpointFromControl].checkpointField != check.checkpointField)
                    {
                        data.listPlotCheckpoints[indexOfMatchedCheckpointFromControl] = check;
                    }
                }
            }
        }
    }

    private void UpdateDialogueHistory(DialogueSequence dialSeq, DialogueLine dialLine)
    {
        if (!dialSeq.runnedAlready)
        {
            if (!data.history.dialogueSequenceIdentifiersList.Contains(dialSeq.identifier))
            {
                data.history.progressInLastRead = 0;
                data.history.dialogueSequenceIdentifiersList.Add(dialSeq.identifier);
            }
            else
            {
                data.history.progressInLastRead++;
            }
            canvasCtrl.dialogueHistoryUpdate(dialSeq, dialLine);
        }
    }

    public void PlayClickSound()
    {
        if (sfxSource != null && clickSound != null)
        {
            sfxSource.PlayOneShot(clickSound);
        }
    }
    public void PlayHoverSound()
    {
        if (sfxSource != null && hoverSound != null) sfxSource.PlayOneShot(hoverSound);
    }

    public void PlayBackgroundMusic()
    {
        if (musicSource != null && backgroundMusic != null)
        {
            musicSource.clip = backgroundMusic;
            musicSource.loop = true; 
            musicSource.Play();
        }
    }

}