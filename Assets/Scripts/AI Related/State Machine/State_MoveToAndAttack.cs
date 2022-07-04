using GenericFunctions;
using UnityEngine;
using UnityEngine.AI;

public class State_MoveToAndAttack : State<NPCInput>
{
    private Weapon _weapon;
    private readonly float _attackTime;
    private float _cooldownTimer;
    private float _makeMoveCooldown;
    private readonly float _spellWeaponCooldown; //TODO: Weapon checks should only be done on inventory changes
    private readonly bool _hasMeleeWeapon;
    private bool inWeaponRange;
    private readonly bool _busy;

    // #############
    private float _chargeTime;
    private float _chargeTimer;
    private readonly float _readyTime = 1;
    private float cancelTimer = 2f;
    private float _releaseTime;
    private float _releaseTimer;
    private readonly float _releaseDelayMin = 0.0f;
    private readonly float _releaseDelayMax = 1f;
    private Transform _arrowAnchor;
    private readonly Transform _bowHand;
    private GameObject _projectileObject;
    private readonly Animator _weaponAnimator;
    private readonly NPCInput _ctrl;

    //enum WeaponState
    //{
    //    SetupVisuals,
    //    ExecuteLogic,
    //    DisableVisuals
    //}
    //WeaponState _weaponState;

    private enum AttackState
    {
        Prepare,
        ReadyLoop,
        Attack,
        Cancel
    }

    private AttackState _attackState;
    //###########################

    public State_MoveToAndAttack(NPCInput ctrl)
    {
        _ctrl = ctrl;
    }

    public override void Enter(NPCInput ctrl)
    {
        if(ctrl.debugAnimation)
            Debug.Log(ctrl.GetName() + ":<color=cyan>#</color> Attack State Enter");
        ctrl.Combat.Execute_EquipBestWeapon(Constants.EQUIP_ANY, true, true);
        ctrl.Combat.Execute_DrawWeapon();
        _weapon = ctrl.Equipment.equippedWeapon.Weapon;
        CalculateWeaponVariables(ctrl.Equipment.equippedWeapon, ctrl);
        ctrl.ChangeMovementSpeed(MovementSpeed.Run);
        _attackState = AttackState.Prepare;
    }

    public override void Execute(NPCInput self)
    {
        if(self.Combat.GetHostileTarget() == null)
        {
            return;
        }

        ActorInput attackTarget = self.Combat.GetHostileTarget();
        Vector3 targetPos = attackTarget.transform.position;
        Vector3 targetDir = targetPos - self.transform.position;
        float distToTarget = targetDir.magnitude;

        bool hasLOS = HelperFunctions.LineOfSightH2H(self.transform, targetPos, 1.6f, 1.6f);
        bool inFOV = Get.IsInFOV(self.transform, attackTarget.transform.position, 10) == true;
        inWeaponRange = distToTarget <= _weapon.range;
        bool inCombatRange = distToTarget <= _weapon.range + 2;

        if(inFOV == false)
        {
            // Rotation happens in CombatState
            if(self.debugAnimation)
                Debug.Log(self.GetName() + ":<color=green>#</color> Not in FOV");
            HelperFunctions.RotateTo(self.transform, attackTarget.transform.position, 200);
            hasLOS = false;
        }

        if(_weapon == null)
        {
            if(self.debugAnimation)
                Debug.Log(self.GetName() + ":<color=green>#</color> Weapon null");
            return;
        }

        if(inWeaponRange == false)
        {
            if(self.debugAnimation)
                Debug.Log(self.GetName() + ":<color=green>#</color> Not in attack range");

            self.SetDestination(targetPos, _weapon.range);
        }

        self.ChangeMovementSpeed(inCombatRange ? MovementSpeed.Walk : MovementSpeed.Run);

        _cooldownTimer -= Time.deltaTime;

        if(inFOV && inWeaponRange && _cooldownTimer <= 0 && self.Combat.CanAttack())
        {
            switch(_weapon.weaponCategory)
            {
                case WeaponCategory.Unarmed:
                case WeaponCategory.Dagger:
                case WeaponCategory.LongSword:
                case WeaponCategory.ShortSword:

                    switch(_attackState)
                    {
                        case AttackState.Prepare:

                            if(self.debugAnimation)
                                Debug.Log(self.GetName() + ":<color=green>#</color> State: Preparing attack");
                            if(self.Equipment.WeaponInHand() == false)
                            {
                                Debug.Log(self.GetName() + ":<color=red>#</color> State: No weapon in hand");
                                return;
                            }
                            _attackState = AttackState.Attack;
                            self.Animation.Animator.SetInteger("iAttackVariant", Random.Range(0, 11));

                            break;

                        case AttackState.Attack:

                            if(_ctrl.Animation.isEvading)
                            {
                                Debug.Log(self.GetName() + ":<color=yellow>#</color> State: Evading");
                                return;
                            }

                            if(self.debugAnimation)
                                Debug.Log(self.GetName() + ":<color=green>#</color> State: Attack");

                            if(_releaseTimer == _releaseTime) // Stuff that should be done once at release start
                            {
                                if(self.debugAnimation)
                                    Debug.Log(self.GetName() + ":<color=green>#</color> State: Attack Swing");

                                bool doCombo = Random.value > 0.5f;
                                if(doCombo)
                                    _releaseTimer = 2f;
                                self.Animation.Animator.SetBool("bCombo", doCombo);


                                self.Combat.Execute_Attack();
                                // ctrl.SetDestination(ctrl.transform.position + ctrl.transform.forward * 0.3f, 0.2f);
                            }

                            _releaseTimer -= Time.deltaTime;

                            if(_releaseTimer <= 0)
                            {
                                _releaseTimer = _releaseTime;
                                //ctrl.agent.navAgent.isStopped = false;
                                _attackState = AttackState.Prepare;
                                _cooldownTimer = Random.Range(0.2f, 1.5f);
                                //ctrl.agent.navAgent.isStopped = false;
                            }

                            break;
                        case AttackState.Cancel:
                            if(self.debugAnimation)
                                Debug.Log(self.GetName() + ":<color=green>#</color> State: Cancel");
                            self.Animation.Animator.Play("Cancel", 1);
                            //ctrl.agent.navAgent.updatePosition = true;
                            break;
                    }
                    break;
                case WeaponCategory.XBow:
                case WeaponCategory.Shortbow:
                case WeaponCategory.Longbow:

                    switch(_attackState)
                    {
                        case AttackState.Prepare:


                            if(_chargeTime == _chargeTimer) // Stuff that should be done once at draw start
                            {
                                self.Animation.PlayMotion_HandleBow(0);
                                _ctrl.Equipment.equippedWeapon.Animator.CrossFade("Draw", 0.2f, 0);
                            }

                            _chargeTimer -= Time.deltaTime;

                            if(_ctrl.Animation.InBowAimingLoop())
                            {
                                _chargeTimer = _chargeTime;
                                _attackState = AttackState.ReadyLoop;
                            }


                            break;
                        case AttackState.ReadyLoop:

                            cancelTimer -= Time.deltaTime;

                            if(hasLOS && inFOV)
                            {
                                _ctrl.HeadTracking.SetIsAimingRangedWeapon(true);

                                _attackState = AttackState.Attack;
                            }
                            else
                            {
                                if(cancelTimer <= 0)
                                {
                                    CancelAiming();
                                    _attackState = AttackState.Prepare;
                                }
                            }

                            break;
                        case AttackState.Attack:

                            //ctrl.agent.navAgent.velocity = Vector3.zero;

                            if(self.Animation.IsIncapacitated() || attackTarget == null || attackTarget.dead)
                            {
                                CancelAiming();
                                _attackState = AttackState.Prepare;
                                return;
                            }

                            self.Animation.PlayMotion_HandleBow(1);

                            _ctrl.Equipment.equippedWeapon.Animator.Play("Cancel", 0);
                            //ShootProjectile(4, attackTarget);

                            _attackState = AttackState.Prepare;
                            _cooldownTimer = Random.Range(0.2f, 1.5f);

                            break;
                    }

                    break;
            }
            //_cooldownTimer = 0.1f;
        }

        _makeMoveCooldown -= Time.deltaTime;

        //if(_weapon.range > 10)
        //{
        if(inWeaponRange && attackTarget != null && _makeMoveCooldown <= 0)
        {
            _makeMoveCooldown = UnityEngine.Random.Range(1f, 4f);

            ActorUtility.Navigation.DoSideStep(self, attackTarget);

            //Debug.Log($"{ctrl.agent.GetName()}:<color=orange>#</color> Attempting combat move");
        }
        //else
        //{
        //    Debug.Log($"{ctrl.agent.GetName()}:<color=orange>#</color><color=red>#</color> Combat move impossible");
        //}
        //}
    }

    public override void Exit(NPCInput ctrl)
    {
        ctrl.Combat.Execute_SheathWeapon();
    }

    private void CancelAiming()
    {
        if(_ctrl.debugAnimation)
            Debug.Log(_ctrl.GetName() + ":<color=orange>#</color> Canceling aiming");
        _ctrl.HeadTracking.SetIsAimingRangedWeapon(false);
        _ctrl.Animation.PlayMotion_HandleBow(-1);
        _ctrl.Equipment.equippedWeapon.Animator.Play("Cancel", 0);
    }

    private void CalculateWeaponVariables(WeaponData weapon, ActorInput actorInput)
    {
        actorInput.Animation.Animator.SetFloat("fWeaponSpeed", weapon.Weapon.speed);

        switch(weapon.Weapon.weaponCategory)
        {
            case WeaponCategory.Unarmed:
            case WeaponCategory.Dagger:
            case WeaponCategory.LongSword:
            case WeaponCategory.ShortSword:
                _chargeTimer = _chargeTime = 0f;
                _releaseTimer = _releaseTime = 1f;
                break;
            //case WeaponCategory.SwordAndShield:
            //    break;
            //case WeaponCategory.Dual:
            //    break;
            //case WeaponCategory.TwoHanded:
            //    break;
            case WeaponCategory.Longbow:
            case WeaponCategory.Shortbow:
            case WeaponCategory.XBow:
                _chargeTimer = _chargeTime = 2f;
                _releaseTimer = _releaseTime = 0.35f;
                _arrowAnchor = weapon.WeaponObject.transform.FindDeepChild("arrow anchor");
                _ctrl.Combat.OnHandleBow = ShootProjectile;
                //_attackTime = 1.1f;
                break;
            default:
                break;
        }
    }

    private void ShootProjectile(int index/*, ActorInput attackTarget*/)
    {
        if(_ctrl.Combat.GetHostileTarget() == null)
        {
            return;
        }


        ActorInput attackTarget = _ctrl.Combat.GetHostileTarget();
        Vector3 attackerDir = attackTarget.transform.position - _ctrl.transform.position;

        switch(index)
        {
            // Bow
            case 1: // Hand on quiver

                //if(_projectileObject != null && _projectileObject.transform.position == _arrowAnchor.transform.position)
                //{
                //    DestroyProjectile();
                //}

                //_projectileObject = ItemDatabase.InstantiatePhysicalItem("iron arrow", _ctrl.Equipment.m_weaponHand);
                //_projectileObject.transform.localEulerAngles -= new Vector3(90, 0, 0);
                //SetParent(_projectileObject.transform, _ctrl.rightHandAnchor);
                break;
            case 2: // Nocking arrow

                //if(_projectileObject != null)
                //    SetParent(_projectileObject.transform, _arrowAnchor);


                //_weaponAnimator.SetTrigger("tDraw");
                break;
            case 3: // Aiming idle start
                break;
            case 4: // Release
                //MonoBehaviour.Destroy(_projectileObject);
                GameObject vfxObj = PoolSystem.GetPoolObject("arrow visual", ObjectPoolingCategory.VFX);
                Debug.Log("<color=green>>></color> Enabled object '" + vfxObj.name + "'");
                //Debug.Log(spellAnchor.name);
                VFXPlayer.TriggerVFX(vfxObj, _arrowAnchor.position, Quaternion.identity, 2);

                Projectile _projectile = vfxObj.GetComponent<Projectile>();

                //_projectile.OnImpact = () =>
                //{
                //    //if(sfxOnHit != null)
                //    //    AgentSFXManager.TriggerSFX(sfxOnHit[Random.Range(0, sfxOnHit.Length)], _projectile.transform.position);
                //    //if(duration > 1)
                //    //    spellTarget.Execute_ApplyStatusEffect(new StatusEffect_Damage(1, false, 1, duration, 1));
                //};
                if(_projectile == null)
                {
                    Debug.LogError("Arrow visual not found");
                }
                //_projectile.Init(null, ProjectileType.Lobber);
                if(_weapon == null)
                {
                    Debug.Log(_ctrl.GetName() + ":<color=red> Weapon = null</color>");
                }
                _projectile.LaunchWithArc(_arrowAnchor.position, _ctrl,
                    attackTarget.Animation.head.position + Random.insideUnitSphere * 0.1f,
                    ActorMeshEffectType.None,
                    DeliveryType.SeekLocation, true,
                    30, _weapon.damage, 0,  20f);
                break;
            case 5: // Cancel Bow Aim: Animation start
                //_weaponAnimator.SetTrigger("tRelease");
                break;
            case 6: // Cancel Bow Aim: Arrow to hand
                //if(_projectileObject != null)
                //{
                //    SetParent(_projectileObject.transform, _ctrl.Equipment.m_weaponHand);
                //}
                break;
            case 7: // Cancel Bow Aim: Arrow deletion/holstering to quiver
                //if(_projectileObject != null)
                //{
                //    MonoBehaviour.Destroy(_projectileObject);
                //}
                break;
        }
    }

    private void DestroyProjectile()
    {
        if(_projectileObject != null)
        {
            if(_weaponAnimator != null)
            {
                _weaponAnimator.SetTrigger("tRelease");
            }
            MonoBehaviour.Destroy(_projectileObject);
        }
    }

    
}
