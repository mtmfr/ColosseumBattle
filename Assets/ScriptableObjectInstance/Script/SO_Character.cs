using UnityEngine;

public class SO_Character : ScriptableObject
{
    [field: SerializeField] public Sprite CharIcon {  get; protected set; }

    [field: SerializeField] public int Health { get; protected set; }
    [field: SerializeField] public int Attack { get; protected set; }
    [field: SerializeField] public int Magic { get; protected set; }
    [field: SerializeField] public int Speed { get; protected set; }
    [field: SerializeField] public int Cost { get; protected set; }
    [field: SerializeField] public float MinRange { get; protected set; }
    [field: SerializeField] public float MaxRange { get; protected set; }
    [field: SerializeField] public float AttSpeed { get; protected set; }
    [field: SerializeField] public LayerMask OpponentLayer { get; protected set; }
}
 