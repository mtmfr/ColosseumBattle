using System.Collections;
using UnityEngine;

public class Tank : Hero
{
    protected override IEnumerator AttackCR(int damage)
    {
        rb.velocity = Vector2.zero;
        anim.Play("Attack");
        CharacterEvent.AttackHit(damage, opponent.GetInstanceID());
        CharacterEvent.ForceRetarget(opponent.GetInstanceID());
        yield return new WaitForSeconds(1/attSpeed);
        OnAttack(damage);
    }
}
