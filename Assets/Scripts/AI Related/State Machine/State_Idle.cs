using System.Collections;
using UnityEngine;

public class State_Idle : State<NPCInput>
{
    public NPCInput ctrl;

    public State_Idle()
    {
    }

    public override void Enter(NPCInput ctrl)
    {
        if(ctrl.debugInput)
            Debug.Log(ctrl.GetName() + ":<color=cyan>#</color> Idle State Enter");
        ctrl.Animation.Animator.CrossFade("Idle", 0.2f, 0);
    }

    public override void Execute(NPCInput ctrl)
    {
        //Debug.Log("# Idle State Update");
        //if(ctrl.agent.attackTarget != null)
        //{
        //    if(ctrl.agent.m_isSpellCaster)
        //    {
        //        ctrl.ChangeState(new State_MoveToAndCast());
        //    }
        //    else
        //        ctrl.ChangeState(new State_MoveToAndAttack(ctrl));
        //}
    }

    public override void Exit(NPCInput ctrl)
    {
        //Debug.Log("# Idle State Exit");
    }
}

//public class State_BleedOut : State<NPCInput>
//{
//    public NPCInput ctrl;


//    public State_BleedOut()
//    {
//    }

//    public override void Enter(NPCInput ctrl)
//    {
//        this.ctrl = ctrl;
//        if(ctrl.debug)
//            Debug.Log(ctrl.GetName() + ":<color=cyan>#</color> Idle State Enter");
//        ctrl.Animation.PlayMotion_BleedOut(0);
//        ctrl.NavAgent.isStopped = true;
//        ctrl.StartCoroutine(CR_BleedOutDone());
//    }

//    public override void Execute(NPCInput ctrl)
//    {
//        //Debug.Log("# Idle State Update");
//        //if(ctrl.agent.attackTarget != null)
//        //{
//        //    if(ctrl.agent.m_isSpellCaster)
//        //    {
//        //        ctrl.ChangeState(new State_MoveToAndCast());
//        //    }
//        //    else
//        //        ctrl.ChangeState(new State_MoveToAndAttack(ctrl));
//        //}
//    }

//    public override void Exit(NPCInput ctrl)
//    {
//        ctrl.NavAgent.isStopped = false;
//    }

//    IEnumerator CR_BleedOutDone()
//    {
//        yield return new WaitForSeconds(4);
//        ctrl.Animation.animator.CrossFade("Cancel BleedOut", 0.2f, 10);
//        yield return new WaitForSeconds(1);
//        ctrl.Animation.inBleedOutState = false;
//        ctrl.NavAgent.isStopped = false;
//    }
//}