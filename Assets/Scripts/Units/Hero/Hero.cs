using System;
using System.Collections;
using UnityEngine;


public abstract class Hero : Unit
{
    //[SerializeField] private GameObject effect;
    #region Unity Function
    override protected void Start()
    {
        base.Start();

        GameManager.Instance.HeroLeftInParty++;

        EventManager.Instance.CharacterEvent.FindClosestEnemyEvent(gameObject);
    }

    override protected void OnEnable()
    {
        base.OnEnable();
        EventManager.Instance.CharacterEvent.FindClosestEnemy += FindClosestEnemy;
        EventManager.Instance.CharacterEvent.Dying += Death;
    }

    override protected void OnDisable()
    {
        base.OnDisable();
        EventManager.Instance.CharacterEvent.FindClosestEnemy -= FindClosestEnemy;
        EventManager.Instance.CharacterEvent.Dying -= Death;
    }
    #endregion

    /// <summary>
    /// Used to find the enemy enemy from the characterS0
    /// </summary>
    /// <param name="gameObject">this gameObject used as an id check</param>
    private void FindClosestEnemy(GameObject gameObject)
    {
        if (gameObject == this.gameObject)
        {
            opponent = null;
            State = CharacterState.Idle;
            GameObject enemyObj = null;
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
                    State = CharacterState.Idle;
                }
            }
        }
    }
    protected override void Attack(ushort attack, GameObject gameObject)
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
    protected abstract IEnumerator AttackCR(ushort damage);

    protected override IEnumerator DeathCoroutine(GameObject killer)
    {
        anim.Play("Death");
        hitSound.Play();
        State = CharacterState.Dying;
        GameManager.Instance.HeroLeftInParty--;
        GameManager.Instance.HeroList.Remove(gameObject);
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }
}