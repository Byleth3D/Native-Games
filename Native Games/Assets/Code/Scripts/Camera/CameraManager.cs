using PrimeTween;
using System;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class CameraManager : LocalSingleton<CameraManager>
{
    public CinemachineCamera gameplayCamera;
    public CinemachinePositionComposer positionComposer;

    [SerializeField] private List<CameraParameter> cameraParameters;
    private int currentCameraParameterIndex = 0;

    private Vector3 currentOffset;
    private Tween offsetTween;

    private Quaternion currentRotation;
    private Tween rotationTween;

    private float currentDistance;
    private Tween distanceTween;

    private CameraParameter FindCameraParameter(int index)
    {
        int parameterCount = cameraParameters.Count;

        if (parameterCount == 0 || index >= parameterCount)
        {
            return null;
        }

        return cameraParameters[index];
    }

    private CameraParameter FindCameraParameter(Checkpoint checkpoint)
    {
        foreach (CameraParameter cameraParameter in cameraParameters)
        {
            if (cameraParameter.checkpointTie != checkpoint)
            {
                continue;
            }

            return cameraParameter;
        }

        return null;
    }

    private int GetCameraParameterIndex(CameraParameter cameraParameter)
    {
        int cameraParametersCount = cameraParameters.Count;

        for (int i = 0; i < cameraParametersCount; i++)
        {
            if (cameraParameters[i] != cameraParameter)
            {
                continue;
            }

            return i;
        }

        return -1;
    }

    public void ResetParameters(int index)
    {
        if (index == -1)
        {
            return;
        }

        CameraParameter cameraParameter = FindCameraParameter(index);

        if (cameraParameter == null)
        {
            return;
        }

        currentCameraParameterIndex = index;

        positionComposer.TargetOffset = cameraParameter.offset;
        gameplayCamera.transform.localRotation = Quaternion.Euler(cameraParameter.eulerRotation);
        positionComposer.CameraDistance = cameraParameter.distance;
    }

    public void ResetParameters(Checkpoint checkpoint)
    {
        if (checkpoint == null)
        {
            return;
        }

        CameraParameter cameraParameter = FindCameraParameter(checkpoint);

        if (cameraParameter == null)
        {
            return;
        }

        currentCameraParameterIndex = GetCameraParameterIndex(cameraParameter);

        offsetTween.Stop();
        rotationTween.Stop();
        distanceTween.Stop();

        positionComposer.TargetOffset = cameraParameter.offset;
        gameplayCamera.transform.localRotation = Quaternion.Euler(cameraParameter.eulerRotation);
        positionComposer.CameraDistance = cameraParameter.distance;

        currentOffset = cameraParameter.offset;
        currentRotation = Quaternion.Euler(cameraParameter.eulerRotation);
        currentDistance = cameraParameter.distance;
    }

    public void SetParameters(int index)
    {
        if (index == -1)
        {
            return;
        }

        CameraParameter cameraParameter = FindCameraParameter(index);

        if (cameraParameter == null)
        {
            return;
        }

        currentCameraParameterIndex = index;

        Vector3 offset = positionComposer.TargetOffset;
        Quaternion rotation = gameplayCamera.transform.localRotation;
        float distance = positionComposer.CameraDistance;


        if (currentOffset != cameraParameter.offset)
        {
            offsetTween.Stop();

            offsetTween = Tween.Custom
            (offset,
            cameraParameter.offset,
            duration: cameraParameter.offsetTweenDuration,
            onValueChange: newValue => positionComposer.TargetOffset = newValue,
            ease: cameraParameter.offsetTweenEase);
        }

        if (currentRotation != Quaternion.Euler(cameraParameter.eulerRotation))
        {
            rotationTween.Stop();

            rotationTween = Tween.LocalRotation
            (gameplayCamera.transform,
            rotation,
            Quaternion.Euler(cameraParameter.eulerRotation),
            duration: cameraParameter.rotationTweenDuration,
            ease: cameraParameter.rotationTweenEase);
        }

        if (!Mathf.Approximately(currentDistance, cameraParameter.distance))
        {
            distanceTween.Stop();

            distanceTween = Tween.Custom
            (distance,
            cameraParameter.distance,
            duration: cameraParameter.distanceTweenDuration,
            onValueChange: newValue => positionComposer.CameraDistance = newValue,
            ease: cameraParameter.distanceTweenEase);
        }

        currentOffset = cameraParameter.offset;
        currentRotation = Quaternion.Euler(cameraParameter.eulerRotation);
        currentDistance = cameraParameter.distance;
    }
}

[Serializable]
public class CameraParameter
{
    public Vector3 offset;
    public float offsetTweenDuration;
    public Ease offsetTweenEase;
    [Space(20f)]

    public Vector3 eulerRotation;
    public float rotationTweenDuration;
    public Ease rotationTweenEase;
    [Space(20f)]

    public float distance;
    public float distanceTweenDuration;
    public Ease distanceTweenEase;
    [Space(20f)]

    public Checkpoint checkpointTie;
}