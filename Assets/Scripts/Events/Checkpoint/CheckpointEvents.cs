using System;

public class CheckpointEvents
{
    public event EventHandler<CheckpointPassedEventArgs> CheckpointPassed;

    public void OnCheckpointPassed(string checkpointName) => CheckpointPassed.Invoke(this, new CheckpointPassedEventArgs(checkpointName));

    public event EventHandler<SplitRecordedEventArgs> SplitRecorded;

    public void OnSplitRecorded(TimeSpan delta) => SplitRecorded.Invoke(this, new SplitRecordedEventArgs(delta));
}

public class CheckpointPassedEventArgs
{
    public CheckpointPassedEventArgs(string name) => Name = name;

    public string Name { get; }
}

public class SplitRecordedEventArgs
{
    public SplitRecordedEventArgs(TimeSpan delta) => Delta = delta;

    public TimeSpan Delta { get; }
}