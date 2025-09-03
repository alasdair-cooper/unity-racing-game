using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class GameController : MonoBehaviour
{
    private readonly HashSet<string> _triggeredCheckpointNames = new();

    [Header("Game")]
    public int BeginRaceAfterSeconds;
    public int MaxLaps;
    public CheckpointInstance LapStartEndCheckpoint;
    public CheckpointInstance[] Checkpoints;

    private int _lapCount = 0;

    public int LapCount => _lapCount;

    void Start()
    {
        foreach (var checkpoint in Checkpoints)
        {
            EventController.Instance.CheckpointEvents.CheckpointPassed += (_, args) => OnCheckpointPassed(args.Name);
        }

        StartCoroutine(BeginRaceAfterDelay());
    }

    IEnumerator BeginRaceAfterDelay()
    {
        var waitForOneSecond = new WaitForSeconds(1);
        for (int i = BeginRaceAfterSeconds; i > 0; i--)
        {
            EventController.Instance.RaceEvents.OnRaceCountdownTick(i);
            yield return waitForOneSecond;
        }
        EventController.Instance.RaceEvents.OnRaceStarted(MaxLaps);
    }

    public void OnCheckpointPassed(string name)
    {
        if (_triggeredCheckpointNames.Count == Checkpoints.Length && name == LapStartEndCheckpoint.name)
        {
            _lapCount++;
            EventController.Instance.LapEvents.OnLapCompleted(_lapCount, MaxLaps);
            _triggeredCheckpointNames.Clear();

            if (_lapCount == MaxLaps)
            {
                EventController.Instance.RaceEvents.OnRaceCompleted();
            }
        }

        _triggeredCheckpointNames.Add(name);
    }
}
