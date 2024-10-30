using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ScriptableObject", menuName = "Character")]
public class CharacterSO : ScriptableObject
{
    [field: SerializeField] public Sprite CharIcon {  get; private set; }

    [field: SerializeField] public int Health { get; private set; }
    [field: SerializeField] public int Attack { get; private set; }
    [field: SerializeField] public int Magic { get; private set; }
    [field: SerializeField] public int Speed { get; private set; }
    [field: SerializeField] public int Cost { get; private set; }
    [field: SerializeField] public float MinRange { get; private set; }
    [field: SerializeField] public float MaxRange { get; private set; }
    [field: SerializeField] public float AttSpeed { get; private set; }
    [field: SerializeField] public LayerMask OpponentMask { get; private set; }
}
 