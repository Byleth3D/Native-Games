using EditorAttributes;
using PrimeTween;
using System;
using Unity.Cinemachine;
using UnityEngine;

public enum Target
{
    A, B
}

public class CameraTriggerZone : MonoBehaviour
{

    [Header("General")]
    [SerializeField] private bool isOneWay = false;
    [SerializeField, Range(0.0f, 180f)] private float backwardsAngleThreshold = 90f;
    [SerializeField, Range(0.0f, 180f)] private float tolerance = 1f;
    private PlayerController playerController;
    private bool isPlayerGoingBackwards;

    public bool IsOneWay => isOneWay;

    [Header("Position Offset")]
    [SerializeField] private Vector3 cameraOffsetA;
    [SerializeField] private Vector3 cameraOffsetB;

    public Vector3 CameraOffsetA => cameraOffsetA;
    public Vector3 CameraOffsetB => cameraOffsetB;

    [SerializeField] private Ease offsetTweenEase = Ease.InOutQuad;
    [SerializeField] private float offsetTweenDuration = 0.25f;

    private Target targetOffset = Target.B;

    public Tween OffsetTween { get; private set; }

    private bool offsetActive = true;

    [Header("Rotation")]
    [SerializeField, Clamp(0.0f, 360f, 0.0f, 360f, 0.0f, 360f)]
    [Tooltip("+ => 0 + ângulo | - => 360 - ângulo")]
    private Vector3 eulerRotationA;

    [SerializeField, Clamp(0.0f, 360f, 0.0f, 360f, 0.0f, 360f)]
    [Tooltip("+ => 0 + ângulo | - => 360 - ângulo")]
    private Vector3 eulerRotationB;

    [SerializeField] private Ease rotationTweenEase = Ease.InOutQuad;
    [SerializeField] private float rotationTweenDuration = 2.0f;

    public Quaternion RotationA { get; private set; }
    public Quaternion RotationB { get; private set; }

    private Target targetRotation = Target.B;

    public Tween RotationTween { get; private set; }

    private bool rotationActive = true;

    [Header("Distance")]
    [SerializeField] private float cameraDistanceA;
    [SerializeField] private float cameraDistanceB;

    public float CameraDistanceA => cameraDistanceA;
    public float CameraDistanceB => cameraDistanceB;

    [SerializeField] private Ease distanceTweenEase = Ease.InOutQuad;
    [SerializeField] private float distanceTweenDuration = 0.5f;

    private Target targetDistance = Target.B;

    public Tween DistanceTween { get; private set; }

    private bool distanceActive = true;

    private CinemachineCamera gameplayCamera;
    private CinemachinePositionComposer positionComposer;


    private void Awake()
    {
        RotationA = Quaternion.Euler(eulerRotationA);
        RotationB = Quaternion.Euler(eulerRotationB);
        GetCameraReferences();
    }

    private void GetCameraReferences()
    {
        GameObject cameraObject = GameObject.FindGameObjectWithTag("GameplayCamera");
        gameplayCamera = cameraObject.GetComponent<CinemachineCamera>();
        CinemachineComponentBase componentBase = gameplayCamera.GetCinemachineComponent(CinemachineCore.Stage.Body);
        positionComposer = componentBase as CinemachinePositionComposer;
    }

    private void OnValidate()
    {
        RotationA = Quaternion.Euler(eulerRotationA);
        RotationB = Quaternion.Euler(eulerRotationB);
    }

    private void OnTriggerExit(Collider other)
    {
        Vector3 triggerZoneForward = gameObject.transform.forward;

        playerController = other.gameObject.GetComponent<PlayerController>();

        if (playerController == null)
        {
            return;
        }

        Vector3 playerDirection = playerController.Forward;

        float dotProduct = Vector3.Dot(triggerZoneForward, playerDirection);
        float x = Vector3.Angle(triggerZoneForward, playerDirection);

        //Debug.Log($"Dot: {dotProduct} | Angle: {x} | AT: {backwardsAngleThreshold} | Backwards: {isPlayerGoingBackwards} |" +
        //$" Trigger Forward: {triggerZoneForward} | Player Velocity Direction: {playerDirection}");

        if (x >= backwardsAngleThreshold - tolerance)
        {
            targetDistance = Target.A;
            targetOffset = Target.A;
            targetRotation = Target.A;
            isPlayerGoingBackwards = true;
        }
        else
        {
            targetDistance = Target.B;
            targetOffset = Target.B;
            targetRotation = Target.B;
            isPlayerGoingBackwards = false;
        }

        DistanceCamera();
        OffsetCamera();
        RotateCamera();
    }

    private void OffsetCamera()
    {
        if (!offsetActive || cameraOffsetA == cameraOffsetB)
        {
            offsetActive = false;
            return;
        }

        OffsetTween.Stop();

        if (positionComposer)
        {
            Vector3 fromOffset = positionComposer.TargetOffset;
            Vector3 toOffset = Vector3.zero;

            if (targetOffset == Target.A)
            {
                toOffset = cameraOffsetA;
                targetOffset = Target.B;
            }
            else
            {
                toOffset = cameraOffsetB;
                targetOffset = Target.A;
            }

            OffsetTween = Tween.Custom
                                    (fromOffset,
                                    toOffset,
                                    duration: offsetTweenDuration,
                                    onValueChange: newValue => positionComposer.TargetOffset = newValue,
                                    ease: offsetTweenEase);

            if (isOneWay && !isPlayerGoingBackwards)
            {
                offsetActive = false;
            }
        }
    }

    private void DistanceCamera()
    {
        if (!distanceActive || cameraDistanceA == cameraDistanceB)
        {
            distanceActive = false;
            return;
        }

        DistanceTween.Stop();

        if (positionComposer)
        {
            float fromDistance = positionComposer.CameraDistance;
            float toDistance = 0.0f;

            if (targetDistance == Target.A)
            {
                toDistance = cameraDistanceA;
                targetDistance = Target.B;
            }
            else
            {
                toDistance = cameraDistanceB;
                targetDistance = Target.A;
            }

            DistanceTween = Tween.Custom
                                        (fromDistance,
                                            toDistance,
                                            duration: distanceTweenDuration,
                                            onValueChange: newValue => positionComposer.CameraDistance = newValue,
                                            ease: distanceTweenEase);
        }

        if (isOneWay && !isPlayerGoingBackwards)
        {
            distanceActive = false;
        }
    }

    private void RotateCamera()
    {
        if (!rotationActive || RotationA == RotationB)
        {
            rotationActive = false;
            return;
        }

        RotationTween.Stop();

        Quaternion fromRotation = gameplayCamera.transform.localRotation;
        Quaternion toRotation = Quaternion.identity;

        if (targetRotation == Target.A)
        {
            toRotation = RotationA;
            targetRotation = Target.B;
        }
        else
        {
            toRotation = RotationB;
            targetRotation = Target.A;
        }

        RotationTween = Tween.LocalRotation
                                    (gameplayCamera.transform,
                                        fromRotation,
                                        toRotation,
                                        duration: rotationTweenDuration,
                                        ease: rotationTweenEase);

        if (isOneWay && !isPlayerGoingBackwards)
        {
            rotationActive = false;
        }
    }

    public void ResetCameraTrigger(bool closestToCheckpoint)
    {
        if (isOneWay)
        {
            offsetActive = true;
            distanceActive = true;
            rotationActive = true;
        }

        targetOffset = Target.B;
        targetDistance = Target.B;
        targetRotation = Target.B;

        OffsetTween.Stop();
        RotationTween.Stop();
        DistanceTween.Stop();

        if (closestToCheckpoint)
        {
            positionComposer.TargetOffset = cameraOffsetA;
            positionComposer.CameraDistance = cameraDistanceA;
            gameplayCamera.transform.localRotation = RotationA;
        }
    }
}
