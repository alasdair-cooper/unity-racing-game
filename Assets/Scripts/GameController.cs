using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    private readonly HashSet<string> _triggeredCheckpointNames = new();

    public CheckpointInstance[] Checkpoints;

    private int _lapCount = 0;

    void Start()
    {
        foreach (var checkpoint in Checkpoints)
        {
            checkpoint.OnPassed += (_, args) => OnCheckpointPassed(args.Name);
        }
    }

    public void OnCheckpointPassed(string name)
    {
        if (_triggeredCheckpointNames.Count == Checkpoints.Length && name == Checkpoints[0].name)
        {
            _triggeredCheckpointNames.Clear();
            _lapCount++;

            Debug.Log($"Lap {_lapCount} completed!");
        }

        _triggeredCheckpointNames.Add(name);

        Debug.Log($"Checkpoint {name} passed!");
    }
}
