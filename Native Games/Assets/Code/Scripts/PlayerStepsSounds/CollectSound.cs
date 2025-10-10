using UnityEngine;

public class CollectSound : MonoBehaviour
{
    public AudioClip collectSound;

    public void PlayCollectSound()
    {
        if (collectSound == null) return;

        AudioSource.PlayClipAtPoint(collectSound, transform.position);

        Destroy(gameObject);
    }
}