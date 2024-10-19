using System;

public class WaveEvent
{
    public event Action OpenShop;
    public void OpenShopEvent()
    {
        OpenShop?.Invoke();
    }

    public event Action<int> WaveStart;
    public void OnStartWave(int wave)
    {
        WaveStart?.Invoke(wave);
    }
}
