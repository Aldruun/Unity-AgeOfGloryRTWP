using AoG.Core;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Profiling;

[System.Serializable]
public class ActorCombat : MonoBehaviour, IAttackable
{
    public Spell equippedSpell;
    public bool WeaponDrawn { get; private set; }
    public bool SpellDrawn { get; private set; }
    public bool SpellEffectsLaunched { get; internal set; }
    public bool IgnorePlayerAttack { get => actorInput.IsAlly; }
    public AttackableType AttackableType { get; set; }


    public System.Action<Vector3, float, bool> OnKnockDown; // force direction, duration, markDead
    //public System.Action<Vector3, float> OnKnockDown;
    public System.Action<Vector3> OnCollapse; // force direction
    private System.Action<ActorInput> OnDeath;
    public System.Action<ActorInput, float, DamageType, bool> OnHit; // source, damage, damageType, hitSuccess
    public System.Action<float> OnEvade;
    public System.Action OnStandUp;
    public System.Action InvokeMagicEffects;
    public System.Action<Spell> OnChargeSpell;
    public System.Action OnReadySpells;
    public System.Action<int> OnSheathSpell; // SpellHand
    public System.Action<int> OnHandleBow; // index set on fbx animation event
    public System.Action<AnimationSet> OnDrawWeapon; // motionIndex
    public System.Action<AnimationSet> OnSheathWeapon; // motionIndex
    public System.Action<AnimationSet> OnStartAttack; // motionIndex
    //! For animator
    public Action OnReleaseSkill; // stage, motionIndex

    public System.Action<float> OnBlockAggroRequest;

    private ActorInput actorInput;
    private ActorAnimation Animation;
    private ActorEquipment Equipment;
    private ActorInput[] targets;
    private Collider[] populatedHitTargetArray;
    private AudioSource voiceAudioSource;
    // private VisualEffect currentWeaponTrail;
    private System.Action<float, float> onHealthChanged; //newCurrent, currentMmax
    private System.Action<ActorStats> onStunChanged;
    //private bool hitSuccess;
    //private bool isBlocking;
    private GameObject _handSpellVFX;
    private CharacterVoiceSet characterVoiceSet;

    internal LayerMask LineOfSightLayerMask;
    private ActorStats stats;
    private Coroutine pushCoroutine;
    //public bool LaunchArrowNow { get; private set; }

    public virtual void Initialize(ActorInput actorInput, ActorStats stats, ActorEquipment equipment, ActorAnimation actorAnimation, CharacterVoiceSet characterVoiceSet, AudioSource voiceAudioSource)
    {
        this.actorInput = actorInput;
        this.stats = stats;
        this.characterVoiceSet = characterVoiceSet;
        Animation = actorAnimation;
        this.Equipment = equipment;

        targets = new ActorInput[2];

        this.voiceAudioSource = voiceAudioSource;

        populatedHitTargetArray = new Collider[10];

        InitializeAttackableInterface();
    }

    public void SetHostileTarget(ActorInput actorInput)
    {
        targets[0] = actorInput;
    }

    public void SetFriendlyTarget(ActorInput actorInput)
    {
        targets[1] = actorInput;
    }

    public ActorInput GetHostileTarget()
    {
        return targets[0];
    }

    public ActorInput GetFriendlyTarget()
    {
        return targets[1];
    }


    public void ApplyDamage(ActorInput source, Weapon weapon, EffectData magicEffect, bool isProjectile, bool hitSuccess = true)
    {
        float finalAmount = 0;
        bool isUnarmedHit = false;
        DamageType damageType = DamageType.SLASHING;
        if(weapon != null)
        {
            damageType = weapon.damageType;
            isUnarmedHit = weapon.animationPack == AnimationSet.UNARMED;
            finalAmount += weapon.damage;
        }

        if(finalAmount == 0)
        {
            Debug.Log("ActorInput.ApplyDamage: Weapon and magic effect value = 0");
        }

        if(Animation.isBlocking)
        {
            SFXPlayer.PlaySound_AttackBlocked(transform.position);
            //Animation.PlayMotion_StopBlocking();
            return;
        }

        if(hitSuccess) //! Currently only failes if target is out of range
        {
            pushCoroutine = StartCoroutine(CR_PushBack(source, actorInput));
            //UIFloatingInfoManager.current.CreatePopup(transform.position, finalAmount.ToString(), 5, 1, 3);

            //if(Animation.inBleedOutState == false)
            //{
            //    ActorUtility.ModifyActorStun(actorInput.ActorRecord, -(int)finalAmount / 2, ModType.ADDITIVE);
            //    if(ActorUtility.GetModdedAttribute(actorInput.ActorRecord, ActorAttribute.STUN) < 1)
            //    {
            //        Animation.PlayMotion_BleedOut(0);
            //        ActorUtility.ModifyActorStun(actorInput.ActorRecord, 1, ModType.ADDITIVE);
            //    }
            //    Callback_OnStunChanged();
            //}

            //! No worries. The stagger function takes care of the bleedout hit
            bool staggerSuccess = UnityEngine.Random.value > 0.3f;

            if(staggerSuccess)
            {
                Animation.PlayMotion_Stagger();
            }

            //finalAmount *= GameStateManager.gameSettings.globalDmgMult;

            if(finalAmount > 0)
            {
                finalAmount = Mathf.CeilToInt(finalAmount);
            }

            ActorUtility.ModifyActorHealth(actorInput.ActorStats, -(int)finalAmount, ModType.ADDITIVE);
            Callback_OnHealthChanged();

            ActivateOnHitCircus(source, weapon, magicEffect, isProjectile);

            OnHit?.Invoke(source, finalAmount, damageType, true);
            //if(source != null && source.ActorRecord.faction == Faction.Heroes)
            //{
            //    int baseExp = (int)(actorInput.ActorRecord.level <= source.ActorRecord.level ? ((actorInput.ActorRecord.level * 5) + 45) : ((actorInput.ActorRecord.level * 5) + 45) * (1 + (0.05f * (source.ActorRecord.level - actorInput.ActorRecord.level))));
            //    float perc = finalAmount / ActorUtility.GetModdedAttribute(actorInput.ActorRecord, ActorAttribute.MAXHEALTH);
            //    float result = perc * baseExp;
            //    source.ActorRecord.Execute_ModifyCurrentXP(result, ModType.ADDITIVE);
            //}
            //m_states[Symbols.LOW_HEALTH] = m_currHealth < 50;

            if(actorInput.dead)
            {
                StopCoroutine(pushCoroutine);
                SFXPlayer.ActorSFX.VerbalConstant(characterVoiceSet, voiceAudioSource, VerbalConstantType.DEAD);

                foreach(NPCInput summoned in actorInput.summonedCreatures)
                {
                    if(summoned == null || summoned.dead)
                    {
                        continue;
                    }

                    summoned.Combat.Kill();
                }

                if(actorInput.ActorStats.HasActorFlag(ActorFlags.ESSENTIAL) == false)
                {
                    //OnDestroyed?.Invoke(this, source, Vector3.zero);
                    Debug.Log(actorInput.GetName() + ":<color=orange> DEAD</color>");
                    gameObject.layer = LayerMask.NameToLayer("Corpses");
                    Vector3 collapseForce = source ? source.transform.forward : -transform.forward;
                    Animation.CollapseDead(source, collapseForce);

                    if(actorInput.ActorStats.HasActorFlag(ActorFlags.ALLY))
                    {
                        GameEventSystem.RequestRemovePortrait?.Invoke(actorInput.PartyIndex);
                    }

                    GameEventSystem.RequestAddGarbage(gameObject);
                    actorInput.spawnpoint.StartRespawnCountdown();

                    Callback_ActorDied();
                }
                else
                {
                    Execute_KnockDown(Vector3.zero, 5, true);
                    ActorUtility.ModifyActorHealth(actorInput.ActorStats, 1, ModType.ABSOLUTE);
                }
            }
            else
            {
                SFXPlayer.ActorSFX.VerbalConstant(characterVoiceSet, voiceAudioSource, VerbalConstantType.HURT);
            }
        }
    }

    public void Kill()
    {
        gameObject.layer = LayerMask.NameToLayer("Corpses");
        Animation.CollapseDead(null, Vector3.zero);
        GameEventSystem.RequestAddGarbage(gameObject);
        Callback_ActorDied();
    }


    #region OnHit Feedback
    private IEnumerator CR_PushBack(ActorInput source, ActorInput pushed)
    {
        float time = 0.2f;

        while(time > 0)
        {
            //Debug.Assert(actorInput != null, "actorInput null");
            //Debug.Assert(actorInput.NavAgent != null, "actorInput.NavAgent null");
            //if(actorInput.NavAgent.enabled == false)
            //{
            //    yield break;
            //}

            time -= Time.deltaTime;

            if(pushed.IsPlayer)
                pushed.cc.Move(source.transform.forward /** 0.5f*/ * Time.deltaTime);
            else
                pushed.NavAgent.Move(source.transform.forward /** 0.5f*/ * Time.deltaTime);
            yield return null;
        }
    }

    private void ActivateOnHitCircus(ActorInput source, Weapon weapon, EffectData magicEffect, bool isProjectile)
    {
        DamageType damageType = DamageType.SLASHING;

        Vector3 dir = source != null ? source.transform.position - transform.position : transform.forward;
        dir.y = 0;
        Vector3 victimHitpoint = GetAttackPoint().position;
        Vector3 finalHitPosition = victimHitpoint;

        if(weapon != null)
        {
            damageType = weapon.damageType;
            finalHitPosition = victimHitpoint + (source.Animation.head.transform.position - victimHitpoint) / 3;
        }
        else if(magicEffect != null)
        {
            damageType = magicEffect.damageType;
            //    if(magicEffect.ID == "effect_fireball")
            //    {
            //        TriggerVFX(PoolSystem.GetPoolObject("vfx_hit_fire", ObjectPoolingCategory.VFX), victim.monoObject.GetComponentInChildren<Collider>().bounds.center, Quaternion.identity);
            //        return;
            //    }
        }
        else
        {
            return;
        }

        VFXPlayer.PlayVFX_OnHit(finalHitPosition, dir, damageType);
        SFXPlayer.PlaySound_OnHit(finalHitPosition, damageType, weapon.weaponType == WeaponType.Unarmed);
        //actorInput.ActorMeshFlashHandler.Flash(new Color(1, 1, 1, 0.4f), 0.15f);

        actorInput.ActorUI.Flash(0.2f);
            
    }

    #endregion OnHit Feedback End

    private bool IsAlly(ActorInput actor)
    {
        return actor.ActorStats.HasActorFlag(ActorFlags.ALLY) && actorInput.ActorStats.HasActorFlag(ActorFlags.ALLY);
    }

    public void Execute_ChargeSpell(Spell spell)
    {

        //Debug.Log(gameObject.name + ": Drawing weapon");
        Animation.PlaySpellAnimation(spell.spellcastMotionIndex);
        //equippedWeaponRight.OnDraw();
    }

    public void SetEquippedSpell(Spell spell)
    {
        if(spell == null)
        {
            Debug.Log(actorInput.GetName() + "<color=red>: SetEquippedSpell: NULL</color>");
        }

        if(spell == equippedSpell)
        {
            Debug.Log(actorInput.GetName() + "<color=yellow>: SetEquippedSpell: Spell '" + spell.Name + "' already equipped</color>");
            return;
        }

        equippedSpell = spell;

        if(equippedSpell != null && SpellDrawn)
        {
            Animation.PlayMotion_SwitchSpell();
            VFXPlayer.RemoveHandVFX(_handSpellVFX);
            _handSpellVFX = VFXPlayer.SetSpellHandVFX(Equipment.spellAnchor, equippedSpell.magicEffectsData[0].vfxid_handidle);
        }
    }

    public void Execute_ReadySpells()
    {
        if(SpellDrawn)
        {
            return;
        }

        if(equippedSpell != null)
        {
            SpellDrawn = true;
            _handSpellVFX ??= VFXPlayer.SetSpellHandVFX(Equipment.spellAnchor, equippedSpell.magicEffectsData[0].vfxid_handidle);
            SFXPlayer.PlaySound_DrawSpell(equippedSpell, transform.position);
            Animation.PlayMotion_ReadySpells();
        }
    }

    public void Execute_SheathSpells()
    {
        if(SpellDrawn == false)
        {
            return;
        }
        SpellDrawn = false;

        Animation.PlayMotion_SheathSpell();
        VFXPlayer.RemoveHandVFX(_handSpellVFX);
        _handSpellVFX = null;
        SFXPlayer.PlaySound_SheathSpell(equippedSpell, transform.position);
        //}
    }

    public void Exec_HandleSpell(int stage, int motionIndex) // 0 = left, 1 = right
    {
        if(stage == 0 && Animation.Animator.GetCurrentAnimatorStateInfo(2).IsName("New State") == false)
        {
            return;
        }
        // stage: 0 = charge, 1 = release
        //if(debug)
        //    Debug.Log(GetName() + ": ### " + (stage == 0 ? "charging" : "releasing") + (" right") + " hand spell");
        Animation.PlaySpellAnimation(motionIndex);
    }

    public void Execute_UnequipArmor(Armor armor, bool silent = false)
    {
        actorInput.Equipment.UnequipArmor(armor);
        ActorUtility.ModifyStatModded(actorInput.ActorStats, ActorStat.AC, -armor.AC, ModType.ADDITIVE);
        //Debug.Log("<color=grey>Dress root object '" + dressRootObject.name + "' being processed</color>");
        //DressUpManager.UnequipArmor(targetSlot);

        //actorInput.character.OnArmorUnequipped?.Invoke(armor, true, silent == false);
    }

    public Weapon GetEquippedWeapon()
    {
        return Equipment.equippedWeapon.Weapon;
    }

    public virtual void Execute_EquipBestWeapon(int WEAPON_TYPE, bool playAnimation, bool playSound)
    {
        if(actorInput.debugGear)
            Debug.Log(actorInput.GetName() + ":<color=orange>2</color> ActorInput.Execute_EquipBestWeapon");
        Weapon weapon = Equipment.EquipBestWeapon(WEAPON_TYPE);
        if(weapon == null)
        {
            Debug.Log(actorInput.GetName() + ":<color=yellow> Execute_EquipBestWeapon failed</color>");
            return;
        }
        //SetStatMod(Stat.APR, GetBaseStat(Stat.APR) + weapon.bonusAPR);
        //OnRequestWeaponDrawEffects?.Invoke(weapon.weaponCategory, true, playAnimation, playSound);

        //actorInput.character.OnDrawWeapon?.Invoke(weapon.animationPack);
    }

    public void Execute_DrawWeapon()
    {
        if(WeaponDrawn)
        {
            if(actorInput.debugGear)
                Debug.LogError(actorInput.GetName() + ":<color=red>*</color> ActorInput.Execute_DrawWeapon: weaponObject already drawn");
            return;
        }

        if(Equipment.equippedWeapon.Weapon == null)
        {
            if(actorInput.debugGear)
                Debug.LogError(actorInput.GetName() + ":<color=red>*</color> ActorInput.Execute_DrawWeapon: weaponObject = null");
            return;
        }

        if(actorInput.debugGear)
            Debug.Log(actorInput.GetName() + "<color=green>*</color>: ActorInput.Execute_DrawWeapon");

        SetWeaponDrawn(true);
        Animation.PlayMotion_DrawWeapon(GetEquippedWeapon().animationPack);
        StartCoroutine(CR_ParentWeaponToHand());
    }

    private IEnumerator CR_ParentWeaponToHand()
    {
        yield return new WaitForSeconds(0.5f);
        Equipment.ParentWeaponToHand();
        SFXPlayer.PlaySound_DrawWeapon(GetEquippedWeapon().weaponCategory, transform.position);
    }

    public void Execute_SheathWeapon()
    {
        if(WeaponDrawn == false)
        {
            if(actorInput.debugGear)
                Debug.LogError(actorInput.GetName() + ":<color=red>*</color> ActorInput.Execute_DrawWeapon: weaponDrawn == false");
            return;
        }

        if(Equipment.equippedWeapon.Weapon == null)
        {
            if(actorInput.debugGear)
                Debug.LogError(actorInput.GetName() + ":<color=red>*</color> ActorInput.Execute_DrawWeapon: weaponObject = null");
            return;
        }

        if(actorInput.debugGear)
            Debug.Log(actorInput.GetName() + ":<color=green>*</color> ActorInput.Execute_SheathWeapon");
        //OnSheathWeapon?.Invoke((int)gearData.equippedWeapon.weaponData.weaponCategory);

        SetWeaponDrawn(false);
        Animation.PlayMotion_SheathWeapon(GetEquippedWeapon().animationPack);
        StartCoroutine(CR_ParentWeaponToHolster());
    }

    private IEnumerator CR_ParentWeaponToHolster()
    {
        yield return new WaitForSeconds(0.5f);
        Equipment.ParentWeaponToHolster();
        SFXPlayer.PlaySound_SheathWeapon(GetEquippedWeapon().weaponCategory, transform.position);
    }

    private void SetWeaponDrawn(bool on)
    {
        if(actorInput.debugGear)
            Debug.Log(actorInput.GetName() + ":<color=cyan>*</color> Setting weapon drawn = '" + on + "'");
        WeaponDrawn = on;
    }

    public virtual void Execute_Attack()
    {
        //if(Animation.isBlocking)
        //{
        //    Animation.PlayMotion_StopBlocking();
        //}

        if(Animation.IsIncapacitated())
        {
            return;
        }

        if(WeaponDrawn == false)
        {
            Debug.Log(actorInput.GetName() + ":<color=red>*</color> ActorInput.ExecuteAttack(): Weapon not drawn");
            return;
        }
        //hitSuccess = true;
        //if(actorInput.debugGear)
        //    Debug.Log(actorInput.GetName() + ":<color=cyan>*</color> ActorInput.Execute_Attack");
        Animation.PlayMotion_Attack(GetEquippedWeapon().animationPack);
        if(GetHostileTarget() != null)
            GetHostileTarget().Combat.SignalIncomingMeleeAttack(GetEquippedWeapon(), transform.position);

        StartCoroutine(CR_DelayAttackEffects());

    }

    private IEnumerator CR_DelayAttackEffects()
    {
        yield return new WaitForSeconds(0.3f);

        if(actorInput.dead /*|| GetHostileTarget() == null || GetHostileTarget().dead*/)
        {
            Debug.Log("<<<<<<<<<<<<<<<<<<<<<<<<<<<<<< Delay failed");
            yield break;
        }
        SFXPlayer.ActorSFX.VerbalConstant(characterVoiceSet, voiceAudioSource, VerbalConstantType.ATTACK, 0.2f);
        SFXPlayer.PlaySound_Swing(transform.position, Equipment.equippedWeapon.Weapon.weaponCategory);
        // hitSuccess = GetHostileTarget().Animation.isAttacking || GetHostileTarget().isDowned || GetHostileTarget().ActorRecord.inSpellChargeLoop ? true : Random.value < 0.7f;
        // if(hitSuccess == false)
        //     GetHostileTarget().Combat.Animation.PlayMotion_Evade(1f);
    }

    private void SignalIncomingMeleeAttack(Weapon weapon, Vector3 attackerPosition)
    {
        if(actorInput.ActorStats.isPlayer)
        {
            return;
        }

        //Do I want to block?
        float angel = Vector3.Angle(transform.forward, attackerPosition - transform.position);
        if(Mathf.Abs(angel) > 50)
            return;

        //bool block = UnityEngine.Random.value > 0.5f && GetEquippedWeapon() != null && GetEquippedWeapon().IsRanged == false
        //    && weapon != null && weapon.IsRanged == false;
        //if(block)
        //    Animation.PlayMotion_Block(weapon.animationPack);
    }

    internal bool CanAttack()
    {
        return true;
    }

    public void Execute_KnockDown(Vector3 force, float duration, bool temporarilyMarkDead = false)
    {
        if(actorInput.isDowned)
        {
            return;
        }
        actorInput.isDowned = true;
        OnKnockDown?.Invoke(force, duration, temporarilyMarkDead);
    }

    public void Execute_StandUp()
    {
        OnStandUp?.Invoke();
    }

    public virtual Transform GetAttackPoint()
    {
        return Animation.head;
    }

    public void Execute_BlockAggro(float duration)
    {
        OnBlockAggroRequest?.Invoke(duration);
    }

    //! FBX Importer Animation Events

    #region Animation Importer Events

    //public void DrawWeapon()
    //{
    //    Equipment.ParentWeaponToHand();
    //}

    //public void SheathWeapon()
    //{
    //    Equipment.ParentWeaponToHolster();
    //}

    public void ReleaseSpell()
    {
        OnReleaseSkill?.Invoke();
    }

    /// <summary>
    /// Called by animation event.
    /// Used for melee attacks and applies damage to attackTarget instantly.
    /// Standard ranged attacks use this to initiate the projectile.
    /// </summary>
    public void Hit()
    {
        if(actorInput.debugAnimation)
            Debug.Log(gameObject.name + ":<color=cyan>*</color> Hit procedure");

        if(Equipment.equippedWeapon == null)
        {
            if(actorInput.debugAnimation)
                Debug.Log($"{actorInput.GetName()}: Applying damage failed: Equipped right hand weapon = null");
        }
        else
        {
            if(Animation.IsIncapacitated())
            {
                if(actorInput.debugAnimation)
                    Debug.Log(gameObject.name + ": <color=yellow>*</color>Can't hit anything -> Incapacitated");
                return;
            }

            Weapon weapon = GetEquippedWeapon();
            if(weapon.projectileIdentifier != null)
            {

                GameObject vfxObj = PoolSystem.GetPoolObject(weapon.projectileIdentifier, ObjectPoolingCategory.VFX);

                Projectile _projectile = vfxObj.GetComponent<Projectile>();

                if(_projectile == null)
                    Debug.LogError("ActorMonoObject: Projectile visual not found");

                _projectile.LaunchStraight(Equipment.m_weaponHand.position, actorInput, transform.forward, ActorMeshEffectType.None, DeliveryType.SeekActor, true, 10, 5, 0, 0, 0, false);
            }
            else
            {
                int numFound = Physics.OverlapSphereNonAlloc(transform.position, weapon.range, populatedHitTargetArray, GameInterface.Instance.DatabaseService.GameSettings.agentLayers);

                if(numFound == 0)
                {
                    //if(actorInput.debugAnimation)
                    //    Debug.Log(gameObject.name + ": <color=orange>*</color>No melee hit targets found");
                    return;
                }

                int maxTargets = weapon.maxHitTargets;

                for(int i = 0; i < numFound; i++)
                {
                    IAttackable targetHit = populatedHitTargetArray[i].GetComponent<IAttackable>();
                    if(targetHit.GetTransform() == transform || (actorInput.IsPlayer && targetHit.AttackableType == AttackableType.PC)
                        /*|| (actorInput.IsAlly && (targetHit.AttackableType == AttackableType.PC || targetHit.AttackableType == AttackableType.PC))*/)
                    {
                        //if(actorInput.debugAnimation)
                        //    Debug.Log(gameObject.name + ": <color=orange>*</color>Target is me! Skipping!");
                        continue;
                    }

                    if(maxTargets <= 0)
                    {
                        break;
                    }

                    maxTargets--;

                    Vector3 directionToTarget = targetHit.GetTransform().position - transform.position;
                    float angle = Vector3.Angle(transform.forward, directionToTarget);
                    //float distance = directionToTarget.magnitude;

                    if(Mathf.Abs(angle) < 50/* && distance < 10*/)
                        targetHit.ApplyDamage(actorInput, weapon, null, false, true);
                }
            }
        }
    }

    private void ActivateWeaponTrail(AnimationEvent animationEvent)
    {
        if(Equipment.equippedWeapon.WeaponTrailVFX == null)
        {
            //Debug.LogError("Weapon trail null");
            return;
        }

        float xAngle = animationEvent.floatParameter;
        bool doStab = xAngle < 0;


        Transform vfxTransform = Equipment.equippedWeapon.WeaponTrailVFX.transform;

        if(doStab)
        {
            Vector3 weaponPos = Equipment.equippedWeapon.WeaponObject.transform.position;
            vfxTransform.position = new Vector3(weaponPos.x, weaponPos.y, transform.position.z);
            Equipment.equippedWeapon.WeaponTrailVFX.SendEvent("PlayStab");
        }
        else
        {
            int zAngle = animationEvent.intParameter;
            Vector3 weaponDir = Equipment.equippedWeapon.WeaponObject.transform.position - transform.position;
            vfxTransform.localPosition = new Vector3(0, Animation.chest.position.y, 0);
            vfxTransform.localEulerAngles = new Vector3(xAngle, Quaternion.LookRotation(weaponDir).y * 0.5f, zAngle);
            Equipment.equippedWeapon.WeaponTrailVFX.SendEvent("PlaySlash");
        }

        // Equipment.equippedWeapon.WeaponTrailVFX.Play();
    }

    public void ActivateSpellEffects()
    {
        if(equippedSpell == null)
        {
            Debug.Log("ActorMonoObject.ActivateSpellEffects(): ActivateSpellEffects - Equipped spell is null");
            return;
        }

        SpellEffectsLaunched = true;
    }

    public void WeaponEvent(int index)
    {
        switch(index)
        {
            // Bow
            case 1: // Hand on quiver
            case 2: // Nocking arrow
            case 3: // Fully drawn
            case 4: // Release
                OnHandleBow?.Invoke(index);
                //LaunchArrowNow = true;
                //HandleBow(4, attackTarget);
                break;
        }
    }

    #endregion Animation Importer Events #RegionEnd

    #region Combat Callbacks
    public void RegisterCallback_OnHealthChanged(System.Action<float, float> method) => onHealthChanged += method;
    public void UnregisterCallback_OnHealthChanged(System.Action<float, float> method) => onHealthChanged -= method;
    public void Callback_OnHealthChanged() => onHealthChanged?.Invoke(ActorUtility.GetModdedAttribute(actorInput.ActorStats, ActorStat.HEALTH),
        ActorUtility.GetModdedAttribute(actorInput.ActorStats, ActorStat.MAXHEALTH));
    public void RegisterCallback_OnStunChanged(System.Action<ActorStats> method) => onStunChanged += method;
    public void UnregisterCallback_OnStunChanged(System.Action<ActorStats> method) => onStunChanged -= method;
    public void Callback_OnStunChanged() => onStunChanged?.Invoke(actorInput.ActorStats);
    public void RegisterCallback_OnDeath(System.Action<ActorInput> method) => OnDeath += method;
    public void UnregisterCallback_OnDeath(System.Action<ActorInput> method) => OnDeath -= method;
    public void Callback_ActorDied() => OnDeath?.Invoke(actorInput);
    #endregion Combat Callbacks End

    #region IAttackable Methods
    public void Execute_ModifyHealth(int value, ModType modType)
    {
        ActorUtility.ModifyActorHealth(stats, value, modType);
    }

    public Transform GetTransform()
    {
        return transform;
    }

    public float GetObjectRadius()
    {
        return 2f;
    }

    public void InitializeAttackableInterface()
    {
        if(actorInput.IsPlayer)
        {
            AttackableType = AttackableType.PC;
        }
        else if(actorInput is NPCInput)
        {
            AttackableType = AttackableType.NPC;
        }
        else
        {
            AttackableType = AttackableType.WORLDOBJECT;
        }
    }
    #endregion IAttackable Methods End
}
