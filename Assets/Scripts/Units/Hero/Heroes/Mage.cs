using UnityEngine;

public class Mage : Hero
{
    protected override void AttackMotion(Unit target, int damageToDeal)
    {
        UnitEvent.DealDamage(target, damageToDeal);
    }
}