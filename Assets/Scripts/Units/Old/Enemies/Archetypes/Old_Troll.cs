using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Old_Troll : Old_Enemy
{
    protected override IEnumerator AttackCR(int damage)
    {
        rb.linearVelocity = Vector2.zero;
        yield return new WaitForSeconds(1 / attSpeed);
    }
}
