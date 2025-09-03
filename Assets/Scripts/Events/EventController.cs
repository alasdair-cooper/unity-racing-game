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
            CheckpointEvents.CheckpointPassed += (_, x) => Debug.Log($"Checkpoint '{x.Name}' passed");
            LapEvents.LapCompleted += (_, x) => Debug.Log($"Lap '{x.CompletedLapCount}' of '{x.MaxLapCount}' completed");
        }
    }

    public CheckpointEvents CheckpointEvents { get; } = new();

    public LapEvents LapEvents { get; } = new();

    public RaceEvents RaceEvents { get; } = new();
}