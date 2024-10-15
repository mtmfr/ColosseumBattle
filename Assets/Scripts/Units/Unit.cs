using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
public abstract class Unit : MonoBehaviour
{
    [SerializeField] protected CharacterSO characterS0;

    #region State
    public CharacterState State { get; protected set; }
    protected bool isAttacking;
    #endregion

    #region Stats
    protected ushort maxHealth;
    protected ushort health;

    protected ushort defense;
    public ushort attack { get; protected set; }
    public ushort magic { get; protected set; }
    protected ushort speed;
    protected float minRange;
    protected float maxRange;
    protected float attSpeed;

    protected LayerMask opponentLayer;
    #endregion

    #region ObjectComponent
    protected Rigidbody2D rb;
    private Collider2D col;
    protected Animator anim;
    #endregion

    #region Sounds
    [Header("Sound")]
    [SerializeField] protected AudioSource hitSound;
    [SerializeField] protected AudioSource moveSound;
    [SerializeField] protected AudioSource swingSound;
    #endregion

    [Header("Opponent")]
    [SerializeField] protected GameObject opponent;
    private Collider2D[] opponents = new Collider2D[10];

    #region Unity Function
    // Start is called before the first frame update
    protected virtual void Start()
    {
        maxHealth = characterS0.Health;
        health = maxHealth;

        defense = characterS0.Defense;
        attack = characterS0.Attack;
        magic = characterS0.Magic;
        speed = characterS0.Speed;
        minRange = characterS0.MinRange;
        maxRange = characterS0.MaxRange;
        attSpeed = characterS0.AttSpeed;
        opponentLayer = characterS0.LayerMask;

        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        anim = GetComponentInChildren<Animator>();

        State = CharacterState.Idle;

        isAttacking = false;
    }

    protected virtual void FixedUpdate()
    {
        DetectZone(minRange, maxRange);
        StateControler();
    }

    protected virtual void OnEnable()
    {
            EventManager.Instance.CharacterEvent.Move += Move;
            EventManager.Instance.CharacterEvent.Flee += Flee;
            EventManager.Instance.CharacterEvent.Attack += Attack;
            EventManager.Instance.CharacterEvent.TakeDamage += TakeDamage;
            EventManager.Instance.CharacterEvent.Heal += Heal;
    }

    protected virtual void OnDisable()
    {
        EventManager.Instance.CharacterEvent.Move -= Move;
        EventManager.Instance.CharacterEvent.Flee -= Flee;
        EventManager.Instance.CharacterEvent.Attack -= Attack;
        EventManager.Instance.CharacterEvent.TakeDamage -= TakeDamage;
        EventManager.Instance.CharacterEvent.Heal -= Heal;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, minRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, maxRange);
    }
    #endregion

    #region state control function
    /// <summary>
    /// Check if there is an opponent between two collider and check wether the collider it's in allow an attack
    /// </summary>
    /// <param name="min">the minimum distance at wich an opponent can be to attack</param>
    /// <param name="max">the maximum distance at wich an opponent can be to attack</param>
    protected void DetectZone(float min, float max)
    {
        int minDetect = Physics2D.OverlapCircleNonAlloc(transform.position, min, opponents, opponentLayer);
        int maxDetect = Physics2D.OverlapCircleNonAlloc(transform.position, max, opponents, opponentLayer);

        if (minDetect > 0)
        {
            State = CharacterState.Fleeing;
        }
        else if (maxDetect == 0 && minDetect == 0)
        {
            State = CharacterState.Moving;
        }
        else if (maxDetect > 0 && minDetect == 0)
        {
            State = CharacterState.Attacking;
        }
    }

    /// <summary>
    /// Control wether the characterS0 should to move, flee, attack
    /// </summary>
    private void StateControler()
    {
        switch (State)
        {
            case CharacterState.Moving:
                EventManager.Instance.CharacterEvent.MoveEvent(speed, gameObject);
                break;
            case CharacterState.Fleeing:
                EventManager.Instance.CharacterEvent.FleeEvent(speed, gameObject);
                break;
            case CharacterState.Attacking:
                if (!isAttacking)
                {
                    EventManager.Instance.CharacterEvent.AttackEvent((ushort)Mathf.Max(attack, magic), gameObject);
                }
                break;
        }
    }
    #endregion

    #region movement function
    /// <summary>
    /// Make the characterS0 Move toward the opponent
    /// </summary>
    /// <param name="speed">the speed at wich the characterS0 moveSound</param>
    /// <param name="gameObject">this gameObject used as an id check</param>
    protected virtual void Move(ushort speed, GameObject gameObject)
    {
        if (gameObject == this.gameObject && opponent)
        {
            float x = opponent.transform.position.x - gameObject.transform.position.x;
            float y = opponent.transform.position.y - gameObject.transform.position.y;

            Vector2 dir = new Vector2(x, y);

            rb.velocity = dir.normalized * speed;

            anim.SetTrigger("IsMoving");
        }
    }

    /// <summary>
    /// make the characterS0 flee from the opponent until it is outside of it's minimal detection range
    /// </summary>
    /// <param name="speed">the speed at wich the characterS0 flee</param>
    /// <param name="gameObject">this gameObject used as an id check</param>
    protected virtual void Flee(ushort speed, GameObject gameObject)
    {
        if (gameObject == this.gameObject)
        {
            float x = gameObject.transform.position.x - opponent.transform.position.x;
            float y = gameObject.transform.position.y - opponent.transform.position.y;

            Vector2 dir = new Vector2(x, y);

            rb.velocity = dir.normalized * speed;
            anim.SetTrigger("IsMoving");
        }
    }
    #endregion

    /// <summary>
    /// make the character attack it's opponent
    /// </summary>
    /// <param name="attack">the damage caused to the opponent</param>
    /// <param name="gameObject">this gameObject used as an id check</param>
    protected abstract void Attack(ushort attack, GameObject gameObject);

    /// <summary>
    /// called when the characterS0 take damage from an opponent
    /// </summary>
    /// <param name="damage">the amount of life lost</param>
    /// <param name="gameObject">this gameObject used as an id check</param>
    /// <param name="striker">the gameobject that inflicted damage</param>
    protected virtual void TakeDamage(ushort damage, GameObject gameObject, GameObject striker)
    {
        if (gameObject == this.gameObject)
        {
            if (health > 0 && health - damage > 0)
            {
                health -= damage;
                anim.Play("Hit");
            }
            else if (health > 0 && (int)(health - damage) < 0)
            {
                Debug.Log("dying");
                State = CharacterState.Dying;
                health = 0;
                EventManager.Instance.CharacterEvent.DyingEvent(gameObject, striker);
            }
            else if (health == 0)
            {
                Debug.Log("dying");
                State = CharacterState.Dying;
                EventManager.Instance.CharacterEvent.DyingEvent(gameObject, striker);
            }
        }
    }

    /// <summary>
    /// Called when the characterS0 receive a heal
    /// </summary>
    /// <param name="heal">the amount getting healed</param>
    private void Heal(ushort heal, GameObject gameObject)
    {
        if (gameObject == this.gameObject)
        {
            if (State != CharacterState.Dying)
            {
                if (health + heal < maxHealth)
                {
                    health += heal;
                }
                else if ((health + heal) > maxHealth)
                {
                    health = maxHealth;
                }
            }
        }
    }

    /// <summary>
    /// Called when the characterS0 dies
    /// </summary>
    /// <param name="gameObject">this gameObject used as an id check</param>
    /// <param name="killer">the game object that kill this gameobject</param>
    protected virtual void Death(GameObject gameObject, GameObject killer)
    {
        if (gameObject == this.gameObject && State != CharacterState.Dying)
        {
            State = CharacterState.Dying;
            EventManager.Instance.CharacterEvent.FindClosestEnemyEvent(killer);
            StartCoroutine(DeathCoroutine(killer));
        }
    }

    /// <summary>
    /// Coutine fore the death of the characterS0
    /// </summary>
    /// <returns></returns>
    protected abstract IEnumerator DeathCoroutine(GameObject killer);
}
