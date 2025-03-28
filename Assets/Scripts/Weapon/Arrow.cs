using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
public class Arrow : MonoBehaviour
{
    private Rigidbody2D rb;
    private Unit target;

    private int damageToDeal;

    private float currentMovementTime = 0;
    private float movementTime;

    private bool canAct;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (canAct)
            GoToTarget();
    }

    private void OnDisable()
    {
        canAct = false;
        target = null;
        currentMovementTime = 0;
    }

    private void GoToTarget()
    {
        if (!target.gameObject.activeSelf)
        {
            ObjectPool.SetObjectInactive(this);
            return;
        }

        rb.transform.position = Vector3.Lerp(transform.position, target.transform.position, currentMovementTime / movementTime);
        currentMovementTime += Time.deltaTime;
    }

    public void SetArrow(Unit target, int damage, float movementTime, int layer)
    {
        this.target = target;
        this.damageToDeal = damage;
        this.movementTime = movementTime;

        gameObject.layer = layer;

        canAct = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.TryGetComponent(out Unit unit))
            return;

        if (unit == target)
        {
            UnitEvent.DealDamage(target, damageToDeal);
            ObjectPool.SetObjectInactive(this);
        }
    }
}
