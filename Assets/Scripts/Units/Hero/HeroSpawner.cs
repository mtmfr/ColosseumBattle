using System;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class HeroSpawner
{
    [SerializeField] private BoxCollider2D heroSpawnZone;

    public void SpawnHeroes(Hero[] heroesToSpawn)
    {
        for (int heroId = 0; heroId < heroesToSpawn.Length; heroId++)
        {
            if (heroesToSpawn[heroId] == null)
                continue;

            ObjectPool.GetObject(heroesToSpawn[heroId], GetRandomSpawnPoint(), Quaternion.identity);
        }
    }

    [ContextMenu("RandomSpawnPos")]
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
