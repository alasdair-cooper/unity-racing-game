using System;

public class RaceEvents
{
    public event EventHandler<RaceStartedEventArgs> RaceStarted;

    public void OnRaceStarted(int maxLapCount) => RaceStarted.Invoke(this, new RaceStartedEventArgs(maxLapCount));

    public event EventHandler RaceCompleted;

    public void OnRaceCompleted() => RaceCompleted.Invoke(this, EventArgs.Empty);

    public event EventHandler<RaceCountdownTickEventArgs> RaceCountdownTick;

    public void OnRaceCountdownTick(int tick) => RaceCountdownTick.Invoke(this, new RaceCountdownTickEventArgs(tick));
}

public class RaceCountdownTickEventArgs
{
    public RaceCountdownTickEventArgs(int tick) => Tick = tick;

    public int Tick { get; }
}

public class RaceStartedEventArgs
{
    public RaceStartedEventArgs(int maxLapCount) => MaxLapCount = maxLapCount;

    public int MaxLapCount { get; }
}
