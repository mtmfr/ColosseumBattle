using System.Collections;
using UnityEngine;

public class Old_Mage : Old_Hero
{
    [Header("Effect")]
    [SerializeField] private GameObject explosion;

    protected override IEnumerator AttackCR(int damage)
    {
        rb.linearVelocity = Vector2.zero;
        anim.Play("Attack");
        var copy = Instantiate(explosion, opponentToTarget.transform.position, Quaternion.identity);
        CharacterEvent.AttackHit(damage, opponentToTarget.GetInstanceID());
        yield return new WaitForSeconds(1 / attSpeed);
        Destroy(copy);
        OnAttack(damage);
    }
}
