using UnityEngine;

public class State_Wander : State<NPCInput>
{
    public NPCInput ctrl;

    private float _idleTimer = 2;
    private bool _init;
    private float _actionTimer;

    public State_Wander()
    {
    }

    public override void Enter(NPCInput ctrl)
    {
        if(ctrl.debugAnimation)
            Debug.Log(ctrl.GetName() + ":<color=cyan>#</color> Wander State Enter");
        this.ctrl = ctrl;
        ctrl.Animation.ChangeForm(AnimationSet.DEFAULT);
        _idleTimer = Random.Range(0, 2);
        _init = true;
        _actionTimer = Random.Range(2, 6);
        ctrl.NavAgent.ResetPath();
        //ctrl.agent.pathfinder.enableRotation = true;
    }

    public override void Execute(NPCInput ctrl)
    {
        bool hasPath = ctrl.NavAgent.hasPath;

        if(hasPath)
        {
            HelperFunctions.RotateTo(ctrl.transform, ctrl.NavAgent.steeringTarget, 100);
        }
        if(_init || ctrl.NavAgent.hasPath == false || ctrl.NavAgent.remainingDistance <= 0.1f)
        {
            _init = false;

            _actionTimer -= Time.deltaTime;
            if(_actionTimer < 0 && Random.value > 0.7f && _idleTimer > 2)
            {
                PickUpFromGround();
                _actionTimer = Random.Range(2, 6);
            }

            _idleTimer -= Time.deltaTime;
            //Debug.Log(ctrl.agent.GetName() + ": Getting random dest");
            if(_idleTimer <= 0)
            {
                //Debug.Log(ctrl.agent.GetName() + ": Picking random dest");

                Vector3 rndPoint = ctrl.startPosition + Random.insideUnitSphere * 10;

                if(Vector3.Distance(rndPoint, ctrl.transform.position) > 10)
                {
                    ctrl.ChangeMovementSpeed(MovementSpeed.Run);
                }
                else
                {
                    ctrl.ChangeMovementSpeed(MovementSpeed.Walk);
                }
                _idleTimer = Random.Range(1, 5);
                ctrl.SetDestination(HelperFunctions.GetSampledNavMeshPositionAroundPoint(ctrl.startPosition, 5, 2, 15), 0.2f);
            }
        }
    }

    public override void Exit(NPCInput ctrl)
    {
        //Debug.Log("# Idle State Exit");
    }

    private void PickUpFromGround()
    {
        ctrl.Animation.Animator.CrossFade("Pick Up From Ground", 0.2f, 5);
    }

    //Vector3 PickRandomPoint()
    //{
    //    var point = ;

    //    point.y = 0;
    //    point += ctrl.agent.pathfinder.position;
    //    return point;
    //}


    //Vector3 GetRandomNavMeshPointAroundOrigin(Vector3 origin)
    //{
    //    return origin + Random.insideUnitSphere * 20;
    //}
}