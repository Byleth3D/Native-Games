using UnityEngine;
using UnityEngine.Video;

public class CutsceneController : MonoBehaviour
{
    private VideoPlayer videoPlayer;

    private void Awake()
    {
        videoPlayer = GetComponent<VideoPlayer>();
    }

    private void OnEnable()
    {
        videoPlayer.loopPointReached += OnCutsceneEnd;
    }

    private void OnDisable()
    {
        videoPlayer.loopPointReached -= OnCutsceneEnd;
    }

    private void OnCutsceneEnd(VideoPlayer source)
    {
        if (videoPlayer.isLooping)
        {
            return;
        }

        SceneLoader.Instance.LoadNextScene();
    }
}
