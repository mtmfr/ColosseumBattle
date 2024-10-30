using System.Collections;
using UnityEngine;


public abstract class Hero : Unit
{

    #region Unity Function
    override protected void Start()
    {
        IsAhero = true;
        base.Start();
    }
    #endregion

    /// <summary>
    /// Used to find the closest enemy from the character
    /// </summary>
    /// <param name="gameObject">this gameObject used as an id check</param>
    protected override void OnSearchClosestOpponen(bool IsAHero)
    {
        if (IsAhero)
        {
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

    protected override void OnAttack(int attack, GameObject gameObject)
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
    /// Coroutine for the OnAttack of the characterS0 call the OnAttack event at the end
    /// </summary>
    /// <param name="damage">the number of damage to deal to the enemy</param>
    /// <returns>1 divided by the attackspeed</returns>
    protected abstract IEnumerator AttackCR(int damage);

    protected override IEnumerator DeathCoroutine(GameObject killer)
    {
        anim.Play("Death");
        hitSound.Play();
        State = CharacterState.Dying;
        WaveManager.Instance.HeroInParty(gameObject.GetInstanceID());
        EventManager.Instance.CharacterEvent.FindClosestOpponentEvent(false);
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }
}