using UnityEngine;

public class SoundsPlayer : MonoBehaviour
{
    public AudioClip passos;
    public AudioClip pulo;

    public float passosInterval = 0.5f;
    public float corridaInterval = 0.3f;

    private float stepTimer = 0f;
    private PlayerController controller;
    private GroundChecker groundChecker;
    private AudioSource audioSource;



    void Awake()
    {
        controller = GetComponent<PlayerController>();
        groundChecker = GetComponent<GroundChecker>();
        audioSource = GetComponent<AudioSource>();


    }


    void Update()
    {
        if (groundChecker.IsGrounded)
        {
            stepTimer -= Time.deltaTime;

            if (stepTimer <= 0f)
            {
                if (controller.Velocity.magnitude > 0f)
                {
                    PlayStep(passos);
                    stepTimer = passosInterval;
                }
            }
        }

        if (!groundChecker.IsGrounded && groundChecker.PreviousGrounded)
        {
            PlayJump();

        }

    }

    void PlayStep(AudioClip clips)
    {
        audioSource.PlayOneShot(clips);
    }

    void PlayJump()
    {
        if (pulo != null)
        {
            audioSource.PlayOneShot(pulo);
        }
    }
}
