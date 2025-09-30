using EditorAttributes;
using PrimeTween;
using UnityEngine;

[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(GroundChecker))]

public class PlayerController : MonoBehaviour, IInteractorAgent
{
    [SerializeField] private Rigidbody body;
    [SerializeField] private CapsuleCollider collider3D;
    [SerializeField] private GameObject gameplayCamera;
    [SerializeField] private GroundChecker groundChecker;

    [Header("Motion")]
    [ShowInInspector] private Vector3 velocity;

    [Header("Horizontal Movement")]
    [SerializeField] private float horizontalSpeed = 3.5f;
    [SerializeField] private float teleportDuration = 0.25f;

    private Vector3 moveDirectionRaw;
    private Vector3 relativeMoveDirection;
    private Tween teleportTween;

    private bool canMove = true;

    [Header("Jump")]
    [SerializeField] private float maxJumpHeight = 2.5f;
    [SerializeField] private float jumpPeakTime = 0.45f;
    [SerializeField] private float jumpFallTime = 0.35f;
    [SerializeField] private float coyoteDuration;
    [SerializeField] private float jumpBufferDuration;

    private float initialJumpVelocity;
    private float jumpGravity;
    private float fallGravity;

    private CountdownTimer coyoteTimer;
    private CountdownTimer jumpBufferTimer;

    private bool canJump;

    [Header("Rotate")]
    [SerializeField] private float angularSpeed = 360.0f;
    [SerializeField] private Transform model;

    [Header("Interaction")]
    public InteractorType Interactor { get; } = InteractorType.Agent;

    private InteractionTrigger currentInteractionTrigger;
    private Vector3 interactionCenter;
    private IInteractable interactable;

    private GameObject interactableGameObject;
    private Vector3 interactableDirection;

    private bool isInteracting;

    private void Awake()
    {
        SetJumpSettings();
    }

    private void OnValidate()
    {
        SetJumpSettings();
    }

    private void OnEnable()
    {
        groundChecker.OnGroundEnter += OnGroundEnter;
        groundChecker.OnGroundExit += OnGroundExit;
    }

    private void OnDisable()
    {
        groundChecker.OnGroundEnter -= OnGroundEnter;
        groundChecker.OnGroundExit -= OnGroundExit;
    }

    private void SetJumpSettings()
    {
        jumpGravity = -2.0f * maxJumpHeight / (jumpPeakTime * jumpPeakTime);
        fallGravity = -2.0f * maxJumpHeight / (jumpFallTime * jumpFallTime);
        initialJumpVelocity = 2.0f * maxJumpHeight / jumpPeakTime;
        coyoteTimer = new CountdownTimer(coyoteDuration);
        jumpBufferTimer = new CountdownTimer(jumpBufferDuration);
    }

    private void Update()
    {
        coyoteTimer.Tick(Time.deltaTime);
        jumpBufferTimer.Tick(Time.deltaTime);
        Jump();
        Interact();
        Rotate();
    }

    private void FixedUpdate()
    {
        ProcessMove();
        PushObject();
        ProcessGravity();
        ProcessJump();
        ApplyVelocity();
    }

    private void ProcessMove()
    {
        if (canMove)
        {
            MoveHorizontally();
        }
        else
        {
            Teleport();
        }
    }

    private void MoveHorizontally()
    {
        Vector2 motionInput = InputManager.Instance.MotionInput;
        Vector3 previousMoveDirection = moveDirectionRaw;
        moveDirectionRaw = new Vector3(motionInput.x, 0.0f, motionInput.y);

        if (isInteracting)
        {
            float absX = Mathf.Abs(moveDirectionRaw.x);
            float previousAbsX = Mathf.Abs(previousMoveDirection.x);

            float absZ = Mathf.Abs(moveDirectionRaw.z);
            float previousAbsZ = Mathf.Abs(previousMoveDirection.z);

            if (absX > 0.0f && absZ > 0.0f)
            {
                if (previousAbsX > 0.0f)
                {
                    moveDirectionRaw.z = 0.0f;
                }
                else if (previousAbsZ > 0.0f)
                {
                    moveDirectionRaw.x = 0.0f;
                }
            }

            moveDirectionRaw.Normalize();
        }

        Vector3 cameraForwardDirection = gameplayCamera.transform.forward;
        cameraForwardDirection.y = 0.0f;
        cameraForwardDirection.Normalize();

        Quaternion cameraForwardRotation = Quaternion.LookRotation(cameraForwardDirection);

        relativeMoveDirection = cameraForwardRotation * moveDirectionRaw;

        velocity.x = relativeMoveDirection.x * horizontalSpeed;
        velocity.z = relativeMoveDirection.z * horizontalSpeed;
    }

    private void Teleport()
    {
        if (!teleportTween.isAlive)
        {
            Vector3 destiny = interactionCenter;
            destiny.y = body.position.y;

            TweenSettings tweenSettings = new TweenSettings();
            tweenSettings.duration = teleportDuration;
            tweenSettings.updateType = UpdateType.FixedUpdate;

            teleportTween = Tween.Custom(body.position, destiny, tweenSettings, onValueChange: newValue => body.position = newValue);
            teleportTween.OnComplete(() => canMove = true);
        }
    }

    private void Rotate()
    {
        Quaternion currentModelRotation = model.localRotation;
        Quaternion targetRotation;
        Vector3 targetDirection;
        float rotationStep = angularSpeed * Time.deltaTime;

        if (isInteracting)
        {
            interactableDirection = interactableGameObject.transform.position - transform.position;
            interactableDirection.y = 0.0f;
            interactableDirection.Normalize();

            targetDirection = interactableDirection;
        }
        else
        {
            if (relativeMoveDirection.magnitude <= 0.0f) return;

            targetDirection = relativeMoveDirection;
        }

        targetRotation = Quaternion.LookRotation(targetDirection);
        model.localRotation = Quaternion.RotateTowards(currentModelRotation, targetRotation, rotationStep);
    }

    private void Jump()
    {
        if (InputManager.Instance.JumpPressed)
        {
            if (groundChecker.IsGrounded)
            {
                canJump = true;
            }
            else
            {
                if (coyoteTimer.IsRunning && body.linearVelocity.y < 0.0f)
                {
                    canJump = true;
                    coyoteTimer.Stop();
                    return;
                }

                jumpBufferTimer.Start();
            }
        }
        else
        {
            if (groundChecker.IsGrounded && jumpBufferTimer.IsRunning)
            {
                canJump = true;
                jumpBufferTimer.Stop();
            }
        }
    }

    private void ProcessJump()
    {
        if (!canJump) return;

        body.AddForce(Vector3.down * body.linearVelocity.y, ForceMode.VelocityChange);
        
        velocity.y = initialJumpVelocity;
        canJump = false;
    }

    private void ProcessGravity()
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
        body.AddForce(velocity - body.linearVelocity, ForceMode.VelocityChange);
    }

    private void Interact()
    {
        if (groundChecker.IsGrounded)
        {
            if (interactable?.Interactable == InteractableType.ObjectPush)
            {
                if (InputManager.Instance.InteractHeld && !isInteracting)
                {
                    isInteracting = true;
                    canMove = false;
                }
                else if (!InputManager.Instance.InteractHeld && isInteracting)
                {
                    isInteracting = false;
                    currentInteractionTrigger.TriggerInteractCancel();
                }
            }
        }
        else if (!groundChecker.IsGrounded && isInteracting)
        {
            isInteracting = false;
            InputManager.Instance.DisableAction("Interact", 0.5f);
        }
    }

    private void PushObject()
    {
        if (interactable == null || !canMove) return;
        if (!isInteracting || interactable.Interactable != InteractableType.ObjectPush) return;
        currentInteractionTrigger.TriggerInteract();
    }

    public Vector3 GetVelocity() => velocity;

    public Vector3 GetHorizontalVelocity() => velocity.WithoutY();

    public Vector3 GetVerticalVelocity() => velocity.WithY();

    public Vector3 GetForwardDirection() => model.transform.forward;

    private void OnGroundEnter() => coyoteTimer.Stop();

    private void OnGroundExit() => coyoteTimer.Start();

    private void OnTriggerEnter(Collider trigger)
    {
        if (trigger == null || isInteracting) return;

        trigger.gameObject.TryGetComponent(out InteractionTrigger interactionTrigger);

        if (interactionTrigger == null) return;
        if (interactionTrigger.Interactable.Interactable == InteractableType.ItemCollect) return;

        currentInteractionTrigger = interactionTrigger;

        interactable = currentInteractionTrigger.Interactable;
        interactionCenter = trigger.bounds.center;
        interactableGameObject = trigger.transform.parent.gameObject;

        currentInteractionTrigger.TriggerEnter(this);
    }

    private void OnTriggerExit(Collider trigger)
    {
        if (trigger == null || isInteracting) return;

        if (currentInteractionTrigger == null || trigger.gameObject != currentInteractionTrigger.gameObject) return;

        interactable = null;
        interactionCenter = Vector3.zero;
        interactableGameObject = null;

        isInteracting = false;

        currentInteractionTrigger.TriggerExit();
        currentInteractionTrigger = null;
    }
}