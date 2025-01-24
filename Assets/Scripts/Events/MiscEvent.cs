using System;
using System.Data;

public static class MiscEvent
{
    public static event Action<int> GoldValueChange;
    public static void OnGoldValueChange(int gold)
    {
        GoldValueChange?.Invoke(gold);
    }

    public static event Action<int> OnTimerValueChange;
    public static void TimerValueChange(int time)
    {
        OnTimerValueChange?.Invoke(time);
    }

    public static event Action OnTimerFinished;
    public static void TimerFinished()
    {
        OnTimerFinished?.Invoke();
    }
}
