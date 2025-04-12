using System;
using System.Linq;
using UnityEngine;

public class FireBall : MonoBehaviour
{
    private LayerMask mask;
    private int damage;

    [SerializeField] private float attackRadius;

    public void SetFireball(LayerMask layerToHit, int damageToDeal)
    {
        mask = layerToHit;
        damage = damageToDeal;

        Attack();
    }

    private void Attack()
    {
        Collider2D[] attackCol = Physics2D.OverlapCircleAll(transform.position, attackRadius, mask);

        foreach (Collider2D collider in attackCol)
        {
            
            if (!collider.TryGetComponent<Unit>(out Unit ToDamage))
                continue;

            UnitEvent.DealDamage(ToDamage, damage);
        }
    }
}
