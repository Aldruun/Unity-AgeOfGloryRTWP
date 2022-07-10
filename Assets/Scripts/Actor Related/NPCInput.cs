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
    public System.Action<float, float> recoveryTimeCallback; // updated value, recovery time
    public System.Action<float> skillCheckIntervalCallback;
    public Vector3 startPosition;
    public Vector3 startEulerAngles;
    private float _checkForCloseActorsTimer;
    private readonly Actor _closestLookAtActor;
    public AIProfile AIProfile { get; set; }

    public Actor EscortTarget { get; private set; }
    public float recoveryTime { get; set; }

    public override void FinalizeActor(ActorConfiguration config)
    {
        base.FinalizeActor(config);

        /*TODO Get rid of this switch statement and create a database
       * to fetch scripts and stats depending on actor class
       */
        switch(ActorStats.Class)
        {
            case Class.FIGHTER:
            case Class.MONK:
            case Class.PALADIN:
            case Class.THIEF:
            case Class.BARBARIAN:
                SetScript(new AIScripts.AI_StandardMelee(this), 0);
                break;
            case Class.RANGER:
                IsRanged = true;
                SetScript(new AIScripts.AI_StandardRanged(this), 0);
                break;
            case Class.SORCERER:
            case Class.SHAMAN:
            case Class.MAGE:
                IsRanged = true;
                SetScript(new AIScripts.AI_WizardAggressive(this), 0);
                break;
            case Class.BARD:
            case Class.CLERIC:
            case Class.DRUID:
                SetScript(new AIScripts.AI_ClericHealer(this), 0);
                break;
        }

        NavAgent = GetComponent<NavMeshAgent>();
        config.ConfigureNavAgent(NavAgent);
    }

    public override void ProcessActions()
    {
        if(waitTimer > 0)
        {
            waitTimer -= Time.deltaTime;
            if(waitTimer > 0)
            {
                return;
            }
        }

        if(CurrentAction == null)
        {
            if(debugActions)
            {
                Debug.Log($"{GetName()}:<color=orange>A/</color> CurrAction null -> popping next");
            }

            CurrentAction = PopNextAction();

            if(debugActions)
            {
                if(CurrentAction == null)
                {
                    Debug.Log($"{GetName()}:<color=orange>A/</color> Popping next action failed");
                }
            }

            return;
        }

        if(debugActions)
        {
            Debug.Log($"{GetName()}:<color=orange>A/</color> Exec CurrAction '{CurrentAction}'");
        }

        if(CurrentAction.Done(this))
        {
            if(debugActions)
            {
                Debug.Log($"{GetName()}:<color=orange>A/</color> CurrAction '{CurrentAction}' done");
            }

            ReleaseCurrentAction();
        }
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
        {
            return;
        }

        EscortTarget.ActorStats.escortsCount--;
        EscortIndex = 0;
        EscortTarget = null;
    }

    public override void UpdateActiveCellBehaviours()
    {
        if(dead)
        {
            return;
        }

        UpdateLocomotion();
        UpdateSummonedCreatureStates();

        ActorUI.Update();
        RoundSystem.ProcessRoundTime();
        
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
        {
            Debug.Log($"{GetName()}: Projectile incoming");
        }

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
        return Animation.head.position + (Vector3.up * 0.1f);
    }

    public override void UpdateLookAtIK()
    {
        _checkForCloseActorsTimer -= Time.deltaTime;

        if(_checkForCloseActorsTimer <= 0)
        {
            _checkForCloseActorsTimer = 1;
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
            UpdateAimIKTarget(transform.position + transform.forward + (Vector3.up * 1.7f));
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

    private ActorStats GetSpellTarget()
    {

        return null;
    }
    private void UpdateAimIKTarget(Vector3 target)
    {
        HeadTracking.lookAtPosition = target;
    }
}
