using AoG.Core;
using GenericFunctions;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public static class ActorUtility
{
    public static class Initialization
    {
        internal static void GenerateRandomLvlOneCharacterSheet(Gender gender, ActorRace race, Class characterClass, ref Dictionary<ActorStat, int> statDict, ref int speed)
        {
            int str = 0;
            int dex = 0;
            int con = 0;
            int intl = 0;
            int wis = 0;
            int cha = 0;

            //if(race == ActorRace.GOBLIN && race != ActorRace.ANIMAL)
            //{
            str = DnD.StartingAttributeRoll();
            dex = DnD.StartingAttributeRoll();
            con = DnD.StartingAttributeRoll();
            intl = DnD.StartingAttributeRoll();
            wis = DnD.StartingAttributeRoll();
            cha = DnD.StartingAttributeRoll();
            //}

            int[] attributes = new int[] { str, dex, con, intl, wis, cha };

            int baseHitDie = DnD.GetHitDie(characterClass, race);

            int rndAttributeIndex = 0;

            switch(race)
            {
                case ActorRace.HUMAN:
                // Human Features: Size (Medium),
                // Base Speed (30 ft.),
                // Languages (Common, one extra language of your choice)
                // Proficiency Bonus: + 2

                case ActorRace.HALFORC:
                // Half-Orc Features: Ability Score Increase (Str +2; Con +1),
                // Size (Medium),
                // Base Speed (30 ft.),
                // Darkvision (60 ft.),
                // Menacing (proficiency in Intimidation),
                // Relentless Endurance (1/long rest),
                // Savage Attacks,
                // Languages (Common, Orc)
                // Proficiency Bonus: +2

                case ActorRace.HALFELF:
                    // Half-Elf Features: Ability Score Increase (Cha +2;
                    // +1 to two other ability scores of your choice),
                    // Size (Medium), Base Speed (30 ft.), Darkvision (60 ft.),
                    // Fey Ancestry (advantage on saving throws against being charmed;
                    // magic can't put you to sleep), Skill Versatility (proficiency in two skills of your choice),
                    // Languages (Common, Elvish, one extra language of your choice)
                    // Proficiency Bonus: + 2

                    rndAttributeIndex = Random.Range(0, 6);
                    attributes[rndAttributeIndex] += 2;

                    speed = 30;

                    break;
                case ActorRace.ELF:
                    // Elf Features: Ability Score Increase (Dex +2), Size (Medium), Base Speed (30 ft.), Darkvision (60 ft.), Keen Senses (proficiency in Perception),
                    // Fey Ancestry (advantage on saving throws against being charmed; magic can't put you to sleep),
                    // Trance (elves meditate for 4 hours instead of sleeping), Languages (Common, Elvish)

                    // Wood Elf Features: Ability Score Increase (Wis +1), Elf Weapon Training (proficiency with longsword, shortsword, shortbow and longbow),
                    // Fleet of Foot (base speed is 35 ft.), Mask of the Wild (attempt to hide when lightly obscured by foliage, heavy rain, falling snow, mist, etc)

                    // High Elf Features: Ability Score Increase (Int +1), Elf Weapon Training (proficiency with longsword, shortsword, shortbow and longbow),
                    // Cantrip (cast one cantrip of your choice from wizard list; spellcasting ability is Int), Extra Language (one extra language of your choice)

                    // Dark Elf(Drow) Features: Ability Score Increase(Cha + 1), Superior Darkvision(120 ft.), Sunlight Sensitivity, Drow Magic(cast dancing lights cantrip;
                    // spellcasting ability is Charisma)

                    speed = 30;

                    break;
                case ActorRace.DWARF:
                    // Dwarf Features: Ability Score Increase (Con +2), Size (Medium), Base Speed (25 ft.),
                    // Darkvision (60 ft.), Dwarven Resilience (advantage on saving throws against poison; resistance to poison damage),
                    // Dwarven Combat Training (proficiency with battleaxe, handaxe, light hammer and warhammer),
                    // Tool Proficiency, Stonecunning, Languages (Common, Dwarvish)

                    // Hill Dwarf Features: Ability Score Increase (Wis +1), Dwarven Toughness (+1 hit point maximum per level)

                    // Mountain Dwarf Features: Ability Score Increase (Str +1), Dwarven Armour Training (proficiency with light and medium armour)

                    baseHitDie = 8;
                    speed = 25;
                    wis += 1;
                    break;
                case ActorRace.GNOME:
                    break;
                case ActorRace.HALFLING:
                    // Halfling Features: Ability Score Increase(Dex + 2),
                    // Size(Small), Base Speed(25 ft.),
                    // Lucky (reroll 1 on d20 roll of attack roll, ability check or saving throw),
                    // Brave(advantage on saving throws against being frightened),
                    // Halfling Nimbleness(move through space of any creature larger than you),
                    // Languages(Common, Halfling)
                    // Proficiency Bonus: +2
                    rndAttributeIndex = Random.Range(0, 6);
                    attributes[rndAttributeIndex] += 2;
                    dex += 2;
                    speed = 25;


                    break;
                case ActorRace.TIEFLING:
                    break;
                case ActorRace.GOBLIN:
                    // Nimble Escape. The goblin can take the Disengage or Hide action as a bonus action on each of its turns.
                    // Scimitar. Melee Weapon Attack: +4 to hit, reach 5 ft., one target. Hit: (1d6 + 2) slashing damage.
                    // Shortbow. Ranged Weapon Attack: +4 to hit, reach 80/320 ft., one target. Hit: (1d6 + 2) piercing damage.
                    speed = 30;

                    str = 8;
                    dex = 14;
                    con = 10;
                    intl = 10;
                    wis = 8;
                    cha = 8;

                    break;
                case ActorRace.ANIMAL:
                    break;
            }

            switch(characterClass)
            {
                case Class.ALCHEMIST:

                    break;
                case Class.BARBARIAN:
                    break;
                case Class.BARD:
                    break;
                case Class.CLERIC:
                    break;
                case Class.DRUID:
                    break;
                case Class.FIGHTER:
                    break;
                case Class.MONK:
                    break;
                case Class.PALADIN:
                    break;
                case Class.RANGER:
                    break;
                case Class.THIEF:
                    break;
                case Class.SORCERER:
                    break;
                case Class.SHAMAN:
                    break;
                case Class.MAGE:
                    break;
            }

            statDict[ActorStat.STRENGTH] = str;
            statDict[ActorStat.DEXTERITY] = dex;
            statDict[ActorStat.CONSTITUTION] = con;
            statDict[ActorStat.INTELLIGENCE] = intl;
            statDict[ActorStat.WISDOM] = wis;
            statDict[ActorStat.CHARISMA] = cha;

            statDict[ActorStat.HITDIE] = baseHitDie;
            statDict[ActorStat.HITPOINTS] = statDict[ActorStat.MAXHITPOINTS] = GetFirstLevelHitpoints(baseHitDie, con);
            statDict[ActorStat.AC] = 10 + DnD.AttributeModifier(dex);
        }

        public static int CalculateHitpointsAll(int level, int baseHitDie, int constitution)
        {
            if(baseHitDie < 1)
            {
                Debug.LogError("baseHitDie too low");
            }

            int hp = GetFirstLevelHitpoints(baseHitDie, constitution);
            int conMod = DnD.AttributeModifier(constitution);

            //continue;
            //}
            //else
            //{
            int numLevelsGained = level - 1; // minus first level
            int avgHD = (baseHitDie / 2) + 1;
            int ssqHP = numLevelsGained * Mathf.Clamp(avgHD + conMod, 1, 1000);
            Debug.Log("CalculateHitpointsAll: Subsequent HP: " + numLevelsGained + " levels * (" + baseHitDie + " (avg hd) + 1) / 2 = " + avgHD + " + " + conMod + " (conMod) = " + ssqHP);
            hp += ssqHP;
            //}
            //}
            Debug.Log("CalculateHitpointsAll: New HP = " + hp);
            return hp;
        }

        private static int GetFirstLevelHitpoints(int hitDie, int constitution)
        {
            int conMod = DnD.AttributeModifier(constitution);
            int hp = hitDie + conMod;
            Debug.Log("First level HP: " + hitDie + " (hd) + " + conMod + " (conMod) = " + hp);

            return hp;
        }

        public static int GetXPNeededForNextLevel(int currActorLvl)
        {
            switch(currActorLvl)
            {
                case 1:
                    return 300;
                case 2:
                    return 900;
                case 3:
                    return 2700;
                case 4:
                    return 6500;
                case 5:
                    return 14000;
                case 6:
                    return 23000;
                case 7:
                    return 34000;
                case 8:
                    return 48000;
                case 9:
                    return 64000;
                case 10:
                    return 85000;
                case 11:
                    return 100000;
                case 12:
                    return 120000;
                case 13:
                    return 140000;
                case 14:
                    return 165000;
                case 15:
                    return 195000;
                case 16:
                    return 225000;
                case 17:
                    return 265000;
                case 18:
                    return 305000;
                case 19:
                    return 355000;
                case 20:
                    return 0;
                default:
                    Debug.LogError("SetLevelXPNeeded: Invalid actor level");
                    return -1;
            }
        }

        public static int[] GetNPCAttributeIncreaseOnLevelUp(Class actorClass)
        {
            switch(actorClass)
            {
                case Class.ALCHEMIST:
                    return new int[] { 0, 0, 0, 1, 0, 0 };
                case Class.BARBARIAN:
                    return new int[] { 1, 0, 0, 0, 0, 0 };
                case Class.BARD:
                case Class.SORCERER:
                    return new int[] { 0, 0, 0, 0, 0, 1 };
                case Class.CLERIC:
                case Class.DRUID:
                case Class.PALADIN:
                    return new int[] { 0, 0, 0, 0, 1, 0 };
                case Class.FIGHTER:
                    return new int[] { 1, 0, 0, 0, 0, 0 };
                case Class.MONK:
                    return new int[] { 0, 1, 0, 0, 0, 0 };
                case Class.RANGER:
                    return new int[] { 0, 1, 0, 0, 0, 0 };
                case Class.THIEF:
                    return new int[] { 0, 1, 0, 0, 0, 0 };
                case Class.SHAMAN:
                    return new int[] { 0, 0, 0, 1, 0, 0 };
                case Class.MAGE:
                    return new int[] { 0, 0, 0, 1, 0, 0 };
                default:
                    Debug.LogError("GetNPCAttributeIncreaseOnLevelUp: Case not handled");
                    return null;
            }
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
                float fleeStrength = 1 - HelperFunctions.GetLinearDistanceAttenuation(self.transform.position, targetPos, 2, 10);
                //float centerStrength = (HelperFunctions.GetLinearDistanceAttenuation(agent.transform.position, CoverGrid.mapCenter, 2, 10));
                newTacticalPosition += fleeDir.normalized * Random.Range(3, 15) * fleeStrength;
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

            Debug.DrawLine(self.transform.position + (Vector3.up * 1), newTacticalPosition + (Vector3.up * 1), Color.red, 1);

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
        {
            return;
        }

        actor.ApplyStatusEffect(status, rounds);
    }

    //public static float GetHealthPercentage(ActorStats stats)
    //{
    //    int hp = GetModdedStat(stats, ActorStat.HITPOINTS);
    //    int hpMax = GetModdedStat(stats, ActorStat.MAXHITPOINTS);
    //    float result = (float)hp / hpMax * 100;
    //    //Debug.Log("HEALTH PERCENTAGE: " + hp + " / " + hpMax + " = " + result);
    //    return result;
    //}

    //public static void ModifyActorHealth(ActorStats stats, int amount, ModType modType)
    //{
    //    ModifyStatBase(stats, ActorStat.HITPOINTS, amount, modType);
    //    stats.StatsBase[ActorStat.HITPOINTS] = Mathf.Clamp(stats.StatsBase[ActorStat.HITPOINTS], 0, stats.StatsBase[ActorStat.MAXHITPOINTS]);

    //}

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
