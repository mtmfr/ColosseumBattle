using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
public abstract class Old_Unit : MonoBehaviour
{
    [SerializeField] protected Old_SO_Character characterS0;
    protected bool IsAhero;

    #region State
    public CharacterState State { get; protected set; }
    protected bool isAttacking;
    #endregion

    #region Stats
    private int maxHealth;
    private int health;

    private int attack;
    private int magic;
    private int speed;
    private float minRange;
    private float maxRange;
    protected float attSpeed;

    protected ContactFilter2D contactFilter;
    #endregion

    #region ObjectComponent
    protected Rigidbody2D rb;
    protected Animator anim;
    private SpriteRenderer sprite;
    #endregion

    #region Sounds
    [Header("Sound")]
    [SerializeField] protected AudioSource hitSound;
    #endregion

    [Header("Opponent")]
    [SerializeField] protected GameObject opponentToTarget;
    private Collider2D[] opponents;


    #region Unity Function
    // Start is called before the first frame update
    protected virtual void Start()
    {
        SetupStats();

        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
        sprite = GetComponentInChildren<SpriteRenderer>();
        sprite.color = Color.white;

        State = CharacterState.Idle;

        isAttacking = false;
        OnSearchClosestOpponent();

    }

    protected virtual void FixedUpdate()
    {
        DetectZone(minRange, maxRange);
        StateControler();
    }

    protected virtual void OnEnable()
    {
        CharacterEvent.OnAttackHit += TakeDamage;
        CharacterEvent.OnHeal += OnHeal;
    }

    protected virtual void OnDisable()
    {
        CharacterEvent.OnAttackHit -= TakeDamage;
        CharacterEvent.OnHeal -= OnHeal;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, minRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, maxRange);
    }
    #endregion

    /// <summary>
    /// Set the stats of the player
    /// </summary>
    private void SetupStats()
    {
        maxHealth = characterS0.Health;
        health = maxHealth;

        attack = characterS0.Attack;
        magic = characterS0.Magic;
        speed = characterS0.Speed;

        minRange = characterS0.MinRange;
        maxRange = characterS0.MaxRange;

        contactFilter = characterS0.ContactFilter;
    }

    #region state control function

    /// <summary>
    /// Get the numbers of opponents between 2 ranges and define the state of the character
    /// </summary>
    /// <param name="min">the minimum distance at wich an opponent can be to OnAttack</param>
    /// <param name="max">the maximum distance at wich an opponent has to be to OnAttack</param>
    protected void DetectZone(float min, float max)
    {
        List<Collider2D> opponentInFleeZone = Physics2D.OverlapCircleAll(transform.position, min, 7).ToList();

        List<Collider2D> opponentInAttackZone = Physics2D.OverlapCircleAll(transform.position, max, 7).ToList();

        Debug.Log($"Opponent in fleeZone : {opponentInFleeZone.Count}.", this);
        Debug.Log($" Opponent in attackZone {opponentInAttackZone.Count}", this);

        //determine the state of the opponent depending on the number of enemy in it's detection zone
        if (opponentInFleeZone.Count > 1 && minRange > 0)
        {
            State = CharacterState.Fleeing;
        }
        else if (opponentInAttackZone.Count == 0 && opponentInFleeZone.Count == 0)
        {
            State = CharacterState.Moving;
        }
        else if (opponentInAttackZone.Count > 1)
        {
            State = CharacterState.Attacking;
            Debug.Log("fire");
        }
    }

    /// <summary>
    /// Switch the comportement of the character depending of it's current state
    /// </summary>
    private void StateControler()
    {
        if (opponentToTarget != null)
        {
            switch (State)
            {
                case CharacterState.Moving:
                    OnMove(speed);
                    break;
                case CharacterState.Fleeing:
                    OnFlee(speed);
                    break;
                case CharacterState.Attacking:
                    if (!isAttacking && opponentToTarget.GetComponent<Old_Unit>().State != CharacterState.Dying)
                    {
                        OnAttack(Mathf.Max(attack, magic));
                    }
                    break;
                default:
                    DetectZone(minRange, maxRange);
                    break;
            }
        }
        else 
            OnSearchClosestOpponent();
    }
    #endregion

    #region movement function

    /// <summary>
    /// Get the closest opponent from the character
    /// </summary>
    protected abstract void OnSearchClosestOpponent(); 

    
    protected virtual void OnMove(int speed)
    {
        float x = opponentToTarget.transform.position.x - gameObject.transform.position.x;
        float y = opponentToTarget.transform.position.y - gameObject.transform.position.y;

        Vector2 dir = new(x, y);

        rb.linearVelocity = dir.normalized * speed;

        anim.SetTrigger("IsMoving");
    }

    
    protected virtual void OnFlee(int speed)
    {
        float x = gameObject.transform.position.x - opponentToTarget.transform.position.x;
        float y = gameObject.transform.position.y - opponentToTarget.transform.position.y;

        Vector2 dir = new(x, y);

        rb.linearVelocity = dir.normalized * speed;
        anim.SetTrigger("IsMoving");
    }
    #endregion

    
    protected abstract void OnAttack(int attack);


    protected virtual void TakeDamage(int damage, int objectId)
    {
        if (objectId != gameObject.GetInstanceID())
            return;

        if (health - damage >= 0)
        {
            health -= damage;
        }
        else OnDeath();
        StartCoroutine(DamageFeedback());
    }

    private IEnumerator DamageFeedback()
    {
        sprite.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        sprite.color = Color.white;
    }

    private void OnHeal(int heal, GameObject gameObject)
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
    private void OnDeath()
    {
        State = CharacterState.Dying;
        StartCoroutine(DeathCoroutine());
    }
    protected abstract IEnumerator DeathCoroutine();
}
