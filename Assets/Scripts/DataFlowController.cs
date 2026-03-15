using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataFlowController : MonoBehaviour
{
    public DataFlowSO data;
    public CanvasController canvasCtrl;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
            yield return new WaitForSeconds(3);
        }
        canvasCtrl.setUIMode(EUIMode.BUTTONS);
    }
}
