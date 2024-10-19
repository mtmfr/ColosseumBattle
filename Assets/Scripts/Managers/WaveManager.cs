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

    public int HeroInParty { get; set; }
    public int EnemyLeftInWave {  get; set; }

    public int CurrentWave { get; set; }
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

    public Vector2 SpawnZone(GameObject spawnZone)
    {
        Collider2D range = spawnZone.GetComponent<Collider2D>();

        Vector2 spawnPoint = new Vector2(
            Random.Range(range.bounds.min.x, range.bounds.max.x),
            Random.Range(range.bounds.min.y, range.bounds.max.y));
        return spawnPoint;
    }
    public void StartSpawn()
    {
        foreach(GameObject hero in GameManager.Instance.HeroList)
        {
            if (hero)
            {
                GameObject heroToSpawn = Instantiate(hero, Instance.SpawnZone(Instance.HeroSpawnZone),Quaternion.identity);
            }
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
            GameManager.Instance.UpdateGameState(GameState.Shop);
    }

    public void EndWave()
    {
        foreach(Hero hero in FindObjectsOfType<Hero>())
        {
            StartCoroutine(HeroToDestroy(hero.gameObject));
        }
    }

    private IEnumerator HeroToDestroy(GameObject hero)
    {
        yield return new WaitForSeconds(2);
        Destroy(hero);
    }
}
