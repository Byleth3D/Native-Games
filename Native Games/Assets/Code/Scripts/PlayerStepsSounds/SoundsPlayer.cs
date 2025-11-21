using UnityEngine;

public class SoundsPlayer : MonoBehaviour
{
    [SerializeField] private AudioClip steps;
    [SerializeField, Range(0.1f, 1f)] public float stepsVolume;
    public float stepsInterval = 0.5f;
    private CountdownTimer stepTimer;

    [SerializeField] public AudioClip jump;
    [SerializeField, Range(0.1f, 1f)] public float jumpVolume;

    private PlayerController controller;
    private GroundChecker groundChecker;
    private AudioSource audioSource;



    void Awake()
    {
        controller = GetComponent<PlayerController>();
        groundChecker = GetComponent<GroundChecker>();
        audioSource = GetComponent<AudioSource>();

        stepTimer = new(stepsInterval);
    }

    private void OnEnable()
    {
        groundChecker.OnGroundExit += PlayJump;
    }

    private void OnDisable()
    {
        groundChecker.OnGroundExit -= PlayJump;
    }

    private void Update()
    {
        if (groundChecker.IsGrounded)
        {
            if (!stepTimer.IsRunning)
            {
                PlayStep();
                return;
            }
        }
        else
        {
            stepTimer.StopAndQueue();
        }
    }

    private void PlayStep()
    {
        if (controller.Velocity.WithoutY().magnitude > 0.0f && !controller.IsPushing)
        {
            PlayClip(steps, jumpVolume);
            stepTimer.StartAndQueue();
        }
    }

    private void PlayJump()
    {
        if (controller.Velocity.y > 0.0f)
        {
            PlayClip(jump, jumpVolume);
        }
    }

    private void PlayClip(AudioClip clip, float volume)
    {
        if (clip == null)
        {
            return;
        }

        audioSource.PlayOneShot(clip, volume);
    }
}