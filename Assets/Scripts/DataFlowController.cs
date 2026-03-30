using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class DataFlowController : MonoBehaviour
{
    public DataFlowSO data;
    public CanvasController canvasCtrl;
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
        populateCanvasWithButtons();
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

    private void SkippingVisibility()
    {
        if (SKIP != canvasCtrl.isSkippingIconVisible)
        {
            canvasCtrl.setSkippingIconVisibility(SKIP);
        }
    }

    private void populateCanvasWithButtons()
    {
        canvasCtrl.testButton0.Button.onClick.AddListener(delegate { OnButtonTest(0); });
        canvasCtrl.testButton1.Button.onClick.AddListener(delegate { OnButtonTest(1); });
    }
    public void OnButtonTest(int index)
    {
        StopAllCoroutines();
        canvasCtrl.setUIMode(EUIMode.DIALOGUE);
        if(index == 1)
        {
            canvasCtrl.testButton1.setRead(true);
        }
        else
        {
            canvasCtrl.testButton0.setRead(true);
        }
        StartCoroutine(DialogueFlow(index));
    }

    IEnumerator DialogueFlow(int index)
    {
        foreach (DialogueLine fullDialogueLine in data.seq[index].lines)
        {
            float timeLineIterator = 0;
            string stringPreviousSubLines = "";
            canvasCtrl.setProceedIconVisibility(false);
            Debug.Log("started with first dialogue line in the sequence");
            foreach (SubDialogueLine partLine in fullDialogueLine.subLines)
            {
                canvasCtrl.setDialogueText(stringPreviousSubLines);
                float timePartLineIterator = 0;
                int partLineDisplayedLength = -1;
                Debug.Log($"Handling \"{partLine.subline}\"");
                do
                {
                    timeLineIterator += Time.unscaledDeltaTime;
                    timePartLineIterator += Time.unscaledDeltaTime;
                    int newLength = (int)(timePartLineIterator / partLine.timeForSingleCharDisplay);
                    bool updateDisplay = partLineDisplayedLength != newLength;
                    partLineDisplayedLength = newLength;
                    Debug.Log($"tli \"{timeLineIterator}\"tpli \"{timePartLineIterator}\",\npartLineDisplayedLength {partLineDisplayedLength}");
                    if (updateDisplay&(partLineDisplayedLength > 0 & partLineDisplayedLength <= partLine.subline.Length))
                    {
                        Debug.Log($"Should display {stringPreviousSubLines + partLine.subline[..(partLineDisplayedLength)]}");
                        canvasCtrl.setDialogueText(stringPreviousSubLines+partLine.subline[..(partLineDisplayedLength)]);
                    }
                    yield return null;
                } while (!(canSkipNow(timePartLineIterator,tillSubDialogueLineSkippable) || isItTimeForNextSubLine(partLine, partLineDisplayedLength)));
                stringPreviousSubLines += partLine.subline;
                Debug.Log($"previousSubLines \"{stringPreviousSubLines}\"");
                confirmNextLine = false;
            }
            float lingerIterator = 0;
            canvasCtrl.setDialogueText(stringPreviousSubLines);
            while (!(canSkipNow(timeLineIterator,tillDialogueLineSkippable) || isItTimeForNextLine(fullDialogueLine, lingerIterator)))
            {
                timeLineIterator += Time.unscaledDeltaTime;
                lingerIterator += Time.unscaledDeltaTime;
                canvasCtrl.setProceedIconVisibility(true);
                yield return null;
            }
            confirmNextLine = false;
        }
        canvasCtrl.setUIMode(EUIMode.BUTTONS);
        SKIP = false;
        bool isItTimeForNextSubLine(SubDialogueLine partLine, int partLineDisplayedLength)
        {
            return confirmNextLine || partLineDisplayedLength > partLine.subline.Length;
        }
        bool isItTimeForNextLine(DialogueLine line, float lingerIterator)
        {
            return confirmNextLine || (line.lingering<0 & lingerIterator >= line.lingering);
        }
        bool canSkipNow(float timeLineIterator, float tillWhat)
        {
            return SKIP & timeLineIterator > tillWhat;
        }
    }
}
