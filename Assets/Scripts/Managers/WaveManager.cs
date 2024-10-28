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

    [field: SerializeField] public List<Enemy> EnemyList { get; private set; } = new List<Enemy>();
    [field: SerializeField] public List<Enemy> enemiesToSpawn { get; private set; } = new List<Enemy>();

    public int HeroInParty { get; set; }
    public int EnemyLeftInWave {  get; set; }

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
        foreach(GameObject hero in GameManager.Instance.HeroList)
        {
            if (hero)
            {
                GameObject heroToSpawn = Instantiate(hero, Instance.SpawnZone(Instance.HeroSpawnZone),Quaternion.identity);
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
        //Get a random enemy from the enemy list (EnemyList) and check if the wave can afford it
        //If the wave can afford it then add it to the list of enemies to spawn (enemiesToSpawn)
        //If the wave can't afford it then create a new List which will contain every enemy that the wave can still afford
        //Once the credit of the wave are down to zero break out of the while loop and spawn all of the enemies in enemiesToSpawn

        List<Enemy> genEnemies = new();
        Instance.enemiesToSpawn.Clear();

        int randEnemyCost;
        int randEnemyId;

        while (Instance.waveCredit > 0)
        {
            randEnemyId = Random.Range(0, Instance.EnemyList.Count);

            randEnemyCost = Instance.EnemyList[randEnemyId].cost;
            if (Instance.waveCredit - randEnemyCost >= 0)
            {
                genEnemies.Add(Instance.EnemyList[randEnemyId]);
                Instance.waveCredit -= randEnemyCost;
            }
            else if (Instance.waveCredit > 0)
            {
                List<Enemy> enemyUnderValue = new();

                foreach (Enemy enemy in EnemyList)
                {
                    if (enemy.cost <= waveCredit)
                    {
                        enemyUnderValue.Add(enemy);
                    }
                    else
                        continue;
                }

                randEnemyId = Random.Range(0, enemyUnderValue.Count);

                randEnemyCost = Instance.EnemyList[randEnemyId].cost;

                genEnemies.Add(Instance.EnemyList[randEnemyId]);
                Instance.waveCredit -= randEnemyCost;
            }
            else 
                break;
        }

        Instance.enemiesToSpawn = genEnemies;

        foreach (Enemy enemy in Instance.enemiesToSpawn)
        {
            GameObject enemyToSpawn = Instantiate(enemy.gameObject, Instance.SpawnZone(Instance.EnemySpawnZone), Quaternion.identity);
        }
    }

    public void FightStart()
    {
        Instance.CurrentWave++;
        EventManager.Instance.WaveEvent.OnStartWave(Instance.CurrentWave);
        EventManager.Instance.MiscEvent.OnTimerChange(Instance.TimePerWave);
    }

    public void EnemyInWave(int enemyLeft)
    {
        if (enemyLeft == 0)
            StartCoroutine(ShopDelay());
    }

    public void EndWave()
    {
        foreach(Hero hero in FindObjectsOfType<Hero>())
        {
            Destroy(hero.gameObject);
        }
    }

    private IEnumerator ShopDelay()
    {
        yield return new WaitForSeconds(1);
        GameManager.Instance.UpdateGameState(GameState.Shop);
    }
}
