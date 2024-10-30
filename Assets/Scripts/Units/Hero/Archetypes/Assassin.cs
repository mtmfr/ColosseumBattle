using System.Collections;
using UnityEngine;

public class Assassin : Hero
{
    [Header("Effect")]
    [SerializeField] private GameObject blood;
    [SerializeField] GameObject bloodSpawn;



    protected override IEnumerator AttackCR(int damage)
    {
        rb.velocity = Vector2.zero;
        anim.Play("Attack");
        var copy = Instantiate(blood, new Vector2(bloodSpawn.transform.position.x, bloodSpawn.transform.position.y), Quaternion.identity);
        EventManager.Instance.CharacterEvent.TakeDamageEvent(damage, opponent, gameObject);
        yield return new WaitForSeconds(1 / AttSpeed);
        Destroy(copy);
        EventManager.Instance.CharacterEvent.AttackEvent(damage, gameObject);
    }
}
