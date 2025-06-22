using UnityEngine;

public abstract class Hero : Unit
{
    [field: SerializeField] public SO_Hero heroSO { get; private set; }

    private void Awake()
    {
        SetParameters(heroSO);
    }

    protected override void SetTarget()
    {
        target = GetTarget<Enemy>();
    }

    protected override void GetAvailableTarget()
    {
        Enemy[] targetArray = GetAllUnits<Enemy>();

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

    protected abstract void AttackMotion(Unit target, int damageToDeal);


    protected override void Death()
    {
        UnitEvent.Dying(this);
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
