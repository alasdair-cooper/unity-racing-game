using System;

public class LapEvents
{
    public event EventHandler<LapCompletedEventArgs> LapCompleted;

    public void OnLapCompleted(int completedLapCount, int maxLapCount) => LapCompleted.Invoke(this, new LapCompletedEventArgs(completedLapCount, maxLapCount));

    public event EventHandler<LapRecordedEventArgs> LapRecorded;

    public void OnLapRecorded(TimeSpan delta) => LapRecorded.Invoke(this, new LapRecordedEventArgs(delta));
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