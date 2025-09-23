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
    [SerializeField] private CinemachineCamera cam;

    [Header("Position")]
    [SerializeField] private Vector3 cameraOffsetA;
    [SerializeField] private Vector3 cameraOffsetB;

    [SerializeField] private Target offsetTarget = Target.B;

    [SerializeField] private Ease offsetTweenEase = Ease.InOutQuad;
    [SerializeField] private float offsetTweenDuration = 0.25f;

    private Tween offsetTweenOperation;

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

    [SerializeField] private bool isOneWay = false;

    private bool isEnabled = true;

    private Quaternion rotationA;
    private Quaternion rotationB;

    private Tween angleTweenOperation;

    [Header("Distance")]
    [SerializeField] private float cameraDistanceA;
    [SerializeField] private float cameraDistanceB;

    [SerializeField] private Target targetDistance = Target.B;

    [SerializeField] private Ease distanceTweenEase = Ease.InOutQuad;
    [SerializeField] private float distanceTweenDuration = 0.5f;

    private Tween distanceTweenOperation;

    private void Awake()
    {
        rotationA = Quaternion.Euler(eulerRotationA);
        rotationB = Quaternion.Euler(eulerRotationB);
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
            DistanceCamera();
            OffsetCamera();
            RotateCamera();
        }
    }

    private void OffsetCamera()
    {
        if (!isEnabled || cameraOffsetA == cameraOffsetB) return;
        offsetTweenOperation.Stop();

        CinemachineComponentBase componentBase = cam.GetCinemachineComponent(CinemachineCore.Stage.Body);

        if (componentBase is CinemachinePositionComposer)
        {
            CinemachinePositionComposer positionComposer = componentBase as CinemachinePositionComposer;
            Vector3 fromOffset = positionComposer.TargetOffset;
            Vector3 toOffset = Vector3.zero;

            if (offsetTarget == Target.A)
            {
                toOffset = cameraOffsetA;
                offsetTarget = Target.B;
            }
            else
            {
                toOffset = cameraOffsetB;
                offsetTarget = Target.A;
            }

            offsetTweenOperation = Tween.Custom
                                    (fromOffset,
                                    toOffset,
                                    duration: offsetTweenDuration,
                                    onValueChange: newValue => positionComposer.TargetOffset = newValue,
                                    ease: offsetTweenEase);

            if (isOneWay)
            {
                offsetTweenOperation.OnComplete(OnRotationComplete);
                isEnabled = false;
            }
        }

    }

    private void DistanceCamera()
    {
        if (!isEnabled || cameraDistanceA == cameraDistanceB) return;
        distanceTweenOperation.Stop();

        CinemachineComponentBase componentBase = cam.GetCinemachineComponent(CinemachineCore.Stage.Body);

        if (componentBase is CinemachinePositionComposer)
        {
            CinemachinePositionComposer positionComposer = componentBase as CinemachinePositionComposer;
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

        if (isOneWay)
        {
            distanceTweenOperation.OnComplete(OnRotationComplete);
            isEnabled = false;
        }
    }

    private void RotateCamera()
    {
        if (!isEnabled || rotationA == rotationB) return;
        angleTweenOperation.Stop();

        Quaternion fromRotation = cam.transform.localRotation;
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
                                    (cam.transform,
                                        fromRotation,
                                        toRotation,
                                        duration: rotationTweenDuration,
                                        ease: rotationTweenEase);

        if (isOneWay)
        {
            angleTweenOperation.OnComplete(OnRotationComplete);
            isEnabled = false;
        }
    }

    private void OnRotationComplete()
    {
        if (angleTweenOperation.progress >= 1.0f && distanceTweenOperation.progress >= 1.0f && offsetTweenOperation.progress >= 1.0f)
        {
            this.gameObject.SetActive(false);
        }
    }
}
