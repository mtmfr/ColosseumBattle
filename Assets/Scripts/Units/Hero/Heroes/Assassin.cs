using UnityEngine;

public class Assassin : Hero
{

    protected override void SetTarget()
    {
        target = GetTarget<Enemy>(SortType.Health);
    }

    protected override void AttackMotion(Unit target, int damageToDeal)
    {
        UnitEvent.DealDamage(target, damageToDeal);
    }
}
