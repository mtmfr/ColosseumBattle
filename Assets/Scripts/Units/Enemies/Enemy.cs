using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public abstract class Enemy : Unit
{
    [Header("GoldDropped")]
    [SerializeField] private ushort goldDrop;

    override protected void Start()
    {
        base.Start();

        GameManager.Instance.EnemyLeftInWave++;
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

    private void FindClosestEnemy(GameObject gameObject)
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

    protected override void Attack(ushort attack, GameObject gameObject)
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
        GameManager.Instance.EnemyLeftInWave--;
        GameManager.Instance.Gold += goldDrop;
        EventManager.Instance.MiscEvent.OnGoldValueChange(GameManager.Instance.Gold);
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }
}
