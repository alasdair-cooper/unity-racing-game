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

    public void Start()
    {
        EventController.Instance.RaceCountdownTick += (_, args) => RaceStartCountdownText.text = string.Format(RaceStartCountdownTextFormat, args.Tick);
        EventController.Instance.RaceStarted += (_, args) =>
        {
            RaceStartCountdownText.enabled = false;
            LapText.text = string.Format(LapTextFormat, 1, args.MaxLapCount);
        };
        EventController.Instance.LapCompleted += (_, args) => LapText.text = string.Format(LapTextFormat, args.CompletedLapCount + 1, args.MaxLapCount);
        EventController.Instance.RaceCompleted += (_, _) => LapText.text = "Race completed!";
    }

    public void Update()
    {
        SpeedometerText.text = string.Format(SpeedometerTextFormat, Player.CurrentSpeed);
    }
}