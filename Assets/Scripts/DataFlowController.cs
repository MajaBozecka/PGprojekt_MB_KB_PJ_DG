using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class DataFlowController : MonoBehaviour
{
    public DataFlowSO data;
    public CanvasController canvasCtrl;
    public float defaultTimeTillNextText = 3f;
    public float defaultTimeTillTextSkippable = 0.5f;
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
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InputSystem.actions.FindAction("Submit").performed += OnSubmitPerformed;
        InputSystem.actions.FindAction("Submit").canceled += OnSubmitCancel;
    }

    // Update is called once per frame
    void Update()
    {
        if (SKIP!=canvasCtrl.isSkippingIconVisible)
        {
            canvasCtrl.setSkippingIconVisibility(SKIP);
        }
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
    public void OnButtonTest(int index)
    {
        StopAllCoroutines();
        canvasCtrl.setUIMode(EUIMode.DIALOGUE);
        StartCoroutine(DialogueFlow());
    }

    IEnumerator DialogueFlow()
    {
        foreach (string item in data.testStrings)
        {
            canvasCtrl.setDialogueText(item);
            yield return new WaitForSeconds(defaultTimeTillTextSkippable);
            float i = defaultTimeTillTextSkippable;
            while (i < defaultTimeTillNextText)
            {
                if (SKIP) break;
                i += Time.unscaledDeltaTime;
                yield return null;
            }
        }
        SKIP = false;
        canvasCtrl.setUIMode(EUIMode.BUTTONS);
    }
}
