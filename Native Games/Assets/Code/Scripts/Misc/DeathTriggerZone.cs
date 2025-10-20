using UnityEngine;

public class DeathTriggerZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        PlayerController playerController = other.gameObject.GetComponent<PlayerController>();

        if (playerController == null)
        {
            return;
        }

        if (ScreenFadeManager.Instance.IsFading)
        {
            return;
        }

        playerController.SetAsDead();

        ScreenFadeManager.Instance.SetConfig("Death");

        ScreenFadeManager.Instance.RequestFadeOut(() =>
        {
            CheckpointManager.Instance.CheckpointTeleport();
            ScreenFadeManager.Instance.RequestFadeIn(() => ScreenFadeManager.Instance.ResetConfig());
        });
    }
}
