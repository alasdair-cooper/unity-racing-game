using System;
using UnityEngine;

public class EventController : MonoBehaviour
{
    public static EventController Instance { get; private set; }

    void Awake() => Instance = this;

    public event EventHandler<CheckpointPassedEventArgs> CheckpointPassed;

    public void OnCheckpointPassed(string checkpointName) => CheckpointPassed.Invoke(this, new CheckpointPassedEventArgs(checkpointName));

    public event EventHandler LapCompleted;

    public void OnLapCompleted() => LapCompleted.Invoke(this, EventArgs.Empty);
}

public class CheckpointPassedEventArgs
{
    public CheckpointPassedEventArgs(string name) => Name = name;

    public string Name { get; }
}
