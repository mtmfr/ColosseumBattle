using System.Collections;
using UnityEngine;

public class Tank : Hero
{
    protected override IEnumerator AttackCR(ushort damage)
    {
        rb.velocity = Vector2.zero;
        anim.Play("Attack");
        EventManager.Instance.CharacterEvent.TakeDamageEvent(damage, opponent, gameObject);
        EventManager.Instance.CharacterEvent.FindClosestEnemyEvent(opponent);
        swingSound.Play();
        yield return new WaitForSeconds(1/attSpeed);
        EventManager.Instance.CharacterEvent.AttackEvent(damage, gameObject);
    }
}
