using System;
using UnityEngine;

public class SMB_DeathAnimFinished : StateMachineBehaviour
{
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        DeathAnimEnd(animator.gameObject.GetInstanceID());
    }

    public static event Action<int> OnDeathAnimEnd;
    public static void DeathAnimEnd(int objectId) => OnDeathAnimEnd?.Invoke(objectId);
}
