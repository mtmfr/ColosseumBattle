using System.Collections;
using UnityEngine;

public class Mage : Hero
{
    [Header("Effect")]
    [SerializeField] private GameObject explosion;

    protected override IEnumerator AttackCR(int damage)
    {
        rb.velocity = Vector2.zero;
        anim.Play("Attack");
        var copy = Instantiate(explosion, opponent.transform.position, Quaternion.identity);
        CharacterEvent.AttackHit(damage, opponent.GetInstanceID());
        yield return new WaitForSeconds(1 / attSpeed);
        Destroy(copy);
        OnAttack(damage);
    }
}
