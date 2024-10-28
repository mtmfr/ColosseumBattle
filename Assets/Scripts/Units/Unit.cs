using System;
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
    public int maxHealth { get { return characterS0.Health; } }
    private int health;

    public int attack { get { return characterS0.Attack; } }
    public int magic { get {return characterS0.Magic; } }
    public int speed {  get { return characterS0.Speed; } }
    public float minRange { get { return characterS0.MinRange; } }
    public float maxRange { get { return characterS0.MaxRange; } }
    public float attSpeed { get { return characterS0.AttSpeed; } }

    protected LayerMask opponentLayer { get { return characterS0.LayerMask; } }

    public int cost {  get { return characterS0.Cost; } }

    public Sprite CharSprite { get { return characterS0.charIcon; } }
    #endregion

    #region ObjectComponent
    protected Rigidbody2D rb;
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
        health = maxHealth;

        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();

        State = CharacterState.Idle;

        isAttacking = false;

        EventManager.Instance.CharacterEvent.FindClosestOpponentEvent(gameObject);
    }

    protected virtual void FixedUpdate()
    {
        DetectZone(minRange, maxRange);
        StateControler();
    }

    protected virtual void OnEnable()
    {
        EventManager.Instance.CharacterEvent.FindClosestOpponent += FindClosestOpponent;
        EventManager.Instance.CharacterEvent.Move += Move;
        EventManager.Instance.CharacterEvent.Flee += Flee;
        EventManager.Instance.CharacterEvent.Attack += Attack;
        EventManager.Instance.CharacterEvent.TakeDamage += TakeDamage;
        EventManager.Instance.CharacterEvent.Heal += Heal;
        EventManager.Instance.CharacterEvent.Dying += Death;
    }

    protected virtual void OnDisable()
    {
        EventManager.Instance.CharacterEvent.FindClosestOpponent -= FindClosestOpponent;
        EventManager.Instance.CharacterEvent.Move -= Move;
        EventManager.Instance.CharacterEvent.Flee -= Flee;
        EventManager.Instance.CharacterEvent.Attack -= Attack;
        EventManager.Instance.CharacterEvent.TakeDamage -= TakeDamage;
        EventManager.Instance.CharacterEvent.Heal -= Heal;
        EventManager.Instance.CharacterEvent.Dying -= Death;
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
    /// Get the numbers of opponents between 2 ranges and define the state of the character
    /// </summary>
    /// <param name="min">the minimum distance at wich an opponent can be to attack</param>
    /// <param name="max">the maximum distance at wich an opponent has to be to attack</param>
    protected void DetectZone(float min, float max)
    {
        //detection range at wich the character will flee from it's opponent
        int fleeZone = Physics2D.OverlapCircleNonAlloc(transform.position, min, opponents, opponentLayer);

        //detection range at which the character will start fleeing from it's opponent
        int attackZone = Physics2D.OverlapCircleNonAlloc(transform.position, max, opponents, opponentLayer);

        //determine the state of the opponent depending on the number of enemy in it's detection zone
        if (fleeZone > 0)
        {
            State = CharacterState.Fleeing;
            EventManager.Instance.CharacterEvent.FindClosestOpponentEvent(gameObject);
        }
        else if (attackZone == 0 && fleeZone == 0)
        {
            State = CharacterState.Moving;
        }
        else if (attackZone > 0 && fleeZone == 0)
        {
            State = CharacterState.Attacking;
        }
    }

    /// <summary>
    /// Switch the comportement of the character depending of it's current state
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
                if (!isAttacking && opponent.GetComponent<Unit>().State != CharacterState.Dying)
                {
                    EventManager.Instance.CharacterEvent.AttackEvent(Mathf.Max(attack, magic), gameObject);
                }
                break;
        }
    }
    #endregion

    #region movement function

    protected abstract void FindClosestOpponent(GameObject gameObject); 

    /// <summary>
    /// Make the character move toward it's opponent
    /// </summary>
    /// <param name="speed">the speed at wich the characterS0 moveSound</param>
    /// <param name="gameObject">this gameObject used as an id check</param>
    protected virtual void Move(int speed, GameObject gameObject)
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
    protected virtual void Flee(int speed, GameObject gameObject)
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

    /// <param name="attack">the damage caused to the opponent</param>
    /// <param name="gameObject">this gameObject used as an id check</param>
    protected abstract void Attack(int attack, GameObject gameObject);


    /// <param name="damage">the amount of life lost</param>
    /// <param name="gameObject">this gameObject used as an id check</param>
    /// <param name="striker">the gameObject that inflicted damage</param>
    protected virtual void TakeDamage(int damage, GameObject gameObject, GameObject striker)
    {
        if (gameObject == this.gameObject)
        {
            if (health > 0 && health - damage > 0)
            {
                health -= damage;
                anim.Play("Hit");
            }
            if (health > 0 && (health - damage) <= 0)
            {
                State = CharacterState.Dying;
                EventManager.Instance.CharacterEvent.DyingEvent(gameObject, striker);
            }
        }
    }

    /// <param name="heal">the amount of health getting healed</param>
    /// <param name="gameObject">the gameObject Used as an id Chack</param>
    private void Heal(int heal, GameObject gameObject)
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

    /// <param name="gameObject">this gameObject used as an id check</param>
    /// <param name="killer">the game object that kill this gameobject</param>
    private void Death(GameObject gameObject, GameObject killer)
    {
        if (gameObject == this.gameObject && State == CharacterState.Dying)
        {
            State = CharacterState.Dying;
            StartCoroutine(DeathCoroutine(killer));
        }
    }
    protected abstract IEnumerator DeathCoroutine(GameObject killer);
}
