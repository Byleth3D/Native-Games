using UnityEngine;

public class DeathTriggerZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        PlayerController playerController = other.gameObject.GetComponent<PlayerController>();
        playerController.SetAsDead();
    }
}
