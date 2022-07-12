using AoG.Core;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

[System.Serializable]
public class ActorCombat : MonoBehaviour, IAttackable
{
    public Spell equippedSpell;
    public bool WeaponDrawn { get; private set; }
    public bool SpellDrawn { get; private set; }
    public bool SpellEffectsLaunched { get; internal set; }
    public bool IgnorePlayerAttack { get => self.IsAlly; }
    public AttackableType AttackableType { get; set; }


    public System.Action<Vector3, float, bool> OnKnockDown; // force direction, duration, markDead
    //public System.Action<Vector3, float> OnKnockDown;
    public System.Action<Vector3> OnCollapse; // force direction
    private System.Action<Actor> OnDeath;
    public System.Action<Actor, float, DamageType, bool> OnHit; // source, damage, damageType, hitSuccess
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
    public System.Action OnReleaseSpell;

    public System.Action<float> OnBlockAggroRequest;

    private Actor self;
    private ActorAnimation Animation;
    private ActorEquipment Equipment;
    private Actor[] targets;
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
    private bool isCasting;
    public bool noStagger;
    internal Actor lastAttacker;

    //public bool LaunchArrowNow { get; private set; }

    public virtual void Initialize(Actor actorInput, ActorStats stats, ActorEquipment equipment, ActorAnimation actorAnimation, CharacterVoiceSet characterVoiceSet, AudioSource voiceAudioSource)
    {
        this.self = actorInput;
        this.stats = stats;
        this.characterVoiceSet = characterVoiceSet;
        Animation = actorAnimation;
        this.Equipment = equipment;

        targets = new Actor[2];

        this.voiceAudioSource = voiceAudioSource;

        populatedHitTargetArray = new Collider[10];

        InitializeAttackableInterface();
    }

    public void SetHostileTarget(Actor actorInput)
    {
        targets[0] = actorInput;
    }

    public void SetFriendlyTarget(Actor actorInput)
    {
        targets[1] = actorInput;
    }

    public Actor GetHostileTarget()
    {
        return targets[0];
    }

    public Actor GetFriendlyTarget()
    {
        return targets[1];
    }

    public int AttackRollSpell(Spell spell)
    {
        int result = Random.Range(1, 21);

        //if(weapon.weaponType == WeaponType.Bow || weapon.weaponType == WeaponType.Crossbow || weapon.weaponType == WeaponType.Sling)
        //{
        //    result += DnD.AttributeModifier(actorData.dexterity);
        //}
        //else
        //{
        //    result += DnD.AttributeModifier(actorData.strength);
        //}

        return result;
    }

    public int MakeDamageRoll(Weapon weapon)
    {
        int result = DnD.Roll(weapon.NumDice, weapon.NumDieSides);

        if(weapon.CombatType == CombatType.RANGED)
        {
            result += stats.dexMod;
        }
        else
        {
            result += stats.strMod;
        }

        return result;
    }

    public int MakeDamageRoll(int numDice, int numSides)
    {
        int result = DnD.Roll(numDice, numSides);

        return result;
    }

    int MakeSavingThrow(SavingThrowType savingThrowType)
    {
        int result = Random.Range(1, 21);
        switch(savingThrowType)
        {
            case SavingThrowType.None:
                result = 0;
                break;
            case SavingThrowType.Strength:
                result += stats.strMod;
                break;
            case SavingThrowType.Dexterity:
                result += stats.dexMod;
                break;
            case SavingThrowType.Constitution:
                result += stats.conMod;
                break;
            case SavingThrowType.Intelligence:
                result += stats.intMod;
                break;
            case SavingThrowType.Wisdom:
                result += stats.wisMod;
                break;
            case SavingThrowType.Charisma:
                result += stats.chaMod;
                break;
            case SavingThrowType.Reflex:
                //result += DnD.AttributeModifier(actorData.dexterity);
                break;
        }

        return result;
    }

    public virtual void ApplyDamage(Actor source, DamageType damageType, int damageRoll, bool percentage)
    {
        if(self.dead)
        {
            return;
        }

        if(damageRoll == 0)
        {
            return;
        }

        int attackRoll = 0;

        int AC = stats.GetStat(ActorStat.AC);


        switch(damageType)
        {
            case DamageType.MAGIC:
                break;
            case DamageType.FIRE:
                break;
            case DamageType.COLD:
                break;
            case DamageType.ELECTRICITY:
                break;
            case DamageType.MAGICFIRE:
                break;
            case DamageType.HEAL:
                //PopupSystem.Instance.CreatePopup(transform.position + Vector3.up * 2, damageRoll.ToString(), 0.1f, 1, 0.3f, Colors.GreenNCS);
                DevConsole.Log("<color=cyan>" + self.GetName() + "</color> was healed from <color=white>" + self.GetName() + "</color> by <color=green>" + damageRoll + "</color> hitpoints.");
                Execute_ModifyHealth(damageRoll, ModType.ADDITIVE);
                return;
                //case EffectType.POISON:
                //    DevConsole.Log("<color=cyan>" + GetName() + "</color> was poisoned by <color=white>" + source.GetName() + "</color>.");
                //    Execute_ApplyStatusEffect(new StatusEffect_Damage(damageType, savingThrowType, 2, false, 10, 10));
                //    break;
                //case EffectType.SLEEP:
                //    if(isDowned || healthDepleted)
                //        return;
                //    //Debug.Log("Adding status effect sleep");
                //    DevConsole.Log("<color=cyan>" + source.GetName() + "</color> put <color=white>" + GetName() + "</color> to sleep.");
                //    Execute_ApplyStatusEffect(new StatusEffect_Sleep(this, damageType, savingThrowType, 0, false, 0, 2));
                //    return;
        }

        if(self.IsPlayer != source.IsPlayer)
        {
            GameInterface.Instance.GetCurrentGame().PartyAttack = true;
            SetHostileTarget(source);
        }

        // isProjectile || isMagicAttack ? true : UnityEngine.Random.value > 0.1f;
        //int savingThrow = savingThrowType != SavingThrowType.None ? MakeSavingThrow(savingThrowType) : AC;
        int DC = 8 + stats.proficiencyBonus + stats.intMod;
        bool hitSuccess = false;
        bool critHitSuccess = false;
        bool guaranteedHit = attackRoll == -1 || self.HasStatusEffect(Status.SLEEP);

        float counterValue = AC;

        //if(savingThrowType != SavingThrowType.None)
        //{
        //    //! roll < 8 + prof + spell modifier - target's save modifier ->
        //    //! p(hit) = (7 + prof + spell modifier - target's save modifier) / 20
        //    attackRoll = DC;
        //    counterValue = savingThrow = MakeSavingThrow(savingThrowType);
        //    hitSuccess = DC >= savingThrow;
        //}
        //else
        if(guaranteedHit == false)
        {
            if(attackRoll >= 20)
            {
                DevConsole.Log("<color=cyan>" + self.GetName() + "</color> received a critical hit from <color=white>" + source.GetName() + "</color>! <color=orange>" + damageRoll + "</color>");
                critHitSuccess = hitSuccess = true;
            }
            else if(attackRoll == 1)
            {
                DevConsole.Log("<color=cyan>" + self.GetName() + "</color>: Natural Miss from <color=white>" + source.GetName() + "</color> because Attack Roll was 1");
                hitSuccess = false;
            }
            else
            {
                //! roll >= AC - prof - attack modifier ->
                //! p(hit) = (21 - (AC - prof - attack modifier)) / 20
                hitSuccess = attackRoll >= AC;
                //DevConsole.Log("<color=white>" + GetName() + "</color>: " + (hitSuccess ? "Hit success: " : "Missed: ") + attackRoll + (hitSuccess ? "(Attack Roll) > " : " <= ") + actorData.AC + "(Saving Throw)");
            }
        }
        else // It's a spell without saving throw
            hitSuccess = true;

        //Debug.Log((hitSuccess ? "Hit success: " : "Missed: ") + attackRoll + (hitSuccess ? " (Attack Roll) > " : " <= ") + savingThrow + " (Saving Throw)");

        //int damage = damageRoll;
        if(hitSuccess)
        {
            self.VerbalConstant(VerbalConstantType.HURT);
            DevConsole.Log("<color=cyan>" + self.GetName() + "</color> was hit by <color=white>" + source.GetName() + "</color> and lost <color=white>" + damageRoll + "</color> hitpoints.");
            //GameEventSystem.OnHeroHit?.Invoke(this, damageRoll, damageType);
            Execute_ModifyHealth(-damageRoll, ModType.ADDITIVE);
        }
        else
        {
            DevConsole.Log("<color=cyan>" + self.GetName() + "</color> evaded <color=white>" + source.GetName() + "</color>'s attack.");
        }

        DevConsole.Log("<color=cyan>" + self.GetName() + "</color>: Attack Roll " + attackRoll + (hitSuccess ? " > " : " <= ") + counterValue + " (Saving Throw/AC) : " + (hitSuccess ? "Hit " : "Miss "));

        ActivateOnHitCircus(source, damageRoll, damageType, hitSuccess);


        //if(blocking)
        //    OnStagger?.Invoke();

        //! Modify morale
        int currHP = stats.StatsBase[ActorStat.HITPOINTS];
        if(damageRoll > 0)
        {
            //if(damageRoll > currHP)
            //{

            //}
            // impact morale when hp thresholds (50 %, 25 %) are crossed for the first time
            int currentRatio = 100 * currHP / stats.StatsBase[ActorStat.MAXHITPOINTS];
            int newRatio = 100 * (currHP + damageRoll) / stats.StatsBase[ActorStat.MAXHITPOINTS];
            if(stats.Faction != Faction.Heroes)
            {
                if(currentRatio > 50 && newRatio < 25)
                {
                    stats.ModifyStatBase(ActorStat.MORALE, -4, ModType.ADDITIVE);
                }
                else if(currentRatio > 50 && newRatio < 50)
                {
                    stats.ModifyStatBase(ActorStat.MORALE, -2, ModType.ADDITIVE);
                }
                else if(currentRatio > 25 && newRatio < 25)
                {
                    stats.ModifyStatBase(ActorStat.MORALE, -2, ModType.ADDITIVE);
                }

                if(stats.GetBaseStat(ActorStat.MORALE) < 10)
                {
                    self.ApplyStatusEffect(Status.PANIC, 3);
                }
            }
        }

        if(self.dead)
        {
            if(stats.NoDead)
            {
                Execute_ModifyHealth(1, ModType.ABSOLUTE);
            }
            else
            {
                Debug.Log(self.GetName() + ":<color=orange> DEAD</color>");
                DevConsole.Log("<color=cyan>" + self.GetName() + "</color> <color=orange>killed by <color=white>" + source.GetName() + "</color>.</color>");
                Die();
                return;
            }
        }

        int con = stats.GetBaseStat(ActorStat.CONSTITUTION);
        bool staggerSuccess = isCasting ? ((10 + equippedSpell.Grade - Mathf.FloorToInt(stats.Level / 2) - Mathf.FloorToInt(con / 2) - 2) > DnD.D20()) : (noStagger ? false : hitSuccess); //critHitSuccess ? true : false; // Random.value > 0.3f;
        if(staggerSuccess && self.isDowned == false)
        {
            Animation.PlayMotion_Stagger();
            self.RoundSystem.FreezeRoundTimer(1);
        }

    }

    public void ApplyStatusEffectDamage(Status statusEffect, DamageType damageType, int attackRoll, bool percentage)
    {
        if(self.dead)
        {
            return;
        }

        int finalAmount = attackRoll;

        //if(percentage)
        //{
        // finalAmount = maxHealth * (attackRoll + 1 * (100 / (100f + def))); // Exmpl: 5 * 100 / 100 * 0
        // //finalAmount *= GameMaster.Instance.gameSettings.globalDmgMult;
        //}
        //else
        //{
        finalAmount = attackRoll; // Exmpl: 5 * 100 / 100 * 0
                                  //finalAmount *= GameMaster.Instance.gameSettings.globalDmgMult;
                                  //}
        self.VerbalConstant(VerbalConstantType.HURT);
        if(finalAmount > 0)
            finalAmount = Mathf.CeilToInt(finalAmount);

        Execute_ModifyHealth(-finalAmount, ModType.ADDITIVE);

        if(self.dead)
        {
            StopCoroutine(pushCoroutine);
            self.VerbalConstant(VerbalConstantType.DEAD);

            foreach(NPCInput summoned in self.summonedCreatures)
            {
                if(summoned == null || summoned.dead)
                {
                    continue;
                }

                summoned.Combat.Kill();
            }

            if(self.ActorStats.HasActorFlag(ActorFlags.ESSENTIAL) == false)
            {
                //OnDestroyed?.Invoke(this, source, Vector3.zero);
                Debug.Log($"{self.GetName()}:<color=orange> DIED from status effect '{statusEffect}'</color>");

                //self.spawnpoint.StartRespawnCountdown();
                Die();
            }
            else
            {
                Execute_KnockDown(Vector3.zero, 5, true);
                Execute_ModifyHealth(1, ModType.ABSOLUTE);
            }
        }
        else
        {
            self.VerbalConstant(VerbalConstantType.HURT);
        }
    }

    public virtual void Die()
    {

        gameObject.layer = LayerMask.NameToLayer("Corpses");
        //Vector3 collapseForce = -transform.forward;
        //Animation.CollapseDead(null, collapseForce);

        Animation.PlayMotion_Die();

        Callback_ActorDied();

        self.VerbalConstant(VerbalConstantType.DEAD);

        self.CancelAnimations();

        if(stats.IsEssential == false)
        {
            if(self.IsPlayer)
            {
                self.ActorUI.Deselect();

                //if(SelectionManager.selected.Count == 0)
                //{
                //    if(Interface.GetCurrentGame().GetPartySize(true) > 0)
                //    {
                //        Actor nextAlive = Interface.GetCurrentGame().PCs.Where(pc => pc.healthDepleted == false).FirstOrDefault();

                //        nextAlive.Select();
                //    }
                //}

                FormationController.ClearFormationVisual(self.PartySlot);
                GameEventSystem.RequestRemovePortrait?.Invoke(self.PartySlot);
            }

            self.ActorUI.Clear();
            StopAllCoroutines();
            GameInterface.Instance.GetCurrentGame().GetCurrentMap().AddGarbage(gameObject);
        }

        self.Stop();
    }

    public void Kill()
    {
        gameObject.layer = LayerMask.NameToLayer("Corpses");
        Animation.CollapseDead(null, Vector3.zero);
        GameEventSystem.RequestAddGarbage(gameObject);
        Callback_ActorDied();
    }

    #region OnHit Feedback
    private IEnumerator CR_PushBack(Actor source, Actor pushed)
    {
        float time = 0.2f;

        while(time > 0)
        {
            time -= Time.deltaTime;

            pushed.NavAgent.Move(source.transform.forward /** 0.5f*/ * Time.deltaTime);
            yield return null;
        }
    }

    private void ActivateOnHitCircus(Actor source, int damageRoll, DamageType damageType, bool hitSuccess)
    {
        OnHit?.Invoke(source, damageRoll, damageType, hitSuccess);

        Vector3 dir = source != null ? source.transform.position - transform.position : transform.forward;
        dir.y = 0;

        Vector3 victimHitpoint = GetAttackPoint().position;
        Vector3 finalHitPosition = victimHitpoint + (source.Animation.head.transform.position - victimHitpoint).normalized * 0.1f;

        VFXPlayer.PlayVFX_OnHit(finalHitPosition, dir, damageType);
        SFXPlayer.PlaySound_OnHit(finalHitPosition, damageType, damageType == DamageType.CRUSHING);

        self.ActorUI.Flash(0.15f);
    }

    #endregion OnHit Feedback End

    private bool IsAlly(Actor actor)
    {
        return actor.ActorStats.HasActorFlag(ActorFlags.ALLY) && self.ActorStats.HasActorFlag(ActorFlags.ALLY);
    }

    public void Execute_UnequipArmor(Armor armor, bool silent = false)
    {
        self.Equipment.UnequipArmor(armor);
        stats.ModifyStatModded(ActorStat.AC, -armor.AC, ModType.ADDITIVE);
        //Debug.Log("<color=grey>Dress root object '" + dressRootObject.name + "' being processed</color>");
        //DressUpManager.UnequipArmor(targetSlot);

        //actorInput.character.OnArmorUnequipped?.Invoke(armor, true, silent == false);
    }

    public WeaponData GetEquippedWeapon()
    {
        return Equipment.equippedWeapon;
    }

    public virtual void Execute_EquipBestWeapon(int WEAPON_TYPE, bool playAnimation, bool playSound)
    {
        if(self.debugCombat)
            Debug.Log(self.GetName() + ":<color=orange>2</color> ActorInput.Execute_EquipBestWeapon");
        Weapon weapon = Equipment.EquipBestWeapon(WEAPON_TYPE);
        if(weapon == null)
        {
            Debug.Log(self.GetName() + ":<color=yellow> Execute_EquipBestWeapon failed</color>");
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
            if(self.debugCombat)
                Debug.LogError(self.GetName() + ":<color=red>*</color> ActorInput.Execute_DrawWeapon: weaponObject already drawn");
            return;
        }

        if(GetEquippedWeapon()== null)
        {
            if(self.debugCombat)
                Debug.LogError(self.GetName() + ":<color=red>*</color> ActorInput.Execute_DrawWeapon: weaponObject = null");
            return;
        }

        if(self.debugCombat)
            Debug.Log(self.GetName() + "<color=green>*</color>: ActorInput.Execute_DrawWeapon");

        SetWeaponDrawn(true);
        Animation.PlayMotion_DrawWeapon(GetEquippedWeapon().Data.AnimationPack);
        StartCoroutine(CR_ParentWeaponToHand());
    }

    private IEnumerator CR_ParentWeaponToHand()
    {
        yield return new WaitForSeconds(0.5f);
        Equipment.ParentWeaponToHand();
        SFXPlayer.PlaySound_DrawWeapon(GetEquippedWeapon().Data.weaponCategory, transform.position);
    }

    public void Execute_SheathWeapon()
    {
        if(WeaponDrawn == false)
        {
            if(self.debugCombat)
                Debug.LogError(self.GetName() + ":<color=red>*</color> ActorInput.Execute_DrawWeapon: weaponDrawn == false");
            return;
        }

        if(GetEquippedWeapon()== null)
        {
            if(self.debugCombat)
                Debug.LogError(self.GetName() + ":<color=red>*</color> ActorInput.Execute_DrawWeapon: weaponObject = null");
            return;
        }

        if(self.debugCombat)
            Debug.Log(self.GetName() + ":<color=green>*</color> ActorInput.Execute_SheathWeapon");
        //OnSheathWeapon?.Invoke((int)gearData.equippedWeapon.Weapon.weaponCategory);

        SetWeaponDrawn(false);
        Animation.PlayMotion_SheathWeapon(GetEquippedWeapon().Data.AnimationPack);
        StartCoroutine(CR_ParentWeaponToHolster());
    }

    private IEnumerator CR_ParentWeaponToHolster()
    {
        yield return new WaitForSeconds(0.5f);
        Equipment.ParentWeaponToHolster();
        SFXPlayer.PlaySound_SheathWeapon(GetEquippedWeapon().Data.weaponCategory, transform.position);
    }

    private void SetWeaponDrawn(bool on)
    {
        if(self.debugCombat)
            Debug.Log(self.GetName() + ":<color=cyan>*</color> Setting weapon drawn = '" + on + "'");
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
            Debug.Log(self.GetName() + ":<color=red>*</color> ActorInput.ExecuteAttack(): Weapon not drawn");
            return;
        }
        //hitSuccess = true;
        if(self.debugCombat)
            Debug.Log(self.GetName() + ":<color=cyan>*</color> ActorInput.Execute_Attack");
        Animation.PlayMotion_Attack(GetEquippedWeapon().Data.AnimationPack);
        //if(GetHostileTarget() != null)
        //    GetHostileTarget().Combat.SignalIncomingMeleeAttack(GetEquippedWeapon(), transform.position);

        if(GetEquippedWeapon().Data.CombatType == CombatType.MELEE)
            StartCoroutine(CR_DelayAttackEffects());
    }

    private IEnumerator CR_DelayAttackEffects()
    {
        yield return new WaitForSeconds(0.3f);

        if(self.dead /*|| GetHostileTarget() == null || GetHostileTarget().dead*/)
        {
            Debug.Log("<<<<<<<<<<<<<<<<<<<<<<<<<<<<<< Delay failed");
            yield break;
        }
        SFXPlayer.ActorSFX.VerbalConstant(characterVoiceSet, voiceAudioSource, VerbalConstantType.ATTACK, 0.2f);
        SFXPlayer.PlaySound_Swing(transform.position, GetEquippedWeapon().Data.weaponCategory);
        // hitSuccess = GetHostileTarget().Animation.isAttacking || GetHostileTarget().isDowned || GetHostileTarget().ActorRecord.inSpellChargeLoop ? true : Random.value < 0.7f;
        // if(hitSuccess == false)
        //     GetHostileTarget().Combat.Animation.PlayMotion_Evade(1f);
    }

    private void SignalIncomingMeleeAttack(Weapon weapon, Vector3 attackerPosition)
    {
        if(self.ActorStats.isPlayer)
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
        if(self.isDowned)
        {
            return;
        }
        self.isDowned = true;
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

    public virtual Vector3 GetAttackVector()
    {
        return Animation.head.position;
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
        OnReleaseSpell?.Invoke();
    }

    /// <summary>
    /// Called by animation event.
    /// Used for melee attacks and applies damage to attackTarget instantly.
    /// Standard ranged attacks use this to initiate the projectile.
    /// </summary>
    public void Hit()
    {
        if(self.debugCombat)
            Debug.Log(gameObject.name + ":<color=cyan>*</color> Hit procedure");

        if(GetEquippedWeapon() == null)
        {
            if(self.debugAnimation)
                Debug.Log($"{self.GetName()}: Applying damage failed: Equipped right hand weapon = null");
        }
        else
        {
            if(Animation.IsIncapacitated())
            {
                if(self.debugCombat)
                    Debug.Log(gameObject.name + ": <color=yellow>*</color>Can't hit anything -> Incapacitated");
                return;
            }

            Weapon weapon = GetEquippedWeapon().Data;
            if(weapon.projectileIdentifier != null)
            {

                GameObject vfxObj = PoolSystem.GetPoolObject(weapon.projectileIdentifier, ObjectPoolingCategory.VFX);

                Projectile _projectile = vfxObj.GetComponent<Projectile>();

                if(_projectile == null)
                    Debug.LogError("ActorMonoObject: Projectile visual not found");

                _projectile.Launch(self, Equipment.m_weaponHand.position, GetHostileTarget(), DeliveryType.SeekActor,
                    ProjectileType.Lobber, DamageType.MISSILE, 15, 0, 0, 0);
            }
            else
            {
                int numFound = Physics.OverlapSphereNonAlloc(transform.position, weapon.Range, populatedHitTargetArray, GameInterface.Instance.DatabaseService.GameSettings.agentLayers);

                if(numFound == 0)
                {
                    //if(actorInput.debugCombat)
                    //    Debug.Log(gameObject.name + ": <color=orange>*</color>No melee hit targets found");
                    return;
                }

                int maxTargets = weapon.MaxHitTargets;

                for(int i = 0; i < numFound; i++)
                {
                    IAttackable targetHit = populatedHitTargetArray[i].GetComponent<IAttackable>();
                    if(targetHit.GetTransform() == transform || (self.IsPlayer && targetHit.AttackableType == AttackableType.PC)
                        /*|| (actorInput.IsAlly && (targetHit.AttackableType == AttackableType.PC || targetHit.AttackableType == AttackableType.PC))*/)
                    {
                        //if(actorInput.debugCombat)
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
                        targetHit.ApplyDamage(self, weapon.damageType, weapon.BaseDamageRoll, false);
                }
            }
        }
    }

    private void ActivateWeaponTrail(AnimationEvent animationEvent)
    {
        if(GetEquippedWeapon().WeaponTrailVFX == null)
        {
            //Debug.LogError("Weapon trail null");
            return;
        }

        float xAngle = animationEvent.floatParameter;
        bool doStab = xAngle < 0;


        Transform vfxTransform = GetEquippedWeapon().WeaponTrailVFX.transform;

        if(doStab)
        {
            Vector3 weaponPos = GetEquippedWeapon().WeaponObject.transform.position;
            vfxTransform.position = new Vector3(weaponPos.x, weaponPos.y, transform.position.z);
            GetEquippedWeapon().WeaponTrailVFX.SendEvent("PlayStab");
        }
        else
        {
            int zAngle = animationEvent.intParameter;
            Vector3 weaponDir = GetEquippedWeapon().WeaponObject.transform.position - transform.position;
            vfxTransform.localPosition = new Vector3(0, Animation.chest.position.y, 0);
            vfxTransform.localEulerAngles = new Vector3(xAngle, Quaternion.LookRotation(weaponDir).y * 0.5f, zAngle);
            GetEquippedWeapon().WeaponTrailVFX.SendEvent("PlaySlash");
        }

        // Combat.GetEquippedWeaponTrailVFX.Play();
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
    public void Callback_OnHealthChanged() => onHealthChanged?.Invoke(stats.GetStat(ActorStat.HITPOINTS),
        stats.GetStat(ActorStat.MAXHITPOINTS));
    public void RegisterCallback_OnStunChanged(System.Action<ActorStats> method) => onStunChanged += method;
    public void UnregisterCallback_OnStunChanged(System.Action<ActorStats> method) => onStunChanged -= method;
    public void Callback_OnStunChanged() => onStunChanged?.Invoke(self.ActorStats);
    public void RegisterCallback_OnDeath(System.Action<Actor> method) => OnDeath += method;
    public void UnregisterCallback_OnDeath(System.Action<Actor> method) => OnDeath -= method;
    public void Callback_ActorDied() => OnDeath?.Invoke(self);
    #endregion Combat Callbacks End

    #region IAttackable Methods
    public void Execute_ModifyHealth(int value, ModType modType)
    {
        stats.ModifyActorHP(value, modType);
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
        if(self.IsPlayer)
        {
            AttackableType = AttackableType.PC;
        }
        else if(self is NPCInput)
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
