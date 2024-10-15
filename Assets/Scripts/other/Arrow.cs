using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Arrow : MonoBehaviour
{
    [SerializeField] private float speed;
    private Rigidbody2D rb;

    public GameObject target;

    private GameObject archer;

    private ushort damageToDeal;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        if (target)
        {
            rb.velocity = new Vector2(target.transform.position.x - transform.position.x, target.transform.position.y - transform.position.y).normalized * speed;
        }
        if (!target)
        {
            Destroy(gameObject);
        }
    }

    public void SetDamage(ushort damage)
    {
        damageToDeal = damage;
    }

    void InflictDamage()
    {
        EventManager.Instance.CharacterEvent.TakeDamageEvent(damageToDeal, target, archer);
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == target)
        {
            InflictDamage();
        }
    }
}
