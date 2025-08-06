using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
public abstract class Unit : MonoBehaviour
{
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    protected Animator animator;

    private UnitState currentState;

    private int animMoveHash;
    private int animDeathHash;
    protected int animAttackHash;

    protected MovementParameters movementParameters;
    protected AttackParameters attackParameters;
    protected MiscParameters miscParameters;
    protected UIParameters uiParameters;

    public int health { get; private set; }

    protected float attackTimer = 0;
    protected float fleeTimer = 0;

    protected bool isFirstAttack = true;
    protected bool hasStoppedFleeing = false;

    private float timeWithoutAttack = 0;

    protected Unit target;
    protected List<Unit> availableTargets = new();

    protected bool wasDead = true;

    Color32 hitColor = new (255, 170, 150, 255);

    #region UnityFunctions

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        animMoveHash = Animator.StringToHash("Moving");
        animAttackHash = Animator.StringToHash("Attack");
        animDeathHash = Animator.StringToHash("Died");
    }

    private void OnEnable()
    {
        if (wasDead)
            health = miscParameters.health;

        spriteRenderer.color = Color.white;
        currentState = UnitState.Idle;

        UnitEvent.OnDamageReceived += TakeDamage;
        UnitEvent.OnHeal += GetHeal;
        UnitEvent.OnTargetDeath += TargetDied;

        UnitEvent.OnRetarget += Retarget;
        SMB_DeathAnimFinished.OnDeathAnimEnd += Death;
    }

    private void OnDisable()
    {
        target = null;
        availableTargets.Clear();

        UnitEvent.OnDamageReceived -= TakeDamage;
        UnitEvent.OnHeal -= GetHeal;
        UnitEvent.OnTargetDeath -= TargetDied;

        UnitEvent.OnRetarget -= Retarget;
        SMB_DeathAnimFinished.OnDeathAnimEnd -= Death;

        StopAllCoroutines();
    }

    private void FixedUpdate()
    {
        if (GameManager.currentGameState != GameState.Wave)
        {
            currentState = UnitState.Idle;
            return;
        }

        if (WaveManager.instance.waveState != WaveManager.WaveState.Middle)
            return;

        SetCurrentState();
        ResetIsFirstAttack();

        switch (currentState)
        {
            case UnitState.Idle:
                if (availableTargets == null || availableTargets.Count == 0)
                    GetAvailableTarget();

                if (target == null)
                    SetTarget();
                break;
            case UnitState.Moving:
                animator.SetTrigger(animMoveHash);
                Move(target.transform.position);
                break;
            case UnitState.Fleeing:
                Flee(target.transform.position);
                break;
            case UnitState.Attacking:
                rb.linearVelocity = Vector2.zero;
                timeWithoutAttack = 0;
                Attack(target);
                break;
            default:
                break;
        }
    }
    #endregion

    protected void SetParameters(SO_Unit unitSO)
    {
        movementParameters = unitSO.movementParameters;
        attackParameters = unitSO.attackParameters;
        miscParameters = unitSO.miscParameters;
        uiParameters = unitSO.uiParameters;
    }

    #region Target
    protected abstract void GetAvailableTarget();

    protected Unit[] GetAllUnits<T>() where T : Unit
    {
        return FindObjectsByType<T>(FindObjectsInactive.Exclude, FindObjectsSortMode.None).ToArray();
    }

    protected void TargetDied(Unit deadUnit)
    {
        if (deadUnit == target)
        {
            availableTargets.Remove(target);
            target = null;
        }

        SetTarget();
    }

    /// <summary>
    /// Get the target that meet the required conditions. 
    /// </summary>
    /// <param name="sortType">The sorting to use</param>
    /// <returns>Either the closest target or the one with the lowest hp</returns>
    protected T GetTarget<T>(SortType sortType = SortType.Distance) where T : Unit
    {
        List <Unit> targets = sortType switch
        {
            SortType.Distance => availableTargets.Where(target =>
            {
                bool isActive = target.isActiveAndEnabled;
                bool isCorrectType = target is T;
                bool isNotThis = target != this;
                bool isNotDead = target.currentState != UnitState.Dead;

                return isActive && isCorrectType && isNotThis && isNotDead;
            })
            .OrderBy(target => Vector2.Distance(target.transform.position, transform.position))
            .ToList(),

            SortType.Health => availableTargets.Where(target =>
            {
                bool isActive = target.isActiveAndEnabled;
                bool isCorrectType = target is T;
                bool isNotThis = target != this;
                bool isNotDead = target.currentState != UnitState.Dead;

                return isActive && isCorrectType && isNotThis && isNotDead;
            })
            .OrderBy(target => target.health)
            .ThenBy(target => Vector2.Distance(target.transform.position, transform.position))
            .ToList(),

            _ => availableTargets.Where(target =>
            {
                bool isActive = target.isActiveAndEnabled;
                bool isCorrectType = target is T;
                bool isNotThis = target != this;
                bool isNotDead = target.currentState != UnitState.Dead;

                return isActive && isCorrectType && isNotThis && isNotDead;
            })
            .OrderBy(target => Vector2.Distance(target.transform.position, transform.position))
            .ToList(),
        };

        if (targets.Count > 0)
            return targets[0] as T;
        else return null;
    }

    /// <summary>
    /// The sorting type of GetTarget
    /// </summary>
    protected enum SortType : byte
    {
        None,
        Distance,
        Health
    }

    protected abstract void SetTarget();

    private void Retarget(Unit thisUnit, Unit newTarget)
    {
        if (thisUnit != this)
            return;

        target = newTarget;
    }
    #endregion

    #region Movement
    private void Move(Vector3 targetPos)
    {
        Vector3 direction = targetPos - transform.position;

        rb.linearVelocity = direction.normalized * movementParameters.maxApproachSpeed;

        if (this is Slime) 
        {
            Debug.Log(rb.constraints);
        }

        SetSpriteDirection(rb.linearVelocityX);
    }

    private void Flee(Vector3 attackerPos)
    {
        if (fleeTimer > miscParameters.maxFleeTime)
        {
            hasStoppedFleeing = true;
            rb.linearVelocity = Vector2.zero;
            GetAvailableTarget();
            return;
        }

        fleeTimer += Time.fixedDeltaTime;

        Vector3 direction = transform.position - attackerPos;

        rb.linearVelocity = direction.normalized * movementParameters.maxFleeSpeed;

        SetSpriteDirection(rb.linearVelocityX);
    }

    private void SetSpriteDirection(float xVelocity)
    {
        if (xVelocity < 0)
            transform.rotation = Quaternion.Euler(0, 180, 0);
        else
            transform.rotation = Quaternion.Euler(Vector3.zero);
    }
    #endregion

    #region Attack
    protected abstract void Attack(Unit target);

    private void ResetIsFirstAttack()
    {
        if (currentState == UnitState.Attacking)
            return;

        if (isFirstAttack)
            return;

        if (timeWithoutAttack < attackParameters.attackCooldown)
            timeWithoutAttack += Time.fixedDeltaTime;
        else
            isFirstAttack = true;

    }
    #endregion

    #region Health
    private void GetHeal(Unit healedUnit, int damageToHeal)
    {
        if (healedUnit != this)
            return;

        int maxHealth = miscParameters.health;

        health += damageToHeal;

        if (health > maxHealth)
            health = maxHealth;
    }

    private void TakeDamage(Unit damagedUnit, int damageReceived)
    {
        if (damagedUnit != this)
            return;

        int damageToReceive = damageReceived - (damageReceived + miscParameters.defense) / damageReceived;

        if (damageToReceive <= 0)
            damageToReceive = 1;

        if (health - damageToReceive <= 0)
        {
            animator.SetTrigger(animDeathHash);
            UnitEvent.Dying(this);
        }
        else
        {
            health -= damageToReceive;
            StartCoroutine(DamageFeedBack());
        }
    }

    private IEnumerator DamageFeedBack()
    {
        spriteRenderer.color = hitColor;
        yield return new WaitForSeconds(0.15f);
        spriteRenderer.color = Color.white;
    }

    protected virtual void Death(int gameObjectId)
    {
        wasDead = true;
    }
    #endregion

    #region State
    private void SetCurrentState()
    {
        if (target == null)
        {
            currentState = UnitState.Idle;
            return;
        }

        Vector2 targetPos = target.transform.position;

        bool isSameSide = target.GetType().BaseType != GetType().BaseType;
        bool enemiesInFleeZone = Vector2.Distance(targetPos, transform.position) < attackParameters.fleeDistance && isSameSide;
        bool enemiesInAttackZone =  Vector2.Distance(targetPos, transform.position) < attackParameters.attackDistance;

        if (enemiesInFleeZone)
        {
            if (!hasStoppedFleeing)
                currentState = UnitState.Fleeing;
            else currentState = UnitState.Attacking;
        }
        else if (enemiesInAttackZone)
        {
            currentState = UnitState.Attacking;
            hasStoppedFleeing = false;
            fleeTimer = 0;
        }
        else
        {
            currentState = UnitState.Moving;
            hasStoppedFleeing = false;
            fleeTimer = 0;
        }
    }

    protected enum UnitState
    {
        Idle,
        Moving,
        Fleeing,
        Attacking,
        Dead
    }
    #endregion

    protected virtual void OnDrawGizmosSelected()
    {
        
    }
}

public static class UnitEvent
{
    public static event Action<Unit> OnTargetDeath;
    public static void Dying(Unit thisUnit) => OnTargetDeath?.Invoke(thisUnit);

    public static event Action<Unit, int> OnDamageReceived;
    public static void DealDamage(Unit unitToDamage, int damageToDeal) => OnDamageReceived?.Invoke(unitToDamage, damageToDeal);

    public static event Action<Unit, int> OnHeal;
    public static void HealDamage(Unit unitToHeal, int damageToHeal) => OnHeal?.Invoke(unitToHeal, damageToHeal);

    public static event Action<Unit, Unit> OnRetarget;
    public static void ForceRetarget(Unit thisUnit, Unit retargeted) => OnRetarget?.Invoke(thisUnit, retargeted);
}