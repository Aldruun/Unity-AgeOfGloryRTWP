using AoG.Core;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using AoG.Serialization;
using Newtonsoft.Json;
using System;

public enum Faction
{
    Heroes,
    Bandits,
    Insects,
    Monsters,
    Traders,
    Wildlife
    // None
}

public enum Gender
{
    Female,
    Male,
    Other
}

public enum Class
{
    ALCHEMIST,
    BARBARIAN,
    BARD,
    CLERIC,
    DRUID,
    FIGHTER,
    MONK,
    PALADIN,
    RANGER,
    THIEF,
    SORCERER,
    SHAMAN,
    MAGE,
}

[Flags]
public enum ActorRace
{
    HUMAN,
    HALFORC,
    HALFELF,
    ELF,
    DWARF,
    GNOME,
    HALFLING,
    TIEFLING,
    GOBLIN,
    ANIMAL
}

public enum ModType
{
    ADDITIVE,
    ABSOLUTE,
    PERCENT,
    MULTIPLICATIVE
}

public enum Specialization
{
    COMBAT,
    MAGIC,
    STEALTH
}

public enum ActorStat
{
    HITPOINTS,
    MAXHITPOINTS,
    MANAPOINTS,
    MAXMANAPOINTS,
    HITDIE,
    AC,
    APR,
    ENCUMBRANCE,
    STRENGTH,
    DEXTERITY,
    CONSTITUTION,
    INTELLIGENCE,
    WISDOM,
    CHARISMA,
    LUCK,
    MORALE
}

/*
 * The strength of armor is known as Armor Rating, or AR. The AR for each piece of armor is BaseAR * ( ArmorSkill / 30 ).
 * The AR for each unarmored slot, including shield, is Unarmored Skill * Unarmored Skill * 0.0065.
 */

/* Attacker hit chance
 * (Weapon Skill + (Agility / 5) + (Luck / 10)) * (0.75 + 0.5 * Current Fatigue / Maximum Fatigue) + Fortify Attack Magnitude - Blind Magnitude
 */

/* Victim evasion chance
 * ((Agility / 5) + (Luck / 10)) * (0.75 + 0.5 * Current Fatigue / Maximum Fatigue) + Sanctuary Magnitude
 */

[System.Serializable]
public class ActorStats
{
    private static Dictionary<ActorStat, int> statsDictionaryTemplate;
    public static Dictionary<ActorStat, int> StatsDictionaryTemplate
    {
        get
        {
            if(statsDictionaryTemplate == null)
            {
                statsDictionaryTemplate = new Dictionary<ActorStat, int>();

                foreach(ActorStat stat in Enum.GetValues(typeof(ActorStat)))
                {
                    statsDictionaryTemplate.Add(stat, 0);
                }
            }

            return statsDictionaryTemplate;
        }
    }

    //public AIPath pathfinder;

    ////////////////////////////////
    //? SERIALIZATION DATA
    ////////////////////////////////

    public string Name;
    public Faction Faction;
    public Gender Gender;
    public Class Class;
    public ActorRace Race;
    public Dictionary<ActorStat, int> StatsBase;
    public Dictionary<ActorStat, int> StatsModified;
    //public List<ActorSkill> skills;
    //StatusEffectSystem _statusEffectSystem;
    //public Dictionary<EffectData, float> appliedEffects;
    //public List<StatEffect> statEffects;

    public float manaRegenerationPerSecond;
    public float racialMagickaModifier;
    public float birthsignMagickaModifier;
    public string voicesetID;

    private int _actorFlags;
    private int _enemyFlags;

    public int Level;
    public int levelProgress;
    public float currentExp { get; private set; }
    public int expNeeded { get; set; }
    public int gold;
    public float stun { get; set; }
    public float maxStun { get; private set; }
    //Stack<> _aiPackageStack;
    ///////////////////////////
    // Settings
    ///////////////////////////
    //[Header("Settings")]
    public bool isEssential;
    public bool isBeast;
    public bool IsEssential;

    public bool hitSuccess;

    public int escortsCount;

    public bool isPlayer => (_actorFlags & (int)ActorFlags.PC) != 0;
    public bool IsCompanion => (_actorFlags & (int)ActorFlags.ALLY) != 0;
    public bool inCombat { get; private set; }
    public bool isSpellCaster { get; set; }
    public bool isBeingHealed { get; set; }
    public bool isBeingBuffed { get; set; }

    public SerializableVector3 currentPosition;
    public SerializableVector3 currentEulerAngles;
    public int intMod;
    public int proficiencyBonus;
    public int strMod;
    public int dexMod;
    public int conMod;
    public int wisMod;
    public int chaMod;
    public int spellcastingAbility;
    public int spellSaveDC;
    public int spellAttackModifier;
    public int Speed;
    public int[] currentSpellSlots;
    public bool CanLevelUp;

    public float ActorRadius { get; set; }
    public bool NoDead { get; internal set; }

    public float HPPercentage => (float)GetStat(ActorStat.HITPOINTS) / GetStat(ActorStat.MAXHITPOINTS);

    public Action<int, int, int, int, int, int> OnStatsChanged;
  
    /// <summary>
    /// Initializes stuff that absolutely needs to be ready from the start.
    /// </summary>
    public ActorStats()
    {
        InitializeStatDictionaries();

        _enemyFlags = (_actorFlags & (int)ActorFlags.HOSTILE) != 0 ? (int)ActorFlags.PC | (int)ActorFlags.ALLY : (int)ActorFlags.HOSTILE;

        switch (Faction)
        {
            case Faction.Heroes:
                break;

            case Faction.Bandits:
                break;

            case Faction.Insects:
            case Faction.Monsters:
                // weaponDrawn = true;
                isBeast = true;
                break;

            case Faction.Traders:
                break;

            default:
                break;
        }
    }

    /// <summary>
    /// This method is used to simulate character ActorStat increases.
    /// For leveling up, we use the 'IncreaseLevel' function.
    /// </summary>
    /// <param name="level"></param>
    public void GenerateNPCLevel(int lvl)
    {
        SetInitialStats(); // Set everything to level 1 before increasing stats

        for(int i = 1; i < lvl; i++)
        {
            if((i % 4) == 0)
                ActorUtility.Initialization.GetNPCAttributeIncreaseOnLevelUp(Class);

            Level++;
        }

        expNeeded = ActorUtility.Initialization.GetXPNeededForNextLevel(Level);
        CalculateCharacterStats(Level);
        currentExp = levelProgress = 0;
    }

    private void InitializeStatDictionaries()
    {
        StatsBase = new Dictionary<ActorStat, int>(StatsDictionaryTemplate);
        StatsModified = new Dictionary<ActorStat, int>(StatsBase);
    }

    private void CalculateCharacterStats(int level)
    {
        proficiencyBonus = Mathf.CeilToInt((level / 4) + 1);
        //if(debug)
        //    DevConsole.Log(Name + ": <color=grey>Recalc proficiency: CharLvl " + level + "/4 + 1 = " + proficiencyBonus + "</color>");

        SetBaseStat(ActorStat.HITDIE, DnD.GetHitDie(Class, Race) + level - 1);
        if((level - 1) < 0)
        {
            Debug.LogError($"Level passed was {level}");
        }

        strMod = DnD.AttributeModifier(GetBaseStat(ActorStat.STRENGTH));
        dexMod = DnD.AttributeModifier(GetBaseStat(ActorStat.DEXTERITY));
        conMod = DnD.AttributeModifier(GetBaseStat(ActorStat.CONSTITUTION));
        intMod = DnD.AttributeModifier(GetBaseStat(ActorStat.INTELLIGENCE));
        wisMod = DnD.AttributeModifier(GetBaseStat(ActorStat.WISDOM));
        chaMod = DnD.AttributeModifier(GetBaseStat(ActorStat.CHARISMA));

        switch(Class)
        {
            case Class.CLERIC:
            case Class.DRUID:
                spellcastingAbility = wisMod;
                break;
            case Class.BARD:
            case Class.PALADIN:
            case Class.SORCERER:
                spellcastingAbility = chaMod;
                break;
            case Class.ALCHEMIST:
            case Class.SHAMAN:
            case Class.MAGE:
                spellcastingAbility = intMod;
                break;
        }

        spellSaveDC = 8 + proficiencyBonus /** (1 + level - 1)*/ + spellcastingAbility;
        //if(debug)
        //    DevConsole.Log(GetName() + ": <color=grey>Recalc spellSaveDC: 8 + PB " + proficiencyBonus + " + SCA " + spellcastingAbility + " = " + spellSaveDC + "</color>");

        spellAttackModifier = spellcastingAbility + proficiencyBonus;
        //if(debug)
        //    DevConsole.Log(Name + ": <color=grey>Recalc spellAttackBonus: SCA " + spellcastingAbility + " + PB " + proficiencyBonus + " = " + spellAttackModifier + "</color>");

        SetBaseStat(ActorStat.HITPOINTS, ActorUtility.Initialization.CalculateHitpointsAll(level, GetBaseStat(ActorStat.HITDIE), GetBaseStat(ActorStat.CONSTITUTION)));
        SetBaseStat(ActorStat.MAXHITPOINTS, GetBaseStat(ActorStat.HITPOINTS));

        //if(debug)
        //    Debug.Log(Name + ": Recalculated HP = " + StatsBase[ActorStat.HITPOINTS] + " Recalculated MaxHP = " + StatsBase[ActorStat.MAXHITPOINTS]);
        //if(debug)
        //    DevConsole.Log(GetName() + ": <color=grey>Recalc HP: SCA " + spellcastingAbility + " + PB " + proficiencyBonus + " = " + spellAttackBonus + "</color>");

        currentSpellSlots = GameInterface.Instance.DatabaseService.SpellCompendium.GetAllSpellSlotsAtLevel(Class, level);

        Debug.Assert(currentSpellSlots != null);
        //Debug.Assert(_currentSpellSlots.Length > 0);

        //if(debug)
        //    DevConsole.Log(Name + ": <color=grey>" + Class.ToString() + "(" + level + ") spell slots: " + string.Join(", ", currentSpellSlots) + "</color>");

        int newAPR = Class == Class.MONK ? (level < 4 ? 1 : level < 7 ? 2 : level < 10 ? 3 : level < 13 ? 4 : level < 16 ? 5 : 6)
            : Class == Class.FIGHTER ? (level < 6 ? 1 : level < 11 ? 2 : level < 16 ? 3 : 4)
            : 2; //! Actually 1, I just want actors to attack more often during development
                 //if(debug)
                 //{
                 //    Debug.Log(Name + ": Initial APR = " + newAPR);
                 //}
        SetBaseStat(ActorStat.APR, newAPR);
    }

    private void SetInitialStats()
    {
        Level = 1;

        expNeeded = 300;

        //TODO Random for now
        ActorUtility.Initialization.GenerateRandomLvlOneCharacterSheet(
            //ref GetName(),
            Gender,
            Race,
            Class,
            ref StatsBase,
            ref Speed
            );

        StatsModified = new Dictionary<ActorStat, int>(StatsBase);
        //Hack UI Portrait Init() function takes care of updating the portrait healthbar

        switch(Faction)
        {
            case Faction.Heroes:
                CanLevelUp = true;
                break;

            case Faction.Bandits:
                break;

            case Faction.Monsters:
                break;
                //case Faction.None:
                // break;
        }

        SetBaseStat(ActorStat.APR, 1);

        //if(debug)
        //{
        //    Debug.Log(GetBaseStat(Stat.HITPOINTS));
        //}
    }

    public ActorFlags GetEnemyFlags()
    {
        return (ActorFlags)_enemyFlags;
    }

    public void SetActorFlags(ActorFlags eaValue)
    {
        _actorFlags |= (int)eaValue;
    }

    public ActorFlags GetActorFlags()
    {
        return (ActorFlags)_actorFlags;
    }

    public void OverrideActorFlags(ActorFlags eaValue)
    {
        _actorFlags = 0;
        _actorFlags |= (int)eaValue;
    }

    public bool HasActorFlag(ActorFlags eaValue)
    {
        return (_actorFlags & (int)eaValue) != 0;
    }

    //! Spell Related # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # #

    public void Execute_ModifyCurrentXP(float amount, ModType modType)
    {
        switch (modType)
        {
            case ModType.ADDITIVE:
                currentExp = currentExp + amount;
                break;

            case ModType.ABSOLUTE:
                currentExp = amount;
                break;
            //case ModType.PERCENT:
            //    break;
            case ModType.MULTIPLICATIVE:
                //currentExp = Mathf.Clamp(currentExp * amount, 0, 100);
                break;
        }

        int prevLvl = Level;

        //while (currentExp >= expNeeded)
        //{
        //    switch (Class)
        //    {
        //        case Class.Warrior:
        //            IncreaseStats(0, 4, 5, 2, 0, 4, 0, 3);
        //            break;

        //        case Class.Wizard:
        //            IncreaseStats(0, 4, 5, 2, 2, 1, 5, 1);
        //            break;

        //        case Class.Priest:
        //            IncreaseStats(0, 4, 5, 1, 2, 2, 4, 1);
        //            break;

        //        case Class.Rogue:
        //            IncreaseStats(0, 4, 5, 2, 1, 7, 0, 2);
        //            break;
        //    }

        //    Level++;
        //    //skillpoints++;
        //    currentExp -= expNeeded;
        //    //m_levelProgress = m_currentExp;
        //    expNeeded += expNeeded * 2;
        //}
    }

    private void IncreaseStats(int strength, int dexterity, int constitution, int intelligence, int wisdom, int charisma)
    {
        StatsBase[ActorStat.STRENGTH] += strength;
        StatsBase[ActorStat.DEXTERITY] += dexterity;
        StatsBase[ActorStat.CONSTITUTION] += constitution;
        StatsBase[ActorStat.INTELLIGENCE] += intelligence;
        StatsBase[ActorStat.WISDOM] += wisdom;
        StatsBase[ActorStat.CHARISMA] += charisma;

        OnStatsChanged?.Invoke(StatsBase[ActorStat.STRENGTH], StatsBase[ActorStat.DEXTERITY], StatsBase[ActorStat.CONSTITUTION], StatsBase[ActorStat.INTELLIGENCE], StatsBase[ActorStat.WISDOM], StatsBase[ActorStat.CHARISMA]);
    }

    private void SetStats(int strength, int dexterity, int constitution, int intelligence, int wisdom, int charisma)
    {
        StatsBase[ActorStat.STRENGTH] = strength;
        StatsBase[ActorStat.DEXTERITY] = dexterity;
        StatsBase[ActorStat.CONSTITUTION] = constitution;
        StatsBase[ActorStat.INTELLIGENCE] = intelligence;
        StatsBase[ActorStat.WISDOM] = wisdom;
        StatsBase[ActorStat.CHARISMA] = charisma;

        OnStatsChanged?.Invoke(strength, charisma, constitution, intelligence, wisdom, charisma);
    }

    public void Execute_ModifyLevel(int value, ModType modType)
    {
        switch (modType)
        {
            case ModType.ADDITIVE:
                Level = Mathf.Clamp(Level + value, 0, 100);
                break;

            case ModType.ABSOLUTE:
                Level = Mathf.Clamp(value, 0, 100);
                break;
            //case ModType.PERCENT:
            //    break;
            case ModType.MULTIPLICATIVE:
                Level = Mathf.Clamp(Level * value, 0, 100);
                break;
        }
    }

    private void IncreaseStats(int ac, int apr, int strength, int endurance, int agility, int intelligence, int willpower, int personality)
    {
        StatsBase[ActorStat.AC] += ac;
        StatsBase[ActorStat.APR] += apr;
        StatsBase[ActorStat.STRENGTH] += strength;
        StatsBase[ActorStat.DEXTERITY] += agility;
        StatsBase[ActorStat.CONSTITUTION] += endurance;
        StatsBase[ActorStat.INTELLIGENCE] += intelligence;
        StatsBase[ActorStat.WISDOM] += willpower;
        StatsBase[ActorStat.CHARISMA] += personality;
        //OnStatsChanged?.Invoke(
        //    attributesBase[ActorAttribute.AR],
        //    attributesBase[ActorAttribute.STRENGTH],
        //    attributesBase[ActorAttribute.STUN],
        //    attributesBase[ActorAttribute.AGILTIY],
        //    attributesBase[ActorAttribute.SPEED],
        //    attributesBase[ActorAttribute.INTELLIGENCE],
        //    attributesBase[ActorAttribute.WILLPOWER],
        //    attributesBase[ActorAttribute.PERSONALITY],
        //    attributesBase[ActorAttribute.LUCK]
        //    );
    }

    private void SetStats(int ac, int apr, int strength, int endurance, int agility, int speed, int intelligence, int willpower, int personality)
    {
        StatsBase[ActorStat.AC] = ac;
        StatsBase[ActorStat.APR] = apr;
        StatsBase[ActorStat.STRENGTH] = strength;
        StatsBase[ActorStat.CONSTITUTION] = endurance;
        StatsBase[ActorStat.DEXTERITY] = agility;
        StatsBase[ActorStat.INTELLIGENCE] = intelligence;
        StatsBase[ActorStat.WISDOM] = willpower;
        StatsBase[ActorStat.CHARISMA] = personality;

        //OnStatsChanged?.Invoke(
        //    attributesBase[ActorAttribute.AR],
        //    attributesBase[ActorAttribute.STRENGTH],
        //    attributesBase[ActorAttribute.STUN],
        //    attributesBase[ActorAttribute.AGILTIY],
        //    attributesBase[ActorAttribute.SPEED],
        //    attributesBase[ActorAttribute.INTELLIGENCE],
        //    attributesBase[ActorAttribute.WILLPOWER],
        //    attributesBase[ActorAttribute.PERSONALITY],
        //    attributesBase[ActorAttribute.LUCK]
        //    );
    }

    public int GetBaseStat(ActorStat baseStat)
    {
        return StatsBase[baseStat];
    }

    public void SetBaseStat(ActorStat baseStat, int value)
    {
        //if(debug)
        //{
        //    Debug.Log(GetName() + ": Setting ActorStat " + baseStat + " to " + value);
        //}
        int diff = StatsModified[baseStat] - StatsBase[baseStat];

        StatsBase[baseStat] = value;
        SetStatMod(baseStat, value + diff);
    }

    public int GetStat(ActorStat stat)
    {
        return StatsModified[stat];
    }

    //public void NewStat(ActorStat baseStat, int value)
    //{
    //    StatsBase.Add(baseStat, value);
    //    StatsModified.Add(baseStat, value);
    //}

    public void SetStatMod(ActorStat stat, int value)
    {
        //if(debug)
        //{
        //    Debug.Log(GetName() + ": Setting ActorStat " + baseStat + " to " + value);
        //}
        StatsModified[stat] = value;
    }

    public int GetStatModValue(ActorStat stat)
    {
        return StatsModified[stat] - StatsBase[stat];
    }

    public void RevertToBaseStat(ActorStat stat)
    {
        SetStatMod(stat, GetBaseStat(stat));
    }

    /// <summary>
    /// Change an existing base ActorStat value based on the modifier type.
    /// </summary>
    /// <param name="baseStat"></param>
    /// <param name="modValue"></param>
    /// <param name="modifierType"></param>
    public int ModifyStatBase(ActorStat baseStat, int modValue, ModType modifierType)
    {
        int oldmod = StatsBase[baseStat];

        switch(modifierType)
        {
            case ModType.ADDITIVE:
                SetBaseStat(baseStat, StatsBase[baseStat] + modValue);
                break;
            case ModType.ABSOLUTE:
                SetBaseStat(baseStat, modValue);
                break;
            case ModType.PERCENT:
                SetBaseStat(baseStat, StatsBase[baseStat] + modValue / 100);
                break;
        }

        return StatsBase[baseStat] - oldmod;
    }

    /// <summary>
    /// Change an existing modified ActorStat value based on the modifier type.
    /// </summary>
    /// <param name="stat"></param>
    /// <param name="modValue"></param>
    /// <param name="modifierType"></param>
    public int ModifyStatModded(ActorStat stat, int modValue, ModType modifierType)
    {
        int oldmod = StatsModified[stat];

        switch(modifierType)
        {
            case ModType.ADDITIVE:
                SetStatMod(stat, StatsModified[stat] + modValue);
                break;
            case ModType.ABSOLUTE:
                SetStatMod(stat, modValue);
                break;
            case ModType.PERCENT:
                SetStatMod(stat, StatsBase[stat] + modValue / 100);
                break;
            case ModType.MULTIPLICATIVE:
                SetStatMod(stat, StatsBase[stat] + modValue);
                break;
        }

        return StatsModified[stat] - oldmod;
    }

    internal void ModifyActorHP(int amount, ModType modType)
    {
        ModifyStatBase(ActorStat.HITPOINTS, amount, modType);
        StatsBase[ActorStat.HITPOINTS] = Mathf.Clamp(StatsBase[ActorStat.HITPOINTS], 0, StatsBase[ActorStat.MAXHITPOINTS]);
    }
}