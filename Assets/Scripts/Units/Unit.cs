using System;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
public abstract class Unit : MonoBehaviour
{
    Rigidbody2D rb;

    private UnitState currentState;

    public MovementParameters movementParameters { get; private set; } 
    public AttackParameters attackParameters { get; private set; }
    public MiscParameters miscParameters { get; private set; }

    public int health { get; private set; }

    protected float attackTimer = 0;
    protected float fleeTimer = 0;

    protected bool isFirstAttack = true;
    protected bool hasStoppedFleeing = false;

    private float timeWithoutAttack = 0;

    protected Unit target;
    protected Unit[] availableTarget;

    protected bool wasDead = true;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        if (wasDead)
            health = miscParameters.health;

        GetAvailableTarget();

        UnitEvent.OnDamageReceived += GetDamage;
        UnitEvent.OnHeal += GetHeal;
        UnitEvent.OnTargetDeath += UpdateTarget;

        UnitEvent.OnRetarget += Retarget;
    }

    private void OnDisable()
    {
        target = null;
        currentState = UnitState.Idle;

        UnitEvent.OnDamageReceived -= GetDamage;
        UnitEvent.OnHeal -= GetHeal;
        UnitEvent.OnTargetDeath -= UpdateTarget;

        UnitEvent.OnRetarget -= Retarget;
    }

    private void FixedUpdate()
    {
        if (WaveManager.instance.waveState != WaveManager.WaveState.Middle)
            return;

        SetCurrentState();
        ResetIsFirstAttack();

        switch (currentState)
        {
            case UnitState.Idle:
                if (target != null)
                    break;

                SetTarget();
                break;
            case UnitState.Moving:
                Move(target.transform.position);
                break;
            case UnitState.Fleeing:
                Flee(target.transform.position);
                break;
            case UnitState.Attacking:
                timeWithoutAttack = 0;
                Attack(target);
                break;
            default:
                break;
        }
    }

    protected void SetParameters(SO_Unit unitSO)
    {
        movementParameters = unitSO.movementParameters;
        attackParameters = unitSO.attackParameters;
        miscParameters = unitSO.miscParameters;
    }

    #region Target
    protected abstract void GetAvailableTarget();

    protected T[] GetAllUnits<T>() where T : Unit
    {
        return FindObjectsByType<T>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
    }

    private void UpdateTarget(Unit deadUnit)
    {
        if (deadUnit != target)
            return;

        target = null;
    }

    protected T GetClosestTarget<T>() where T : Unit
    {
        availableTarget.Where(target => target.isActiveAndEnabled)
            .OrderBy(target => Vector2.Distance(target.transform.position, transform.position));

        return (T)availableTarget[0];
    }

    /// <summary>
    /// Get the target that meet the required conditions. 
    /// </summary>
    /// <param name="sortType">The sorting to use</param>
    /// <returns>Either the closest target or the one with the lowest hp</returns>
    protected T GetTarget<T>(SortType sortType = SortType.Distance) where T : Unit
    {
        switch (sortType)
        {
            case SortType.Distance:
                availableTarget.Where(target => target.isActiveAndEnabled && target is T)
                    .OrderBy(target => Vector2.Distance(target.transform.position, transform.position));
                break;
            case SortType.Health:
                availableTarget.Where(target => target.isActiveAndEnabled && target is T).OrderBy(target => health);
                break;
            default:
                Debug.LogException(new Exception("Incorect type for the sorting type of  GetTarget"), this);
                return null;
        }

        return (T)availableTarget[0];
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

    protected virtual void SetTarget()
    {
        if (this is Hero)
            target = GetTarget<Enemy>();
        else if (this is Enemy)
            target = GetTarget<Hero>();
        else Debug.LogError("this Unit is neither a hero or an enemy", this);
    }

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
        Vector3 newPos = Vector2.MoveTowards(transform.position, targetPos, movementParameters.maxApproachSpeed * Time.fixedDeltaTime);

        Vector2 dir = newPos - transform.position;

        if (rb.linearVelocity.sqrMagnitude < movementParameters.maxApproachSpeed * movementParameters.maxApproachSpeed)
            rb.AddForce(dir * movementParameters.acceleration, ForceMode2D.Force);
    }

    private void Flee(Vector3 attackerPos)
    {
        if (fleeTimer > miscParameters.maxFleeTime)
        {
            hasStoppedFleeing = true;
            return;
        }

        fleeTimer += Time.fixedDeltaTime;

        Vector3 newPos = Vector2.MoveTowards(attackerPos, transform.position, movementParameters.maxFleeSpeed * Time.fixedDeltaTime);

        Vector2 dir = transform.position - newPos;

        if (rb.linearVelocity.sqrMagnitude < movementParameters.maxFleeSpeed * movementParameters.maxFleeSpeed)
        rb.AddForce(movementParameters.acceleration * dir, ForceMode2D.Force);
    }

    protected void Decelerate()
    {
        Vector3 currentSpeed = rb.linearVelocity;

        Vector3 targetSpeed = Vector3.MoveTowards(currentSpeed, Vector2.zero, movementParameters.maxDecelerationSpeed * Time.fixedDeltaTime);

        Vector3 newSpeed = targetSpeed - currentSpeed;

        rb.AddForce(newSpeed * movementParameters.deceleration, ForceMode2D.Force);
    }
    #endregion

    #region Attack
    protected virtual void Attack(Unit target)
    {
        if (!rb.linearVelocity.Approximately(Vector2.zero))
        {
            Decelerate();
            return;
        }
    }

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

    private void GetDamage(Unit damagedUnit, int damageReceived)
    {
        if (damagedUnit != this)
            return;

        int damageToReceive = damageReceived - (damageReceived + miscParameters.defense) / damageReceived;

        if (damageToReceive <= 0)
            damageToReceive = 1;

        if (health - damageToReceive <= 0)
            Death();
        else health -= damageToReceive;
    }

    protected abstract void Death();
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
        bool enemiesInFleeZone = Vector2.Distance(targetPos, transform.position) < attackParameters.fleeDistance;
        bool enemiesInAttackZone =  Vector2.Distance(targetPos, transform.position) < attackParameters.attackDistance;

        int caseId = (enemiesInFleeZone ? 1 : 0) + (enemiesInAttackZone ? 2 : 0);

        switch (caseId)
        {
            case 0:
                currentState = UnitState.Moving;
                hasStoppedFleeing = false;
                fleeTimer = 0;
                break;
            case 1:
            case 3:
                if (!hasStoppedFleeing)
                    currentState = UnitState.Fleeing;
                else currentState = UnitState.Attacking;
                    break;
            case 2:
                currentState = UnitState.Attacking;
                hasStoppedFleeing = false;
                fleeTimer = 0;
                break;
            default:
                Debug.LogError("No state for this situation");
                break;
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
    public static void HealedDamage(Unit unitToHeal, int damageToHeal) => OnHeal?.Invoke(unitToHeal, damageToHeal);

    public static event Action<Unit, Unit> OnRetarget;
    public static void ForceRetarget(Unit thisUnit, Unit retargeted) => OnRetarget?.Invoke(thisUnit, retargeted);
}