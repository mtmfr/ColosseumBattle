using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
public abstract class Unit : MonoBehaviour
{
    Rigidbody2D rb;

   private UnitState currentState;

    float maxSpeed = 10;
    float maxFleeSpeed = 12;
    float speed = 5;
    float fleeSpeed = 5.5f;
    float acceleration = 4;
    float deceleration = 20;


    public float fleeDistance;
    public float attackDistance;

    protected int attackPower;

    protected float attackCooldown = 0;
    protected float attackTimer = 0;
    protected float firstAttackCooldown;
    protected bool isFirstAttack;
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

    private void Move(Vector3 targetPos)
    {
        Vector3 newPos = Vector2.MoveTowards(transform.position, targetPos, speed * Time.fixedDeltaTime);

        Vector2 dir = newPos - transform.position;

        if (rb.linearVelocity.sqrMagnitude < maxSpeed * maxSpeed)
            rb.AddForce(dir * acceleration, ForceMode2D.Force);
    }

    private void Flee(Vector3 attackerPos)
    {
        Vector3 newPos = Vector2.MoveTowards(attackerPos, transform.position, fleeSpeed * Time.fixedDeltaTime);

        Vector2 dir = transform.position - newPos;

        if (rb.linearVelocity.sqrMagnitude < maxFleeSpeed * maxFleeSpeed)
        rb.AddForce(acceleration * dir, ForceMode2D.Force);
    }

    private void Decelerate()
    {
        Vector3 currentSpeed = rb.linearVelocity;

        Vector3 targetSpeed = Vector3.MoveTowards(currentSpeed, Vector2.zero, deceleration * Time.fixedDeltaTime);

        Vector3 newSpeed = targetSpeed - currentSpeed;

        rb.AddForce(newSpeed * deceleration, ForceMode2D.Force);
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

        if (timeWithoutAttack < attackCooldown)
            timeWithoutAttack += Time.fixedDeltaTime;
        else
            isFirstAttack = true;

    }

    private void SetCurrentState()
    {
        Vector2 targetPos = target.transform.position;
        bool enemiesInFleeZone = Vector2.Distance(targetPos, transform.position) < fleeDistance;
        bool enemiesInAttackZone =  Vector2.Distance(targetPos, transform.position) < attackDistance;

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

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(transform.position, fleeDistance);

        Gizmos.color = Color.green;
        
        Gizmos.DrawWireSphere(transform.position, attackDistance);
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