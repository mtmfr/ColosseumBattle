using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public static WaveManager instance { get; private set; }

    #region Enemies
    [SerializeField] private EnemySpawner enemySpawner;

    private Enemy[] nextWaveEnemies;
    private int enemiesInWave;
    #endregion

    [SerializeField] private HeroSpawner heroSpawner;

    private Hero[] heroFrontline = new Hero[5];
    public List<Hero> heroesInParty { get; private set; } = new();
    private int heroesInBattle;

    public int currentWave;

    public WaveState waveState { get; private set; }

    private WaveManager() { }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else Destroy(gameObject);
    }

    private void OnDestroy()
    {
        if (instance == this)
            instance = null;
    }

    public void GenerateNextWave()
    {
        nextWaveEnemies = enemySpawner.GetNextEnemies(currentWave);
    }

    public void UpdateWaveState(WaveState newState)
    {

        switch (newState)
        {
            case WaveState.Start:
                if (nextWaveEnemies.Count() == 0)
                    throw new ArgumentNullException("No enemies in wave", "there is no enemies to be spawn in the wave");

                enemiesInWave = nextWaveEnemies.Length;
                enemySpawner.SpawnEnemies(nextWaveEnemies);

                nextWaveEnemies = null;

                heroSpawner.SpawnHeroes(heroFrontline);
                heroesInBattle = heroFrontline.Length;

                currentWave++;
                break;
            case WaveState.Middle:
                break;
            case WaveState.End:
                GenerateNextWave();
                waveState = WaveState.None;
                GameManager.UpdateGameState(GameState.Shop);
                break;
            default:
                Debug.LogWarning("State not recognised");
                break;
        }

        waveState = newState;

        if (waveState == WaveState.Start)
            UpdateWaveState(WaveState.Middle);
    }

    public void EnemyDied()
    {
        enemiesInWave--;

        if (enemiesInWave <= 0)
            UpdateWaveState(WaveState.End);
    }

    public void AddHeroToParty(Hero heroToAdd)
    {
        int lastId = Array.FindLastIndex(heroFrontline, e => e != null);

        if (lastId < 0)
        {
            heroFrontline[0] = heroToAdd;
        }
        else if (lastId < 5)
        {
            heroFrontline[lastId + 1] = heroToAdd;
        }
        else heroesInParty.Add(heroToAdd);
    }

    public void HeroDied(Hero deadHero)
    {
        int deadIndex = Array.FindIndex(heroFrontline, hero => hero == deadHero);
        heroFrontline[deadIndex] = null;

        heroesInBattle--;

        if (heroesInBattle <= 0)
            GameManager.UpdateGameState(GameState.GameOver);
    }

    public void ResetWaveCount() => currentWave = 0;

    public enum WaveState
    {
        None,
        /// <summary>
        /// The start of the wave to summon the units
        /// </summary>
        Start,
        /// <summary>
        /// The moment where the units are fighting
        /// </summary>
        Middle,
        /// <summary>
        /// The end of the wave juste before the shop
        /// </summary>
        End
    }
}
