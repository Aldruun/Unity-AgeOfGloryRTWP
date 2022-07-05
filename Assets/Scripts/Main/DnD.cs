using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class DnD
{
    public static int D20()
    {
        return Random.Range(1, 21);
    }

    public static int RollBG(int dice, int size, int add)
    {
        if(dice < 1)
        {
            return add;
        }
        if(size < 1)
        {
            return add;
        }
        if(dice > 100)
        {
            return add + dice * size / 2;
        }
        for(int i = 0; i < dice; i++)
        {
            add += Random.Range(1, size + 1);
        }
        return add;
    }

    public static int Roll(int diceCount, int dieSides)
    {
        int result = 0;
        for(int i = 0; i < diceCount; i++)
        {
            result += Random.Range(1, dieSides + 1);
        }
        return result;
    }

    public static int GetHitDie(Class characterClass, ActorRace race)
    {
        //Dice die = default(Dice);
        int die = 4; // Default = small creature
        switch(race)
        {
            case ActorRace.HUMAN:
            case ActorRace.HALFORC:
            case ActorRace.HALFELF:
            case ActorRace.ELF:
            case ActorRace.DWARF:
            case ActorRace.GNOME:
            case ActorRace.HALFLING:
            case ActorRace.TIEFLING:
                switch(characterClass)
                {
                    case Class.SORCERER:
                    case Class.MAGE:
                        die = 6;
                        break;
                    case Class.DRUID:
                    case Class.SHAMAN:
                    case Class.BARD:
                    case Class.MONK:
                    case Class.THIEF:
                    case Class.CLERIC:
                        die = 8;
                        //SetStats(100, 100, 1, 5, 1, 5);
                        break;
                    case Class.RANGER:
                    case Class.FIGHTER:
                    case Class.PALADIN:
                        die = 10;
                        //SetStats(100, 100, 4, 0, 3, 1);
                        break;
                    case Class.BARBARIAN:
                        die = 12;
                        //SetStats(100, 100, 4, 0, 3, 1);
                        break;
                }
                break;
            case ActorRace.GOBLIN:
                die = 6; //TODO numDice = 2

                break;
            case ActorRace.ANIMAL:
                break;
        }

        return die;
    }

    internal static int GetRestBonus(int level, int hitDie, int constitution)
    {
        return level * (hitDie + AttributeModifier(constitution));
    }

    internal static int AttributeModifier(int attribute)
    {
        return (int)Mathf.Floor((attribute - 10f) / 2f);
    }

    internal static int StartingAttributeRoll()
    {
        return new[] { Random.Range(1, 7), Random.Range(1, 7), Random.Range(1, 7), Random.Range(1, 7) }.OrderBy(v => v).Skip(1).Sum();
    }

    //public static List<SpellSlotTable> CreateSpellSlotTables()
    //{

    //switch(characterClass)
    //{
    //    case Class.Alchemist:

    //        slotMap[1] = new[] { 0, 0, 0, 0, 0, 0, 0, 0 };
    //        slotMap[2] = new[] { 0, 0, 0, 0, 0, 0, 0, 0 };
    //        slotMap[3] = new[] { 0, 0, 0, 0, 0, 0, 0, 0 };
    //        slotMap[4] = new[] { 0, 0, 0, 0, 0, 0, 0, 0 };
    //        slotMap[5] = new[] { 0, 0, 0, 0, 0, 0, 0, 0 };
    //        slotMap[6] = new[] { 0, 0, 0, 0, 0, 0, 0, 0 };
    //        slotMap[7] = new[] { 0, 0, 0, 0, 0, 0, 0, 0 };
    //        slotMap[8] = new[] { 0, 0, 0, 0, 0, 0, 0, 0 };
    //        slotMap[9] = new[] { 0, 0, 0, 0, 0, 0, 0, 0 };
    //        slotMap[10] = new[] { 0, 0, 0, 0, 0, 0, 0, 0 };
    //        slotMap[11] = new[] { 0, 0, 0, 0, 0, 0, 0, 0 };
    //        slotMap[12] = new[] { 0, 0, 0, 0, 0, 0, 0, 0 };
    //        slotMap[13] = new[] { 0, 0, 0, 0, 0, 0, 0, 0 };
    //        slotMap[14] = new[] { 0, 0, 0, 0, 0, 0, 0, 0 };
    //        slotMap[15] = new[] { 0, 0, 0, 0, 0, 0, 0, 0 };
    //        slotMap[16] = new[] { 0, 0, 0, 0, 0, 0, 0, 0 };
    //        slotMap[17] = new[] { 0, 0, 0, 0, 0, 0, 0, 0 };
    //        slotMap[18] = new[] { 0, 0, 0, 0, 0, 0, 0, 0 };
    //        slotMap[19] = new[] { 0, 0, 0, 0, 0, 0, 0, 0 };
    //        slotMap[20] = new[] { 0, 0, 0, 0, 0, 0, 0, 0 };

    //        break;
    //    case Class.Barbarian:
    //        break;
    //    case Class.Bard:
    //        break;
    //    case Class.Cleric:
    //        break;
    //    case Class.Druid:
    //        break;
    //    case Class.Fighter:
    //        break;
    //    case Class.Monk:
    //        break;
    //    case Class.Paladin:
    //        break;
    //    case Class.Ranger:
    //        break;
    //    case Class.Rogue:
    //        break;
    //    case Class.Sorcerer:
    //        break;
    //    case Class.Warlock:
    //        break;
    //    case Class.Wizard:
    //        break;
    //}

    //return slotMap;
    //}

}

[System.Serializable]
public struct Dice
{
    public int numDice;
    public int numSides;
    public int add;

    public Dice(int numRolls, int numSides, int add)
    {
        this.numDice = numRolls;
        this.numSides = numSides;
        this.add = add;
    }

    public void Set(int numRolls, int numSides, int add)
    {
        this.numDice = numRolls;
        this.numSides = numSides;
        this.add = add;
    }

    public int Roll()
    {
        int result = 0;
        for(int i = 0; i < numDice; i++)
        {
            result += Random.Range(1, numSides + 1);
        }
        return result + add;
    }
}