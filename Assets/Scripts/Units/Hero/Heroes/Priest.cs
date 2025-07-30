using System.Linq;
using UnityEngine;

public class Priest : Hero
{
    [SerializeField] private Spell_AttackLight attackLight;
    [SerializeField] private Spell_HealingLight healingLight;

    protected override void GetAvailableTarget()
    {
        if (hasStoppedFleeing)
            availableTargets = GetAllUnits<Enemy>().ToList();
        else availableTargets = GetAllUnits<Unit>().Where(availableTarget => availableTarget is not Priest).ToList();
    }

    protected override void SetTarget()
    {
        target = GetTarget<Hero>(SortType.Health);

        if (target == null)
            target = GetTarget<Enemy>();
    }

    protected override void AttackMotion(Unit target, int damageToDeal)
    {
        if (target is Hero heroToHeal)
        {
            int healedAmount = Mathf.FloorToInt(damageToDeal * (heroToHeal.health / (float)heroToHeal.heroSO.miscParameters.health));
            Spell_HealingLight light = ObjectPool.GetObject(healingLight, target.transform.position, Quaternion.identity);
            light.Initialize(target, healedAmount);
            
        }

        if (target is Enemy)
        {
            Spell_AttackLight light = ObjectPool.GetObject(attackLight, target.transform.position, Quaternion.identity);
            light.Launch(target, damageToDeal);
        }
    }
}
