using System.Collections;
using UnityEngine;

public class Old_Warrior : Old_Hero
{
    protected override IEnumerator AttackCR(int damage)
    {
        rb.linearVelocity = Vector2.zero;
        anim.Play("Attack");
        CharacterEvent.AttackHit(damage, opponentToTarget.GetInstanceID());
        yield return new WaitForSeconds(1 / attSpeed);
        OnAttack(damage);
    }
}
