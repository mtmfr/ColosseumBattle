using UnityEngine;

public class Assassin : Hero
{

    protected override void SetTarget()
    {
        target = GetTarget<Hero>(SortType.Health);
    }

    protected override void AttackMotion(Unit target, int damageToDeal)
    {
        UnitEvent.DealDamage(target, damageToDeal);
    }
}
