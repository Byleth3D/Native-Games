using UnityEngine;

public class DialogueAudioManager : LocalSingleton<DialogueAudioManager>
{
    [field: SerializeField] public AudioSource AudioSource { get; private set; }

    public void PlayDialogue(AudioClip dialogue, float volume)
    {
        StopDialogue();

        AudioSource.clip = dialogue;
        AudioSource.volume = volume;
        AudioSource.Play();
    }

    public void StopDialogue()
    {
        AudioSource.Stop();
        AudioSource.clip = null;
        AudioSource.volume = 1f;
    }
}
