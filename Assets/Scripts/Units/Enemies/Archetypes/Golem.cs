using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Golem : Enemy
{
    protected override IEnumerator AttackCR(int damage)
    {
        rb.linearVelocity = Vector2.zero;
        anim.Play("Attack");
        CharacterEvent.AttackHit(damage, opponent.GetInstanceID());
        yield return new WaitForSeconds(1 / attSpeed);
        OnAttack(damage);
    }
}
