using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class Old_Enemy : Old_Unit
{
    [SerializeField] private int goldDrop;

    protected override void OnSearchClosestOpponent()
    {
        State = CharacterState.Idle;
        List<Old_Hero> heroList = FindObjectsByType<Old_Hero>(FindObjectsInactive.Exclude, FindObjectsSortMode.None).ToList();
        
        opponentToTarget = heroList.OrderBy(hero => Vector3.Distance(hero.transform.position, transform.position)).FirstOrDefault().gameObject;
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
        Old_GameManager.Instance.Gold += goldDrop;
        MiscEvent.OnGoldValueChange(Old_GameManager.Instance.Gold);
        OnSearchClosestOpponent();
        yield return new WaitForSeconds(0.2f);
        Destroy(gameObject);
    }
}
