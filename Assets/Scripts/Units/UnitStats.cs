using System;
using UnityEngine;

[Serializable]
public class MovementParameters
{
    [field: Header("Speed limit")]
    [field: SerializeField] public float maxApproachSpeed { get; private set; }
    [field: SerializeField] public float maxFleeSpeed { get; private set; }
}

[Serializable]
public class AttackParameters
{
    [field: Header("Opponent distance")]
    [field: SerializeField] public float fleeDistance { get; private set; }
    [field: SerializeField] public float attackDistance { get; private set; }

    [field: Header("Attack")]
    [field: SerializeField] public int attackPower { get; private set; }

    [field: Header("Cooldown")]
    [field: SerializeField] public float firstAttackCooldown { get; private set; }
    [field: SerializeField] public float attackCooldown { get; private set; }
}

[Serializable]
public class MiscParameters
{
    [field: SerializeField] public int maxFleeTime { get; private set; }
    [field: SerializeField] public int health { get; private set; }
    [field: SerializeField] public int defense { get; private set; }
    [field: SerializeField] public int cost { get; private set; }
}

[Serializable]
public class UIParameters
{
    [field: SerializeField] public string heroDescription { get; private set; }
    [field: SerializeField] public Sprite characterSprite { get; private set; }
}