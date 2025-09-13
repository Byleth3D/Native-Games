using System;
using UnityEngine;

public abstract class Timer
{
    private float initialTime;

    public float CurrentTime { get; protected set; }
    public bool IsRunning { get; protected set; }


    public event Action OnTimerStart;
    public event Action OnTimerStop;

    protected Timer() { }
    protected Timer(float initialTime) => this.initialTime = initialTime;

    public void Start()
    {
        CurrentTime = initialTime;

        if (!IsRunning)
        {
            IsRunning = true;
            OnTimerStart?.Invoke();
        }
    }

    public void Stop()
    {
        if (IsRunning)
        {
            IsRunning = false;
            OnTimerStop?.Invoke();
        }
    }

    public void Pause() => IsRunning = false;
    public void Resume() => IsRunning = true;

    public virtual void Reset() => CurrentTime = initialTime;

    public virtual void Reset(float newTime)
    {
        initialTime = newTime;
        Reset();
    }

    public abstract void Tick(float deltaTime);
}

public class CountdownTimer : Timer
{
    public CountdownTimer(float initialTime) : base(initialTime)
    {

    }

    public override void Tick(float deltaTime)
    {
        if (!IsRunning) return;

        if (CurrentTime <= 0.0f)
        {
            Stop();
            return;
        }

        CurrentTime -= deltaTime;
    }
}

public class StopwatchTimer : Timer
{
    public override void Tick(float deltaTime)
    {
        CurrentTime += deltaTime;
    }
}
