using System;
using System.Data;

public static class MiscEvent
{
    public static event Action<int> GoldValueChange;
    public static void OnGoldValueChange(int gold)
    {
        GoldValueChange?.Invoke(gold);
    }

    public static event Action<int> TimerValueChange;
    public static void OnTimerChange(int time)
    {
        TimerValueChange?.Invoke(time);
    }
}
