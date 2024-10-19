using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroEquipment : ScriptableObject
{
    [field: SerializeField] public int HealthMod { get; protected set; }

    public int AttMod { get; protected set; }
    public int MagMod { get; protected set; }
    public int AttSpMod { get; protected set; }
    public int DefMod { get; protected set; }

    public float MaxRangMod { get; protected set; }
    public float MinRangMod { get; protected set; }
}
