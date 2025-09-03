using System;
using UnityEngine;

public class EventController : MonoBehaviour
{
    public bool IsLoggingEnabled;

    public static EventController Instance { get; private set; }

    void Awake()
    {
        Instance = this;

        if (IsLoggingEnabled)
        {
            CheckpointPassed += (_, x) => Debug.Log($"Checkpoint '{x.Name}' passed");
            LapCompleted += (_, x) => Debug.Log($"Lap '{x.CompletedLapCount}' of '{x.MaxLapCount}' completed");
        }
    }

    public event EventHandler<CheckpointPassedEventArgs> CheckpointPassed;

    public void OnCheckpointPassed(string checkpointName) => CheckpointPassed.Invoke(this, new CheckpointPassedEventArgs(checkpointName));

    public event EventHandler<SplitRecordedEventArgs> SplitRecorded;

    public void OnSplitRecorded(TimeSpan delta) => SplitRecorded.Invoke(this, new SplitRecordedEventArgs(delta));

    public event EventHandler<LapCompletedEventArgs> LapCompleted;

    public void OnLapCompleted(int completedLapCount, int maxLapCount) => LapCompleted.Invoke(this, new LapCompletedEventArgs(completedLapCount, maxLapCount));

    public event EventHandler<LapRecordedEventArgs> LapRecorded;

    public void OnLapRecorded(TimeSpan delta) => LapRecorded.Invoke(this, new LapRecordedEventArgs(delta));

    public event EventHandler<RaceStartedEventArgs> RaceStarted;

    public void OnRaceStarted(int maxLapCount) => RaceStarted.Invoke(this, new RaceStartedEventArgs(maxLapCount));

    public event EventHandler RaceCompleted;

    public void OnRaceCompleted() => RaceCompleted.Invoke(this, EventArgs.Empty);

    public event EventHandler<RaceCountdownTickEventArgs> RaceCountdownTick;

    public void OnRaceCountdownTick(int tick) => RaceCountdownTick.Invoke(this, new RaceCountdownTickEventArgs(tick));
}

public class CheckpointPassedEventArgs
{
    public CheckpointPassedEventArgs(string name) => Name = name;

    public string Name { get; }
}

public class RaceStartedEventArgs
{
    public RaceStartedEventArgs(int maxLapCount) => MaxLapCount = maxLapCount;

    public int MaxLapCount { get; }
}

public class RaceCountdownTickEventArgs
{
    public RaceCountdownTickEventArgs(int tick) => Tick = tick;

    public int Tick { get; }
}

public class SplitRecordedEventArgs
{
    public SplitRecordedEventArgs(TimeSpan delta) => Delta = delta;

    public TimeSpan Delta { get; }
}

public class LapRecordedEventArgs
{
    public LapRecordedEventArgs(TimeSpan delta) => Delta = delta;

    public TimeSpan Delta { get; }
}

public class LapCompletedEventArgs
{
    public LapCompletedEventArgs(int completedLapCount, int maxLapCount) => (CompletedLapCount, MaxLapCount) = (completedLapCount, maxLapCount);

    public int CompletedLapCount { get; }

    public int MaxLapCount { get; }
}
