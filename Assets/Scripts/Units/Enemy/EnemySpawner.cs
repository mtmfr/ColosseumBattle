using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private BoxCollider2D enemySpawnZone;

    private List<Enemy> spawnableEnemies = new();
    public Enemy[] enemiesInWave { get; private set; }

    /// <summary>
    /// Get the available credit for the next wave
    /// </summary>
    private int GetWaveCredit(int currentWave)
    {
        int nextWave = currentWave + 1;
        if (nextWave <= 0)
            return 0;

        return nextWave + currentWave;
    }

    /// <summary>
    /// Create the next wave of enemies
    /// </summary>
    public Enemy[] GetNextEnemies(int currentWave)
    {
        int waveCredit = GetWaveCredit(currentWave);

        List<Enemy> availableEnemies = spawnableEnemies;
        List<Enemy> enemiesToSpawn = new();

        while (waveCredit > 0)
        {
            if (availableEnemies.Count == 0)
                break;

            Enemy enemyToAdd = GetRandomEnemy(availableEnemies, out int generatedId);

            if (enemyToAdd == null)
                break;

            int enemyCost = enemyToAdd.miscParameters.cost;

            if (enemyCost > waveCredit)
            {
                availableEnemies.RemoveAt(generatedId);
                continue;
            }

            enemiesToSpawn.Add(enemyToAdd);
            waveCredit -= enemyCost;
        }

        if (enemiesToSpawn.Count == 0)
        {
            Debug.LogError("No enemies in next wave");
            return null;
        }

        return enemiesToSpawn.ToArray();
    }

    /// <summary>
    /// Get a random enemy to spawn from the passed list
    /// </summary>
    /// <param name="enemyList"></param>
    /// <param name="enemyId"></param>
    /// <returns></returns>
    private Enemy GetRandomEnemy(List<Enemy> enemyList, out int enemyId)
    {
        int spawnableCount = enemyList.Count;

        if (spawnableCount <= 0)
        {
            enemyId = -1;
            return null;
        }

        enemyId = Random.Range(0, spawnableCount);

        return enemyList[enemyId];
    }

    public void SpawnEnemies(Enemy[] enemiesToSpawn)
    {
        for (int enemyId = 0; enemyId < enemiesToSpawn.Length; enemyId++)
        {
            ObjectPool.GetObject(enemiesToSpawn[enemyId], GetRandomSpawnPoint(), Quaternion.identity);
        }
    }

    [ContextMenu("RandomSpawnPos")]
    private Vector2 GetRandomSpawnPoint()
    {
        float minXPos = enemySpawnZone.transform.position.x - enemySpawnZone.size.x / 2;
        float maxXPos = enemySpawnZone.transform.position.x + enemySpawnZone.size.x / 2;

        float randomXPos = Random.Range(minXPos, maxXPos);

        float minYPos = enemySpawnZone.transform.position.y - enemySpawnZone.size.y / 2;
        float maxYPos = enemySpawnZone.transform.position.y + enemySpawnZone.size.y / 2;

        float randmYPos = Random.Range(minYPos, maxYPos);

        Debug.Log(new Vector2(randomXPos, randmYPos));

        return new Vector2(randomXPos, randmYPos);
    }
}
