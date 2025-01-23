using System;
using UnityEngine;

public static class WaveEvent
{
    public static event Action OpenShop;
    public static void OpenShopEvent()
    {
        OpenShop?.Invoke();
    }

    public static event Action<int> WaveStart;
    public static void StartWaveEvent(int wave)
    {
        WaveStart?.Invoke(wave);
    }

    public static event Action GameOver;
    public static void GameOverEvent()
    {
        GameOver?.Invoke();
    }

    public static event Action<GameObject> OnAddHeroToList;
    public static void AddHeroToList(GameObject hero)
    {
        OnAddHeroToList?.Invoke(hero);
    }

    public static event Action<GameObject> OnRemoveHeroFromList;
    public static void RemoveHeroFromList(GameObject hero)
    {
        OnRemoveHeroFromList?.Invoke(hero);
    }

    public static event Action<int> OnHeroListChanged;
    public static void HeroAddedToList(int heroCount)
    {
        OnHeroListChanged?.Invoke(heroCount);
    }

    public static event Action<GameObject> OnRemoveEnemyFromWave;
    public static void RemoveEnemyFromWave(GameObject enemy)
    {
        OnRemoveEnemyFromWave?.Invoke(enemy);
    }

    public static event Action OnWaveEnded;
    public static void WaveEnded()
    {
        OnWaveEnded?.Invoke();
    }
}
