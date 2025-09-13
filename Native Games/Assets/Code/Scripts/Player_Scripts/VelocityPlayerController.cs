using EditorAttributes;
using KBCore.Refs;
using Unity.Cinemachine;
using UnityEngine;

public class VelocityPlayerController : ValidatedMonoBehaviour
{
    [SerializeField, Scene, HideProperty]
    private CinemachineCamera cinemachineCamera;

    [SerializeField, Self, HideProperty]
    private GroundChecker groundChecker;

    [SerializeField, Self, HideProperty]
    private Rigidbody rigidBody;

    [Header("Motion")]
    [ShowInInspector] private Vector3 velocity;

    [Header("Horizontal Movement")]
    [SerializeField, Range(0.0f, 100.0f)] float horizontalSpeed = 3.5f;
    private Vector3 moveDirectionRaw;
    private Vector3 relativeMoveDirection;

    [Header("Jump")]
    [SerializeField, Range(0.1f, 100f)] private float maxJumpHeight = 2.5f;
    [SerializeField, Range(0.01f, 60f)] private float jumpPeakTime = 0.45f;
    [SerializeField, Range(0.01f, 60f)] private float jumpFallTime = 0.35f;

    private float initialJumpVelocity;
    private float jumpGravity;
    private float fallGravity;

    private bool canJump;

    [Header("Rotate")]
    [SerializeField] private float angularSpeed = 360.0f;
    [SerializeField] private Transform model;

    private void Awake()
    {
        SetJumpSettings();
    }

    protected override void OnValidate()
    {
        base.OnValidate();
        SetJumpSettings();
    }

    private void SetJumpSettings()
    {
        jumpGravity = -2.0f * maxJumpHeight / (jumpPeakTime * jumpPeakTime);
        fallGravity = -2.0f * maxJumpHeight / (jumpFallTime * jumpFallTime);
        initialJumpVelocity = 2.0f * maxJumpHeight / jumpPeakTime;
    }

    private void Update()
    {
        Jump();
        Rotate();
    }

    private void FixedUpdate()
    {
        MoveHorizontally();
        ApplyGravity();
        ProcessJump();
        ApplyVelocity();
    }

    private void MoveHorizontally()
    {
        Vector2 motionInput = InputManager.Instance.MotionInput;
        moveDirectionRaw = new Vector3(motionInput.x, 0.0f, motionInput.y);

        Vector3 cameraForwardDirection = cinemachineCamera.transform.forward;
        cameraForwardDirection.y = 0.0f;
        cameraForwardDirection.Normalize();

        Quaternion cameraForwardRotation = Quaternion.LookRotation(cameraForwardDirection);

        relativeMoveDirection = cameraForwardRotation * moveDirectionRaw;

        velocity.x = relativeMoveDirection.x * horizontalSpeed;
        velocity.z = relativeMoveDirection.z * horizontalSpeed;
    }

    private void Rotate()
    {
        if (relativeMoveDirection.magnitude <= 0.0f) return;

        Quaternion currentModelRotation = model.localRotation;
        Quaternion targetRotation = Quaternion.LookRotation(relativeMoveDirection);
        float rotationStep = angularSpeed * Time.deltaTime;
        model.localRotation = Quaternion.RotateTowards(currentModelRotation, targetRotation, rotationStep);
    }

    private void ApplyGravity()
    {
        if (groundChecker.IsGrounded)
        {
            velocity.y = 0.0f;
        }
        else
        {
            float currentGravity = velocity.y > 0.0f ? jumpGravity : fallGravity;
            velocity.y += currentGravity * Time.deltaTime;
        }
    }

    private void ApplyVelocity()
    {
        rigidBody.AddForce(velocity - rigidBody.linearVelocity, ForceMode.VelocityChange);
    }

    private void Jump()
    {
        if (InputManager.Instance.JumpPressed && groundChecker.IsGrounded)
        {
            canJump = true;
        }
    }

    private void ProcessJump()
    {
        if (!canJump) return;

        velocity.y = initialJumpVelocity;
        canJump = false;
    }
}
