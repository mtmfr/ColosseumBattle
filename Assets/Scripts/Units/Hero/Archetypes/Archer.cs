using System.Collections;
using UnityEngine;

public class Archer : Hero
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
        swingSound.Play();
        yield return new WaitForSeconds(1/attSpeed);
        EventManager.Instance.CharacterEvent.AttackEvent(damage, gameObject);
    }
}
