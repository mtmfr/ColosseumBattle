using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Arrow : MonoBehaviour
{
    [SerializeField] private float speed;
    private Rigidbody2D rb;

    public GameObject target;

    private int damageToDeal;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        if (target)
        {
            rb.linearVelocity = new Vector2(target.transform.position.x - transform.position.x, target.transform.position.y - transform.position.y).normalized * speed;
        }
        if (!target)
        {
            Destroy(gameObject);
        }
    }

    public void SetDamage(int damage)
    {
        damageToDeal = damage;
    }

    void InflictDamage()
    {
        CharacterEvent.AttackHit(damageToDeal, target.GetInstanceID());
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == target)
        {
            InflictDamage();
        }
    }
}
