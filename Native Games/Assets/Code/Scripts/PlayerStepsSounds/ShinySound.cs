using UnityEngine;

public class ShinySound : MonoBehaviour
{
    public AudioClip shinySound;

    public void PlayShinySound()
    {
        if (shinySound == null) return;

        AudioSource.PlayClipAtPoint(shinySound, transform.position);
    }
}