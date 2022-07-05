using AoG.Core;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Profiling;
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

    public virtual void ApplyDamage(Actor source, SavingThrowType savingThrowType, DamageType damageType, SpellAttackRollType attackRollType, int damageRoll, bool percentage)
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

        int AC = ActorUtility.GetModdedStat(stats, ActorStat.AC);

        switch(attackRollType)
        {
            case SpellAttackRollType.Melee:
                attackRoll = DnD.D20() + stats.strMod;
                if(attackRoll <= 0)
                    attackRoll = 1;
                break;
            case SpellAttackRollType.Ranged:
                attackRoll = DnD.D20() + stats.dexMod;
                if(attackRoll <= 0)
                    attackRoll = 1;
                break;
            case SpellAttackRollType.None:
                attackRoll = -1;
                break;
        }


        // Attack Roll:
        // Your attack roll is 1d20 + your ability modifier +your proficiency bonus if you're proficient with the weapon you’re using.

        // Damage Roll:
        // When attacking with a weapon, you add your ability modifier—the same modifier used for the attack roll—to the damage.

        //if(debug)
        //{
        //    Debug.Log(GetName() + ": attackRoll: " + attackRoll + " damageRoll: " + damageRoll);
        //}



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
        int savingThrow = savingThrowType != SavingThrowType.None ? MakeSavingThrow(savingThrowType) : AC;
        int DC = 8 + stats.proficiencyBonus + stats.intMod;
        bool hitSuccess = false;
        bool critHitSuccess = false;
        bool guaranteedHit = attackRoll == -1 || self.HasStatusEffect(Status.SLEEP);

        float counterValue = AC;

        if(savingThrowType != SavingThrowType.None)
        {
            //! roll < 8 + prof + spell modifier - target's save modifier ->
            //! p(hit) = (7 + prof + spell modifier - target's save modifier) / 20
            attackRoll = DC;
            counterValue = savingThrow = MakeSavingThrow(savingThrowType);
            hitSuccess = DC >= savingThrow;
        }
        else if(guaranteedHit == false)
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
            SFXPlayer.ActorSFX.VerbalConstant(characterVoiceSet, voiceAudioSource, VerbalConstantType.HURT);
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
                    ActorUtility.ModifyStatBase(stats, ActorStat.MORALE, -4, ModType.ADDITIVE);
                }
                else if(currentRatio > 50 && newRatio < 50)
                {
                    ActorUtility.ModifyStatBase(stats, ActorStat.MORALE, -2, ModType.ADDITIVE);
                }
                else if(currentRatio > 25 && newRatio < 25)
                {
                    ActorUtility.ModifyStatBase(stats, ActorStat.MORALE, -2, ModType.ADDITIVE);
                }

                if(ActorUtility.GetStatBase(stats, ActorStat.MORALE) < 10)
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
                gameObject.layer = LayerMask.NameToLayer("Corpses");
                Vector3 collapseForce = -transform.forward;
                Animation.CollapseDead(null, collapseForce);

                if(self.ActorStats.HasActorFlag(ActorFlags.ALLY))
                {
                    GameEventSystem.RequestRemovePortrait?.Invoke(self.PartySlot);
                }

                GameEventSystem.RequestAddGarbage(gameObject);
                self.spawnpoint.StartRespawnCountdown();

                Callback_ActorDied();

                DevConsole.Log("<color=cyan>" + self.GetName() + "</color> <color=orange>killed by <color=white>" + source.GetName() + "</color>.</color>");
                Die();
                return;
            }
        }

        int con = ActorUtility.GetStatBase(stats, ActorStat.CONSTITUTION);
        bool staggerSuccess = isCasting ? ((10 + equippedSpell.grade - Mathf.FloorToInt(stats.Level / 2) - Mathf.FloorToInt(con / 2) - 2) > DnD.D20()) : (noStagger ? false : hitSuccess); //critHitSuccess ? true : false; // Random.value > 0.3f;
        if(staggerSuccess && self.isDowned == false)
        {
            Animation.PlayMotion_Stagger();
            self.RoundSystem.FreezeRoundTimer(1);
        }

    }

    public void ApplyStatusEffectDamage(DamageType effectType, int attackRoll, bool percentage)
    {
        if(self.dead)
        {
            return;
        }

        bool hitSuccess = true; // isProjectile || isMagicAttack ? true : UnityEngine.Random.value > 0.1f;

        //hitSuccess = attackRoll <= MakeSavingThrow(savingThrowType);

        int finalAmount = attackRoll;

        switch(effectType)
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
                break;
            case DamageType.POISON:
                break;
        }

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
        SFXPlayer.ActorSFX.VerbalConstant(characterVoiceSet, voiceAudioSource, VerbalConstantType.HURT);
        if(finalAmount > 0)
            finalAmount = Mathf.CeilToInt(finalAmount);

        Execute_ModifyHealth(-finalAmount, ModType.ADDITIVE);

        if(self.dead)
        {
            StopCoroutine(pushCoroutine);
            SFXPlayer.ActorSFX.VerbalConstant(characterVoiceSet, voiceAudioSource, VerbalConstantType.DEAD);

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
                Debug.Log(self.GetName() + ":<color=orange> DEAD</color>");
                gameObject.layer = LayerMask.NameToLayer("Corpses");
                Vector3 collapseForce = -transform.forward;
                Animation.CollapseDead(null, collapseForce);

                if(self.ActorStats.HasActorFlag(ActorFlags.ALLY))
                {
                    GameEventSystem.RequestRemovePortrait?.Invoke(self.PartySlot);
                }

                GameEventSystem.RequestAddGarbage(gameObject);
                self.spawnpoint.StartRespawnCountdown();

                Callback_ActorDied();
            }
            else
            {
                Execute_KnockDown(Vector3.zero, 5, true);
                ActorUtility.ModifyActorHealth(self.ActorStats, 1, ModType.ABSOLUTE);
            }
        }
        else
        {
            SFXPlayer.ActorSFX.VerbalConstant(characterVoiceSet, voiceAudioSource, VerbalConstantType.HURT);
        }
    }

    public virtual void Die()
    {
        SFXPlayer.ActorSFX.VerbalConstant(characterVoiceSet, voiceAudioSource, VerbalConstantType.DEAD);

        if(stats.IsEssential == false)
        {
            StopAllCoroutines();
            GameInterface.Instance.GetCurrentGame().GetCurrentMap().AddGarbage(gameObject);
        }

        self.CancelAnimations();

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
        }

        self.Stop();
        self.ActorUI.Clear();

        //if(debug)
        //    Debug.Log(self.GetName() + "<color=orange> died [Actions left: " + actionQueue.Count + "]</color>");
    }

    //public void ApplyDamage(Actor source, Weapon weapon, EffectData magicEffect, bool isProjectile, bool hitSuccess = true)
    //{
    //    float finalAmount = 0;
    //    bool isUnarmedHit = false;
    //    DamageType damageType = DamageType.SLASHING;
    //    if(weapon != null)
    //    {
    //        damageType = weapon.damageType;
    //        isUnarmedHit = weapon.AnimationPack == AnimationSet.UNARMED;
    //        finalAmount += weapon.BaseDamageRoll;
    //    }

    //    if(finalAmount == 0)
    //    {
    //        Debug.Log("ActorInput.ApplyDamage: Weapon and magic effect value = 0");
    //    }

    //    if(Animation.isBlocking)
    //    {
    //        SFXPlayer.PlaySound_AttackBlocked(transform.position);
    //        //Animation.PlayMotion_StopBlocking();
    //        return;
    //    }

    //    if(hitSuccess) //! Currently only failes if target is out of range
    //    {
    //        pushCoroutine = StartCoroutine(CR_PushBack(source, self));
    //        //UIFloatingInfoManager.current.CreatePopup(transform.position, finalAmount.ToString(), 5, 1, 3);

    //        //if(Animation.inBleedOutState == false)
    //        //{
    //        //    ActorUtility.ModifyActorStun(actorInput.ActorRecord, -(int)finalAmount / 2, ModType.ADDITIVE);
    //        //    if(ActorUtility.GetModdedAttribute(actorInput.ActorRecord, ActorAttribute.STUN) < 1)
    //        //    {
    //        //        Animation.PlayMotion_BleedOut(0);
    //        //        ActorUtility.ModifyActorStun(actorInput.ActorRecord, 1, ModType.ADDITIVE);
    //        //    }
    //        //    Callback_OnStunChanged();
    //        //}

    //        //! No worries. The stagger function takes care of the bleedout hit
    //        bool staggerSuccess = UnityEngine.Random.value > 0.3f;

    //        if(staggerSuccess)
    //        {
    //            Animation.PlayMotion_Stagger();
    //        }

    //        //finalAmount *= GameStateManager.gameSettings.globalDmgMult;

    //        if(finalAmount > 0)
    //        {
    //            finalAmount = Mathf.CeilToInt(finalAmount);
    //        }

    //        ActorUtility.ModifyActorHealth(self.ActorStats, -(int)finalAmount, ModType.ADDITIVE);
    //        Callback_OnHealthChanged();

    //        ActivateOnHitCircus(source, weapon, magicEffect, isProjectile);

    //        OnHit?.Invoke(source, finalAmount, damageType, true);
    //        //if(source != null && source.ActorRecord.faction == Faction.Heroes)
    //        //{
    //        //    int baseExp = (int)(actorInput.ActorRecord.level <= source.ActorRecord.level ? ((actorInput.ActorRecord.level * 5) + 45) : ((actorInput.ActorRecord.level * 5) + 45) * (1 + (0.05f * (source.ActorRecord.level - actorInput.ActorRecord.level))));
    //        //    float perc = finalAmount / ActorUtility.GetModdedAttribute(actorInput.ActorRecord, ActorAttribute.MAXHEALTH);
    //        //    float result = perc * baseExp;
    //        //    source.ActorRecord.Execute_ModifyCurrentXP(result, ModType.ADDITIVE);
    //        //}
    //        //m_states[Symbols.LOW_HEALTH] = m_currHealth < 50;

    //        if(self.dead)
    //        {
    //            StopCoroutine(pushCoroutine);
    //            SFXPlayer.ActorSFX.VerbalConstant(characterVoiceSet, voiceAudioSource, VerbalConstantType.DEAD);

    //            foreach(NPCInput summoned in self.summonedCreatures)
    //            {
    //                if(summoned == null || summoned.dead)
    //                {
    //                    continue;
    //                }

    //                summoned.Combat.Kill();
    //            }

    //            if(self.ActorStats.HasActorFlag(ActorFlags.ESSENTIAL) == false)
    //            {
    //                //OnDestroyed?.Invoke(this, source, Vector3.zero);
    //                Debug.Log(self.GetName() + ":<color=orange> DEAD</color>");
    //                gameObject.layer = LayerMask.NameToLayer("Corpses");
    //                Vector3 collapseForce = source ? source.transform.forward : -transform.forward;
    //                Animation.CollapseDead(source, collapseForce);

    //                if(self.ActorStats.HasActorFlag(ActorFlags.ALLY))
    //                {
    //                    GameEventSystem.RequestRemovePortrait?.Invoke(self.PartySlot);
    //                }

    //                GameEventSystem.RequestAddGarbage(gameObject);
    //                self.spawnpoint.StartRespawnCountdown();

    //                Callback_ActorDied();
    //            }
    //            else
    //            {
    //                Execute_KnockDown(Vector3.zero, 5, true);
    //                ActorUtility.ModifyActorHealth(self.ActorStats, 1, ModType.ABSOLUTE);
    //            }
    //        }
    //        else
    //        {
    //            SFXPlayer.ActorSFX.VerbalConstant(characterVoiceSet, voiceAudioSource, VerbalConstantType.HURT);
    //        }
    //    }
    //}

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

    private void ActivateOnHitCircus(Actor source, int damageRoll, DamageType damageType, bool hitSuccess)
    {
        OnHit?.Invoke(source, damageRoll, damageType, hitSuccess);

        Vector3 dir = source != null ? source.transform.position - transform.position : transform.forward;
        dir.y = 0;
        Vector3 victimHitpoint = GetAttackPoint().position;
        Vector3 finalHitPosition = victimHitpoint;

        finalHitPosition = victimHitpoint + (source.Animation.head.transform.position - victimHitpoint).normalized * 0.1f;
        
        VFXPlayer.PlayVFX_OnHit(finalHitPosition, dir, damageType);
        SFXPlayer.PlaySound_OnHit(finalHitPosition, damageType, damageType == DamageType.CRUSHING);

        self.ActorUI.Flash(0.15f);
    }

    #endregion OnHit Feedback End

    private bool IsAlly(Actor actor)
    {
        return actor.ActorStats.HasActorFlag(ActorFlags.ALLY) && self.ActorStats.HasActorFlag(ActorFlags.ALLY);
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
            Debug.Log(self.GetName() + "<color=red>: SetEquippedSpell: NULL</color>");
        }

        if(spell == equippedSpell)
        {
            Debug.Log(self.GetName() + "<color=yellow>: SetEquippedSpell: Spell '" + spell.Name + "' already equipped</color>");
            return;
        }

        equippedSpell = spell;

        if(equippedSpell != null && SpellDrawn)
        {
            Animation.PlayMotion_SwitchSpell();
            VFXPlayer.RemoveHandVFX(_handSpellVFX);
            _handSpellVFX = VFXPlayer.SetSpellHandVFX(Equipment.spellAnchor, equippedSpell.magicEffects[0].id_VFXHandCharge);
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
            _handSpellVFX ??= VFXPlayer.SetSpellHandVFX(Equipment.spellAnchor, equippedSpell.magicEffects[0].id_VFXHandCharge);
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

    public void Execute_HandleSpell(Spell spell, int stage, int motionIndex) // 0 = left, 1 = right
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
        self.Equipment.UnequipArmor(armor);
        ActorUtility.ModifyStatModded(self.ActorStats, ActorStat.AC, -armor.AC, ModType.ADDITIVE);
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
        if(self.debugGear)
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
            if(self.debugGear)
                Debug.LogError(self.GetName() + ":<color=red>*</color> ActorInput.Execute_DrawWeapon: weaponObject already drawn");
            return;
        }

        if(Equipment.equippedWeapon.Weapon == null)
        {
            if(self.debugGear)
                Debug.LogError(self.GetName() + ":<color=red>*</color> ActorInput.Execute_DrawWeapon: weaponObject = null");
            return;
        }

        if(self.debugGear)
            Debug.Log(self.GetName() + "<color=green>*</color>: ActorInput.Execute_DrawWeapon");

        SetWeaponDrawn(true);
        Animation.PlayMotion_DrawWeapon(GetEquippedWeapon().AnimationPack);
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
            if(self.debugGear)
                Debug.LogError(self.GetName() + ":<color=red>*</color> ActorInput.Execute_DrawWeapon: weaponDrawn == false");
            return;
        }

        if(Equipment.equippedWeapon.Weapon == null)
        {
            if(self.debugGear)
                Debug.LogError(self.GetName() + ":<color=red>*</color> ActorInput.Execute_DrawWeapon: weaponObject = null");
            return;
        }

        if(self.debugGear)
            Debug.Log(self.GetName() + ":<color=green>*</color> ActorInput.Execute_SheathWeapon");
        //OnSheathWeapon?.Invoke((int)gearData.equippedWeapon.Weapon.weaponCategory);

        SetWeaponDrawn(false);
        Animation.PlayMotion_SheathWeapon(GetEquippedWeapon().AnimationPack);
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
        if(self.debugGear)
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
        //if(actorInput.debugGear)
        //    Debug.Log(actorInput.GetName() + ":<color=cyan>*</color> ActorInput.Execute_Attack");
        Animation.PlayMotion_Attack(GetEquippedWeapon().AnimationPack);
        if(GetHostileTarget() != null)
            GetHostileTarget().Combat.SignalIncomingMeleeAttack(GetEquippedWeapon(), transform.position);

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
        SFXPlayer.PlaySound_Swing(transform.position, Equipment.equippedWeapon.Weapon.weaponCategory);
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
        if(self.debugAnimation)
            Debug.Log(gameObject.name + ":<color=cyan>*</color> Hit procedure");

        if(Equipment.equippedWeapon == null)
        {
            if(self.debugAnimation)
                Debug.Log($"{self.GetName()}: Applying damage failed: Equipped right hand weapon = null");
        }
        else
        {
            if(Animation.IsIncapacitated())
            {
                if(self.debugAnimation)
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

                _projectile.Launch(null, self, Equipment.spellAnchor.position, GetHostileTarget(), new StatusEffectData(), SpellTargetType.Foe, DeliveryType.SeekActor,
                    ProjectileType.Lobber, DamageType.MISSILE, SavingThrowType.None, SpellAttackRollType.Ranged, new Dice(weapon.NumDice, weapon.NumDieSides, 0), 15, 0, 0, 0);
            }
            else
            {
                int numFound = Physics.OverlapSphereNonAlloc(transform.position, weapon.Range, populatedHitTargetArray, GameInterface.Instance.DatabaseService.GameSettings.agentLayers);

                if(numFound == 0)
                {
                    //if(actorInput.debugAnimation)
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
                        targetHit.ApplyDamage(self, SavingThrowType.Constitution, weapon.damageType, SpellAttackRollType.Melee, weapon.BaseDamageRoll, false);
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
    public void Callback_OnHealthChanged() => onHealthChanged?.Invoke(ActorUtility.GetModdedStat(self.ActorStats, ActorStat.HITPOINTS),
        ActorUtility.GetModdedStat(self.ActorStats, ActorStat.MAXHITPOINTS));
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
