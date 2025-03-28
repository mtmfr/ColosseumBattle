using UnityEngine;

public abstract class SO_Unit : ScriptableObject
{
    [Header("Health")]
    [field: SerializeField] public int health { get; protected set; }

    [Header("Attack")]
    [field: SerializeField] public int attackPower { get; protected set; }
    [field: SerializeField] public float attackSpeed {  get; protected set; }

    [Header("Movement")]
    [field: SerializeField] public float movementSpeed { get; protected set; }

    [Header("ActionZone")]
    [field: SerializeField] public float fleeRange { get; protected set; }
    [field: SerializeField] public float attackRange { get; protected set; }

    [Header("Price")]
    [field: SerializeField] public int cost { get; protected set; }
}
