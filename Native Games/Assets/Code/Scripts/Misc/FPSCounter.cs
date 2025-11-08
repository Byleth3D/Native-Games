using System.Collections;
using TMPro;
using UnityEngine;

public class FPSCounter : MonoBehaviour
{
    private TextMeshProUGUI fpsCounterText;

    private void Awake()
    {
        fpsCounterText = GetComponent<TextMeshProUGUI>();
    }

    private void OnEnable()
    {
        StopCoroutine(GetAverageFrameRate());
        StartCoroutine(GetAverageFrameRate());
    }

    private void OnDisable()
    {
        StopCoroutine(GetAverageFrameRate());
    }

    private IEnumerator GetAverageFrameRate()
    {
        float time = 0;
        int frames = 0;

        while (true)
        {
            time += Time.deltaTime;
            frames++;

            if (time >= 1.0f)
            {
                float average = 1f / (time / frames);
                fpsCounterText.text = $"{average:F0} | {Application.targetFrameRate}";
                time = 0;
                frames = 0;
            }

            yield return null;
        }
    }
}
