using UnityEngine;

public class EnableRootMotion : StateMachineBehaviour
{
    //public bool applyRootMotion;
    //private const string boolName = "bApplyRootMotion";
    //public enum TargetState
    //{
    //    Enter,
    //    Update,
    //    Exit
    //}
    //public TargetState targetState;

    //public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    if(targetState == TargetState.Enter)
    //    {
    //        //animator.applyRootMotion = applyRootMotion;

    //        if(boolName != "")
    //        {
    //            animator.SetBool(boolName, applyRootMotion);
    //        }
    //    }
    //}

    //public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    if(targetState == TargetState.Update)
    //    {
    //        //animator.applyRootMotion = applyRootMotion;

    //        if(boolName != "")
    //        {
    //            animator.SetBool(boolName, applyRootMotion);
    //        }
    //    }
    //}

    //public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    if(targetState == TargetState.Exit)
    //    {
    //        //animator.applyRootMotion = applyRootMotion;

    //        if(boolName != "")
    //        {
    //            animator.SetBool(boolName, applyRootMotion);
    //        }
    //    }

    //    foreach(var p in animator.parameters)
    //    {
    //        if(p.type == AnimatorControllerParameterType.Trigger)
    //        {
    //            animator.SetBool(boolName, applyRootMotion);
    //        }
    //    }
    //}
}

//public override void OnStateMachineExit(Animator animator, int stateMachinePathHash)
//{
//    if(targetState == TargetState.Exit)
//    {
//        animator.applyRootMotion = applyRootMotion;
//    }
//}

//public class AWeaponSFXBehaviour : StateMachineBehaviour
//{
//    public AudioClip[] clipsToPlay;
//    public float normalizedDelay;
//    float _timer;

//    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
//    {
//        animator.GetComponent<AgentMonoController>().StartCoroutine(CR_PlaySound(animator));
//    }

//    IEnumerator CR_PlaySound(Animator animator)
//    {
//        yield return new WaitForSeconds(normalizedDelay);
//        Debug.Log("TRIGGER SFX");
//        AgentSFXManager.TriggerSFX(clipsToPlay[Random.Range(0, clipsToPlay.Length)], animator.transform.position);
//    }
//}