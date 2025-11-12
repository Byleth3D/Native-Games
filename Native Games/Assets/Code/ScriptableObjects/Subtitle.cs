using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Subtitle", menuName = "Scriptable Objects/Subtitle")]
public class Subtitle : ScriptableObject
{
    public List<SubtitleLine> lines;
}

[Serializable]
public class SubtitleLine
{
    public string startTimeInMinutes;
    public string endTimeInMinutes;

    [TextArea] public string text;

    public float StartTimeInSeconds
    {
        get
        {
            string[] split = startTimeInMinutes.Split(":");

            float timeInSeconds = float.Parse(split[1]);
            timeInSeconds += float.Parse(split[0]) * 60f;

            return timeInSeconds;
        }
    }

    public float EndTimeInSeconds
    {
        get
        {
            string[] split = endTimeInMinutes.Split(":");

            float timeInSeconds = float.Parse(split[1]);
            timeInSeconds += float.Parse(split[0]) * 60f;

            return timeInSeconds;
        }
    }
}