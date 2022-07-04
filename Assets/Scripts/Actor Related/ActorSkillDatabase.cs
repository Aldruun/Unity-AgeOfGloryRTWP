using AoG.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/ActorSkillDatabase", fileName = "ActorSkillDatabase")]
public class ActorSkillDatabase : ScriptableObject
{
    public string skillAssetPath = "Assets/Resources/Actor Skills";
    public List<ActorSkillData> actorSkills;
    public static ActorSkillData[] combatSkills;
    public static ActorSkillData[] magicSkills;
    public static ActorSkillData[] stealthSkills;
    public static List<ActorSkill> actorSkillTemplates;

    public static int[,] racialBonuses;

    public static void SetActorSkills(ActorStats actor, int level)
    {

    }
    public static void SetNPCRandomSkills(ActorStats actor, int level)
    {

    }

    //public ActorSkill[] GetActorSkillsBySkillFocus(ActorRecord actor, SkillFocus skillFocus)
    //{
    //    return actor.skills.Where(s => s.skillFocus == skillFocus).ToArray();
    //}

    public ActorSkillData[] GetActorSkillsBySkillType(Skills skill)
    {
        return actorSkills.Where(s => s.skill == skill).ToArray();
    }

    public ActorSkillData[] GetActorSkillsBySpecialization(Specialization specialization)
    {
        return actorSkills.Where(s => s.specialization == specialization).ToArray();
    }

    //public ActorSkillData[] GetActorSkillsBySpecialization(ActorAttribute attribute)
    //{
    //    return actorSkills.Where(s => s.attribute == attribute).ToArray();
    //}

    /// <summary>
    /// Should be called in GameMaster.Awake()
    /// </summary>
    internal void CreateSkillTemplates(ActorSkillData[] templateCombatSkills, ActorSkillData[] templateMgicSkills, ActorSkillData[] templateStealthSkills)
    {
        combatSkills = templateCombatSkills;
        magicSkills = templateMgicSkills;
        stealthSkills = templateStealthSkills;

        actorSkillTemplates = new List<ActorSkill>();

        foreach(ActorSkillData skillData in actorSkills)
        {
            ActorSkill actorSkill = new ActorSkill(skillData);
            actorSkillTemplates.Add(actorSkill);
        }

        racialBonuses = new int[Enum.GetValues(typeof(ActorRace)).Length, Enum.GetValues(typeof(Skills)).Length];

        foreach(Skills skill in Enum.GetValues(typeof(Skills)))
        {
            switch(skill)
            {
                case Skills.Alchemy:
                    //racialBonuses[(int)Race.Altmer, (int)skill] = 15;
                    //racialBonuses[(int)Race.Argonian, (int)skill] = 15;
                    //racialBonuses[(int)Race.Bosmer, (int)skill] = 20;
                    //racialBonuses[(int)Race.Breton, (int)skill] = 20;
                    //racialBonuses[(int)Race.Dunmer, (int)skill] = 20;
                    //racialBonuses[(int)Race.Imperial, (int)skill] = 15;
                    //racialBonuses[(int)Race.Khajiit, (int)skill] = 20;
                    //racialBonuses[(int)Race.Nord, (int)skill] = 15;
                    //racialBonuses[(int)Race.Orsimer, (int)skill] = 15;
                    //racialBonuses[(int)Race.Redguard, (int)skill] = 15;
                    SetRacialSkillBonus(skill, 15, 15, 20, 20, 20, 15, 20, 15, 15, 15);
                    break;
                case Skills.Alteration:
                    SetRacialSkillBonus(skill, 20, 20, 15, 20, 20, 15, 15, 15, 15, 20);
                    break;
                case Skills.Archery:
                    SetRacialSkillBonus(skill, 15, 15, 25, 15, 15, 15, 20, 15, 15, 20);
                    break;
                case Skills.Block:
                    SetRacialSkillBonus(skill, 15, 15, 15, 15, 15, 20, 15, 20, 20, 20);
                    break;
                case Skills.Conjuration:
                    SetRacialSkillBonus(skill, 20, 15, 15, 25, 15, 15, 15, 15, 15, 15);
                    break;
                case Skills.Destruction:
                    SetRacialSkillBonus(skill, 20, 15, 15, 15, 25, 20, 15, 15, 15, 20);
                    break;
                case Skills.Enchanting:
                    SetRacialSkillBonus(skill, 20, 15, 15, 15, 15, 20, 15, 15, 20, 15);
                    break;
                case Skills.HeavyArmor:
                    SetRacialSkillBonus(skill, 15, 15, 15, 15, 15, 20, 15, 15, 25, 15);
                    break;
                case Skills.Illusion:
                    SetRacialSkillBonus(skill, 25, 15, 15, 20, 20, 15, 15, 15, 15, 15);
                    break;
                case Skills.LightArmor:
                    SetRacialSkillBonus(skill, 15, 20, 20, 15, 20, 15, 15, 20, 15, 15);
                    break;
                case Skills.Lockpicking:
                    SetRacialSkillBonus(skill, 15, 25, 20, 15, 15, 15, 20, 15, 15, 15);
                    break;
                case Skills.OneHanded:
                    SetRacialSkillBonus(skill, 15, 15, 15, 15, 15, 20, 20, 20, 20, 25);
                    break;
                case Skills.Pickpocket:
                    SetRacialSkillBonus(skill, 15, 20, 20, 15, 15, 15, 20, 15, 15, 15);
                    break;
                case Skills.Restoration:
                    SetRacialSkillBonus(skill, 20, 20, 15, 20, 15, 25, 15, 15, 15, 15);
                    break;
                case Skills.Smithing:
                    SetRacialSkillBonus(skill, 15, 15, 15, 15, 15, 15, 15, 20, 20, 20);
                    break;
                case Skills.Sneak:
                    SetRacialSkillBonus(skill, 15, 20, 20, 15, 20, 15, 25, 15, 15, 15);
                    break;
                case Skills.Speech:
                    SetRacialSkillBonus(skill, 15, 15, 15, 20, 15, 15, 15, 20, 15, 15);
                    break;
                case Skills.TwoHanded:
                    SetRacialSkillBonus(skill, 15, 15, 15, 15, 15, 15, 15, 25, 20, 15);
                    break;
            }
        }
    }

    public List<ActorSkill> GetModifiedBaseSkills(Class actorClass, ActorRace race)
    {
        List<ActorSkill> actorSkills = new List<ActorSkill>(actorSkillTemplates);
        foreach(ActorSkill skill in actorSkills)
        {
            skill.level += racialBonuses[(int)race, (int)skill.data.skill];
        }

        return actorSkills;
    }

    private static void SetRacialSkillBonus(Skills skill, int human, int elf, int orc, int goblin, int ogre, int troll, int dwarf, int angel, int undead, int demon)
    {
        racialBonuses[(int)ActorRace.Human, (int)skill] = human;
        racialBonuses[(int)ActorRace.Elf, (int)skill] = elf;
        racialBonuses[(int)ActorRace.Dwarf, (int)skill] = dwarf;
        racialBonuses[(int)ActorRace.Orc, (int)skill] = orc;
        racialBonuses[(int)ActorRace.Goblin, (int)skill] = goblin;
        racialBonuses[(int)ActorRace.Ogre, (int)skill] = ogre;
        racialBonuses[(int)ActorRace.Troll, (int)skill] = troll;
        racialBonuses[(int)ActorRace.Angel, (int)skill] = angel;
        racialBonuses[(int)ActorRace.Demon, (int)skill] = demon;
        racialBonuses[(int)ActorRace.Undead, (int)skill] = undead;
    }
}
