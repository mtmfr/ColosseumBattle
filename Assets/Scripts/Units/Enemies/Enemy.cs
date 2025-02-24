using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class Enemy : Unit
{
    [SerializeField] private int goldDrop;

    override protected void Start()
    {
        base.Start();

        IsAhero = false;
    }

    protected override void OnSearchClosestOpponent()
    {
        State = CharacterState.Idle;
        List<Hero> heroList = FindObjectsOfType<Hero>().ToList();
        
        opponent = heroList.OrderBy(hero => Vector3.Distance(hero.transform.position, transform.position)).FirstOrDefault().gameObject;
    }

    protected override void OnAttack(int attack)
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

    protected abstract IEnumerator AttackCR(int damage);

    protected override IEnumerator DeathCoroutine()
    {
        anim.Play("Death");
        WaveEvent.RemoveEnemyFromWave(gameObject);
        GameManager.Instance.Gold += goldDrop;
        MiscEvent.OnGoldValueChange(GameManager.Instance.Gold);
        OnSearchClosestOpponent();
        yield return new WaitForSeconds(0.2f);
        Destroy(gameObject);
    }
}
