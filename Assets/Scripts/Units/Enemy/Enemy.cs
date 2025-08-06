using System.Linq;
using UnityEngine;

public abstract class Enemy : Unit
{
    [field: SerializeField] public SO_Enemy enemySO { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        SetParameters(enemySO);
    }

    protected override void SetTarget()
    {
        target = GetTarget<Hero>();
    }

    protected override void GetAvailableTarget()
    {
        availableTargets = GetAllUnits<Hero>().ToList();
    }

    protected override void Attack(Unit target)
    {
        if (target == null)
        {
            attackTimer = 0;
            return;
        }

        if (isFirstAttack)
        {
            if (attackTimer > attackParameters.firstAttackCooldown)
            {
                animator.SetTrigger(animAttackHash);
                AttackMotion(target, attackParameters.attackPower);
                attackTimer = 0;
                isFirstAttack = false;
            }
            else
                attackTimer += Time.fixedDeltaTime;
        }
        else
        {
            if (attackTimer > attackParameters.attackCooldown)
            {
                animator.SetTrigger(animAttackHash);
                AttackMotion(target, attackParameters.attackPower);
                attackTimer = 0;
            }
            else
                attackTimer += Time.fixedDeltaTime;
        }
    }

    protected virtual void AttackMotion(Unit target, int damageToDeal)
    {
        UnitEvent.DealDamage(target, damageToDeal);
    }

    protected override void Death(int gameObjectId)
    {
        if (gameObjectId != gameObject.GetInstanceID())
            return;

        base.Death(gameObjectId);
        ObjectPool.SetObjectInactive(this);
        GameManager.AddGold(enemySO.goldDrop);

        WaveManager.instance.EnemyDied();
    }
}
