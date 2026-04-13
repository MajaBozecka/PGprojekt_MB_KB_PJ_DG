using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class DataFlowController : MonoBehaviour
{
    public DataFlowSO data;
    public CanvasController canvasCtrl;
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
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InputAction skip = InputSystem.actions.FindAction("SKIP");
        skip.performed += OnSkipPerformed;
        skip.canceled += OnSkipCanceled;
        InputAction confirm = InputSystem.actions.FindAction("Submit");
        confirm.canceled += OnSubmitCancel;
        InputAction history = InputSystem.actions.FindAction("History");
        history.started += OnHistoryLookUp;
        populateCanvasWithButtons();
        ////////////////////////////////////////////////////////
        ///This one to make sure for testing those were not yet read. In the future we need to think how to register on savefile which were and which were not read
        foreach (var item in data.seq)
        {
            item.runnedAlready = false;
        }
    }

    // Update is called once per frame
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
        for (int i = 0; i < 2; i++)
        {
            int ii = i;
            canvasCtrl.testButtons[i].Button.onClick.AddListener(delegate { StartDialogueSequence(ii); });//I hate lambda expressions
        }
    }

    public void StartDialogueSequence(int index)
    {
        StopAllCoroutines();
        canvasCtrl.UIMode = EUIMode.DIALOGUE;
        if (!data.seq[index].runnedAlready)
        {
            foreach (DialogueButton butt in canvasCtrl.testButtons)
            {
                if (butt.dialogueSequenceId == index)
                {
                    butt.setRead(true);
                }
            }
        }
        StartCoroutine(DialogueFlow(index));
    }

    IEnumerator DialogueFlow(int index)
    {
        foreach (DialogueLine fullDialogueLine in data.seq[index].lines)
        {
            float timeLineIterator = 0;
            int compoundLength = 0;
            canvasCtrl.setDialogueText(fullDialogueLine.dumpWholeLine());
            canvasCtrl.showDialogueText(0);
            canvasCtrl.setProceedIconVisibility(false);
            foreach (SubDialogueLine partLine in fullDialogueLine.subLines)
            {
                float timePartLineIterator = 0;
                int partLineDisplayedLength = 0;
                do
                {
                    timeLineIterator += Time.deltaTime;
                    timePartLineIterator += Time.deltaTime;
                    int newLength = (int)(timePartLineIterator / partLine.timeForSingleCharDisplay);
                    bool updateDisplay = partLineDisplayedLength != newLength;
                    partLineDisplayedLength = newLength;
                    if (updateDisplay&(partLineDisplayedLength > 0 & partLineDisplayedLength <= partLine.subline.Length))
                    {
                        canvasCtrl.showDialogueText(compoundLength + partLineDisplayedLength);
                    }
                    yield return null;
                } while (!(canSkipNow(timePartLineIterator,tillSubDialogueLineSkippable) || isItTimeForNextSubLine(partLine, partLineDisplayedLength)));
                compoundLength += partLine.subline.Length;
                confirmNextLine = false;
            }
            float lingerIterator = 0;
            canvasCtrl.showDialogueText(-1);
            if (!data.seq[index].runnedAlready)
            {
                canvasCtrl.dialogueHistoryUpdate(data.seq[index], fullDialogueLine);
            }
            while (!(canSkipNow(timeLineIterator,tillDialogueLineSkippable) || isItTimeForNextLine(fullDialogueLine, lingerIterator)))
            {
                timeLineIterator += Time.deltaTime;
                lingerIterator += Time.deltaTime;
                canvasCtrl.setProceedIconVisibility(true);
                yield return null;
            }
            confirmNextLine = false;
        }
        if (!data.seq[index].runnedAlready)
        {
            data.seq[index].runnedAlready = true;
        }
        canvasCtrl.UIMode = EUIMode.BUTTONS;
        SKIP = false;
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
    }
}
