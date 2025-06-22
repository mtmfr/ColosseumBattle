using UnityEngine;

public abstract class Enemy : Unit
{
    [field: SerializeField] public SO_Enemy enemySO { get; private set; }

    private void Awake()
    {
        SetParameters(enemySO);
    }

    protected override void SetTarget()
    {
        target = GetTarget<Hero>();
    }

    protected override void GetAvailableTarget()
    {
        Hero[] targetArray = GetAllUnits<Hero>();

        for (int id = 0; id < targetArray.Length; id++)
        {
            availableTarget.Add(targetArray[id]);
        }
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
            if (attackTimer < attackParameters.firstAttackCooldown)
            {
                attackTimer += Time.fixedDeltaTime;
                return;
            }
            else
            {
                AttackMotion(target, attackParameters.attackPower);
                attackTimer = 0;
                isFirstAttack = false;
            }
        }

        if (attackTimer < attackParameters.attackCooldown)
            attackTimer += Time.fixedDeltaTime;
        else
        {
            AttackMotion(target, attackParameters.attackPower);
            attackTimer = 0;
        }
    }

    protected virtual void AttackMotion(Unit target, int damageToDeal)
    {
        UnitEvent.DealDamage(target, damageToDeal);
    }

    protected override void Death()
    {
        UnitEvent.Dying(this);
        GameManager.AddGold(enemySO.goldDrop);
        ObjectPool.SetObjectInactive(this);

        WaveManager.instance.EnemyDied();
    }
}
