using System.Collections;
using UnityEngine;

public class Healer : Hero
{
    [Header("Effect")]
    [SerializeField] private GameObject lightPillar;

    protected override IEnumerator AttackCR(int damage)
    {
        rb.linearVelocity = Vector2.zero;
        anim.Play("Attack");
        var copy = Instantiate(lightPillar, opponent.transform.position, Quaternion.identity);
        CharacterEvent.AttackHit(damage, opponent.GetInstanceID());
        yield return new WaitForSeconds(1 / attSpeed);
        Destroy(copy);
        OnAttack(damage);
    }
}
