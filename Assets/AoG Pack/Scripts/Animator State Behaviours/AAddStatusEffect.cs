using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StatusEffectType
{
    Cloak,
    Root
}

public class AAddStatusEffect : StateMachineBehaviour
{

    public StatusEffectType statusEffectType;
    public bool onEnter;
    public bool statusEffectEnabled;
    private ActorInput _self;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _self = animator.GetComponent<ActorInput>();

        if(onEnter)
        {
            _self.StartCoroutine(CR_AddStatusEffect(animator));
        }
    }

    private IEnumerator CR_AddStatusEffect(Animator animator)
    {
        //Debug.Log("TRIGGER SFX");
        //_audioSource = AgentSFXManager.TriggerSFX(clipsToPlay[Random.Range(0, clipsToPlay.Length)],
        //    animator.transform.position);

        switch(statusEffectType)
        {
            case StatusEffectType.Cloak:

                break;
            case StatusEffectType.Root:

                if(statusEffectEnabled)
                {
                    _self.Combat.Execute_BlockAggro(2);
                    //_self.HoldPosition();
                }
                else
                {
                    //_self.Unroot();
                }

                break;
        }

        yield return null;
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(onEnter == false)
        {
            _self.StartCoroutine(CR_AddStatusEffect(animator));
        }
    }
}
