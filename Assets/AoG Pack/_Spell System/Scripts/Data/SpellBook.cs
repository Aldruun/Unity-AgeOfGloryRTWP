using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellBook : ScriptableObject
{
    public List<SpellPropertyDrawer> spellPropertyDrawers;
    public List<Spell> spells;

    public List<Spell> Init(ActorStats npc, ref float minManaCost, ref SpellCastingFlags spellCastingFlags)
    {
        minManaCost = 999999;

        spells = new List<Spell>();
        // At runtime, instantiate skills so you don't modify design-time originals.
        // Assumes this SkillDatabase is itself already an instantiated copy.
        for(int i = 0; i < this.spellPropertyDrawers.Count; i++)
        {
            Spell spell = Instantiate(this.spellPropertyDrawers[i].spell);

            if(spell.cost < minManaCost)
            {
                minManaCost = spell.cost;
            }

            foreach(MagicEffect magicEffect in spell.magicEffects)
            {
                if(magicEffect.keywords.HasFlag(Keyword.HealSelf))
                {
                    spellCastingFlags |= SpellCastingFlags.CanHealSelf;
                }
                if(magicEffect.keywords.HasFlag(Keyword.HealOther))
                {
                    spellCastingFlags |= SpellCastingFlags.CanHealOther;
                }
                if(magicEffect.keywords.HasFlag(Keyword.FireDamage | Keyword.DamageHealth | Keyword.DamageFatigue | Keyword.LightningDamage | Keyword.PoisonDamage | Keyword.FrostDamage))
                {
                    spellCastingFlags |= SpellCastingFlags.HasDamageSpell;
                }
            }
            //if(spell.keywords.HasFlag(Keyword.DebuffHealth | Keyword.DebuffMana))
            //{
            //    hasDebuffSpell = true;
            //}
            //if(spell.keywords.HasFlag(Keyword.BuffHealth | Keyword.BuffMana))
            //{
            //    hasBuffSpell = true;
            //}

            spell.Init(npc);
            spells.Add(spell);
        }

        return spells;
    }

    public bool Empty()
    {
        return spells.Count == 0;
    }
}
