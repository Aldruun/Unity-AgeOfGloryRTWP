using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AMeleeComboBehaviour : StateMachineBehaviour
{
    public int comboStage;
    public int nextComboStage;
    public float successStartThreshold = 0.7f;

    private void Awake()
    {
        
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(stateInfo.normalizedTime >= successStartThreshold)
        {
            animator.SetInteger("iComboStageIndex", nextComboStage);
        }
    }
}
