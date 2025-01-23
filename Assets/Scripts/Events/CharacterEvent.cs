using System;
using UnityEngine;


//The events related to the character the GameObject type is used as an Id check
public static class CharacterEvent
{
    public static event Action<int, int> OnAttackHit;
    public static void AttackHit(int damage, int objectId)
    {
        OnAttackHit?.Invoke(damage, objectId);
    }

    public static event Action<int> OnDeath;
    public static void Death(int objectId)
    {
        OnDeath?.Invoke(objectId);
    }

    public static event Action<int> OnForcedRetarget;
    public static void ForceRetarget(int objectId)
    {
        OnForcedRetarget?.Invoke(objectId);
    }

    public static event Action<int, GameObject> OnHeal;
    public static void Heal(int heal, GameObject gameObject)
    {
        OnHeal?.Invoke(heal, gameObject);
    }

}
