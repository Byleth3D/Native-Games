using KBCore.Refs;
using Unity.Cinemachine;
using UnityEngine;

public class PlayerController : ValidatedMonoBehaviour
{
    [SerializeField, Self] CharacterController characterController;
    [SerializeField, Scene] CinemachineCamera cinemachineCamera;

    [SerializeField, Range(0.0f, 100.0f)] float moveSpeed = 3.5f;

    private void Update()
    {
        Vector2 motionInput = InputManager.Instance.MotionInput;
        Vector3 motionVector = new Vector3(motionInput.x, 0.0f, motionInput.y);
        
        Vector3 cameraForwardDirection = cinemachineCamera.transform.forward;
        cameraForwardDirection.y = 0.0f;
        cameraForwardDirection.Normalize();

        Quaternion cameraForwardRotation = Quaternion.LookRotation(cameraForwardDirection);

        Vector3 relativeMotionVector = cameraForwardRotation * motionVector;
        characterController.Move(relativeMotionVector * moveSpeed * Time.deltaTime);
    }
}
