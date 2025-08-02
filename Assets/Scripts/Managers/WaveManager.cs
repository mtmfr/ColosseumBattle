using System;
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
    private int aliveHeroes;

    public int currentWave;

    public WaveState waveState { get; private set; }

    private WaveManager() { }

    private void Awake()
    {
        if (instance != null)
            Destroy(instance);
        
        instance = this;
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

    /// <summary>
    /// Set the new state of the wave
    /// </summary>
    /// <param name="newState"></param>
    public void UpdateWaveState(WaveState newState)
    {
        switch (newState)
        {
            case WaveState.Start:
                if (nextWaveEnemies.Count() == 0)
                    throw new ArgumentNullException("No enemies in wave", "there is no enemies to be spawn in the wave");

                enemiesInWave = nextWaveEnemies.Length;
                enemySpawner.SpawnEnemies(ref nextWaveEnemies);

                nextWaveEnemies = null;

                heroSpawner.SpawnHeroes(ref PartyManager.heroesInBattle);
                SetAliveHeroes(ref aliveHeroes);

                currentWave++;
                break;
            case WaveState.Middle:
                break;
            case WaveState.End:
                GenerateNextWave();
                waveState = WaveState.NotInWave;
                heroSpawner.DespawnHeroes(PartyManager.heroesInBattle);
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

    private void SetAliveHeroes(ref int aliveHeroes)
    {
        Hero[] heroes = PartyManager.heroesInBattle;

        int foundHeroes = 0;

        for (int i = 0; i < heroes.Length; i++)
        {
            foundHeroes += heroes[i] == null ? 0 : 1;
        }

        aliveHeroes = foundHeroes;
    }

    public void EnemyDied()
    {
        enemiesInWave--;

        if (enemiesInWave <= 0)
            UpdateWaveState(WaveState.End);
    }

    public void HeroDied()
    {
        aliveHeroes--;

        if (aliveHeroes <= 0)
            GameManager.UpdateGameState(GameState.GameOver);
    }

    public void ResetWaveCount() => currentWave = 0;

    public enum WaveState
    {
        NotInWave,
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
