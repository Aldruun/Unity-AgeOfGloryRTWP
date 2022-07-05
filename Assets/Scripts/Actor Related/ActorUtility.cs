using AoG.Core;
using GenericFunctions;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public static class ActorUtility
{
    public static class Initialization
    {
        //public static void AddInitialActorMonoBehaviours(GameObject actor)
        //{
        //    actor.ForceGetComponent<ActorCombat>();
        //    actor.ForceGetComponent<NavMeshAgent>();
        //}

        public static void CalculateDnDStats(ActorStats stats, int level)
        {
            stats.proficiencyBonus = Mathf.CeilToInt((level / 4) + 1);
            //if(debug)
            //    DevConsole.Log(Name + ": <color=grey>Recalc proficiency: CharLvl " + level + "/4 + 1 = " + proficiencyBonus + "</color>");

            ActorUtility.SetBaseStat(stats, ActorStat.HITDIE, DnD.GetHitDie(stats.Class, stats.Race) + level - 1);
            if((level - 1) < 0)
                Debug.LogError("Oh oh");

            stats.strMod = DnD.AttributeModifier(ActorUtility.GetStatBase(stats, ActorStat.STRENGTH));
            stats.dexMod = DnD.AttributeModifier(ActorUtility.GetStatBase(stats, ActorStat.DEXTERITY));
            stats.conMod = DnD.AttributeModifier(ActorUtility.GetStatBase(stats, ActorStat.CONSTITUTION));
            stats.intMod = DnD.AttributeModifier(ActorUtility.GetStatBase(stats, ActorStat.INTELLIGENCE));
            stats.wisMod = DnD.AttributeModifier(ActorUtility.GetStatBase(stats, ActorStat.WISDOM));
            stats.chaMod = DnD.AttributeModifier(ActorUtility.GetStatBase(stats, ActorStat.CHARISMA));

            switch(stats.Class)
            {
                case Class.CLERIC:
                case Class.DRUID:
                    stats.spellcastingAbility = stats.wisMod;
                    break;
                case Class.BARD:
                case Class.PALADIN:
                case Class.SORCERER:
                    stats.spellcastingAbility = stats.chaMod;
                    break;
                case Class.ALCHEMIST:
                case Class.SHAMAN:
                case Class.MAGE:
                    stats.spellcastingAbility = stats.intMod;
                    break;
            }

            stats.spellSaveDC = 8 + stats.proficiencyBonus /** (1 + level - 1)*/ + stats.spellcastingAbility;
            //if(debug)
            //    DevConsole.Log(GetName() + ": <color=grey>Recalc spellSaveDC: 8 + PB " + proficiencyBonus + " + SCA " + spellcastingAbility + " = " + spellSaveDC + "</color>");

            stats.spellAttackModifier = stats.spellcastingAbility + stats.proficiencyBonus;
            //if(debug)
            //    DevConsole.Log(Name + ": <color=grey>Recalc spellAttackBonus: SCA " + spellcastingAbility + " + PB " + proficiencyBonus + " = " + spellAttackModifier + "</color>");

            ActorUtility.SetBaseStat(stats, ActorStat.HITPOINTS, DnD.CalculateHitpointsAll(level, ActorUtility.GetStatBase(stats, ActorStat.HITDIE), ActorUtility.GetStatBase(stats, ActorStat.CONSTITUTION)));
            ActorUtility.SetBaseStat(stats, ActorStat.MAXHITPOINTS, ActorUtility.GetStatBase(stats, ActorStat.HITPOINTS));

            //if(debug)
            //    Debug.Log(Name + ": Recalculated HP = " + StatsBase[ActorStat.HITPOINTS] + " Recalculated MaxHP = " + StatsBase[ActorStat.MAXHITPOINTS]);
            //if(debug)
            //    DevConsole.Log(GetName() + ": <color=grey>Recalc HP: SCA " + spellcastingAbility + " + PB " + proficiencyBonus + " = " + spellAttackBonus + "</color>");

            stats.currentSpellSlots = GameInterface.Instance.DatabaseService.SpellCompendium.GetAllSpellSlotsAtLevel(stats.Class, level);

            Debug.Assert(stats.currentSpellSlots != null);
            //Debug.Assert(_currentSpellSlots.Length > 0);

            //if(debug)
            //    DevConsole.Log(Name + ": <color=grey>" + Class.ToString() + "(" + level + ") spell slots: " + string.Join(", ", currentSpellSlots) + "</color>");

            int newAPR = stats.Class == Class.MONK ? (level < 4 ? 1 : level < 7 ? 2 : level < 10 ? 3 : level < 13 ? 4 : level < 16 ? 5 : 6)
                : stats.Class == Class.FIGHTER ? (level < 6 ? 1 : level < 11 ? 2 : level < 16 ? 3 : 4)
                : 2; //! Actually 1, I just want actors to attack more often during development
                     //if(debug)
                     //{
                     //    Debug.Log(Name + ": Initial APR = " + newAPR);
                     //}
            ActorUtility.SetBaseStat(stats, ActorStat.APR, newAPR);
        }
    }

    public static class Navigation
    {
        public static void DoSideStep(Actor self, Actor target)
        {
            Vector3 newTacticalPosition = self.transform.position;
            Vector3 targetPos = target.transform.position;
            Vector3 fleeDir = (self.transform.position - targetPos).normalized;
            bool isRanged = self.ActorStats.isSpellCaster || (self.Equipment.equippedWeapon.Weapon != null && self.Equipment.equippedWeapon.Weapon.Range > 10);

            //blocked = true;
            //if(agent.attackTarget != null && inRange)
            //{

            //if(agent.m_isSpellCaster)
            //{
            //    AgentMonoController friendly = AgentFunctions.GetStrongestFriendlyInRangeNonAlloc(agent, 50, _friends);

            //    newTacticalPosition = fleeDir
            //        //* UnityEngine.Random.Range(4, 8)
            //        + (UnityEngine.Random.insideUnitSphere
            //        * UnityEngine.Random.Range(0, 3))
            //        + (friendly != null ? (friendly.transform.position - agent.transform.position) : HelperFunctions.GetSampledNavMeshPosition(agent.transform.position - agent.transform.forward));

            //    if(Vector3.Distance(newTacticalPosition, agent.transform.position) < 1.5f)
            //    {
            //        newTacticalPosition = agent.transform.position + -fleeDir + UnityEngine.Random.insideUnitSphere + fleeDir.normalized * 0.2f;
            //    }
            //}
            //else
            //{


            if(isRanged)
            {
                float fleeStrength = (1 - HelperFunctions.GetLinearDistanceAttenuation(self.transform.position, targetPos, 2, 10));
                //float centerStrength = (HelperFunctions.GetLinearDistanceAttenuation(agent.transform.position, CoverGrid.mapCenter, 2, 10));
                newTacticalPosition += (fleeDir.normalized * Random.Range(3, 15)) * fleeStrength;
                //newTacticalPosition += (CoverGrid.mapCenter - agent.transform.position).normalized * centerStrength * (fleeStrength);
            }

            //bool leftRight = UnityEngine.Random.value > 0.5f;
            else
            {
                float stepRange = 0;
                //if(HelperFunctions.LineOfSight(HelperFunctions.GetSampledNavMeshPosition(newTacticalPosition), targetPos) == false)
                //{
                for(int i = 0; i < 6; i++)
                {
                    stepRange += 1;

                    newTacticalPosition = self.transform.position + (self.transform.right * stepRange);
                    if(HelperFunctions.LineOfSight(HelperFunctions.GetSampledNavMeshPosition(newTacticalPosition), targetPos))
                    {
                        //Debug.DrawLine(agent.transform.position, newTacticalPosition, Color.green, 0.2f);
                        //Debug.DrawLine(newTacticalPosition, targetPos, Color.green, 0.2f);
                        break;
                    }
                    else
                    {
                        newTacticalPosition = self.transform.position + (-self.transform.right * stepRange);
                        if(HelperFunctions.LineOfSight(HelperFunctions.GetSampledNavMeshPosition(newTacticalPosition), targetPos))
                        {
                            //Debug.DrawLine(agent.transform.position, newTacticalPosition, Color.green, 0.2f);
                            //Debug.DrawLine(newTacticalPosition, targetPos, Color.green, 0.2f);
                            break;
                        }
                        else
                        {
                            newTacticalPosition = self.transform.position + (-self.transform.forward * stepRange);
                            if(HelperFunctions.LineOfSight(HelperFunctions.GetSampledNavMeshPosition(newTacticalPosition), targetPos))
                            {
                                //Debug.DrawLine(agent.transform.position, newTacticalPosition, Color.green, 0.2f);
                                //Debug.DrawLine(newTacticalPosition, targetPos, Color.green, 0.2f);
                                break;
                            }

                        }
                    }
                }
            }
            //}

            //Vector3 inNormalPos = (targetPos - newTacticalPosition).normalized;
            //newTacticalPosition = Vector3.Reflect(targetPos - agent.transform.position, inNormalPos); 

            NavMeshHit navHit;
            if(NavMesh.SamplePosition(newTacticalPosition, out navHit, float.PositiveInfinity, NavMesh.AllAreas))
            {
                newTacticalPosition = navHit.position;
            }

            Debug.DrawLine(self.transform.position + Vector3.up * 1, newTacticalPosition + Vector3.up * 1, Color.red, 1);

            self.SetDestination(newTacticalPosition, 0.1f, "", 1);
            //}
            //else
            //{
            //    return;
            //}
            //}

            //if(agent.isBeast                                                                                              == false && Random.value < 0.1f * (1 - HelperFunctions.GetLinearDistanceAttenuation(targetPos, agent.transform.position, 1, 20)))
            //{
            //    NPCAdvancedMotionProtocol.JumpNonLink(agent, agent.navAgent.velocity.normalized, 0.5f);
            //}


        }
    }

    public static bool IsValidTarget(Actor target)
    {
        return target.dead == false;

    }

    public static void ApplyEffect(Actor actor, Status status, int rounds)
    {
        if(actor.HasStatusEffect(status))
            return;

        actor.ApplyStatusEffect(status, rounds);
    }

    //public static void ApplyEffect(ActorStats stats, EffectData effectData, float duration)
    //{
    //    if(CheckHasEffectApplied(stats, effectData.effect))
    //        return;

    //    stats.appliedEffects.Add(effectData, duration);
    //}

    //public static bool CheckHasEffectApplied(ActorStats stats, Effects effect)
    //{
    //    foreach (EffectData eff in stats.appliedEffects.Keys)
    //    {
    //        if (eff.effect == effect)
    //        {
    //            return true;
    //        }
    //    }

    //    return false;
    //}

    public static float GetHealthPercentage(ActorStats stats)
    {
        int hp = GetModdedStat(stats, ActorStat.HITPOINTS);
        int hpMax = GetModdedStat(stats, ActorStat.MAXHITPOINTS);
        float result = ((float)hp / hpMax) * 100;
        //Debug.Log("HEALTH PERCENTAGE: " + hp + " / " + hpMax + " = " + result);
        return result;
    }

    public static void ModifyActorHealth(ActorStats stats, int amount, ModType modType)
    {
        ModifyStatBase(stats, ActorStat.HITPOINTS, amount, modType);
        stats.StatsBase[ActorStat.HITPOINTS] = Mathf.Clamp(stats.StatsBase[ActorStat.HITPOINTS], 0, stats.StatsBase[ActorStat.MAXHITPOINTS]);

    }

    //public static void ModifyActorMagicka(ActorRecord stats, int amount, ModType modType)
    //{
    //    ModifyAttributeBase(stats, ActorStats.STUN, amount, modType);
    //    stats.attributesBase[ActorStats.MAGICKA] = Mathf.Clamp(stats.attributesBase[ActorStats.MAGICKA], 0, stats.attributesBase[ActorStats.MAXMAGICKA]);

    //}

    //public static void ModifyActorStun(ActorRecord stats, int amount, ModType modType)
    //{
    //    ModifyAttributeBase(stats, ActorStats.STUN, amount, modType);
    //    stats.attributesBase[ActorStats.STUN] = Mathf.Clamp(stats.attributesBase[ActorStats.STUN], 0, stats.attributesBase[ActorStats.MAXSTUN]);

    //}

    public static void SetBaseStat(ActorStats stats, ActorStat baseStat, int value)
    {
        //if(debug)
        //{
        //    Debug.Log(GetName() + ": Setting stat " + baseStat + " to " + value);
        //}
        int diff = stats.StatsModified[baseStat] - stats.StatsBase[baseStat];

        stats.StatsBase[baseStat] = value;
        SetStatMod(stats, baseStat, value + diff);
    }

    private static void SetStatMod(ActorStats stats, ActorStat stat, int value)
    {
        //if(debug)
        //{
        //    Debug.Log(GetName() + ": Setting stat " + baseStat + " to " + value);
        //}
        stats.StatsModified[stat] = value;
    }

    public static int GetStatBase(ActorStats stats, ActorStat baseStat)
    {
        return stats.StatsBase[baseStat];
    }

    private static void SetAttributeBase(ActorStats stats, ActorStat baseStat, int value)
    {
        //if(debug)
        //{
        //    Debug.Log(GetName() + ": Setting stat " + baseStat + " to " + value);
        //}
        int diff = stats.StatsModified[baseStat] - stats.StatsBase[baseStat];

        stats.StatsBase[baseStat] = value;
        SetAttributeMod(stats, baseStat, value + diff);
    }

    private static void SetAttributeMod(ActorStats stats, ActorStat stat, int value)
    {
        //if(debug)
        //{
        //    Debug.Log(GetName() + ": Setting stat " + baseStat + " to " + value);
        //}
        stats.StatsModified[stat] = value;
    }

    public static int GetModdedStat(ActorStats stats, ActorStat stat)
    {
        return stats.StatsModified[stat];
    }

    public static int GetAttributeModValue(ActorStats stats, ActorStat stat)
    {
        return stats.StatsModified[stat] - stats.StatsBase[stat];
    }

    //public void NewStat(Stat baseStat, int value)
    //{
    //    statsBase.Add(baseStat, value);
    //    statsModified.Add(baseStat, value);
    //}

    public static void RevertToBaseStat(ActorStats stats, ActorStat stat)
    {
        SetAttributeMod(stats, stat, GetStatBase(stats, stat));
    }

    /// <summary>
    /// Change an existing base stat value based on the modifier type.
    /// </summary>
    /// <param name="baseStat"></param>
    /// <param name="modValue"></param>
    /// <param name="modifierType"></param>
    public static int ModifyStatBase(ActorStats stats, ActorStat baseStat, int modValue, ModType modifierType)
    {
        int oldmod = stats.StatsBase[baseStat];

        switch (modifierType)
        {
            case ModType.ADDITIVE:
                SetAttributeBase(stats, baseStat, stats.StatsBase[baseStat] + modValue);
                break;

            case ModType.ABSOLUTE:
                SetAttributeBase(stats, baseStat, modValue);
                break;

            case ModType.PERCENT:
                SetAttributeBase(stats, baseStat, stats.StatsBase[baseStat] + modValue / 100);
                break;
        }

        return stats.StatsBase[baseStat] - oldmod;
    }

    /// <summary>
    /// Change an existing modified stat value based on the modifier type.
    /// </summary>
    /// <param name="stat"></param>
    /// <param name="modValue"></param>
    /// <param name="modifierType"></param>
    public static int ModifyStatModded(ActorStats stats, ActorStat stat, int modValue, ModType modifierType)
    {
        int oldmod = stats.StatsModified[stat];

        switch (modifierType)
        {
            case ModType.ADDITIVE:
                SetAttributeMod(stats, stat, stats.StatsModified[stat] + modValue);
                break;

            case ModType.ABSOLUTE:
                SetAttributeMod(stats, stat, modValue);
                break;

            case ModType.PERCENT:
                SetAttributeMod(stats, stat, stats.StatsBase[stat] + modValue / 100);
                break;

            case ModType.MULTIPLICATIVE:
                SetAttributeMod(stats, stat, stats.StatsBase[stat] + modValue);
                break;
        }

        return stats.StatsModified[stat] - oldmod;
    }

    public static bool IsEnemy(ActorStats agent1, ActorStats agent2)
    {
        return FactionExentions.IsEnemy(agent1, agent2);
    }

    //public static ActorInput CreateActor(GameObject actorPrefab, bool isPlayer, Vector3 position, Quaternion rotation)
    //{
    //    GameObject spawnedActorObj = Object.Instantiate(actorPrefab, position, rotation);

    //    CharacterController characterController = spawnedActorObj.EnsureHasComponent<CharacterController>();
    //    characterController.radius = 0.3f;

    //    NavMeshAgent navAgent = null;
    //    ActorInput spawnedActor;
    //    if(isPlayer)
    //    {
    //        spawnedActor = spawnedActorObj.EnsureHasComponent<PlayerInput>();
    //        //spawnedActor.isPlayer = true;
    //    }
    //    else
    //    {
    //        spawnedActor = spawnedActorObj.EnsureHasComponent<NPCInput>();
    //        navAgent = spawnedActorObj.EnsureHasComponent<NavMeshAgent>();
    //        navAgent.updateRotation = false;
    //        navAgent.radius = 0.3f;
    //    }


    //    spawnedActor.debug = debugActor;
    //    spawnedActor.debugInput = debugActorInput;
    //    spawnedActor.debugGear = debugActorGear;
    //    spawnedActor.debugInitialization = debugInitialization;

    //    ActorStats actor = spawnedActorObj.EnsureHasComponent<ActorStats>();
    //    actor.EASet(actorFlags);
    //    actor.gender = gender;
    //    actor.faction = faction;
    //    actor.race = race;
    //    actor.m_class = actorClass;
    //    actor.isFollower = isFollower;
    //    actor.GetName() = ResourceManager.GetRandomName(gender);
    //    actor.Execute_ModifyLevel(Random.Range(levelMin, levelMax), ModType.ABSOLUTE);
    //    actor.noDeath = noDeath;
    //    actor.startPosition = transform.position;
    //    actor.Initialize();

    //    ActorSFX actorVoice = new ActorSFX(actor, -1, spawnedActorObj.transform.Find("Audio/AS Voice").GetComponent<AudioSource>());

    //    ActorCombat actorCombat = spawnedActorObj.EnsureHasComponent<ActorCombat>();
    //    NPCLocomotion animation = spawnedActorObj.EnsureHasComponent<NPCLocomotion>();
    //    //Inventory inventory = spawnedActorObj.EnsureHasComponent<Inventory>();
    //    Inventory inventory = GetComponent<Inventory>();
    //    spawnedActor.Initialize(actor, actorCombat, inventory, animation, navAgent, actorVoice);

    //    return spawnedActor;
    //}

    public static GameObject CreateDuplicate(Actor actor)
    {
        GameObject duplicateObj = Object.Instantiate(actor.gameObject, actor.transform.right, actor.transform.rotation);
        Actor actorClone = duplicateObj.ForceGetComponent<Actor>();
        Object.Destroy(duplicateObj.transform.Find("actorindicator(Clone)").gameObject);
        Object.Destroy(duplicateObj.transform.Find("unitfow(Clone)").gameObject);
        duplicateObj.layer = LayerMask.NameToLayer("Default");
        duplicateObj.name = actor.GetName() + " Clone";
        //Destroy(duplicateObj.GetComponent<ActorCombat>());
        //Equipment = null;
        //ActorUI = null;
        Object.Destroy(duplicateObj.GetComponent<Rigidbody>());
        Object.Destroy(duplicateObj.GetComponent<HighlightPlus.HighlightEffect>());
        Object.Destroy(duplicateObj.GetComponent<SmartFootstepSystem>());
        Object.Destroy(duplicateObj.GetComponent<CharacterController>());
        Object.Destroy(duplicateObj.GetComponent<NavMeshAgent>());
        Object.Destroy(duplicateObj.GetComponent<ActorHeadTracking>());
        //CharacterVoiceSet = null;
        //Destroy(duplicateObj.GetComponent<Animation>());
        ActorAnimation animComp = duplicateObj.ForceGetComponent<ActorAnimation>();
        animComp.SetAnimator(duplicateObj.GetComponent<Animator>());
        //animComp.Animator.StartPlayback();
        Object.Destroy(duplicateObj.GetComponent<Actor>());
        //animComp.SetAnimator(actor.Animation.Animator);
        animComp.ChangeForm(actor.Animation.GetCurrentAnimationSet());

        //actor.AddCompanion();

        return duplicateObj;
    }
}
