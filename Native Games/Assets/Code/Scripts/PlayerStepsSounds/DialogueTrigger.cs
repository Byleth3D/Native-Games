using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [SerializeField] private AudioClip dialogue;
    [SerializeField, Range(0.01f, 1f)] private float volume = 0.5f;
    [SerializeField] private Subtitle subtitle;

    private void OnTriggerEnter(Collider other)
    {
        DialogueAudioManager.Instance.PlayDialogue(dialogue, volume);
        AudioSource audioSource = DialogueAudioManager.Instance.AudioSource;

        SubtitleManager.Instance.TurnOnSubtitle(subtitle, audioSource);

        ObjectStateManager.Instance.SetAsPlayed(gameObject);
        gameObject.SetActive(false);
    }
}