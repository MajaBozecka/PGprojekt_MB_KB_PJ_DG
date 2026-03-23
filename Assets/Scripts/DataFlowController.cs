using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class DataFlowController : MonoBehaviour
{
    public DataFlowSO data;
    public CanvasController canvasCtrl;
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
    private float tillNextText
    {
        get { return data.defaultTimeTillNextText; }
    }
    private float tillSkippable
    {
        get { return data.defaultTimeTillTextSkippable; }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InputSystem.actions.FindAction("Submit").performed += OnSubmitPerformed;
        InputSystem.actions.FindAction("Submit").canceled += OnSubmitCancel;
        populateCanvasWithButtons();
    }

    // Update is called once per frame
    void Update()
    {
        SkippingVisibility();
    }


    private void OnSubmitPerformed(InputAction.CallbackContext context)
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
        foreach (DialogueLine line in data.seq[index].lines)
        {
            canvasCtrl.setDialogueText(line.halfLine[0].halfline);
            yield return new WaitForSeconds(tillSkippable);
            float i = tillSkippable;
            while (i < tillNextText)
            {
                if (SKIP) break;
                i += Time.unscaledDeltaTime;
                yield return null;
            }
        }
        canvasCtrl.setUIMode(EUIMode.BUTTONS);
        SKIP = false;
    }
}
