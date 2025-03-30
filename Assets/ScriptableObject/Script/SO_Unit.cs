using UnityEngine;

public abstract class SO_Unit : ScriptableObject
{
    [field: SerializeField] public MovementParameters movementParameters {  get; private set; }
    [field: SerializeField] public AttackParameters attackParameters { get; private set; }
    [field: SerializeField] public MiscParameters miscParameters { get; private set; }
}
