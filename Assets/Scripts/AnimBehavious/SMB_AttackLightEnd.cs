using UnityEngine;

public class SMB_AttackLightEnd : StateMachineBehaviour
{

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        AnimEnd();
    }

    private void AnimEnd() => Spell_AttackLight.isAnimFinished = true;
}


