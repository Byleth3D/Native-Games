using UnityEngine;

public class TalkFour : MonoBehaviour
{
    public AudioClip soundClip;
    public float volume = 3f;
    private AudioSource audioSource;

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
         AudioSource.PlayClipAtPoint(soundClip, transform.position, volume);
        
        }

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }
         public void Speak()
    {
        if (audioSource != null && !audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }
    
}