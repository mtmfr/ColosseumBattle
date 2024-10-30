using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
public abstract class Unit : MonoBehaviour
{
    [SerializeField] protected CharacterSO characterS0;
    protected bool IsAhero;

    #region State
    public CharacterState State { get; protected set; }
    protected bool isAttacking;
    #endregion

    #region Stats
    public int MaxHealth { get { return characterS0.Health; } }
    private int health;

    public int Attack { get { return characterS0.Attack; } }
    public int Magic { get {return characterS0.Magic; } }
    public int Speed {  get { return characterS0.Speed; } }
    public float MinRange { get { return characterS0.MinRange; } }
    public float MaxRange { get { return characterS0.MaxRange; } }
    public float AttSpeed { get { return characterS0.AttSpeed; } }

    protected LayerMask OpponentLayer { get { return characterS0.OpponentMask; } }

    public int Cost {  get { return characterS0.Cost; } }

    public Sprite CharSprite { get { return characterS0.CharIcon; } }
    #endregion

    #region ObjectComponent
    protected Rigidbody2D rb;
    protected Animator anim;
    #endregion

    #region Sounds
    [Header("Sound")]
    [SerializeField] protected AudioSource hitSound;
    #endregion

    [Header("Opponent")]
    [SerializeField] protected GameObject opponent;
    private Collider2D[] opponents = new Collider2D[byte.MaxValue];


    #region Unity Function
    // Start is called before the first frame update
    protected virtual void Start()
    {
        health = MaxHealth;

        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();

        State = CharacterState.Idle;

        isAttacking = false;
        EventManager.Instance.CharacterEvent.FindClosestOpponentEvent(IsAhero);

    }

    protected virtual void FixedUpdate()
    {
        DetectZone(MinRange, MaxRange);
        StateControler();
    }

    protected virtual void OnEnable()
    {
        EventManager.Instance.CharacterEvent.FindClosestOpponent += OnSearchClosestOpponen;
        EventManager.Instance.CharacterEvent.Move += OnMove;
        EventManager.Instance.CharacterEvent.Flee += OnFlee;
        EventManager.Instance.CharacterEvent.Attack += OnAttack;
        EventManager.Instance.CharacterEvent.TakeDamage += OnDamageTaken;
        EventManager.Instance.CharacterEvent.Heal += OnHeal;
        EventManager.Instance.CharacterEvent.Dying += OnDeath;
    }

    protected virtual void OnDisable()
    {
        EventManager.Instance.CharacterEvent.FindClosestOpponent -= OnSearchClosestOpponen;
        EventManager.Instance.CharacterEvent.Move -= OnMove;
        EventManager.Instance.CharacterEvent.Flee -= OnFlee;
        EventManager.Instance.CharacterEvent.Attack -= OnAttack;
        EventManager.Instance.CharacterEvent.TakeDamage -= OnDamageTaken;
        EventManager.Instance.CharacterEvent.Heal -= OnHeal;
        EventManager.Instance.CharacterEvent.Dying -= OnDeath;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, MinRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, MaxRange);
    }
    #endregion

    #region state control function
    /// <summary>
    /// Get the numbers of opponents between 2 ranges and define the state of the character
    /// </summary>
    /// <param name="min">the minimum distance at wich an opponent can be to OnAttack</param>
    /// <param name="max">the maximum distance at wich an opponent has to be to OnAttack</param>
    protected void DetectZone(float min, float max)
    {
        //detection range at wich the character will flee from it's opponent
        int fleeZone = Physics2D.OverlapCircleNonAlloc(transform.position, min, opponents, OpponentLayer);

        //detection range at which the character will start fleeing from it's opponent
        int attackZone = Physics2D.OverlapCircleNonAlloc(transform.position, max, opponents, OpponentLayer);

        //determine the state of the opponent depending on the number of enemy in it's detection zone
        if (fleeZone > 0)
        {
            State = CharacterState.Fleeing;
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
        if (opponent != null)
        {
            switch (State)
            {
                case CharacterState.Moving:
                    EventManager.Instance.CharacterEvent.MoveEvent(Speed, gameObject);
                    break;
                case CharacterState.Fleeing:
                    EventManager.Instance.CharacterEvent.FleeEvent(Speed, gameObject);
                    break;
                case CharacterState.Attacking:
                    if (!isAttacking && opponent.GetComponent<Unit>().State != CharacterState.Dying)
                    {
                        EventManager.Instance.CharacterEvent.AttackEvent(Mathf.Max(Attack, Magic), gameObject);
                    }
                    break;
                default:
                    DetectZone(MinRange, MaxRange);
                    break;
            }
        }
        else 
            OnSearchClosestOpponen(IsAhero);
    }
    #endregion

    #region movement function

    /// <summary>
    /// Get the closest opponent from the character
    /// </summary>
    protected abstract void OnSearchClosestOpponen(bool IsAHero); 

    /// <summary>
    /// Make the character move toward it's opponent
    /// </summary>
    /// <param name="speed">the Speed at wich the characterS0 moveSound</param>
    /// <param name="gameObject">this gameObject used as an id check</param>
    protected virtual void OnMove(int speed, GameObject gameObject)
    {
        if (gameObject == this.gameObject && opponent)
        {
            float x = opponent.transform.position.x - gameObject.transform.position.x;
            float y = opponent.transform.position.y - gameObject.transform.position.y;

            Vector2 dir = new(x, y);

            rb.velocity = dir.normalized * speed;

            anim.SetTrigger("IsMoving");
        }
    }

    /// <summary>
    /// make the characterS0 flee from the opponent until it is outside of it's minimal detection range
    /// </summary>
    /// <param name="speed">the Speed at wich the characterS0 flee</param>
    /// <param name="gameObject">this gameObject used as an id check</param>
    protected virtual void OnFlee(int speed, GameObject gameObject)
    {
        if (gameObject == this.gameObject)
        {
            float x = gameObject.transform.position.x - opponent.transform.position.x;
            float y = gameObject.transform.position.y - opponent.transform.position.y;

            Vector2 dir = new(x, y);

            rb.velocity = dir.normalized * speed;
            anim.SetTrigger("IsMoving");
        }
    }
    #endregion

    /// <param name="attack">the damage caused to the opponent</param>
    /// <param name="gameObject">this gameObject used as an id check</param>
    protected abstract void OnAttack(int attack, GameObject gameObject);


    /// <param name="damage">the amount of life lost</param>
    /// <param name="gameObject">this gameObject used as an id check</param>
    /// <param name="striker">the gameObject that inflicted damage</param>
    protected virtual void OnDamageTaken(int damage, GameObject gameObject, GameObject striker)
    {
        if (gameObject == this.gameObject)
        {
            if (health > 0 && health - damage > 0)
            {
                health -= damage;
                hitSound.Play();
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
    private void OnHeal(int heal, GameObject gameObject)
    {
        if (gameObject == this.gameObject)
        {
            if (State != CharacterState.Dying)
            {
                if (health + heal < MaxHealth)
                {
                    health += heal;
                }
                else if ((health + heal) > MaxHealth)
                {
                    health = MaxHealth;
                }
            }
        }
    }

    /// <param name="gameObject">this gameObject used as an id check</param>
    /// <param name="killer">the game object that kill this gameobject</param>
    private void OnDeath(GameObject gameObject, GameObject killer)
    {
        if (gameObject == this.gameObject && State == CharacterState.Dying)
        {
            killer.GetComponent<Unit>().opponent = null;
            State = CharacterState.Dying;
            StartCoroutine(DeathCoroutine(killer));
        }
    }
    protected abstract IEnumerator DeathCoroutine(GameObject killer);
}
