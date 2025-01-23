using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Troll : Enemy
{
    protected override IEnumerator AttackCR(int damage)
    {
        rb.velocity = Vector2.zero;
        yield return new WaitForSeconds(1 / attSpeed);
    }
}
