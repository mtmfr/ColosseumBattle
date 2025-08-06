using System.Linq;
using UnityEngine;

public abstract class Hero : Unit
{
    [field: SerializeField] public SO_Hero heroSO { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        SetParameters(heroSO);
    }

    protected override void SetTarget()
    {
        target = GetTarget<Enemy>();
    }

    protected override void GetAvailableTarget()
    {
        availableTargets = GetAllUnits<Enemy>().ToList();
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

    protected abstract void AttackMotion(Unit target, int damageToDeal);


    protected override void Death(int gameObjectId)
    {
        if (gameObjectId != gameObject.GetInstanceID())
            return;

        base.Death(gameObjectId);
        UnitEvent.Dying(this);
        ObjectPool.SetObjectInactive(this);
        PartyManager.RemoveHeroFromParty(this);

        WaveManager.instance.HeroDied();
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
