using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameController : MonoBehaviour
{
    private readonly HashSet<string> _triggeredCheckpointNames = new();

    [Header("Game")]
    public int MaxLaps;
    public CheckpointInstance[] Checkpoints;

    [Header("UI")]
    public string LapTextFormat;
    public TMP_Text LapText;

    private int _lapCount = 0;

    void Start()
    {
        UpdateLapCounter();

        foreach (var checkpoint in Checkpoints)
        {
            checkpoint.OnPassed += (_, args) => OnCheckpointPassed(args.Name);
        }
    }

    public void OnCheckpointPassed(string name)
    {
        if (_triggeredCheckpointNames.Count == Checkpoints.Length && name == Checkpoints[0].name)
        {
            _lapCount++;
            _triggeredCheckpointNames.Clear();
            UpdateLapCounter();

            if (_lapCount == MaxLaps)
            {
                OnRaceCompleted();
            }
        }

        _triggeredCheckpointNames.Add(name);
    }

    public void UpdateLapCounter()
    {
        LapText.text = string.Format(LapTextFormat, _lapCount + 1, MaxLaps);
    }

    public void OnRaceCompleted()
    {
        LapText.text = "Race completed!";
        UiManager.Instance.SetScreen(GameScreen.RaceCompleted);
    }
}
