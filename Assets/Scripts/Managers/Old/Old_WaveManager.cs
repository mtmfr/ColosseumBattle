using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Old_WaveManager : MonoBehaviour
{
    [SerializeField] private GameObject heroSpawnZone;
    [SerializeField] private GameObject enemySpawnZone;

    [SerializeField] private List<GameObject> HeroList = new();

    [SerializeField] private List<Old_SO_EnemyStats> enemyStats;
    [SerializeField] private List<Old_Enemy> enemyList;
    List<GameObject> EnemiesInWave = new();

    public int CurrentWave { get; set; }
    private int waveCredit;
    [field: SerializeField] public int TimePerWave { get; set; }

    private void OnEnable()
    {
        Old_GameManager.Instance.OnGameStateChanged += ResetCurrentWave;
        Old_GameManager.Instance.OnGameStateChanged += FightStarted;

        WaveEvent.OnAddHeroToList += AddHeroToList;
        WaveEvent.OnRemoveHeroFromList += RemoveHeroFromList;

        WaveEvent.OnRemoveEnemyFromWave += RemoveEnemyFromList;

        WaveEvent.OnWaveEnded += EndWave;
    }

    private void OnDisable()
    {
        Old_GameManager.Instance.OnGameStateChanged -= ResetCurrentWave;

        WaveEvent.OnAddHeroToList -= AddHeroToList;
        WaveEvent.OnRemoveHeroFromList -= RemoveHeroFromList;

        WaveEvent.OnRemoveEnemyFromWave -= RemoveEnemyFromList;

        WaveEvent.OnWaveEnded -= EndWave;
    }

    #region Hero spawn
    //Get a random point in the Collider of the spawner
    public Vector2 SpawnZone(GameObject spawnZone)
    {
        Collider2D range = spawnZone.GetComponent<Collider2D>();

        Vector2 spawnPoint = new (
            Random.Range(range.bounds.min.x, range.bounds.max.x),
            Random.Range(range.bounds.min.y, range.bounds.max.y));
        return spawnPoint;
    }

    public void StartHeroSpawn()
    {
        foreach(GameObject hero in HeroList)
        {
            if (hero)
            {
                GameObject heroToSpawn = hero;
                Instantiate(heroToSpawn, SpawnZone(heroSpawnZone),Quaternion.identity);
            }
        }
    }
    #endregion

    #region enemy spawn
    public void GenerateWave()
    {
        waveCredit = CurrentWave * 5;
        SpawnEnemies();
    }

    /// <summary>
    /// Get random enemies from the enemy list and make them spawn at the start of the fight
    /// </summary>
    public void SpawnEnemies()
    {
        //Once the credit of the wave are down to zero break out of the while loop and spawn all of the enemies in EnemiesInWave

        List<Old_Enemy> genEnemies = new();
        EnemiesInWave.Clear();

        int randEnemyCost;
        int randEnemyId;

        while (waveCredit > 0)
        {
            //Get a random enemy from the enemy list (enemyList) and check if the wave can afford it
            randEnemyId = Random.Range(0, enemyList.Count);

            randEnemyCost = enemyStats[randEnemyId].Cost;
            if (waveCredit - randEnemyCost >= 0)
            {
                //If the wave can afford it then add it to the list of enemies to spawn (genEnemies)
                genEnemies.Add(enemyList[randEnemyId]);
                waveCredit -= randEnemyCost;

                //check the credit of the wave to break out of the while if it is 0
                if (waveCredit == 0)
                    break;
            }
            //If the wave can't afford it then create a new List which will contain every enemy that the wave can still afford
            else if (waveCredit > 0)
            {
                //Create a new list that will hold all the enemy that have a cost under he wave credit
                List<Old_Enemy> enemyUnderValue = new();

                for (int Id = 0; Id < enemyList.Count; Id++)
                {
                    Old_Enemy enemy = enemyList[Id];
                    if (enemyStats[Id].Cost <= waveCredit)
                    {
                        enemyUnderValue.Add(enemy);
                    }
                    else
                        continue;
                }

                //break out of the while if the list of enemy that can still be afforded is null
                if (enemyUnderValue.Count == 0)
                    break;

                //randomise the enemy to spawn from enemyUnderValue
                randEnemyId = Random.Range(0, enemyUnderValue.Count);

                randEnemyCost = enemyStats[randEnemyId].Cost;

                genEnemies.Add(enemyUnderValue[randEnemyId]);
                waveCredit -= randEnemyCost;

                //check the credit of the wave to break out of the while if it is 0
                if (    waveCredit == 0)
                    break;
            }
        }
        
        //spawn all the enemies in genEnemies
        foreach(Old_Enemy enemy in genEnemies)
        {
            GameObject enemyCopy = Instantiate(enemy.gameObject, SpawnZone(enemySpawnZone), Quaternion.identity);
            EnemiesInWave.Add(enemyCopy);
        }
    }

    #endregion

    #region wave control
    public void StartWave()
    {
        CurrentWave++;
        WaveEvent.StartWaveEvent(CurrentWave);
    }

    private void EndWave()
    {
        foreach (Old_Hero hero in FindObjectsByType<Old_Hero>(FindObjectsInactive.Exclude, FindObjectsSortMode.None))
        {
            Destroy(hero.gameObject);
        }
    }
    #endregion

    #region Control list
    private void AddHeroToList(GameObject hero)
    {
        HeroList.Add(hero);
    }

    private void RemoveHeroFromList(GameObject hero)
    {
        HeroList.Remove(hero);

        if (HeroList.Count == 0)
            GameOver();
    }

    private void RemoveEnemyFromList(GameObject enemy)
    {
        EnemiesInWave.Remove(enemy);

        if (EnemiesInWave.Count == 0)
            StartCoroutine(ShopDelay());
    }
    #endregion

    private void ResetCurrentWave(Old_GameState gameState)
    {
        if (gameState != Old_GameState.Start)
            return;

        CurrentWave = 0;
    }

    private void FightStarted(Old_GameState gameState)
    {
        if (gameState != Old_GameState.Fight)
            return;

        StartWave();
        StartHeroSpawn();
        GenerateWave();
    }

    #region gameManager update state
    private void GameOver()
    {
        Old_GameManager.Instance.UpdateGameState(Old_GameState.Lose);
    }

    private IEnumerator ShopDelay()
    {
        yield return new WaitForSeconds(1);
        Old_GameManager.Instance.UpdateGameState(Old_GameState.Shop);
    }
    #endregion
}
