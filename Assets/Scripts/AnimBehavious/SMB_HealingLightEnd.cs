using UnityEngine;

public class SMB_HealingLightEnd : StateMachineBehaviour
{

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        AnimEnd();
    }

    private void AnimEnd() => Spell_HealingLight.isAnimFinished = true;
}