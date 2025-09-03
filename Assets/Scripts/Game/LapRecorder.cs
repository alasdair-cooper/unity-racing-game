using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class LapRecorder : MonoBehaviour
{
    private readonly Stack<LapInfo> _laps = new();
    private double _timeSinceLastLap;

    public void Start() => EventController.Instance.LapEvents.LapCompleted += (_, _) => OnLapCompleted();

    public void Update() => _timeSinceLastLap += Time.deltaTime;

    private void OnLapCompleted()
    {
        var newLap = new LapInfo(TimeSpan.FromSeconds(_timeSinceLastLap));
        var previousLap = _laps.OrderBy(x => x.Time).FirstOrDefault();
        _laps.Push(newLap);
        _timeSinceLastLap = 0;

        if (previousLap != null)
        {
            EventController.Instance.LapEvents.OnLapRecorded(previousLap.Time - newLap.Time);
        }
    }
}

public class LapInfo
{
    public LapInfo(TimeSpan time) => Time = time;

    public TimeSpan Time { get; }
}
