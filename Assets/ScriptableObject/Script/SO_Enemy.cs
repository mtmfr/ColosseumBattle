using UnityEngine;

[CreateAssetMenu(fileName = "SO_Enemy", menuName = "Scriptable Objects/Unit/Enemy")]
public class SO_Enemy : SO_Unit
{
    [field: SerializeField] public int goldDrop { get; private set; }
}
