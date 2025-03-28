using System.Collections;
using UnityEngine;

public class Old_Goblin : Old_Enemy
{
    protected override IEnumerator AttackCR(int damage)
    {
        while (true)
        {
            rb.linearVelocity = Vector2.zero;
            anim.Play("Attack");
            CharacterEvent.AttackHit(damage, opponentToTarget.GetInstanceID());
            yield return new WaitForSeconds(1 / attSpeed);
        }
    }
}

