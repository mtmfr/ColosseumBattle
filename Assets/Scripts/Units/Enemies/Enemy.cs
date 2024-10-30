using System.Collections;
using UnityEngine;

public abstract class Enemy : Unit
{
    [SerializeField] private int goldDrop;

    override protected void Start()
    {
        base.Start();

        IsAhero = false;
    }

    protected override void OnSearchClosestOpponen(bool IsAHero)
    {
        if (!IsAhero)
        {
            opponent = null;
            State = CharacterState.Idle;
            GameObject heroObj;
            foreach (Hero hero in FindObjectsOfType<Hero>())
            {
                if (hero && hero.State != CharacterState.Dying)
                {
                    heroObj = hero.gameObject;
                    if (!opponent)
                    {
                        opponent = heroObj;
                    }
                    else if (opponent && (heroObj.transform.position - transform.position).magnitude < (opponent.transform.position - transform.position).magnitude)
                    {
                        opponent = heroObj;
                    }
                }
                else
                {
                    State = CharacterState.Idle;
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
                StartCoroutine(AttackCR(attack));
                isAttacking = true;
            }
            else
            {
                StopCoroutine(AttackCR(attack));
                isAttacking = false;
            }
        }
    }

    protected abstract IEnumerator AttackCR(int damage);

    protected override IEnumerator DeathCoroutine(GameObject killer)
    {
        anim.Play("Death");
        WaveManager.Instance.EnemyInWave(gameObject.GetInstanceID());
        GameManager.Instance.Gold += goldDrop;
        EventManager.Instance.MiscEvent.OnGoldValueChange(GameManager.Instance.Gold);
        EventManager.Instance.CharacterEvent.FindClosestOpponentEvent(true);
        yield return new WaitForSeconds(0.2f);
        Destroy(gameObject);
    }
}
