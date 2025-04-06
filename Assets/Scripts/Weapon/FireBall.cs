using UnityEngine;

public class FireBall : MonoBehaviour
{
    private LayerMask mask;
    private int damage;

    public void SetFireball(LayerMask layerToHit, int damageToDeal)
    {
        mask = layerToHit;
        damage = damageToDeal;
    }
}
