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

        ScreenFadeManager.Instance.CurrentFadeDelay = 0.0f;
        ScreenFadeManager.Instance.CurrentCallbackDelay = 0.5f;
        ScreenFadeManager.Instance.CurrentFadeInDuration = 1.5f;
        ScreenFadeManager.Instance.CurrentFadeOutDuration = 2.0f;

        ScreenFadeManager.Instance.RequestFadeOut(() =>
        {
            CheckpointManager.Instance.CheckpointTeleport();
            ScreenFadeManager.Instance.RequestFadeIn(() => ScreenFadeManager.Instance.ResetValues());
        });
    }
}
