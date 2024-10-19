using System;
using UnityEngine;


//The events related to the character the GameObject type is used as an Id check
public class CharacterEvent
{
    public event Action<GameObject> FindClosestOpponent;
    public void FindClosestOpponentEvent(GameObject gameObject)
    {
        FindClosestOpponent?.Invoke(gameObject);
    }

    public event Action<int, GameObject> Move;
    public void MoveEvent(int speed, GameObject gameObject)
    {
        Move?.Invoke(speed, gameObject);
    }

    public event Action<int, GameObject> Flee;
    public void FleeEvent(int speed, GameObject gameObject)
    {
        Flee?.Invoke(speed, gameObject);
    }

    public event Action<int, GameObject> Attack;
    public void AttackEvent(int damage, GameObject gameObject)
    {
        Attack?.Invoke(damage, gameObject);
    }

    public event Action<int, GameObject, GameObject> TakeDamage;
    public void TakeDamageEvent(int damage, GameObject gameObject, GameObject striker)
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

    public event Action<int, GameObject> Heal;
    public void HealEvent(int heal, GameObject gameObject)
    {
        Heal?.Invoke(heal, gameObject);
    }

}
