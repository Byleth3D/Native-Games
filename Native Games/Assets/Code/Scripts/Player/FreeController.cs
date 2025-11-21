using UnityEngine;

public class FreeController : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private GameObject freeCamera;
    [SerializeField] private Vector3 positionOffset;
    private Transform parent;

    private void Awake()
    {
        parent = transform.parent;
    }

    public void SetParent(bool toNull = false)
    {
        transform.parent = toNull ? null : parent;
        transform.localPosition = toNull ? transform.position : Vector3.zero + positionOffset;
    }

    private void Update()
    {
        Vector2 motionInput = InputManager.Instance.Cheats.FreeMove;
        Vector3 inputDirection = new Vector3(motionInput.x, 0.0f, motionInput.y).normalized;

        transform.Translate(inputDirection * speed * Time.deltaTime);
    }

    private void LateUpdate()
    {
        transform.rotation = freeCamera.transform.rotation;
    }
}