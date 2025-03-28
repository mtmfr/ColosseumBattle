using System.Collections;
using UnityEngine;

public class Old_Healer : Old_Hero
{
    [Header("Effect")]
    [SerializeField] private GameObject lightPillar;

    protected override IEnumerator AttackCR(int damage)
    {
        rb.linearVelocity = Vector2.zero;
        anim.Play("Attack");
        var copy = Instantiate(lightPillar, opponentToTarget.transform.position, Quaternion.identity);
        CharacterEvent.AttackHit(damage, opponentToTarget.GetInstanceID());
        yield return new WaitForSeconds(1 / attSpeed);
        Destroy(copy);
        OnAttack(damage);
    }
}
