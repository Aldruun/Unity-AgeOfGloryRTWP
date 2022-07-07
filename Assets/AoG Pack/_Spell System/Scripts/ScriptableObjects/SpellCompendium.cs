using GenericFunctions;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Create SpellSlotTable", fileName = "SpellSlotTable")]
public class SpellCompendium : ScriptableObject
{
    public List<Spell> spells;

    public List<SpellSlotTableData> spellSlotTable;

    public int GetSpellSlotsAtLevel(Class characterClass, int characterLevel, int spellGrade)
    {
        SpellSlotTableData data = spellSlotTable.Where(d => d.characterClass == characterClass).FirstOrDefault();

        if(data == null)
        {
            //Debug.LogError("No spell slot data found for class '" + characterClass.ToString() + "'");
            return -1;
        }
        if(characterLevel <= 0 || characterLevel > 20)
        {
            Debug.Log("<color=" + ColorExtensions.ToRGBHex(Colors.RedCrayola) + ">characterLevel invalid (" + characterLevel + ")</color>");
        }
        if(spellGrade <= 0 || spellGrade > 9)
        {
            Debug.Log("<color=" + ColorExtensions.ToRGBHex(Colors.RedCrayola) + ">spellGrade invalid (" + spellGrade + ")</color>");
        }

        return data.levels[characterLevel - 1].slots[spellGrade];
    }

    public List<Spell> GetSpellsForClass(Class actorClass)
    {
        List<Spell> foundSpells = new List<Spell>();

        foreach(Spell spell in spells)
        {
            for(int i = 0; i < spell.targetClasses.Length; i++)
            {
                if(actorClass == spell.targetClasses[i])
                {
                    foundSpells.Add(spell);
                }
            }
        }

        return foundSpells;
    }

    public List<Spell> GetSpellsForClassAtLevel(Class actorClass, int level)
    {
        List<Spell> targetClassSpells = new List<Spell>(GetSpellsForClass(actorClass));
        List<Spell> foundSpells = new List<Spell>();
        //Debug.Log($"<color={ColorExtensions.ToRGBHex(Colors.AntiFlashWhite)}>Getting spells for class '{actorClass}' at level '{level}'</color>");
        
        foreach(Spell spell in targetClassSpells)
        {
            //Debug.Log($"<color={ColorExtensions.ToRGBHex(Colors.AntiFlashWhite)}>Checking spell '{spell.name}'</color>");
            if(UsableAtLvl(actorClass, level, spell.Grade) == false)
            {
                //Debug.Log($"<color={ColorExtensions.ToRGBHex(Colors.YellowCrayola)}>Spell '{spell.name}' of grade {spell.Grade} not usable by class '{actorClass}' at level '{level}'</color>");
                continue;
            }

            //Debug.Log($"<color={ColorExtensions.ToRGBHex(Colors.GreenCyan)}>Adding spell '{spell.name}' of grade {spell.Grade} for class '{actorClass}' at level '{level}'</color>");

            foundSpells.Add(spell);
        }

        return foundSpells;
    }

    public int[] GetAllSpellSlotsAtLevel(Class characterClass, int characterLevel)
    {
        SpellSlotTableData data = spellSlotTable.Where(d => d.characterClass == characterClass).FirstOrDefault();

        if(data == null)
        {
            //TODO Handle non spell caster classes
            //For now we just satisfy the compiler
            Debug.Log("<color=grey>No spell slot data found for class '" + characterClass.ToString() + "'</color>");
            int[] placeholder = new int[0];
            return placeholder;
        }
        if(characterLevel <= 0 || characterLevel > 20)
        {
            Debug.Log("<color=grey>characterLevel invalid (" + characterLevel + ")</color>");
        }

        return data.levels[characterLevel - 1].slots; //! spell levels from index 1 - 9
    }

    private bool UsableAtLvl(Class actorClass, int level, int spellGrade)
    {
        if(GetSpellSlotsAtLevel(actorClass, level, spellGrade) <= 0)
        {
            return false;
        }

        return true;
    }

    private List<Spell> GetAllSpellsOfMaxGrade(Actor actor)
    {
        bool arcane = false;

        switch(actor.ActorStats.Class)
        {
            case Class.BARBARIAN:
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
            case Class.ALCHEMIST:
                break;
            case Class.BARD:
            case Class.SORCERER:
            case Class.SHAMAN:
            case Class.MAGE:
                arcane = true;
                break;
        }

        return null;
    }

    public static void UpdateMaxSpellUsages(List<SpellData> spellData)
    {
        foreach(SpellData data in spellData)
        {
            data.usages = data.maxUsages;
        }
    }
}

[System.Serializable]
public class SpellSlotTableData
{
    public Class characterClass;

    public Grades[] levels;

}

[System.Serializable]
public struct Grades
{
    public int[] slots;
}
