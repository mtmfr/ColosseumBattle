using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    private static WaveManager _instance;

    public static WaveManager Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.LogError("No Wave Manager");
            }
            return _instance;
        }
    }

    [field: SerializeField] public GameObject HeroSpawnZone { get; private set; }
    [field: SerializeField] public GameObject EnemySpawnZone { get; private set; }

    [field: SerializeField] public List<GameObject> HeroList { get; private set; } = new List<GameObject>();


    [field: SerializeField] public List<Enemy> EnemyList { get; private set; } = new List<Enemy>();
    [field: SerializeField] public List<GameObject> EnemiesInWave { get; private set; } = new List<GameObject>();


    public int CurrentWave { get; set; }
    private int waveCredit;
    [field: SerializeField] public int TimePerWave { get; set; }

    private void Awake()
    {
        if (_instance)
            Destroy(gameObject);
        else
        {
            _instance = this;
        }

        DontDestroyOnLoad(this);
    }

    //Get a random point in the Collied of the spawner
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
        foreach(GameObject hero in Instance.HeroList)
        {
            if (hero)
            {
                GameObject heroToSpawn = hero;
                Instantiate(heroToSpawn, Instance.SpawnZone(Instance.HeroSpawnZone),Quaternion.identity);
            }
        }
    }

    public void GenerateWave()
    {
        Instance.waveCredit = Instance.CurrentWave * 5;
        SpawnEnemies();
    }

    /// <summary>
    /// Get random enemies from the enemy list and make them spawn at the start of the fight
    /// </summary>
    public void SpawnEnemies()
    {
        //Once the credit of the wave are down to zero break out of the while loop and spawn all of the enemies in EnemiesInWave

        List<Enemy> genEnemies = new();
        Instance.EnemiesInWave.Clear();

        int randEnemyCost;
        int randEnemyId;

        while (Instance.waveCredit > 0)
        {
            //Get a random enemy from the enemy list (EnemyList) and check if the wave can afford it
            randEnemyId = Random.Range(0, Instance.EnemyList.Count);

            randEnemyCost = Instance.EnemyList[randEnemyId].Cost;
            if (Instance.waveCredit - randEnemyCost >= 0)
            {
                //If the wave can afford it then add it to the list of enemies to spawn (genEnemies)
                genEnemies.Add(Instance.EnemyList[randEnemyId]);
                Instance.waveCredit -= randEnemyCost;

                //check the credit of the wave to break out of the while if it is 0
                if (Instance.waveCredit == 0)
                    break;
            }
            //If the wave can't afford it then create a new List which will contain every enemy that the wave can still afford
            else if (Instance.waveCredit > 0)
            {
                //Create a new list that will hold all the enemy that have a cost under he wave credit
                List<Enemy> enemyUnderValue = new();

                foreach (Enemy enemy in Instance.EnemyList)
                {
                    if (enemy.Cost <= Instance.waveCredit)
                    {
                        enemyUnderValue.Add(enemy);
                    }
                    else
                        continue;
                }

                //break out of the while if the lsit of enemy that can still be afforded is null
                if (enemyUnderValue.Count == 0)
                    break;

                //randomise the enmy to spawn from enemyUnderValue
                randEnemyId = Random.Range(0, enemyUnderValue.Count);

                randEnemyCost = Instance.EnemyList[randEnemyId].Cost;

                genEnemies.Add(enemyUnderValue[randEnemyId]);
                Instance.waveCredit -= randEnemyCost;

                //check the credit of the wave to break out of the while if it is 0
                if (Instance.waveCredit == 0)
                    break;
            }
        }
        
        //spawn all the enemies in genEnemies
        foreach(Enemy enemy in genEnemies)
        {
            GameObject enemyCopy = Instantiate(enemy.gameObject, Instance.SpawnZone(Instance.EnemySpawnZone), Quaternion.identity);
            Instance.EnemiesInWave.Add(enemyCopy);
        }
    }

    public void FightStart()
    {
        Instance.CurrentWave++;
        EventManager.Instance.WaveEvent.StartWaveEvent(Instance.CurrentWave);
        EventManager.Instance.MiscEvent.OnTimerChange(Instance.TimePerWave);
    }

    ///<summary>
    ///Get the Id of the dead hero and remove the enemy with the corresponding id from the hero list
    ///</summary>
    public void HeroInParty(int heroId)
    {
        for (int Index = 0; Index < Instance.HeroList.Count; Index++)
        {
            GameObject HeroInParty = Instance.HeroList[Index];
            if (HeroInParty.GetInstanceID() == heroId)
            {
                Instance.HeroList.Remove(HeroInParty);

                if (Instance.HeroList.Count == 0)
                    GameOver();
                else
                    break;
            }
            else
                continue;
        }
    }


    /// <summary>
    /// Get the Id of the dead enemy and remove the enemy with the corresponding id from the enemy list
    /// </summary>
    /// <param name="enemyId"></param>
    public void EnemyInWave(int enemyId)
    {
        for (int Index = 0; Index < Instance.EnemiesInWave.Count; Index++)
        {
            GameObject enemyInWave = Instance.EnemiesInWave[Index];
            if (enemyInWave.GetInstanceID() == enemyId)
            {
                Instance.EnemiesInWave.Remove(enemyInWave);

                if(Instance.EnemiesInWave.Count == 0)
                    StartCoroutine(ShopDelay());
                else
                    break;
            }
            else
                continue;
        }
    }

    public void EndWave()
    {
        foreach(Hero hero in FindObjectsOfType<Hero>())
        {
            Destroy(hero.gameObject);
        }
    }

    private void GameOver()
    {
        GameManager.Instance.UpdateGameState(GameState.Lose);
    }

    private IEnumerator ShopDelay()
    {
        yield return new WaitForSeconds(1);
        GameManager.Instance.UpdateGameState(GameState.Shop);
    }
}
