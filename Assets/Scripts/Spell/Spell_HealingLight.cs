using System.Threading.Tasks;
using UnityEngine;

public class Spell_HealingLight : MonoBehaviour
{
    public static bool isAnimFinished = false;

    Unit toHeal;
    int damageToHeal;

    private void Update()
    {
        Deactivate();
    }

    public void Initialize(Unit toHeal, int damageToHeal)
    {
        this.toHeal = toHeal;
        this.damageToHeal = damageToHeal;
    }

    private async Task<bool> WaitForAnimEnd()
    {
        while (!isAnimFinished)
        {
            await Task.Yield();
        }

        return true;
    }

    private async void Deactivate()
    {
        if (await WaitForAnimEnd())
        {
            UnitEvent.HealDamage(toHeal, damageToHeal);
            isAnimFinished = false;
            ObjectPool.SetObjectInactive(this);
        }
    }
}
