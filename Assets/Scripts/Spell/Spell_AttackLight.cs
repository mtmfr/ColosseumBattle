using System.Threading.Tasks;
using UnityEngine;

public class Spell_AttackLight : MonoBehaviour
{
    public static bool isAnimFinished = false;

    public void Launch(Unit target, int damageToDeal)
    {
        UnitEvent.DealDamage(target, damageToDeal);
    }

    private async Task<bool> WaitForAnimEnd()
    {
        while(!isAnimFinished)
        {
            await Task.Yield();
        }

        return true;
    }

    private void Update()
    {
        Deactivate();
    }

    private async void Deactivate()
    {
        if (await WaitForAnimEnd())
        {
            isAnimFinished = false;
            ObjectPool.SetObjectInactive(this);
        }
    }
}
