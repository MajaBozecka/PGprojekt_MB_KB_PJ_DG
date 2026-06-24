using UnityEngine;

public class MainMenuAudio : MonoBehaviour
{
    public AudioSource sfxSource;
    public AudioClip clickSound;
    public AudioClip hoverSound;

    public void PlayClick()
    {
        if (sfxSource != null && clickSound != null) sfxSource.PlayOneShot(clickSound);
    }

    public void PlayHover()
    {
        if (sfxSource != null && hoverSound != null) sfxSource.PlayOneShot(hoverSound);
    }
}