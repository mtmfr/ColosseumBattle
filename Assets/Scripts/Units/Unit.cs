using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
public abstract class Unit : MonoBehaviour
{
    Rigidbody2D rb;

    private UnitState currentState;

    private MovementParameters movementParameters;
    protected AttackParameters attackParameters;

    protected float attackTimer = 0;

    protected bool isFirstAttack = true;

    private float timeWithoutAttack = 0;

    public GameObject target;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        SetCurrentState();
        ResetIsFirstAttack();

        switch (currentState)
        {
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
    }

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

        Vector3 targetSpeed = Vector3.MoveTowards(currentSpeed, Vector2.zero, movementParameters.deceleration * Time.fixedDeltaTime);

        Vector3 newSpeed = targetSpeed - currentSpeed;

        rb.AddForce(newSpeed * movementParameters.deceleration, ForceMode2D.Force);
    }

    protected virtual void Attack(GameObject target)
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

    private void SetCurrentState()
    {
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

    protected virtual void OnDrawGizmosSelected()
    {
        
    }
}

public static class UnitEvent
{
    public static event Action<Unit> OnDeath;
    public static void Dying(Unit thisUnit) => OnDeath?.Invoke(thisUnit);

    public static event Action<Unit, int> OnDamageReceived;
    public static void DealDamage(Unit unitToDamage, int damageToDeal) => OnDamageReceived?.Invoke(unitToDamage, damageToDeal);

    public static event Action<Unit, int> OnHeal;
    public static void HealedDamage(Unit unitToHeal, int damageToHeal) => OnHeal?.Invoke(unitToHeal, damageToHeal);
}