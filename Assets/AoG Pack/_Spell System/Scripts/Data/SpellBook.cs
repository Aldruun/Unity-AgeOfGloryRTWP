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
    public SpellSlotTableData slotTable;

    public void SetSpells(List<Spell> spells)
    {
        SpellData = new List<SpellData>();

        for(int i = 0; i < spells.Count; i++)
        {
            Spell spell = Object.Instantiate(spells[i]);

            spell.priority = spells[i].priority;
            spell.Init();
            SpellData.Add(new SpellData(spell, false, 0, 0));
        }
    }
}

public struct SpellData
{
    public readonly Spell spell;
    public readonly bool available;
    public readonly int usages;
    public  readonly int maxUsages;

    public SpellData(Spell spell, bool available, int usages, int maxUsages)
    {
        this.spell = spell;
        this.available = available;
        this.usages = usages;
        this.maxUsages = maxUsages;
    }
}