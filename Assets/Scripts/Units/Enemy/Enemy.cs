using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class Enemy : Unit
{
    private List<Hero> heroList;
    [SerializeField] private SO_Enemy enemyStats;

    protected override void Start()
    {
        base.Start();
        GetHeroesInWave(GameState.WaveStart);
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        //SetStats(enemyStats);
    }

    protected override void Death()
    {
        
    }

    private void GetHeroesInWave(GameState gameState)
    {
        if (gameState != GameState.WaveStart)
            return;

        heroList = FindObjectsByType<Hero>(FindObjectsInactive.Exclude, FindObjectsSortMode.None).ToList();
    }

    private void RemoveHero(Unit unitThatDied)
    {
        if (unitThatDied != opponent)
            return;

        heroList.Remove((Hero)unitThatDied);

        //TODO end the wave if hero list is empty
    }

    protected override void GetTarget()
    {
        opponent = heroList.OrderBy(enemy => Vector2.Distance(enemy.transform.position, transform.position)).FirstOrDefault();
    }
}
