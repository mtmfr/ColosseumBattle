using UnityEngine;

public class Warrior : Hero
{
    protected override void Attack(GameObject target)
    {
        base.Attack(target);

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

    private void AttackMotion(GameObject target, int damageToDeal)
    {
        Debug.Log("I am attacking you");
    }
}
