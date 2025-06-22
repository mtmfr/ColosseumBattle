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
    public void SpawnHeroes(ref Hero[] heroesToSpawn)
    {
        for (int heroId = 0; heroId < heroesToSpawn.Length; heroId++)
        {
            Hero hero = heroesToSpawn[heroId];

            if (hero == null)
                continue;

            Hero spawnedHero = ObjectPool.GetObject(hero, GetRandomSpawnPoint(), Quaternion.identity);

            //Replace the referece in the array to the spawned hero
            //Without it the object pooling can't deactivate it
            if (spawnedHero.GetInstanceID() != hero.GetInstanceID())
                heroesToSpawn[heroId] = spawnedHero;
        }
    }

    public void DespawnHeroes(Hero[] heroesToDespawn)
    {
        for (int heroId = 0; heroId < heroesToDespawn.Length; heroId++)
        {
            if (heroesToDespawn[heroId] == null)
                continue;

            ObjectPool.SetObjectInactive(heroesToDespawn[heroId]);
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
