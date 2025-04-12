using UnityEngine;

public class Priest : Hero
{
    protected override void GetAvailableTarget()
    {
        availableTarget = GetAllUnits<Unit>();
    }

    protected override void SetTarget()
    {
        target = GetTarget<Hero>(SortType.Health);

        if (target == null)
            target = GetTarget<Enemy>();
    }

    protected override void AttackMotion(Unit target, int damageToDeal)
    {
        if (target is Hero)
        {
            int healedAmount = Mathf.FloorToInt(damageToDeal * (target.health/(float)target.miscParameters.health));

            UnitEvent.HealedDamage(target, healedAmount);
        }

        if (target is Enemy)
            UnitEvent.DealDamage(target, damageToDeal);
    }
}
