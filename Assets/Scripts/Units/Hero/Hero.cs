using UnityEngine;

public abstract class Hero : Unit
{
    [SerializeField] private SO_Hero heroSO;

    private void Awake()
    {
        SetParameters(heroSO);
    }

    protected override void Death()
    {
        ObjectPool.SetObjectInactive(this);
    }

    protected override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();

        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(transform.position, heroSO.attackParameters.fleeDistance);

        Gizmos.color = Color.green;

        Gizmos.DrawWireSphere(transform.position, heroSO.attackParameters.attackDistance);
    }
}
