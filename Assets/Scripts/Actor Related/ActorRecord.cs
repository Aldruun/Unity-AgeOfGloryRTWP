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
    Warrior,
    Ranger,
    Rogue,
    Wizard,
    Cleric,
    Priest,
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
    HEALTH,
    MAXHEALTH,
    AC,
    APR,
    ENCUMBRANCE,
    STRENGTH,
    DEXTERITY,
    CONSTITUTION,
    INTELLIGENCE,
    WISDOM,
    CHARISMA
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
    public int m_levelProgress;
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
   
    public bool IsRanged { get; set; }
    public float ActorRadius { get; set; }

    public ActorStats()
    {
        StatsBase = new Dictionary<ActorStat, int>(ResourceManager.actorAttributesTemplate);
        StatsModified = new Dictionary<ActorStat, int>(ResourceManager.actorAttributesTemplate);
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

    public void ApplyStatusEffectDamage(DamageType damageType, float amount)
    {
        //int def = isMagicAttack ? m_mDefence : m_pDefence;

        int finalAmount = 0;

        /*if(percentage)
        {
            //finalAmount = m_maxHealth * ((int)amount + (1 * (100 / (100 + def)))); // Exmpl: 5 * 100 / 100 * 0
            //finalAmount *= GameMaster.gameSettings.globalDmgMult;
        }
        else
        {
            //finalAmount = amount + (1 * (100 / (100 + def))); // Exmpl: 5 * 100 / 100 * 0
            //finalAmount *= GameMaster.gameSettings.globalDmgMult;
        }*/
        //VerbalConstant(VerbalConstantType.HURT);
        if (finalAmount > 0)
        {
            finalAmount = Mathf.CeilToInt(finalAmount);
        }
    }

    // public void Execute_ApplyStatusEffect(Status statusEffect, int rounds)
    // {
    //     switch (statusEffect)
    //     {
    //         case Status.BERSERK:
    //         case Status.CONFUSED:
    //         case Status.ENTANGLED:
    //         case Status.FEEBLEMINDED:
    //         case Status.HELD:
    //         case Status.PANIC:
    //         case Status.PETRIFIED:
    //         case Status.POLYMORPHED:
    //         case Status.UNCONSCIOUS:
    //         case Status.STUN:
    //             break;

    //         case Status.SLEEP:
    //             //isDowned = true;
    //             break;

    //         case Status.WEBBED:
    //             //ClearActions();
    //             break;
    //     }

    //     //_statusEffectSystem.ApplyStatusEffect(statusEffect, rounds);
    //     OnStatusEffectAdded?.Invoke(statusEffect);
    // }



    //public virtual void Die()
    //{
    //    //internalAiBehaviours.Clear();
    //    //internalAiBehaviours = null;
    //    //Destroy(this);
    //}




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

        while (currentExp >= expNeeded)
        {
            switch (Class)
            {
                case Class.Warrior:
                    IncreaseStats(0, 4, 5, 2, 0, 4, 0, 3);
                    break;

                case Class.Wizard:
                    IncreaseStats(0, 4, 5, 2, 2, 1, 5, 1);
                    break;

                case Class.Priest:
                    IncreaseStats(0, 4, 5, 1, 2, 2, 4, 1);
                    break;

                case Class.Rogue:
                    IncreaseStats(0, 4, 5, 2, 1, 7, 0, 2);
                    break;
            }

            Level++;
            //skillpoints++;
            currentExp -= expNeeded;
            //m_levelProgress = m_currentExp;
            expNeeded += expNeeded * 2;
        }
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
}