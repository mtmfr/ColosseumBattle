using UnityEngine;

public class FireBall : MonoBehaviour
{
    [SerializeField] private float explosionRadius;

    private LayerMask mask;
    private int damage;

    public void SummonFireball(LayerMask layerToHit, int damageToDeal)
    {
        mask = layerToHit;
        damage = damageToDeal;

        Attack();
    }

    private void Attack()
    {
        Collider2D[] attackCol = Physics2D.OverlapCircleAll(transform.position, explosionRadius, mask);

        foreach (Collider2D collider in attackCol)
        {
            if (attackCol[0] == null)
                break;
            
            if (!collider.TryGetComponent(out Unit ToDamage))
                continue;

            UnitEvent.DealDamage(ToDamage, damage);
        }
        //ObjectPool.SetObjectInactive(this);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
