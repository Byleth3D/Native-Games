using UnityEngine;
using System.Collections;

public class SoundsPlayer : MonoBehaviour
{
    private PlayerController controller;
    private GroundChecker groundChecker;
    private AudioSource audioSource;

    public AudioClip steps;
    public AudioClip jump;

    public float stepsInterval = 0.5f;

    private CountdownTimer stepTimer;

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

            stepTimer.Tick(Time.deltaTime);
        }
    }

    private void PlayStep()
    {
        if (controller.Velocity.WithoutY().magnitude > 0.0f && !controller.IsInteracting)
        {
            PlayClip(steps);
            stepTimer.Start();
        }
    }

    private void PlayJump()
    {
        if (controller.Velocity.y > 0.0f)
        {
            PlayClip(jump);
        }
    }

    private void PlayClip(AudioClip clip)
    {
        if (clip == null)
        {
            return;
        }

        audioSource.PlayOneShot(clip);

    }

}