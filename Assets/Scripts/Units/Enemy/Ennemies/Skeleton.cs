using UnityEngine;

public class Skeleton : Enemy
{
    [SerializeField] private Arrow arrow;

    protected override void AttackMotion(Unit target, int damageToDeal)
    {
        Arrow arrowToShoot = ObjectPool.GetObject(arrow, transform.position, Quaternion.identity);
        arrowToShoot.SetArrow(target, damageToDeal, attackParameters.attackCooldown);
    }
}
