using NUnit.Framework;
using System;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class HeroSpawner
{
    [Tooltip("The authorized position of spawn for the heroes")]
    [SerializeField] private BoxCollider2D heroSpawnZone;

    /// <summary>
    /// Spawn the hero in the referenced array
    /// </summary>
    /// <param name="heroesToSpawn">the heroes to spawn</param>
    public void SpawnHeroes(Hero[] heroesToSpawn)
    {
        for (int heroId = 0; heroId < heroesToSpawn.Length; heroId++)
        {
            if (heroesToSpawn[heroId] == null)
                continue;

            ObjectPool.GetObject(heroesToSpawn[heroId], GetRandomSpawnPoint(), Quaternion.identity);
        }
    }

    /// <summary>
    /// Get a random spawn point from the hero spawn zone
    /// </summary>
    private Vector2 GetRandomSpawnPoint()
    {
        float minXPos = heroSpawnZone.bounds.min.x;
        float maxXPos = heroSpawnZone.bounds.max.x;

        float randomXPos = Random.Range(minXPos, maxXPos);

        float minYPos = heroSpawnZone.bounds.min.y;
        float maxYPos = heroSpawnZone.bounds.max.y;

        float randmYPos = Random.Range(minYPos, maxYPos);

        return new Vector2(randomXPos, randmYPos);
    }
}
