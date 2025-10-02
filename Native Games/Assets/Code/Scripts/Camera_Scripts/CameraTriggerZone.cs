using EditorAttributes;
using PrimeTween;
using System;
using System.Collections;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;

public enum Target
{
    A, B
}

public class CameraTriggerZone : MonoBehaviour
{

    [Header("General")]
    [SerializeField] private bool isOneWay = false;
    bool isPlayerGoingBackwards;

    [Header("Position Offset")]
    [SerializeField] private Vector3 cameraOffsetA;
    [SerializeField] private Vector3 cameraOffsetB;

    [SerializeField] private Target targetOffset = Target.B;

    [SerializeField] private Ease offsetTweenEase = Ease.InOutQuad;
    [SerializeField] private float offsetTweenDuration = 0.25f;

    private Tween offsetTweenOperation;
    private bool offsetActive = true;

    [Header("Rotation")]
    [SerializeField, Clamp(0.0f, 360f, 0.0f, 360f, 0.0f, 360f)]
    [Tooltip("+ => 0 + ângulo | - => 360 - ângulo")]
    private Vector3 eulerRotationA;

    [SerializeField, Clamp(0.0f, 360f, 0.0f, 360f, 0.0f, 360f)]
    [Tooltip("+ => 0 + ângulo | - => 360 - ângulo")]
    private Vector3 eulerRotationB;

    [SerializeField] private Target targetRotation = Target.B;

    [SerializeField] private Ease rotationTweenEase = Ease.InOutQuad;
    [SerializeField] private float rotationTweenDuration = 2.0f;

    private Quaternion rotationA;
    private Quaternion rotationB;

    private Tween angleTweenOperation;

    private bool rotationActive = true;

    [Header("Distance")]
    [SerializeField] private float cameraDistanceA;
    [SerializeField] private float cameraDistanceB;

    [SerializeField] private Target targetDistance = Target.B;

    [SerializeField] private Ease distanceTweenEase = Ease.InOutQuad;
    [SerializeField] private float distanceTweenDuration = 0.5f;

    private Tween distanceTweenOperation;

    private bool distanceActive = true;

    private CinemachineCamera gameplayCamera;
    private CinemachinePositionComposer positionComposer;


    private void Awake()
    {
        rotationA = Quaternion.Euler(eulerRotationA);
        rotationB = Quaternion.Euler(eulerRotationB);
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
        rotationA = Quaternion.Euler(eulerRotationA);
        rotationB = Quaternion.Euler(eulerRotationB);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Vector3 triggerZoneForward = gameObject.transform.forward;

            Vector3 playerVelocityDirection = other.attachedRigidbody.linearVelocity;
            playerVelocityDirection.y = 0.0f;
            playerVelocityDirection.Normalize();

            float dotProduct = Vector3.Dot(triggerZoneForward, playerVelocityDirection);

            isPlayerGoingBackwards = dotProduct < 0.0f;

            if (isPlayerGoingBackwards)
            {
                targetDistance = Target.A;
                targetOffset = Target.A;
                targetRotation = Target.A;
            }

            DistanceCamera();
            OffsetCamera();
            RotateCamera();
        }
    }

    private void OffsetCamera()
    {
        if (!offsetActive || cameraOffsetA == cameraOffsetB)
        {
            offsetActive = false;
            return;
        }

        offsetTweenOperation.Stop();

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

            offsetTweenOperation = Tween.Custom
                                    (fromOffset,
                                    toOffset,
                                    duration: offsetTweenDuration,
                                    onValueChange: newValue => positionComposer.TargetOffset = newValue,
                                    ease: offsetTweenEase);

            if (isOneWay && !isPlayerGoingBackwards)
            {
                offsetTweenOperation.OnComplete(OnTweenCompleted);
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

        distanceTweenOperation.Stop();

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

            distanceTweenOperation = Tween.Custom
                                        (fromDistance,
                                            toDistance,
                                            duration: distanceTweenDuration,
                                            onValueChange: newValue => positionComposer.CameraDistance = newValue,
                                            ease: distanceTweenEase);
        }

        if (isOneWay && !isPlayerGoingBackwards)
        {
            distanceTweenOperation.OnComplete(OnTweenCompleted);
            distanceActive = false;
        }
    }

    private void RotateCamera()
    {
        if (!rotationActive || rotationA == rotationB)
        {
            rotationActive = false;
            return;
        }

        angleTweenOperation.Stop();

        Quaternion fromRotation = gameplayCamera.transform.localRotation;
        Quaternion toRotation = Quaternion.identity;

        if (targetRotation == Target.A)
        {
            toRotation = rotationA;
            targetRotation = Target.B;
        }
        else
        {
            toRotation = rotationB;
            targetRotation = Target.A;
        }

        angleTweenOperation = Tween.LocalRotation
                                    (gameplayCamera.transform,
                                        fromRotation,
                                        toRotation,
                                        duration: rotationTweenDuration,
                                        ease: rotationTweenEase);

        if (isOneWay && !isPlayerGoingBackwards)
        {
            angleTweenOperation.OnComplete(OnTweenCompleted);
            rotationActive = false;
        }
    }

    private void OnTweenCompleted()
    {
        if (!offsetActive && !distanceActive && !rotationActive)
        {
            this.gameObject.SetActive(false);
        }
    }
}
