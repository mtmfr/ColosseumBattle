using UnityEngine;

public abstract class Hero : Unit
{
    [field: SerializeField] public SO_Hero heroSO { get; private set; }

    private void Awake()
    {
        SetParameters(heroSO);
    }

    protected override void GetAvailableTarget()
    {
        availableTarget = GetAllUnits<Enemy>();
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

    protected abstract void AttackMotion(Unit target, int damageToDeal);


    protected override void Death()
    {
        ObjectPool.SetObjectInactive(this);
    }

    protected override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();

        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(transform.position, heroSO.attackParameters.fleeDistance);

        Gizmos.color = Color.green;

        Gizmos.DrawWireSphere(transform.position, heroSO.attackParameters.attackDistance);
    }
}
