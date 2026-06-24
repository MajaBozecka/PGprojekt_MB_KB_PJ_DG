using UnityEngine;
using UnityEngine.EventSystems; 

public class HoverSlideUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private RectTransform rectTransform;

    [Header("Ustawienia Pozycji (Anchored Position)")]
    [Tooltip("Pozycja X i Y, gdy przycisk jest schowany w rogu.")]
    public Vector2 hiddenPosition;

    [Tooltip("Pozycja X i Y, na któr¹ przycisk ma wyjechaæ po najechaniu.")]
    public Vector2 visiblePosition;

    [Header("Prêdkoœæ Animacji")]
    public float slideSpeed = 10f;

    private bool isHovered = false;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        rectTransform.anchoredPosition = hiddenPosition;
    }

    void Update()
    {
        Vector2 targetPosition = isHovered ? visiblePosition : hiddenPosition;

        rectTransform.anchoredPosition = Vector2.Lerp(rectTransform.anchoredPosition, targetPosition, Time.deltaTime * slideSpeed);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovered = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;
    }
}