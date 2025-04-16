using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public static WaveManager instance { get; private set; }

    [SerializeField] private EnemySpawner enemySpawner;

    private Enemy[] nextWaveEnemies;
    private int enemiesInWave;

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

    public void GenerateNextWave()
    {
        enemySpawner.GetNextEnemies(currentWave);
    }

    public void SwitchWaveState(WaveState newState)
    {
        switch (newState)
        {
            case WaveState.Start:
                if (nextWaveEnemies.Length == 0)
                    throw new System.ArgumentNullException("No enemies in wave", "there is no enemies to be spawn in the wave");

                enemiesInWave = nextWaveEnemies.Length;
                enemySpawner.SpawnEnemies(nextWaveEnemies);

                nextWaveEnemies = null;

                break;
            case WaveState.Middle:
                break;
            case WaveState.End:
                nextWaveEnemies = enemySpawner.GetNextEnemies(currentWave);
                GameManager.UpdateGameState(GameState.Shop);
                break;
            default:
                Debug.LogWarning("State not recognised");
                break;
        }

        waveState = newState;
    }

    public void EnemyDied()
    {
        enemiesInWave--;

        if (enemiesInWave <= 0)
            SwitchWaveState(WaveState.End);
    }

    public void ResetWave() => currentWave = 0;

    public enum WaveState
    {
        None,
        Start,
        Middle,
        End
    }
}
