using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
public class Arrow : MonoBehaviour
{
    private Rigidbody2D rb;

    Unit target;
    int damage;
    float flyTime;
    float timer;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void SetArrow(Unit toTarget, int damageToDeal, float TimebetweenShot)
    {
        target = toTarget;
        damage = damageToDeal;
        flyTime = TimebetweenShot;
        timer = 0;
    }

    private void FixedUpdate()
    {
        if (timer/flyTime < 0.99f)
        {
            Vector3 newPos = Vector3.Lerp(transform.position, target.transform.position, timer/flyTime);
            rb.MovePosition(newPos);
            timer += Time.fixedDeltaTime;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        int collisionLayer = 1 << other.gameObject.layer;
        int targetLayer = 1 << target.gameObject.layer;

        if ((collisionLayer & targetLayer) == 0)
            return;

        if (!other.TryGetComponent(out Unit unit))
            return;

        if (unit != target)
            return;

        UnitEvent.DealDamage(target, damage);
        ObjectPool.SetObjectInactive(this);
    }
}
