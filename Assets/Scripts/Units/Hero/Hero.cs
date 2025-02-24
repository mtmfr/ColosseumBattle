using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public abstract class Hero : Unit
{
    /// <summary>
    /// Used to find the closest enemy from the character
    /// </summary>
    /// <param name="gameObject">this gameObject used as an id check</param>
    protected override void OnSearchClosestOpponent()
    {
        State = CharacterState.Idle;
        
        List<Enemy> enemyList = FindObjectsByType<Enemy>(FindObjectsInactive.Exclude, FindObjectsSortMode.None).ToList();

        opponent = enemyList.OrderBy(enemy => Vector3.Distance(enemy.transform.position, transform.position)).FirstOrDefault().gameObject;
    }

    protected override void OnAttack(int attack)
    {
        Debug.Log("roar");
        if (State == CharacterState.Attacking)
        {
            isAttacking = true;
            StartCoroutine(AttackCR(attack));
        }
        else
        {
            State = CharacterState.Idle;
            isAttacking = false;
            StopCoroutine(AttackCR(attack));
        }
    }

    /// <summary>
    /// Coroutine for the OnAttack of the characterS0 call the OnAttack event at the end
    /// </summary>
    /// <param name="damage">the number of damage to deal to the enemy</param>
    /// <returns>1 divided by the attackspeed</returns>
    protected abstract IEnumerator AttackCR(int damage);

    protected override IEnumerator DeathCoroutine()
    {
        anim.Play("Death");
        hitSound.Play();
        WaveEvent.RemoveHeroFromList(gameObject);
        OnSearchClosestOpponent();
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }
}