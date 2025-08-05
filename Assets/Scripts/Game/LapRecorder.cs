using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class LapRecorder : MonoBehaviour
{
    private readonly Stack<LapInfo> _laps = new();
    private double _timeSinceLastLap;

    [Header("HUD")]
    public string TimerFormat;
    public TMP_Text TimerText;
    public string LapDelta;
    public TMP_Text LapDeltaText;

    public void Start() => EventController.Instance.LapCompleted += (_, _) => OnLapCompleted();

    public void Update()
    {
        _timeSinceLastLap += Time.deltaTime;
        TimerText.text = TimeSpan.FromSeconds(_timeSinceLastLap).ToString(TimerFormat);
    }

    private void OnLapCompleted()
    {
        var newLap = new LapInfo(TimeSpan.FromSeconds(_timeSinceLastLap));
        var previousLap = _laps.OrderBy(x => x.Time).FirstOrDefault();
        _laps.Push(newLap);
        _timeSinceLastLap = 0;

        if (previousLap != null)
        {
            var lapDelta = previousLap.Time - newLap.Time;
            switch (lapDelta.Ticks)
            {
                case > 0:
                    LapDeltaText.text = "+" + lapDelta.ToString(LapDelta);
                    LapDeltaText.color = Color.green;
                    break;
                case < 0:
                    LapDeltaText.text = "-" + lapDelta.ToString(LapDelta);
                    LapDeltaText.color = Color.red;
                    break;
                case 0:
                    LapDeltaText.text = lapDelta.ToString(LapDelta);
                    LapDeltaText.color = Color.white;
                    break;

            }
        }
    }
}

public class LapInfo
{
    public LapInfo(TimeSpan time) => Time = time;

    public TimeSpan Time { get; }
}
