using System;
using UnityEngine;

[Serializable]
public class MovementParameters
{
    [field: Header("Speed limit")]
    [field: SerializeField] public float maxApproachSpeed { get; private set; }
    [field: SerializeField] public float maxFleeSpeed { get; private set; }

    [field: Header("Acceleration")]
    [field: SerializeField] public float acceleration { get; private set; }
    [field: SerializeField] public float deceleration { get; private set; }
}

[Serializable]
public class AttackParameters
{
    [field: Header("oppnent distance")]
    [field: SerializeField] public float fleeDistance { get; private set; }
    [field: SerializeField] public float attackDistance { get; private set; }

    [field: Header("Attack")]
    [field: SerializeField] public int attackPower { get; private set; }

    [field: Header("Cooldown")]
    [field: SerializeField] public float firstAttackCooldown { get; private set; }
    [field: SerializeField] public float attackCooldown { get; private set; }
}
