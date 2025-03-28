using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class Hero : Unit
{
     List<Enemy> enemyList;
    [SerializeField] private SO_Hero heroStats;

    protected override void Start()
    {
        base.Start();
        GetEnemiesInWave(GameState.WaveStart);
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        SetStats(heroStats);
        attackCooldown = attackSpeed;
    }

    private void GetEnemiesInWave(GameState gameState)
    {
        if (gameState != GameState.WaveStart)
            return;

        enemyList = FindObjectsByType<Enemy>(FindObjectsInactive.Exclude, FindObjectsSortMode.None).ToList();
    }

    protected override void UpdateState()
    {
        Vector2 closestEnemy = enemyList.OrderBy(enemy => Vector2.Distance(enemy.transform.position, transform.position))
            .FirstOrDefault().transform.position;

        float distance = Vector2.Distance(closestEnemy, transform.position);

        int caseId = distance > fleeRange ? 0 : 1 + distance > attackRange ? 0 : 2;

        switch (caseId)
        {
            case 0:
                ChangeCurrentState(UnitState.Idle);
                break;
            case 1:
            case 3:
                ChangeCurrentState(UnitState.Fleeing);
                break;
            case 2:
                ChangeCurrentState(UnitState.Attacking);
                break;
        }
    }

    protected override void Death()
    {
        ObjectPool.SetObjectInactive(this);
        UnitEvent.Dying(this);
        opponent = null;
        wasDead = true;
    }

    private void RemoveEnemy(Unit unitThatDied)
    {
        if (unitThatDied != opponent)
            return;

        enemyList.Remove((Enemy)unitThatDied);

        //TODO end the wave if enemy list is empty
    }

    protected override void OnHeal(Unit unit, int healedHp)
    {
        if (unit != this)
            return;

        health += healedHp;
        health = Mathf.Clamp(health, 0, heroStats.health);
    }

    protected override void GetTarget()
    {
        opponent = enemyList.OrderBy(enemy => Vector2.Distance(enemy.transform.position, transform.position)).FirstOrDefault();
    }
}
