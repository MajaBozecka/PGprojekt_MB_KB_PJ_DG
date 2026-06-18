using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class RandomizeSprite : MonoBehaviour
{
    [Tooltip("Wstaw tutaj wszystkie wersje obrazu (np. 3 sprite'y)")]
    public Sprite[] availableSprites;

    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void OnEnable()
    {
        if (availableSprites != null && availableSprites.Length > 0)
        {
            int randomIndex = Random.Range(0, availableSprites.Length);

            spriteRenderer.sprite = availableSprites[randomIndex];
        }
    }
}