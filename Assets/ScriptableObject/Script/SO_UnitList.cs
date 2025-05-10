using UnityEngine;

[CreateAssetMenu(fileName = "SO_UnitList", menuName = "Scriptable Objects/SO_UnitList")]
public class SO_UnitList : ScriptableObject
{
    [field: SerializeField] public Unit[] units { get; private set; }
}
