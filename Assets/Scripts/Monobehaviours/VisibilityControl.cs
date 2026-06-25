using UnityEngine;

public class VisibilityControl : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Collider2D characterCollider;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        characterCollider = GetComponent<Collider2D>();
    }

    private void OnEnable()
    {
        DataFlowController.OnSequenceStarted += HideCharacter;
       DataFlowController.OnConversationFinished += ShowCharacter;
    }

    private void OnDisable()
    {
        DataFlowController.OnSequenceStarted -= HideCharacter;
        DataFlowController.OnConversationFinished -= ShowCharacter;
    }

    private void HideCharacter()
    {
        if (spriteRenderer != null) spriteRenderer.enabled = false;
        if (characterCollider != null) characterCollider.enabled = false;
    }

    private void ShowCharacter()
    {
        if (spriteRenderer != null) spriteRenderer.enabled = true;
        if (characterCollider != null) characterCollider.enabled = true;
    }
}
