using UnityEngine;

public class Mage : Hero
{
    [SerializeField] private FireBall fireBall;
    protected override void AttackMotion(Unit target, int damageToDeal)
    {
        UnitEvent.DealDamage(target, damageToDeal);

        FireBall castedFireball = ObjectPool.GetObject(fireBall, target.transform.position, Quaternion.identity);
        castedFireball.SetFireball(1 << target.gameObject.layer, damageToDeal);
    }
}