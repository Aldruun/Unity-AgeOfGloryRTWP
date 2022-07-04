using GenericFunctions;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Profiling;

public class State_MoveToAndCast : State<NPCInput>
{
    private SpellCastingMotor spellCastingProcedure;
    private float _recoveryCountdown;
    private readonly float _chargeTime = 1;
    private float _makeMoveCooldown;
    private float _spellCheckCooldown;
    private NPCInput _ctrl;

    private ActorInput spellTarget;
    private Spell selectedSpell;


    public State_MoveToAndCast()
    {
    }

    public override void Enter(NPCInput ctrl)
    {
        if(ctrl.debugAnimation)
            Debug.Log(ctrl.GetName() + ":<color=cyan>#</color> Cast State Enter");
        _ctrl = ctrl;
        //_ctrl.Combat.OnStagger += () => { ctrl.Combat.Exec_HandleSpell(-1, 0); _castState = CastState.Select; };
        _ctrl = ctrl;
        ctrl.ChangeMovementSpeed(MovementSpeed.Run);
        //self.CombatControllerCallback_CastSpell = Activate;
        //self.CombatControllerCallback_StopSpell = DeactivateProjectile;

        if(spellCastingProcedure == null)
        {
            spellCastingProcedure = new SpellCastingMotor(ctrl);
        }

        ctrl.Combat.Execute_ReadySpells();
    }

    public override void Exit(NPCInput ctrl)
    {
        ctrl.Combat.Execute_SheathSpells();
    }

    public override void Execute(NPCInput ctrl)
    {
        if(ctrl.debugAnimation)
        {
            Debug.Log($"{ctrl.GetName()}: <color=green>Executing spellcasting fsm</color>");
        }

        if(selectedSpell == null)
        {
            EquipBestSpell(ctrl, ref selectedSpell, ref spellTarget);
            
            if(selectedSpell == null)
            {
                if(_ctrl.debugAnimation)
                {
                    Debug.Log($"{_ctrl.GetName()}: <color=#a67541>CSM</color> No effective spell found");
                }
                return;
            }
        }

        if(MoveIntoRange() == false)
            return;
        if(GetLineOfSight() == false)
            return;
        if(FaceTarget() == false)
            return;

        Profiler.BeginSample("CAST STATE: Check isStaggered");
        if(ctrl.Animation.IsIncapacitated() /*|| ctrl.Animation.animator.GetCurrentAnimatorStateInfo(5).IsName("New State") == false*/)
        {
            return;
        }
        Profiler.EndSample();


        if(_recoveryCountdown <= 0)
        {
            if(spellCastingProcedure.CastingDone(spellTarget, selectedSpell))
            {
                _recoveryCountdown = 2;
            }
        }

        if(spellTarget != null && ActorUtility.IsEnemy(spellTarget.ActorStats, ctrl.ActorStats))
            HandleCombatMovement(ctrl);
    }

    private void HandleCombatMovement(NPCInput self)
    {
        _makeMoveCooldown -= Time.deltaTime;

        if(_makeMoveCooldown <= 0)
        {
            if(Vector3.Distance(self.transform.position, spellTarget.transform.position) <= 2)
                _makeMoveCooldown = 0.5f;
            else
                _makeMoveCooldown = UnityEngine.Random.Range(1f, 6f);

            ActorUtility.Navigation.DoSideStep(self, spellTarget);
        }
    }

    private bool FaceTarget()
    {
        bool inFOW = Get.IsInFOV(_ctrl.transform, spellTarget.transform.position, 5);
        if(inFOW == false)
        {
            HelperFunctions.RotateTo(_ctrl.transform, spellTarget.transform.position, 250);
        }

        return inFOW;
    }

    private bool MoveIntoRange()
    {
        bool inRange = Vector3.Distance(_ctrl.transform.position, _ctrl.Combat.GetHostileTarget().transform.position) <= _ctrl.Combat.equippedSpell.activationRange;
        if(inRange == false)
        {
            HelperFunctions.RotateTo(_ctrl.transform, spellTarget.transform.position, 250);
            if(_ctrl.debugAnimation)
                Debug.Log($"{_ctrl.GetName()}: <color=#72CF84>CSM</color> InRange == false");
            _ctrl.SetDestination(spellTarget.transform.position, 0.1f, "", 0);
        }

        return inRange;
    }

    private bool GetLineOfSight()
    {
        float projectileReleaseHeight = 1.5f;
        float projectileRadius = _ctrl.Combat.equippedSpell.GetEffectDiameter() / 2;

        bool lineOfSight = HelperFunctions.LineOfSightH2H(_ctrl.transform, spellTarget.transform.position,
            projectileReleaseHeight, projectileReleaseHeight - projectileRadius);
        if(lineOfSight == false)
        {
            HelperFunctions.RotateTo(_ctrl.transform, _ctrl.NavAgent.steeringTarget, 250);
            if(_ctrl.debugAnimation)
                Debug.Log($"{_ctrl.GetName()}: <color=#72CF84>CSM</color> lineOfSight == false");
            _ctrl.SetDestination(spellTarget.transform.position, 0.1f, "", 0);
        }

        return lineOfSight;
    }

    private void EquipBestSpell(NPCInput ctrl, ref Spell spell, ref ActorInput spellTarget)
    {
        _recoveryCountdown -= Time.deltaTime;
        ctrl.recoveryTimeCallback?.Invoke(_recoveryCountdown, ctrl.recoveryTime); // UI callback

        _spellCheckCooldown -= Time.deltaTime;
        ctrl.skillCheckIntervalCallback?.Invoke(_spellCheckCooldown);
        if(_spellCheckCooldown <= 0 && _recoveryCountdown <= 0)
        {
            
            _spellCheckCooldown = 0.7f;
            spellTarget = _ctrl.Combat.GetHostileTarget();
            spell = SpellCasting.ChooseBestSpell(ctrl, spellTarget);
            //if(spell != null)
            //{
            //    _recoveryCountdown = ctrl.recoveryTime = 0.5f + spell.recoveryTime;

            //    switch(spell.deliveryType)
            //    {
            //        case DeliveryType.Self:
            //            spellTarget = ctrl;
            //            break;
            //        case DeliveryType.Contact:
            //            break;
            //        case DeliveryType.Aimed:
            //        case DeliveryType.TargetActor:
            //        case DeliveryType.TargetLocation:
            //            spellTarget = ctrl.Combat.GetHostileTarget();
            //            break;
            //        default:
            //            break;
            //    }

                ctrl.Combat.SetEquippedSpell(spell);

                //return;
            //}
        }
    }

    public void CancelCasting(NPCInput ctrl)
    {
        ctrl.Animation.Animator.Play("Cancel", 2);
        ctrl.Combat.SetEquippedSpell(null);
        selectedSpell = null;
    }
}


public class SpellCastingMotor
{
    private float _recoveryCountdown;
    private float _concentrationTime = 3f;
    private float _concentrationTimer = 3f;
    private float _preFreezeTimer = 0.5f;
    private float _postFreezeTimer = 0.7f;
    private float _chargeTimer;
    private readonly float _chargeTime = 1;
    private readonly NPCInput ctrl;

    private bool _inChargeIdle;

    private enum CastState
    {
        Prepare,
        Cast,
        ConcentrationCast,
        Cooldown,
        Charge,
        KeepUp
    }

    private CastState _castState = CastState.Prepare;

    public SpellCastingMotor(NPCInput ctrl)
    {
        this.ctrl = ctrl;
        _chargeTimer = _chargeTime;
    }

    // Cast once and flag as done
    public bool CastingDone(ActorInput spellTarget, Spell spell)
    {

        switch(_castState)
        {
            case CastState.Prepare:
                ctrl.Combat.SpellEffectsLaunched = false;
                //if(ctrl.currentSpell != null && ctrl.currentSpell.spellTarget != null)
                //{

                _castState = CastState.Charge;
                //}
                break;
            case CastState.Charge:

                // Agents may charge F&F spells even if they have no LOS or are out of range


                if(ctrl.debugAnimation)
                    Debug.Log($"{ctrl.GetName()}: <color=green>Charging spell</color>");

                if(_chargeTimer == _chargeTime)
                {
                    if(ctrl.debugAnimation)
                        Debug.Log($"{ctrl.GetName()}: <color=#72CF84>Animation callback: Charge Spell</color>");
                    ctrl.Combat.Exec_HandleSpell(0, ctrl.Combat.equippedSpell.spellcastMotionIndex);
                    //self.Execute_ChargeSpell(spell);
                }

                _chargeTimer -= Time.deltaTime;

                if(_chargeTimer <= 0)
                {
                    //_keepUpTimer = Random.Range(0f, 1f);
                    _chargeTimer = _chargeTime;
                    _castState = CastState.Cast;
                    ctrl.Combat.Exec_HandleSpell(1, spell.spellcastMotionIndex);
                    ctrl.Combat.Execute_BlockAggro(2);
                }

                break;
            //case CastState.KeepUp:

            //    // Loop this animation until LOS gain

            //    if(ctrl.debug)
            //        Debug.Log($"{ctrl.GetName()}: <color=green>Keeping spell up</color>");

            //    //_keepUpTimer -= Time.deltaTime;

            //    //if(lineOfSight && inRange && inFOW)
            //    //{
            //        if(ctrl.debug)
            //            Debug.Log($"{ctrl.GetName()}: <color=green>Going to cast spell now</color>");
            //        ctrl.Hold();
            //        ctrl.Combat.Execute_BlockAggro(2);
            //        ctrl.Combat.Exec_HandleSpell(1, spell.spellcastMotionIndex);
            //        _castState = CastState.Cast;
            //    //}
            //    //else
            //    //{
            //    //    _castState = CastState.Prepare;
            //    //    _inChargeIdle = true;
            //    //}

            //    break;

            //? FireForget Spell ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
            case CastState.Cast:

                //! It's important that all spell casting animations fire the 'ActivateSpellEffects',
                //! otherwise ai will be stuck here.
                if(ctrl.debugAnimation)
                    Debug.Log($"{ctrl.GetName()}: <color=green>Magic effects should be all over the place now</color>");

                if(ctrl.Combat.SpellEffectsLaunched)
                {
                    ctrl.Combat.SpellEffectsLaunched = false;
                    switch(ctrl.Combat.equippedSpell.deliveryType)
                    {
                        case DeliveryType.InstantSelf:
                            SpellCasting.InflictMagicEffectsAtActor(ctrl.Combat.equippedSpell, ctrl, ctrl);
                            break;
                        //case DeliveryType.Contact:
                        //    break;
                        case DeliveryType.SeekLocation:
                            SpellCasting.LaunchMagicProjectile(ctrl.Combat.equippedSpell, ctrl, HelperFunctions.GetPredictedPosition(spellTarget.transform.position, ctrl.transform.position, ctrl.GetMovementVelocity(), 12));
                            break;
                        case DeliveryType.InstantActor:
                            SpellCasting.InflictMagicEffectsAtActor(ctrl.Combat.equippedSpell, ctrl, spellTarget);
                            break;
                        case DeliveryType.InstantLocation:
                            break;
                    }
                    ctrl.Combat.equippedSpell.StartCooldown();
                    //ctrl.agent.SetEquippedSpell(null);
                    if(ctrl.debugAnimation)
                        Debug.Log($"{ctrl.GetName()}: <color=#72CF84>Animation callback: Release Spell</color>");

                    return true;
                }
                break;


            //? Concentration Spell ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
            case CastState.ConcentrationCast:
                if(_preFreezeTimer > 0)
                {
                    _preFreezeTimer -= Time.deltaTime;
                }
                else
                {
                    if(_concentrationTimer == _concentrationTime)
                    {
                        _preFreezeTimer = 0.5f;
                        ctrl.HoldPosition();
                        ctrl.Combat.Execute_BlockAggro(2);

                        ctrl.Animation.Animator.SetInteger("iCastMotionIndex", 3);
                        ctrl.Combat.Exec_HandleSpell(0, 3);
                        //self.monoObject.StartCoroutine(CR_InvokeSpellEffectDelayed(ctrl.currentSpell));
                        //! Projectile is activated by animation event
                        _recoveryCountdown = ctrl.recoveryTime = 0.5f + ctrl.Combat.equippedSpell.recoveryTime;
                        switch(ctrl.Combat.equippedSpell.deliveryType)
                        {
                            case DeliveryType.InstantSelf:

                                break;
                            //case DeliveryType.c:
                            //    break;
                            case DeliveryType.SeekLocation:
                                SpellCasting.CastSpraySpellInDirection(ctrl.Combat.equippedSpell, ctrl, spellTarget.transform.position + Vector3.up * 1.6f, 2.5f);
                                break;
                            case DeliveryType.InstantActor:
                                break;
                            case DeliveryType.InstantLocation:
                                break;
                        }

                        if(ctrl.debugAnimation)
                            Debug.Log($"{ctrl.GetName()}: <color=#72CF84>Animation callback: Release Spell</color>");
                    }
                    _concentrationTimer -= Time.deltaTime;

                    if(_concentrationTimer <= 0)
                    {
                        //ctrl.agent.SetEquippedSpell(null);
                        ctrl.Combat.Exec_HandleSpell(-1, 0);
                        _concentrationTimer = _concentrationTime;
                        _castState = CastState.Cooldown;
                    }
                }
                break;

            case CastState.Cooldown:
                if(_postFreezeTimer > 0)
                {
                    _postFreezeTimer -= Time.deltaTime;
                }
                else
                {
                    _postFreezeTimer = Random.Range(0.7f, 2f);
                }
                break;
        }

        return false;
    }

    public void CancelCasting()
    {
        //_castState = CastState.Select;
        _chargeTimer = _chargeTime;
        _preFreezeTimer = 0.2f;
        _concentrationTime = 3;
        _postFreezeTimer = Random.Range(0.7f, 2f);
        ctrl.Animation.Animator.Play("Cancel", 2);
    }
}

public class SpellTargetingProfile
{
    [System.Flags]
    public enum HealFlags
    {
        HEALSELF,
        HEALALLIES,
        HEALNEUTRALS
    }
    public HealFlags HealSpellFlags { get; private set; }

    [System.Flags]
    public enum CombatSpellFlags
    {
        FIRE,
        FROST,
        LIGHTNING,
        POISON,
        DRAINLIFE,
        DRAINMAGICKA
    }
    public CombatSpellFlags PreferredCombatSpellFlags { get; private set; }

    public void SetHealFlags(HealFlags flags)
    {
        HealSpellFlags |= flags;
    }

    public void SetPreferredCombatSpellFlags(CombatSpellFlags flags)
    {
        PreferredCombatSpellFlags |= flags;
    }
}