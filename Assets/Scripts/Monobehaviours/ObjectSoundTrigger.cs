using UnityEngine;
using UnityEngine.EventSystems; 

public class ObjectSoundTrigger : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler
{
    private DataFlowController audioHub;

    void Start()
    {
        audioHub = FindFirstObjectByType<DataFlowController>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (audioHub != null) audioHub.PlayHoverSound();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (audioHub != null) audioHub.PlayClickSound();
    }
}