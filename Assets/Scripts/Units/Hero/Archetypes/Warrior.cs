using System.Collections;
using UnityEngine;

public class Warrior : Hero
{
    protected override IEnumerator AttackCR(int damage)
    {
        rb.velocity = Vector2.zero;
        anim.Play("Attack");
        swingSound.Play();
        EventManager.Instance.CharacterEvent.TakeDamageEvent(damage, opponent, gameObject);
        yield return new WaitForSeconds(1 / attSpeed);
        EventManager.Instance.CharacterEvent.AttackEvent(attack, gameObject);
    }
}
