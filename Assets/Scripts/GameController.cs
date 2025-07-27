using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    private readonly HashSet<string> _triggeredCheckpointNames = new();

    public CheckpointInstance _firstCheckpoint;

    private int _checkpointCount = 0;
    private int _lapCount = 0;

    void Start() => _checkpointCount = GetComponentsInChildren<CheckpointInstance>().Length;

    public void OnCheckpointPassed(string name)
    {
        if (_triggeredCheckpointNames.Count == _checkpointCount && name == _firstCheckpoint.name)
        {
            _triggeredCheckpointNames.Clear();
            _lapCount++;

            Debug.Log($"Lap {_lapCount} completed!");
        }

        _triggeredCheckpointNames.Add(name);

        Debug.Log($"Checkpoint {name} passed!");
    }
}
