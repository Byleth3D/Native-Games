using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

public static class Clock
{
    public static List<CountdownTimer> Timers { get; }
    public static List<CountdownTimer> TimersToAdd { get; }
    public static List<CountdownTimer> TimersToRemove { get; }

    static Clock()
    {
        Timers = new();
        TimersToAdd = new();
        TimersToRemove = new();
        Tick().Forget();
    }

    public static void QueueToAdd(CountdownTimer timer)
    {
        if (Timers.Contains(timer) || timer == null)
        {
            return;
        }

        TimersToAdd.Add(timer);
    }

    private static void Add()
    {
        Timers.AddRange(TimersToAdd);
        TimersToAdd.Clear();
    }

    public static void QueueToRemove(CountdownTimer timer)
    {
        if (!Timers.Contains(timer) || timer == null)
        {
            return;
        }

        TimersToRemove.Add(timer);
    }

    private static void RemoveTimers()
    {
        Timers.RemoveAll(timer => TimersToRemove.Contains(timer));
        TimersToRemove.Clear();
        Timers.RemoveAll(timer => timer == null || (!timer.IsRunning && timer.CurrentTime <= 0.0f));
    }

    public static async UniTaskVoid Tick()
    {
        while (true)
        {
            Add();

            if (Timers.Count > 0)
            {
                int timersCount = Timers.Count;

                for (int i = 0; i < timersCount; i++)
                {
                    if (Timers[i] == null)
                    {
                        continue;
                    }

                    Timers[i].Tick(Time.deltaTime);
                }

                RemoveTimers();

                await UniTask.NextFrame();
            }
            else
            {
                await UniTask.NextFrame();
            }
        }
    }
}
