using System.Collections;
using UnityEngine;

public class Goblin : Enemy
{
    protected override IEnumerator AttackCR(int damage)
    {
        while (true)
        {
            rb.velocity = Vector2.zero;
            anim.Play("Attack");
            CharacterEvent.AttackHit(damage, opponent.GetInstanceID());
            yield return new WaitForSeconds(1 / attSpeed);
        }
    }
}

