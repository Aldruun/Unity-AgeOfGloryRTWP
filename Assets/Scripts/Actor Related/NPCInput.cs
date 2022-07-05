using UnityEngine;
using UnityEngine.AI;

public enum NPCState
{
    IDLE,
    WANDER,
    MOVEATTACK,
    MOVECAST,
    ESCORT
}

[System.Flags]
public enum AIFlags
{
    AllowMelee = 1,
    AllowRanged = 2,
    AllowOffensiveSpells = 4,
    AllowDefensiveSpells = 8,
    AllowHealing = 16
}
[System.Serializable]
public struct AIProfile
{
    public AIFlags AIFlags;
    public float meleeAttackEnemyHPThreshold;
    public float rangedAttackEnemyHPThreshold;
    public float offensiveAttackEnemyHPThreshold;
    public float healThreshold;

    public AIProfile(AIFlags aiFlags, float meleeAttackEnemyHPThreshold, float rangedAttackEnemyHPThreshold, float offensiveAttackEnemyHPThreshold, float healThreshold)
    {
        AIFlags = aiFlags;
        this.meleeAttackEnemyHPThreshold = meleeAttackEnemyHPThreshold;
        this.rangedAttackEnemyHPThreshold = rangedAttackEnemyHPThreshold;
        this.offensiveAttackEnemyHPThreshold = offensiveAttackEnemyHPThreshold;
        this.healThreshold = healThreshold;
    }
}
public struct WeaponInfo
{
    public int slot;
    public int range;
    public ItemType itemType;
    public bool backStab;
    public int critMult;
    public int critRange;
    public int profDamageBonus;

    public WeaponInfo(int slot, int range, ItemType itemType, bool backStab, int critMult, int critRange, int profDamageBonus)
    {
        this.slot = slot;
        this.range = range;
        this.itemType = itemType;
        this.backStab = backStab;
        this.critMult = critMult;
        this.critRange = critRange;
        this.profDamageBonus = profDamageBonus;
    }
}
public class NPCInput : Actor, IActivatable
{
    public NPCState npcState;
    public System.Action<float> spellCheckIntervalCallback;
    public System.Action<float, float> recoveryTimeCallback;
    // updated value, recovery time
    public System.Action<float> skillCheckIntervalCallback;
    public Vector3 startPosition;
    public Vector3 startEulerAngles;
    private float _checkForCloseActorsTimer;
    private Actor _closestLookAtActor;
    public AIProfile AIProfile { get; set; }

    public Actor EscortTarget { get; private set; }
    public float recoveryTime { get; set; }

    public override void FinalizeActor(ActorConfiguration config)
    {
        base.FinalizeActor(config);
        NavAgent = GetComponent<NavMeshAgent>();
        config.ConfigureNavAgent(NavAgent);

        Combat.Execute_EquipBestWeapon(Constants.EQUIP_ANY, false, true);
        Combat.Execute_DrawWeapon();
    }

    public override void ProcessActions()
    {
        if(waitTimer > 0)
        {
            waitTimer -= Time.deltaTime;
            if(waitTimer > 0)
                return;
        }

        if(CurrentAction == null)
        {
            if(debugActions)
                Debug.Log($"{GetName()}<color=orange>A</color>: CurrAction null -> popping next");
            CurrentAction = PopNextAction();

            if(debugActions)
            {
                if(CurrentAction == null)
                    Debug.Log($"{GetName()}<color=orange>A</color>: Popping next action failed");
            }

            return;
        }

        if(debugActions)
            Debug.Log($"{GetName()}<color=orange>A</color>: Exec CurrAction '{CurrentAction}'");
        if(CurrentAction.Done(this))
        {
            if(debugActions)
                Debug.Log($"{GetName()}<color=orange>A</color>: CurrAction '{CurrentAction} done");


            ReleaseCurrentAction();

        }
        //actionRoutine = CR_ExecuteAction();
        //scrMono.StartCoroutine(actionRoutine);
    }

    internal override void Stop()
    {
        base.Stop();
        NavAgent.SetDestination(transform.position);
    }

    public void SetEscortTarget(Actor actor)
    {
        if(actor == null)
        {
            if(EscortTarget != null)
            {
                EscortTarget.ActorStats.escortsCount--;
                EscortIndex = 0;
                EscortTarget = null;
            }
        }

        EscortTarget = actor;
        EscortIndex = EscortTarget.ActorStats.escortsCount;
        EscortTarget.ActorStats.escortsCount++;

    }

    public void ClearEscortTarget()
    {
        if(EscortTarget == null)
            return;

        EscortTarget.ActorStats.escortsCount--;
        EscortIndex = 0;
        EscortTarget = null;
    }

    public override void UpdateActiveCellBehaviours()
    {
        if(dead)
            return;

        UpdateStates();
        UpdateLocomotion();
        UpdateSummonedCreatureStates();

        ActorUI.Update();
        RoundSystem.ProcessRoundTime();
        UpdateScriptTicks();
        ProcessActions();
    }

    public virtual void UpdateLocomotion()
    {
        float desiredAnimSpeed = 0;

        if(inWater)
        {
            desiredAnimSpeed = 0.5f;
            NavAgent.speed = 1.2f;
        }
        else
        {
            switch(CurrentMovementSpeed)
            {

                case MovementSpeed.Walk:
                    desiredAnimSpeed = 0.5f;
                    NavAgent.speed = 1.2f;
                    break;
                case MovementSpeed.Run:
                    desiredAnimSpeed = 1f;
                    NavAgent.speed = 3.5f;
                    break;
                    //case MovementState.Sprint:
                    //maxSpeed = 1f;
                    //break;
            }
        }

        //Vector3 direction = NavAgent.steeringTarget - transform.position;

        Vector3 localDesiredVelocity = transform.InverseTransformDirection(NavAgent.desiredVelocity).normalized;
        Animation.UpdateMovementAnimations(localDesiredVelocity, localDesiredVelocity.magnitude * desiredAnimSpeed);
        UpdateLookAtIK();
    }

    public override void Signal_ProjectileIncoming(Transform projectile, float aoeRadius)
    {
        if(debugAnimation)
            Debug.Log($"{GetName()}: Projectile incoming");

        Vector3 sideStepPos = transform.position + (aoeRadius > 0 ? -projectile.right * aoeRadius : -projectile.right) + UnityEngine.Random.onUnitSphere;
        NavMeshHit navHit;
        if(NavMesh.SamplePosition(sideStepPos, out navHit, float.PositiveInfinity, NavMesh.AllAreas))
        {
            sideStepPos = navHit.position;

            if(Vector3.Distance(transform.position, sideStepPos) < aoeRadius + 0.1f)
            {
                sideStepPos = transform.position + (aoeRadius > 0 ? projectile.right * aoeRadius : projectile.right) + UnityEngine.Random.onUnitSphere;
            }
        }

        SetDestination(sideStepPos, 0.05f, "", 2);
    }

    public override Vector3 GetMovementVelocity()
    {
        return NavAgent.velocity;
    }

    public override Vector3 GetLookAtPoint()
    {
        return Animation.head.position + Vector3.up * 0.1f;
    }

    public override void UpdateLookAtIK()
    {
        _checkForCloseActorsTimer -= Time.deltaTime;

        if(_checkForCloseActorsTimer <= 0)
        {
            _checkForCloseActorsTimer = 1;

            //_closestLookAtActor = HelperFunctions.GetClosestActor_WithJobs(this, 5, );

        }
        if(Combat.GetHostileTarget() != null && ActorUtility.IsValidTarget(Combat.GetHostileTarget()))
        {
            UpdateAimIKTarget(Combat.GetHostileTarget().GetLookAtPoint());
        }
        else if(_closestLookAtActor != null)
        {
            UpdateAimIKTarget(_closestLookAtActor.GetLookAtPoint());
        }
        else
        {
            UpdateAimIKTarget(transform.position + transform.forward + Vector3.up * 1.7f);
        }
    }

    public float GetBoundingBoxHeight()
    {
        return 1.9f;
    }

    public void DisplayInfo()
    {

    }

    public void Activate(GameObject target)
    {

    }

    public void CloseInfo()
    {

    }

    internal override float GetCharacterRadius()
    {
        return NavAgent.radius;
    }

    private void UpdateStates()
    {
        if(ActorStats == null)
        {
            //Destroy(this);
            Debug.Log(gameObject.name + ":<color=red>Actor record null</color>");
        }

        if(debugAnimation)
            Debug.Log(GetName() + ":<color=cyan>*</color> Updating NPC Input");
        //if(isDowned || Animation.inBleedOutState)
        //{
        //    HeadTracking.SetIsAimingRangedWeapon(false);
        //    HeadTracking.SetLooAtWeight(0);
        //    return;
        //}
        //HeadTracking.SetLooAtWeight(1);

        //ActorInput enemy = HelperFunctions.GetClosestEnemy_WithJobs(this, 20);
        //Combat.SetHostileTarget(enemy);

        //if(enemy != null)
        //{
        //    //if(HasSpells && outOfMana == false && Vector3.Distance(transform.position, Combat.GetHostileTarget().transform.position) > 5)
        //    //{
        //    //    ChangeState(NPCState.MOVECAST);
        //    //}
        //    //else
        //    //{
        //    //    ChangeState(NPCState.MOVEATTACK);
        //    //}

        //    //if(tauntTimer <= Time.time)
        //    //{
        //    //    tauntTimer = Time.time + Random.Range(5f, 15f);
        //    //    SoundManager.ActorSFX.VerbalConstant(CharacterVoiceSet, voiceAudioSource, transform.position, VerbalConstantType.TAUNT);
        //    //}
        //    if(Combat.WeaponDrawn == false)
        //    {
        //        Combat.Execute_DrawWeapon();
        //    }
        //    ChangeMovementSpeed(MovementSpeed.Run);
        //    skillController.UpdateAI();
        //}
        //else
        //{
        //    if(EscortTarget != null)
        //    {
        //        ChangeState(NPCState.ESCORT);
        //    }
        //    else
        //    {
        //        ChangeState(NPCState.WANDER);
        //    }
        //    _npcStateMachine.Update(this);
        //}

        if(Immobile())
        {
            return;
        }

        if(hasMovementOrder)
        {
            bool done = false;

            if(AtDestination()) //! At target location
            {
                if(debugInput)
                {
                    Debug.DrawRay(transform.position, desiredTargetReachedDirection * 3, Color.yellow);
                    Debug.DrawRay(transform.position, transform.forward * 3, Color.red);
                }

                if(Mathf.Abs(Vector3.SignedAngle(transform.forward, desiredTargetReachedDirection, Vector3.up)) > 2f) //! Align facing
                {
                    HelperFunctions.RotateTo(transform, transform.position + desiredTargetReachedDirection, 300, "AtDest");
                    done = false;
                }
                else
                {
                    done = true;
                }

                NavAgent.avoidancePriority = 100;
                NavAgent.destination = transform.position;
                OnMovementOrderDone?.Invoke();
                OnMovementOrderDone = null;
            }
            else
            {
                HelperFunctions.RotateTo(transform, NavAgent.steeringTarget, 300, "NPCController: Steer");
            }

            hasMovementOrder = !done;
        }
    }
    private ActorStats GetSpellTarget()
    {

        return null;
    }
    private void UpdateAimIKTarget(Vector3 target)
    {
        HeadTracking.lookAtPosition = target;
    }
}
