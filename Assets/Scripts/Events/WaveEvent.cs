using System;

public class WaveEvent
{
    public event Action OpenShop;
    public void OpenShopEvent()
    {
        OpenShop?.Invoke();
    }

    public event Action<int> WaveStart;
    public void StartWaveEvent(int wave)
    {
        WaveStart?.Invoke(wave);
    }

    public event Action GameOver;
    public void GameOverEvent()
    {
        GameOver?.Invoke();
    }
}
