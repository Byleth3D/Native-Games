using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PushSound : MonoBehaviour
{
    public AudioClip pushSound;
    private AudioSource audioSource;
    private Rigidbody rigidBody;

    public float minVelocityForSound = 0.2f;

    public float volume = 0.2f;

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }
        audioSource.volume = volume;
    }

    private void Update()
    {

        if (rigidBody.linearVelocity.magnitude > 0f && pushSound != null)
        {
            if (!audioSource.isPlaying)
                audioSource.PlayOneShot(pushSound);
        }
    }
}