using AoG.Core;
using System.Collections.Generic;
using UnityEngine;

public enum SpellList
{
    AcidArrow,
    AcidSplash,
    Aid,
    Alarm,
    AlterSelf,
    AnimalFriendship,
    AnimalMessenge,
    AnimalShapes,
    AnimateDead
}

public class SpellBook
{
    public List<SpellData> SpellData;

    //public List<SpellPropertyDrawer> spellPropertyDrawers;

    public SpellSlotTableData slotTable;

    public void Init(Actor actor)
    {
        SpellData = new List<SpellData>();
        List<Spell> classSpells = GetSpellsForClass(actor.ActorStats.Class);

        // At runtime, instantiate skills so you don't modify design-time originals.
        // Assumes this SkillDatabase is itself already an instantiated copy.
        for(int i = 0; i < classSpells.Count; i++)
        {
            if(UsableAtLvl(classSpells[i], actor) == false)
            {
                continue;
            }

            Spell spell = Object.Instantiate(classSpells[i]);

            spell.priority = classSpells[i].priority;
            spell.Init(actor);
            SpellData.Add(new SpellData(spell, false, 0, 0));
        }
    }

    public void RefreshSpellUsages()
    {
        foreach(SpellData spellData in SpellData)
        {
            spellData.usages = spellData.maxUsages;
        }
    }

    void UpdateMaxSpellUsages()
    {
        foreach(SpellData spellData in SpellData)
        {
            spellData.usages = spellData.maxUsages;
        }
    }

    bool UsableAtLvl(Spell spell, Actor actor)
    {
        if(GameInterface.Instance.DatabaseService.SpellCompendium.GetSpellSlotsAtLevel(actor.ActorStats.Class, actor.ActorStats.Level, spell.grade) <= 0)
        {
            return false;
        }

        return true;
    }

    List<Spell> GetAllSpellsOfMaxGrade(Actor actor)
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

    List<Spell> GetSpellsForClass(Class actorClass)
    {
        List<Spell> foundSpells = new List<Spell>();

        foreach(Spell spell in GameInterface.Instance.DatabaseService.SpellCompendium.spells)
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
}

public class SpellData
{
    public Spell spell;
    public bool available;
    public int usages;
    public int maxUsages;

    public SpellData(Spell spell, bool available, int usages, int maxUsages)
    {
        this.spell = spell;
        this.available = available;
        this.usages = usages;
        this.maxUsages = maxUsages;
    }
}