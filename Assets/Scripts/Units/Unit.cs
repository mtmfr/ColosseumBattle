using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
public abstract class Unit : MonoBehaviour
{
    private Rigidbody2D rb;

    protected Unit opponent;
    protected List<Unit> opponents = new List<Unit>();
    protected UnitState currentState { get; private set; }

    #region stats
    protected int health;
    protected int attackPower;
    protected float movementSpeed;
    protected float attackSpeed;

    protected float attackRange;
    protected float fleeRange;

    public int cost { get; private set; }
    #endregion

    protected bool wasDead = true;

    protected float attackCooldown;

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    protected virtual void Update()
    {
        UnitControl();
    }

    protected virtual void OnEnable()
    {
        UnitEvent.OnDamageReceived += OnTakeDamage;
        UnitEvent.OnHeal += OnHeal;
    }

    private void OnDisable()
    {
        UnitEvent.OnDamageReceived -= OnTakeDamage;
        UnitEvent.OnHeal -= OnHeal;
    }

    protected abstract void GetTarget();

    protected abstract void UpdateState();

    protected void SetStats(SO_Unit unitStats)
    {
        if (wasDead)
        {
            health = unitStats.health;
            wasDead = false;
        }

        attackPower = unitStats.attackPower;
        movementSpeed = unitStats.movementSpeed;
        attackRange = unitStats.attackRange;
        fleeRange = unitStats.fleeRange;
        attackSpeed = unitStats.attackSpeed;
    }

    protected void UnitControl()
    {
        if (GameManager.instance.currentGameState switch
        {
            GameState.WaveStart => true,
            GameState.Wave => true,
            _ => false,
        })
            return;

        UpdateState();
        switch (currentState)
        {
            case UnitState.Moving:
                ApproachTarget();
                break;
            case UnitState.Fleeing:
                Flee();
                break;
            case UnitState.Attacking:
                Attack();
                break;
            case UnitState.Idle:
            default:
                GetTarget();
                break;
        }
    }

    #region Character control
    protected void ApproachTarget()
    {
        Vector2 MovementDirection = opponent.transform.position - transform.position;

        rb.linearVelocity = MovementDirection.normalized * movementSpeed;
    }

    protected void Flee()
    {
        Vector2 FleeingDirection = transform.position - opponent.transform.position;

        rb.linearVelocity = movementSpeed * 1.5f * FleeingDirection.normalized;
    }

    protected virtual void OnHeal(Unit unit, int healedHp)
    {

    }

    protected virtual void OnTakeDamage(Unit unit, int DamageToTake)
    {
        if (unit !=  this)
            return;

        if (health - DamageToTake > 0)
            health -= DamageToTake;
        else Death();
    }

    protected abstract void Attack();
    protected abstract void Death();

    protected void ChangeCurrentState(UnitState state)
    {
        currentState = state;
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