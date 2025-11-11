using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public AudioClip soundClip;
    public float volume = 3f;
    private AudioSource audioSource;

    private bool soundPlayed = false;

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

    private void OnTriggerEnter(Collider other)
    {
        if (!soundPlayed && other.CompareTag("Player"))
        {
            soundPlayed = true;
            ObjectStateManager.Instance.SetAsPlayed(gameObject);
            Speak();
        }
    }
}