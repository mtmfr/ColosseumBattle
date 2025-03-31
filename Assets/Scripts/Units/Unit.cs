using System;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
public abstract class Unit : MonoBehaviour
{
    Rigidbody2D rb;

    private UnitState currentState;

    private MovementParameters movementParameters;
    protected AttackParameters attackParameters;
    private MiscParameters miscParameters;

    private int health;
    private int defense;

    protected float attackTimer = 0;

    protected bool isFirstAttack = true;

    private float timeWithoutAttack = 0;

    protected Unit target;
    Unit[] availableTarget;

    protected bool wasDead = true;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (this is Hero)
            GetAllUnits<Enemy>(out availableTarget);
        else if (this is Enemy)
            GetAllUnits<Hero>(out availableTarget);
        else Debug.LogError("this object is neither a hero or an enemy", this);
    }

    private void OnEnable()
    {
        if (wasDead)
            health = miscParameters.health;

        UnitEvent.OnDamageReceived += GetDamage;
        UnitEvent.OnHeal += GetHeal;
        UnitEvent.OnTargetDeath += UpdateTarget;
    }

    private void OnDisable()
    {
        UnitEvent.OnDamageReceived -= GetDamage;
        UnitEvent.OnHeal -= GetHeal;
        UnitEvent.OnTargetDeath -= UpdateTarget;
    }

    private void FixedUpdate()
    {
        SetCurrentState();
        ResetIsFirstAttack();

        switch (currentState)
        {
            case UnitState.Idle:
                if (target != null)
                    break;

                if (this is Hero)
                    GetClosestTarget<Enemy>(out target);
                else if (this is Enemy)
                    GetClosestTarget<Hero>(out target);
                else Debug.LogError("this object is neither a hero or an enemy", this);

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
        }
    }

    protected void SetParameters(SO_Unit unitSO)
    {
        movementParameters = unitSO.movementParameters;
        attackParameters = unitSO.attackParameters;
        miscParameters = unitSO.miscParameters;
    }

    #region Target
    private void GetAllUnits<T>(out Unit[] allUnits) where T : Unit
    {
        allUnits = FindObjectsByType<T>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
    }

    private void UpdateTarget(Unit deadUnit)
    {
        if (deadUnit != target)
            return;

        target = null;
    }

    protected virtual void GetClosestTarget<T>(out Unit target) where T : Unit
    {
        availableTarget.Where(target => target.isActiveAndEnabled)
            .OrderBy(target => Vector2.Distance(target.transform.position, transform.position));

        target = availableTarget[0];
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
        Vector3 newPos = Vector2.MoveTowards(attackerPos, transform.position, movementParameters.maxFleeSpeed * Time.fixedDeltaTime);

        Vector2 dir = transform.position - newPos;

        if (rb.linearVelocity.sqrMagnitude < movementParameters.maxFleeSpeed * movementParameters.maxFleeSpeed)
        rb.AddForce(movementParameters.acceleration * dir, ForceMode2D.Force);
    }

    private void Decelerate()
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
                break;
            case 1:
            case 3:
                currentState = UnitState.Fleeing;
                break;
            case 2:
                currentState = UnitState.Attacking;
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
}