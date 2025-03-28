using UnityEngine;

public class Archer : Hero
{
    [SerializeField] private Arrow arrow;

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
        Attack();
    }

    protected override void Attack()
    {
        if (attackCooldown < attackSpeed)
        {
            attackCooldown += Time.deltaTime;
            return;
        }

        if (opponent == null)
        {
            GetTarget();
            return;
        }

        Arrow arrowToUse = ObjectPool.GetObject(arrow, transform.position, Quaternion.identity);

        if (arrowToUse == null)
            Debug.LogError("there is no arrow to shoot", this);

        arrowToUse.SetArrow(opponent, attackPower, attackSpeed, gameObject.layer);

        attackCooldown = 0;
    }
}
