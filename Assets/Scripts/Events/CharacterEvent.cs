using System;
using UnityEngine;

public class CharacterEvent
{

    public event Action<GameObject> FindClosestEnemy;
    public void FindClosestEnemyEvent(GameObject gameObject)
    {
        FindClosestEnemy?.Invoke(gameObject);
    }

    public event Action<ushort, GameObject> Move;
    public void MoveEvent(ushort speed, GameObject gameObject)
    {
        Move?.Invoke(speed, gameObject);
    }

    public event Action<ushort, GameObject> Flee;
    public void FleeEvent(ushort speed, GameObject gameObject)
    {
        Flee?.Invoke(speed, gameObject);
    }

    public event Action<ushort, GameObject> Attack;
    public void AttackEvent(ushort damage, GameObject gameObject)
    {
        Attack?.Invoke(damage, gameObject);
    }

    public event Action<ushort, GameObject, GameObject> TakeDamage;
    public void TakeDamageEvent(ushort damage, GameObject gameObject, GameObject striker)
    {
        TakeDamage?.Invoke(damage, gameObject, striker);
    }

    public event Action<GameObject> UseUltimate;
    public void UseUltimateEvent(GameObject gameObject)
    {
        UseUltimate?.Invoke(gameObject);
    }

    public event Action<GameObject, GameObject> Dying;
    public void DyingEvent(GameObject gameObject, GameObject killer)
    {
        Dying?.Invoke(gameObject, killer);
    }

    public event Action<ushort, GameObject> Heal;
    public void HealEvent(ushort heal, GameObject gameObject)
    {
        Heal?.Invoke(heal, gameObject);
    }

}
