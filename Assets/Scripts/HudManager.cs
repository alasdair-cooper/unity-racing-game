using System;
using TMPro;
using UnityEngine;

public class HudManager : MonoBehaviour
{
    [Header("Dependencies")]
    public PlayerController Player;

    [Header("Lap UI")]
    public string LapTextFormat;
    public TMP_Text LapText;

    [Header("Speedometer UI")]
    public string SpeedometerTextFormat;
    public TMP_Text SpeedometerText;

    [Header("Race Start Countdown UI")]
    public string RaceStartCountdownTextFormat;
    public TMP_Text RaceStartCountdownText;

    [Header("Race Timer UI")]
    public string TimerFormat;
    public TMP_Text TimerText;

    [Header("Split Delta UI")]
    public string SplitDeltaFormat;
    public TMP_Text SplitDeltaText;

    [Header("Lap Delta UI")]
    public string LapDeltaFormat;
    public TMP_Text LapDeltaText;

    private TimeSpan? timeSinceRaceStart;

    public void Start()
    {
        EventController.Instance.RaceCountdownTick += (_, args) => RaceStartCountdownText.text = string.Format(RaceStartCountdownTextFormat, args.Tick);
        EventController.Instance.RaceStarted += (_, args) =>
        {
            timeSinceRaceStart = TimeSpan.Zero;
            RaceStartCountdownText.gameObject.SetActive(false);
            LapText.text = string.Format(LapTextFormat, 1, args.MaxLapCount);
            LapText.gameObject.SetActive(true);
            SpeedometerText.gameObject.SetActive(true);
            TimerText.gameObject.SetActive(true);
        };
        EventController.Instance.SplitRecorded += (_, args) =>
        {
            switch (args.Delta.Ticks)
            {
                case > 0:
                    SplitDeltaText.text = "+" + args.Delta.ToString(SplitDeltaFormat);
                    SplitDeltaText.color = Color.green;
                    break;
                case < 0:
                    SplitDeltaText.text = "-" + args.Delta.ToString(SplitDeltaFormat);
                    SplitDeltaText.color = Color.red;
                    break;
                case 0:
                    SplitDeltaText.text = args.Delta.ToString(SplitDeltaFormat);
                    SplitDeltaText.color = Color.white;
                    break;

            }
        };
        EventController.Instance.LapCompleted += (_, args) => LapText.text = string.Format(LapTextFormat, args.CompletedLapCount + 1, args.MaxLapCount);
        EventController.Instance.LapRecorded += (_, args) =>
        {
            switch (args.Delta.Ticks)
            {
                case > 0:
                    LapDeltaText.text = "+" + args.Delta.ToString(LapDeltaFormat);
                    LapDeltaText.color = Color.green;
                    break;
                case < 0:
                    LapDeltaText.text = "-" + args.Delta.ToString(LapDeltaFormat);
                    LapDeltaText.color = Color.red;
                    break;
                case 0:
                    LapDeltaText.text = args.Delta.ToString(LapDeltaFormat);
                    LapDeltaText.color = Color.white;
                    break;

            }
        };
        EventController.Instance.RaceCompleted += (_, _) => LapText.text = "Race completed!";
    }

    public void Update()
    {
        SpeedometerText.text = string.Format(SpeedometerTextFormat, Player.CurrentSpeed);

        if (timeSinceRaceStart.HasValue)
        {
            timeSinceRaceStart += TimeSpan.FromSeconds(Time.deltaTime);
            TimerText.text = timeSinceRaceStart.Value.ToString(TimerFormat);
        }
    }
}