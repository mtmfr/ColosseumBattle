using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Scriptable Object", menuName = "Character/Character")]
public class CharacterSO : ScriptableObject
{
    [field: SerializeField] public ushort Health { get; private set; }
    [field: SerializeField] public ushort Attack { get; private set; }
    [field: SerializeField] public ushort Defense { get; private set; }
    [field: SerializeField] public ushort Magic { get; private set; }
    [field: SerializeField] public ushort Speed { get; private set; }
    [field: SerializeField] public float MinRange { get; private set; }
    [field: SerializeField] public float MaxRange { get; private set; }
    [field: SerializeField] public float AttSpeed { get; private set; }
    [field: SerializeField] public LayerMask LayerMask { get; private set; }
}
 