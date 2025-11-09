using Cysharp.Threading.Tasks;
using System.Threading;
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

        if (ScreenFader.Instance.IsFading)
        {
            return;
        }

        playerController.SetAsDead();

        GameplayUIManager.Instance.DisableActiveInGameMenu();

        TeleportPlayer(this.GetCancellationTokenOnDestroy()).Forget();
    }

    private async UniTask TeleportPlayer(CancellationToken cancellationToken)
    {
        await ScreenFader.Instance.Fade("Death", FadeType.FadeOut, cancellationToken, callback: () => CheckpointManager.Instance.CheckpointTeleport());
        await ScreenFader.Instance.Fade("Death", FadeType.FadeIn, cancellationToken);
    }
}
