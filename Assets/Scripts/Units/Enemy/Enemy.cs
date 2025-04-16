using UnityEngine;

public abstract class Enemy : Unit
{
    [SerializeField] private SO_Enemy enemySO;

    private void Awake()
    {
        SetParameters(enemySO);
    }

    protected override void GetAvailableTarget()
    {
        availableTarget = GetAllUnits<Hero>();
    }

    protected override void Attack(Unit target)
    {
        base.Attack(target);

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
        GameManager.AddGold(enemySO.goldDrop);
        ObjectPool.SetObjectInactive(this);
    }
}
