using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public abstract class Enemy : Unit
{
    [Header("GoldDropped")]
    [SerializeField] private int goldDrop;

    override protected void Start()
    {
        base.Start();

        WaveManager.Instance.EnemyLeftInWave++;
    }

    protected override void FindClosestOpponent(GameObject gameObject)
    {
        if (gameObject == this.gameObject)
        {
            opponent = null;
            State = CharacterState.Idle;
            GameObject heroObj = null;
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

    protected override void Attack(int attack, GameObject gameObject)
    {
        if (gameObject == this.gameObject)
        {
            if (State == CharacterState.Attacking)
            {
                isAttacking = true;
            }
            else
            {
                isAttacking = false;
            }

        }
    }

    protected override IEnumerator DeathCoroutine(GameObject killer)
    {
        anim.Play("Death");
        WaveManager.Instance.EnemyLeftInWave--;
        GameManager.Instance.Gold += goldDrop;
        EventManager.Instance.MiscEvent.OnGoldValueChange(GameManager.Instance.Gold);
        yield return new WaitForSeconds(0.2f);
        WaveManager.Instance.EnemyInWave(WaveManager.Instance.EnemyLeftInWave);
        Destroy(gameObject);
    }
}
