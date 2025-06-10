using UnityEngine;

public class Assassin : Hero
{
    protected override void AttackMotion(Unit target, int damageToDeal)
    {
        UnitEvent.DealDamage(target, damageToDeal);
    }
}
