using KBCore.Refs;
using UnityEngine;

public class PlayerController : ValidatedMonoBehaviour
{
    [SerializeField, Self] CharacterController characterController;

    [SerializeField, Range(0.0f, 100.0f)] float moveSpeed = 3.5f;

    private void Update()
    {
        Vector3 motionDisplacement = transform.forward * moveSpeed * Time.deltaTime;
        characterController.Move(motionDisplacement);
    }
}
