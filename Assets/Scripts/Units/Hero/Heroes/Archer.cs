using UnityEngine;

public class Archer : Hero
{
    [SerializeField] private Arrow arrow;

    protected override void AttackMotion(Unit target, int damageToDeal)
    {
        Arrow shotArrow = ObjectPool.GetObject(arrow, transform.position, Quaternion.identity);

        shotArrow.SetArrow(target, damageToDeal, attackParameters.attackCooldown);
    }
}
