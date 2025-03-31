using UnityEngine;

public class Tank : Hero
{
    protected override void AttackMotion(Unit target, int damageToDeal)
    {
        UnitEvent.DealDamage(target, damageToDeal);
        ForceRetarget(target);
    }
}
