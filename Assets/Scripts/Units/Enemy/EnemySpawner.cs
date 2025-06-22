using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class EnemySpawner
{
    [Tooltip("The authorized position of spawn for the enemies")]
    [SerializeField] private BoxCollider2D enemySpawnZone;

    [SerializeField] private List<Enemy> spawnableEnemies = new();
    public Enemy[] enemiesInWave { get; private set; }

    private EnemySpawner() { }

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
    /// Generate the next wave of enemies
    /// </summary>
    public Enemy[] GetNextEnemies(int currentWave)
    {
        int waveCredit = GetWaveCredit(currentWave);

        //Create a list of the enemies that can be spawned
        List<Enemy> availableEnemies = spawnableEnemies;

        //Create a list that will contain the enemies to spawn
        List<Enemy> enemiesToSpawn = new();

        while (waveCredit > 0)
        {
            if (availableEnemies.Count == 0)
                break;

            Enemy enemyToAdd = GetRandomEnemy(availableEnemies, out int generatedId);

            if (enemyToAdd == null)
                break;

            int enemyCost = enemyToAdd.enemySO.miscParameters.cost;

                //If the cost of the enemy is greater than the remaining credit of the wave
            if (enemyCost > waveCredit)
            {
                //Remove it from available enmies and start a new loop
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
    /// <param name="enemyList">the list to select enemy from</param>
    /// <param name="enemyId">the id of the picked enemy</param>
    /// <returns>the random enemy and it's id or null and -1 as the id</returns>
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

    /// <summary>
    /// Spawn the enemies of the wave
    /// </summary>
    /// <param name="enemiesToSpawn">The enemies to spawn</param>
    public void SpawnEnemies(ref Enemy[] enemiesToSpawn)
    {
        for (int enemyId = 0; enemyId < enemiesToSpawn.Length; enemyId++)
        {
            Enemy enemy = enemiesToSpawn[enemyId];
            Vector2 spawnPoint = GetRandomSpawnPoint();

            Enemy spawned = ObjectPool.GetObject(enemy, spawnPoint, Quaternion.identity);

            //Replace the referece in the array to the spawned hero
            //Without it the object pooling can't deactivate it
            if (spawned.GetInstanceID() != enemy.GetInstanceID())
                enemiesToSpawn[enemyId] = spawned;
        }
    }

    /// <summary>
    /// Get a random spawn position in the spawn zone of the enemies
    /// </summary>
    private Vector2 GetRandomSpawnPoint()
    {
        float minXPos = enemySpawnZone.bounds.min.x;
        float maxXPos = enemySpawnZone.bounds.max.x;

        float randomXPos = Random.Range(minXPos, maxXPos);

        float minYPos = enemySpawnZone.bounds.min.y;
        float maxYPos = enemySpawnZone.bounds.max.y;

        float randmYPos = Random.Range(minYPos, maxYPos);

        return new Vector2(randomXPos, randmYPos);
    }
}
