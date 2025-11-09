using System;

public class CountdownTimer
{
    private float initialTime;

    public float CurrentTime { get; protected set; }
    public bool IsRunning { get; protected set; }

    public event Action OnTimerStart;
    public event Action OnTimerStop;
    public event Action OnTimerExpired;

    public CountdownTimer(float initialTime)
    {
        this.initialTime = initialTime;
    }

    public void Start()
    {
        CurrentTime = initialTime;

        if (!IsRunning)
        {
            IsRunning = true;
        }

        OnTimerStart?.Invoke();
    }

    public void StartAndQueue()
    {
        Start();
        Clock.QueueToAdd(this);
    }

    public void Stop()
    {
        if (!IsRunning) return;

        IsRunning = false;

        if (CurrentTime > 0.0f)
        {
            OnTimerStop?.Invoke();
        }
        else
        {
            OnTimerExpired?.Invoke();
        }
    }

    public void StopAndQueue()
    {
        Stop();
        Clock.QueueToRemove(this);
    }

    public void Tick(float deltaTime)
    {
        if (!IsRunning) return;

        if (CurrentTime <= 0.0f)
        {
            Stop();
            return;
        }

        CurrentTime -= deltaTime;
    }

    public void Reset() => CurrentTime = initialTime;

    public void Reset(float newTime)
    {
        initialTime = newTime;
        Reset();
    }
}
