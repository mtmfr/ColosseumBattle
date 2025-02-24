using System.Collections;
using UnityEngine;

public class Archer : Hero
{
    [Header("Effect")]
    [SerializeField] private GameObject arrow;

    private GameObject UsedArrow { get
        {
         GameObject usedArrow = Instantiate(arrow, transform.position, Quaternion.identity);
            return arrow;
        } }

    protected override IEnumerator AttackCR(int damage)
    {
        rb.linearVelocity = Vector2.zero;
        anim.Play("Attack");
        UsedArrow.SetActive(true);
        UsedArrow.GetComponent<Arrow>().target = opponent;
        UsedArrow.GetComponent<Arrow>().SetDamage(damage);
        yield return new WaitForSeconds(1/attSpeed);
        OnAttack(damage);
    }
}
