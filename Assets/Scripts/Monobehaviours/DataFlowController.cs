using NUnit.Framework.Internal;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class DataFlowController : MonoBehaviour
{
    public DataFlowSO data;
    public CanvasController canvasCtrl;
    [SerializeField] SpriteRenderer background;
    public ObjectWithDialogueInteraction testedObjectWithDialogueInteraction;
    [SerializeField]
    private bool pause=false;
    private bool historyLook=false;
    private bool confirmNextLine;
    private bool SKIP
    {
        get
        {
            return data.skipping;
        }
        set 
        {
            data.skipping = value;
        }
    }
    private float tillDialogueLineSkippable
    {
        get { return data.defaultTimeTillTextSkippable; }
    }
    private float tillSubDialogueLineSkippable
    {
        get { return data.defaultTimeTillSubTextSkippable; }
    }
    // Start is called once before the first execution of UpdateCkeckpointFields after the MonoBehaviour is created
    void Start()
    {
        InputAction skip = InputSystem.actions.FindAction("SKIP");
        skip.performed += OnSkipPerformed;
        skip.canceled += OnSkipCanceled;
        InputAction confirm = InputSystem.actions.FindAction("Submit");
        confirm.canceled += OnSubmitCancel;
        InputAction history = InputSystem.actions.FindAction("History");
        history.started += OnHistoryLookUp;
        data.LoadFromJSON();
        //data.easeUpMemoryByFreeingListCollection();
        populateCanvasWithButtons();
        setBackgroundImage();
        canvasCtrl.dialogueHistoryRewrite();
        ////////////////////////////////////////////////////////
        ///This one to make sure for testing those were not yet read. In the future we need to think how to register on savefile which were and which were not read
        foreach (var item in data.dialogueSequenceHashSet)
        {
            item.runnedAlready = false;
        }
    }

    // UpdateCkeckpointFields is called once per frame
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
        if(historyLook || pause)
        {
            Time.timeScale = 0;
        }else
        {
            if(!pause & !historyLook)
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

    private void populateCanvasWithButtons()
    {
        canvasCtrl.setDialogueOptions(testedObjectWithDialogueInteraction.listOfDialogueOptions, StartDialogueSequence);
        canvasCtrl.updateDialogueOptions(data.listPlotCheckpoints);
    }

    private void setBackgroundImage()
    {
        background.sprite = Resources.Load<Sprite>("Sprites/" + $"{data.backgroundImagePath}");
    }

    public void StartDialogueSequence(DialogueOptionData DOD)
    {
        StopAllCoroutines();
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
                float timeLineIterator = 0;
                int compoundLength = 0;
                canvasCtrl.setDialogueSequence(fullDialogueLine.dumpWholeLine(),data.getSpeaker(fullDialogueLine.speakerID));
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
                        if (updateDisplay & (partLineDisplayedLength > 0 & partLineDisplayedLength <= partLine.subline.Length))
                        {
                            canvasCtrl.showDialogueText(compoundLength + partLineDisplayedLength);
                        }
                        yield return null;
                    } while (!(canSkipNow(timePartLineIterator, tillSubDialogueLineSkippable) || isItTimeForNextSubLine(partLine, partLineDisplayedLength)));
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
            bool finishDetected = false;
            if(!TestedDialogueSequence.runnedAlready)
            {
                updatePlotCheckpoints();
                canvasCtrl.updateDialogueOptions(data.listPlotCheckpoints);
                TestedDialogueSequence.runnedAlready = true;
                finishDetected=data.PlotCheckpointCheckForChapterEnd();
            }
            if(finishDetected)
                Debug.Log("We should trigger chapter swap...somehow");
            canvasCtrl.UIMode = EUIMode.BUTTONS;
            SKIP = false;
        }
        bool isItTimeForNextSubLine(SubDialogueLine partLine, int partLineDisplayedLength)
        {
            return confirmNextLine || partLineDisplayedLength > partLine.subline.Length;
        }
        bool isItTimeForNextLine(DialogueLine line, float lingerIterator)
        {
            return confirmNextLine || (line.lingering>=0 & lingerIterator >= line.lingering);
        }
        bool canSkipNow(float timeLineIterator, float tillWhat)
        {
            return SKIP & timeLineIterator > tillWhat;
        }
        void updatePlotCheckpoints()
        {
            foreach (PlotCheckpoint check in DOD.updatedCheckpointValues)
            {
                int indexOfMatchedCheckpointFromControl = data.listPlotCheckpoints.BinarySearch(check);
                if (indexOfMatchedCheckpointFromControl < 0)
                    continue;
                if(check.isAdditive)
                {
                    PlotCheckpoint tempDOD = new PlotCheckpoint(data.listPlotCheckpoints[indexOfMatchedCheckpointFromControl].checkpointField+1,check.id, data.listPlotCheckpoints[indexOfMatchedCheckpointFromControl].isAdditive);
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
}
