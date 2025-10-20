using System;
using System.Text;
using TMPro;
using UnityEngine;

public class FPSCounter : MonoBehaviour
{
    private TextMeshProUGUI fpsCounterText;

    private void Awake()
    {
        fpsCounterText = GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        fpsCounterText.text = $"{1f / Time.deltaTime} | {Application.targetFrameRate}";
    }
}
