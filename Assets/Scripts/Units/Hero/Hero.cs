using System;
using System.Collections;
using System.Linq.Expressions;
using Unity.Mathematics;
using UnityEngine;


public abstract class Hero : Unit
{
    #region Unity Function
    override protected void Start()
    {
        base.Start();
        GameManager.Instance.HeroLeftInParty++;
    }
    #endregion

    /// <summary>
    /// Used to find the closest enemy from the character
    /// </summary>
    /// <param name="gameObject">this gameObject used as an id check</param>
    protected override void FindClosestOpponent(GameObject gameObject)
    {
        if (gameObject == this.gameObject)
        {
            opponent = null;
            State = CharacterState.Idle;
            GameObject enemyObj;
            foreach (Enemy enemy in FindObjectsOfType<Enemy>())
            {
                if (enemy && enemy.State != CharacterState.Dying)
                {
                    enemyObj = enemy.gameObject;
                    if (!opponent)
                    {
                        opponent = enemyObj;
                    }
                    else if (opponent && (enemyObj.transform.position - transform.position).magnitude < (opponent.transform.position - transform.position).magnitude)
                    {
                        opponent = enemyObj;
                    }
                }
                else
                {
                    continue;
                }
            }
        }
    }

    protected override void Attack(int attack, GameObject gameObject)
    {
        if (gameObject == this.gameObject)
        {
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
    }

    /// <summary>
    /// Coroutine for the attack of the characterS0 call the attack event at the end
    /// </summary>
    /// <param name="damage">the number of damage to deal to the enemy</param>
    /// <returns>1 divided by the attackspeed</returns>
    protected abstract IEnumerator AttackCR(int damage);

    protected override IEnumerator DeathCoroutine(GameObject killer)
    {
        anim.Play("Death");
        hitSound.Play();
        State = CharacterState.Dying;
        GameManager.Instance.HeroLeftInParty--;
        GameManager.Instance.HeroList.Remove(gameObject);
        EventManager.Instance.CharacterEvent.FindClosestOpponentEvent(gameObject);
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }
}