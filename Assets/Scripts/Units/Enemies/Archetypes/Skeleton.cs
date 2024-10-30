using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skeleton : Enemy
{
    [Header("Effect")]
    [SerializeField] private GameObject arrow;

    protected override IEnumerator AttackCR(int damage)
    {
        rb.velocity = Vector2.zero;
        anim.Play("Attack");
        GameObject arrowCopy = Instantiate(arrow, transform.position, Quaternion.identity);
        arrowCopy.GetComponent<Arrow>().target = opponent;
        arrowCopy.GetComponent<Arrow>().SetArcher(gameObject);
        arrowCopy.GetComponent<Arrow>().SetDamage(damage);
        yield return new WaitForSeconds(1 / AttSpeed);
        EventManager.Instance.CharacterEvent.AttackEvent(damage, gameObject);
    }
}
