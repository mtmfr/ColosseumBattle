using System.Collections;
using UnityEngine;

public class Archer : Hero
{
    [Header("Effect")]
    [SerializeField] private GameObject arrow;

    protected override IEnumerator AttackCR(ushort damage)
    {
        rb.velocity = Vector2.zero;
        anim.Play("Attack");
        var copy = Instantiate(arrow, transform.position, Quaternion.identity);
        copy.GetComponent<Arrow>().target = opponent;
        copy.GetComponent<Arrow>().SetDamage(damage);
        swingSound.Play();
        yield return new WaitForSeconds(1/attSpeed);
        EventManager.Instance.CharacterEvent.AttackEvent(damage, gameObject);
    }
}
