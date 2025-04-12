using UnityEngine;

public abstract class Enemy : Unit
{
    [SerializeField] private SO_Enemy enemySO;

    private void Awake()
    {
        SetParameters(enemySO);
    }

    protected override void GetAvailableTarget()
    {
        availableTarget = GetAllUnits<Hero>();
    }

    protected override void Death()
    {
        GameManager.instance.AddGold(enemySO.goldDrop);
        ObjectPool.SetObjectInactive(this);
    }
}
