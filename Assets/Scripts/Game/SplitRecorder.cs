using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class SplitRecorder : MonoBehaviour
{
    private readonly Stack<SplitInfo> _splits = new();
    private string _lastCheckpointName;
    private double _timeSinceLastCheckpoint;

    [Header("HUD")]
    public string SplitDeltaFormat;
    public TMP_Text SplitDeltaText;

    public void Start() => EventController.Instance.CheckpointPassed += (_, args) => OnCheckpointPassed(args.Name);

    public void Update() => _timeSinceLastCheckpoint += Time.deltaTime;

    private void OnCheckpointPassed(string checkpointName)
    {
        if (_lastCheckpointName != null)
        {
            var newSplit = new SplitInfo(_lastCheckpointName, checkpointName, TimeSpan.FromSeconds(_timeSinceLastCheckpoint));
            var previousSplit = _splits.Where(x => x.StartCheckpointName == newSplit.StartCheckpointName && x.EndCheckpointName == newSplit.EndCheckpointName).OrderBy(x => x.Time).FirstOrDefault();

            _splits.Push(newSplit);

            if (previousSplit != null)
            {
                var splitDelta = previousSplit.Time - newSplit.Time;
                switch (splitDelta.Ticks)
                {
                    case > 0:
                        SplitDeltaText.text = "+" + splitDelta.ToString(SplitDeltaFormat);
                        SplitDeltaText.color = Color.green;
                        break;
                    case < 0:
                        SplitDeltaText.text = "-" + splitDelta.ToString(SplitDeltaFormat);
                        SplitDeltaText.color = Color.red;
                        break;
                    case 0:
                        SplitDeltaText.text = splitDelta.ToString(SplitDeltaFormat);
                        SplitDeltaText.color = Color.white;
                        break;

                }
            }
        }

        _timeSinceLastCheckpoint = 0;
        _lastCheckpointName = checkpointName;
    }
}

public class SplitInfo
{
    public SplitInfo(string startCheckpointName, string endCheckpointName, TimeSpan time)
    {
        StartCheckpointName = startCheckpointName;
        EndCheckpointName = endCheckpointName;
        Time = time;
    }

    public string StartCheckpointName { get; }

    public string EndCheckpointName { get; }

    public TimeSpan Time { get; }
}
