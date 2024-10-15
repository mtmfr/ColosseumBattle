using System.Collections;
using UnityEngine;

public class Healer : Hero
{
    [Header("Effect")]
    [SerializeField] private GameObject lightPillar;

    protected override IEnumerator AttackCR(ushort damage)
    {
        rb.velocity = Vector2.zero;
        anim.Play("Attack");
        swingSound.Play();
        var copy = Instantiate(lightPillar, opponent.transform.position, Quaternion.identity);
        EventManager.Instance.CharacterEvent.TakeDamageEvent(damage, opponent, gameObject);
        yield return new WaitForSeconds(1 / attSpeed);
        Destroy(copy);
        EventManager.Instance.CharacterEvent.AttackEvent((ushort)Mathf.Max(attack, magic), gameObject);
    }
}
