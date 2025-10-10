using UnityEngine;

public class TalkFour : MonoBehaviour
{ 
    public AudioClip soundClip;

    private bool soundPlayed = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!soundPlayed && other.CompareTag("Player"))
        {
            soundPlayed = true;
            PlaySound();
        }
    }

    private void PlaySound()
    {
        if (soundClip == null) return;
        AudioSource.PlayClipAtPoint(soundClip, transform.position, 3.0f);
    }
}