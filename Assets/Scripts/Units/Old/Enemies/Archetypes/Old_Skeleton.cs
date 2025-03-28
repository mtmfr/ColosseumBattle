using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Old_Skeleton : Old_Enemy
{
    [Header("Effect")]
    [SerializeField] private GameObject arrow;
    private GameObject UsedArrow { get
        {
            GameObject usedArrow = Instantiate(arrow, transform.position, Quaternion.identity);
            return usedArrow;
        } }

    protected override IEnumerator AttackCR(int damage)
    {
        rb.linearVelocity = Vector2.zero;
        anim.Play("Attack");
        UsedArrow.SetActive(true);
        UsedArrow.GetComponent<Old_Arrow>().target = opponentToTarget;
        UsedArrow.GetComponent<Old_Arrow>().SetDamage(damage);
        yield return new WaitForSeconds(1 / attSpeed);
        OnAttack(damage);
    }
}
