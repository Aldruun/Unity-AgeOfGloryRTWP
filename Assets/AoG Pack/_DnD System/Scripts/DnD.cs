//using System.Collections.Generic;
//using System.Linq;
//using UnityEngine;

//public static class DnD
//{
//    public static int D20()
//    {
//        return Random.Range(1, 21);
//    }

//    public static int RollBG(int dice, int size, int add)
//    {
//        if(dice < 1)
//        {
//            return add;
//        }
//        if(size < 1)
//        {
//            return add;
//        }
//        if(dice > 100)
//        {
//            return add + dice * size / 2;
//        }
//        for(int i = 0; i < dice; i++)
//        {
//            add += Random.Range(1, size + 1);
//        }
//        return add;
//    }

//    public static int Roll(int diceCount, int dieSides)
//    {
//        int result = 0;
//        for(int i = 0; i < diceCount; i++)
//        {
//            result += Random.Range(1, dieSides + 1);
//        }
//        return result;
//    }

//    internal static void GenerateRandomLvlOneCharacterSheet(Gender gender, Race race, Class characterClass, ref Dictionary<Stat, int> statDict, ref int speed)
//    {
//        int str = 0;
//        int dex = 0;
//        int con = 0;
//        int intl = 0;
//        int wis = 0;
//        int cha = 0;

//        //if(race == Race.GOBLIN && race != Race.ANIMAL)
//        //{
//            str = StartingAttributeRoll();
//            dex = StartingAttributeRoll();
//            con = StartingAttributeRoll();
//            intl = StartingAttributeRoll();
//            wis = StartingAttributeRoll();
//            cha = StartingAttributeRoll();
//        //}

//        int[] attributes = new int[] { str, dex, con, intl, wis, cha };

//        int baseHitDie = GetHitDie(characterClass, race);

//        int rndAttributeIndex = 0;

//        switch(race)
//        {
//            case Race.HUMAN:
//            // Human Features: Size (Medium),
//            // Base Speed (30 ft.),
//            // Languages (Common, one extra language of your choice)
//            // Proficiency Bonus: + 2

//            case Race.HALFORC:
//            // Half-Orc Features: Ability Score Increase (Str +2; Con +1),
//            // Size (Medium),
//            // Base Speed (30 ft.),
//            // Darkvision (60 ft.),
//            // Menacing (proficiency in Intimidation),
//            // Relentless Endurance (1/long rest),
//            // Savage Attacks,
//            // Languages (Common, Orc)
//            // Proficiency Bonus: +2

//            case Race.HALFELF:
//                // Half-Elf Features: Ability Score Increase (Cha +2;
//                // +1 to two other ability scores of your choice),
//                // Size (Medium), Base Speed (30 ft.), Darkvision (60 ft.),
//                // Fey Ancestry (advantage on saving throws against being charmed;
//                // magic can't put you to sleep), Skill Versatility (proficiency in two skills of your choice),
//                // Languages (Common, Elvish, one extra language of your choice)
//                // Proficiency Bonus: + 2

//                rndAttributeIndex = Random.Range(0, 6);
//                attributes[rndAttributeIndex] += 2;

//                speed = 30;

//                break;
//            case Race.ELF:
//                // Elf Features: Ability Score Increase (Dex +2), Size (Medium), Base Speed (30 ft.), Darkvision (60 ft.), Keen Senses (proficiency in Perception),
//                // Fey Ancestry (advantage on saving throws against being charmed; magic can't put you to sleep),
//                // Trance (elves meditate for 4 hours instead of sleeping), Languages (Common, Elvish)

//                // Wood Elf Features: Ability Score Increase (Wis +1), Elf Weapon Training (proficiency with longsword, shortsword, shortbow and longbow),
//                // Fleet of Foot (base speed is 35 ft.), Mask of the Wild (attempt to hide when lightly obscured by foliage, heavy rain, falling snow, mist, etc)

//                // High Elf Features: Ability Score Increase (Int +1), Elf Weapon Training (proficiency with longsword, shortsword, shortbow and longbow),
//                // Cantrip (cast one cantrip of your choice from wizard list; spellcasting ability is Int), Extra Language (one extra language of your choice)

//                // Dark Elf(Drow) Features: Ability Score Increase(Cha + 1), Superior Darkvision(120 ft.), Sunlight Sensitivity, Drow Magic(cast dancing lights cantrip;
//                // spellcasting ability is Charisma)

//                speed = 30;

//                break;
//            case Race.DWARF:
//                // Dwarf Features: Ability Score Increase (Con +2), Size (Medium), Base Speed (25 ft.),
//                // Darkvision (60 ft.), Dwarven Resilience (advantage on saving throws against poison; resistance to poison damage),
//                // Dwarven Combat Training (proficiency with battleaxe, handaxe, light hammer and warhammer),
//                // Tool Proficiency, Stonecunning, Languages (Common, Dwarvish)

//                // Hill Dwarf Features: Ability Score Increase (Wis +1), Dwarven Toughness (+1 hit point maximum per level)

//                // Mountain Dwarf Features: Ability Score Increase (Str +1), Dwarven Armour Training (proficiency with light and medium armour)

//                baseHitDie = 8;
//                speed = 25;
//                wis += 1;
//                break;
//            case Race.GNOME:
//                break;
//            case Race.HALFLING:
//                // Halfling Features: Ability Score Increase(Dex + 2),
//                // Size(Small), Base Speed(25 ft.),
//                // Lucky (reroll 1 on d20 roll of attack roll, ability check or saving throw),
//                // Brave(advantage on saving throws against being frightened),
//                // Halfling Nimbleness(move through space of any creature larger than you),
//                // Languages(Common, Halfling)
//                // Proficiency Bonus: +2
//                rndAttributeIndex = Random.Range(0, 6);
//                attributes[rndAttributeIndex] += 2;
//                dex += 2;
//                speed = 25;


//                break;
//            case Race.TIEFLING:
//                break;
//            case Race.GOBLIN:
//                // Nimble Escape. The goblin can take the Disengage or Hide action as a bonus action on each of its turns.
//                // Scimitar. Melee Weapon Attack: +4 to hit, reach 5 ft., one target. Hit: (1d6 + 2) slashing damage.
//                // Shortbow. Ranged Weapon Attack: +4 to hit, reach 80/320 ft., one target. Hit: (1d6 + 2) piercing damage.
//                speed = 30;

//                str = 8;
//                dex = 14;
//                con = 10;
//                intl = 10;
//                wis = 8;
//                cha = 8;

//                break;
//            case Race.ANIMAL:
//                break;
//        }

//        switch(characterClass)
//        {
//            case Class.BARD:
//                break;
//            case Class.PRIESTESS:
//                break;
//            case Class.CLERIC:
//                break;
//            case Class.WARRIOR:
//                break;
//            case Class.RANGER:
//                break;
//            case Class.ROGUE:
//                break;
//            case Class.WIZARD:
//                break;
//        }

//        statDict[Stat.STRENGTH] = str;
//        statDict[Stat.DEXTERITY] = dex;
//        statDict[Stat.CONSTITUTION] = con;
//        statDict[Stat.INTELLIGENCE] = intl;
//        statDict[Stat.WISDOM] = wis;
//        statDict[Stat.CHARISMA] = cha;

//        statDict[Stat.HITDIE] = baseHitDie;
//        statDict[Stat.HITPOINTS] = statDict[Stat.MAXHITPOINTS] = CalculateFirstLevelHitpoints(baseHitDie, con);
//        //Debug.Log("RndSheet: HP: " + statDict[Stat.HITPOINTS] + " MaxHP: " + statDict[Stat.MAXHITPOINTS]);
//        statDict[Stat.AC] = 10 + AttributeModifier(dex);
//    }

//    public static int GetHitDie(Class characterClass, Race race)
//    {
//        //Dice die = default(Dice);
//        int die = 4; // Default = small creature
//        switch(race)
//        {
//            case Race.HUMAN:
//            case Race.HALFORC:
//            case Race.HALFELF:
//            case Race.ELF:
//            case Race.DWARF:
//            case Race.GNOME:
//            case Race.HALFLING:
//            case Race.TIEFLING:
//                switch(characterClass)
//                {
//                    case Class.WIZARD:
//                        die = 6;
//                        break;
//                    case Class.PRIESTESS:
//                    case Class.BARD:
//                    case Class.ROGUE:
//                    case Class.CLERIC:
//                        die = 8;
//                        //SetStats(100, 100, 1, 5, 1, 5);
//                        break;
//                    case Class.RANGER:
//                    case Class.WARRIOR:
//                        die = 10;
//                        //SetStats(100, 100, 4, 0, 3, 1);
//                        break;
//                }
//                break;
//            case Race.GOBLIN:
//                die = 6; //TODO numDice = 2

//                break;
//            case Race.ANIMAL:
//                break;
//        }

//        return die;
//    }

//    internal static int GetRestBonus(int level, int hitDie, int constitution)
//    {
//        return level * (hitDie + AttributeModifier(constitution));
//    }

//    internal static int AttributeModifier(int attribute)
//    {
//        return (int)Mathf.Floor((attribute - 10f) / 2f);
//    }

//    internal static int StartingAttributeRoll()
//    {
//        return new[] { Random.Range(1, 7), Random.Range(1, 7), Random.Range(1, 7), Random.Range(1, 7) }.OrderBy(v => v).Skip(1).Sum();
//    }

//    internal static int CalculateFirstLevelHitpoints(int hitDie, int constitution)
//    {
//        int conMod = AttributeModifier(constitution);
//        int hp = hitDie + conMod;
//        Debug.Log("First level HP: " + hitDie + " (hd) + " + conMod + " (conMod) = " + hp);

//        return hp;
//    }

//    internal static int CalculateHitpointsAll(int level, int baseHitDie, int constitution)
//    {
//        if(baseHitDie < 1)
//        {
//            Debug.LogError("baseHitDie too low");
//        }

//        int hp = CalculateFirstLevelHitpoints(baseHitDie, constitution);
//        int conMod = AttributeModifier(constitution);

//        //continue;
//        //}
//        //else
//        //{
//        int numLevelsGained = level - 1; // minus first level
//        int avgHD = ((baseHitDie) / 2) + 1;
//        int ssqHP = numLevelsGained * Mathf.Clamp(avgHD + conMod, 1, 1000);
//        Debug.Log("CalculateHitpointsAll: Subsequent HP: " + numLevelsGained + " levels * (" + baseHitDie + " (avg hd) + 1) / 2 = " + avgHD + " + " + conMod + " (conMod) = " + ssqHP);
//        hp += ssqHP;
//        //}
//        //}
//        Debug.Log("CalculateHitpointsAll: New HP = " + hp);
//        return hp;
//    }

//    //public static List<SpellSlotTable> CreateSpellSlotTables()
//    //{

//    //switch(characterClass)
//    //{
//    //    case Class.Alchemist:

//    //        slotMap[1] = new[] { 0, 0, 0, 0, 0, 0, 0, 0 };
//    //        slotMap[2] = new[] { 0, 0, 0, 0, 0, 0, 0, 0 };
//    //        slotMap[3] = new[] { 0, 0, 0, 0, 0, 0, 0, 0 };
//    //        slotMap[4] = new[] { 0, 0, 0, 0, 0, 0, 0, 0 };
//    //        slotMap[5] = new[] { 0, 0, 0, 0, 0, 0, 0, 0 };
//    //        slotMap[6] = new[] { 0, 0, 0, 0, 0, 0, 0, 0 };
//    //        slotMap[7] = new[] { 0, 0, 0, 0, 0, 0, 0, 0 };
//    //        slotMap[8] = new[] { 0, 0, 0, 0, 0, 0, 0, 0 };
//    //        slotMap[9] = new[] { 0, 0, 0, 0, 0, 0, 0, 0 };
//    //        slotMap[10] = new[] { 0, 0, 0, 0, 0, 0, 0, 0 };
//    //        slotMap[11] = new[] { 0, 0, 0, 0, 0, 0, 0, 0 };
//    //        slotMap[12] = new[] { 0, 0, 0, 0, 0, 0, 0, 0 };
//    //        slotMap[13] = new[] { 0, 0, 0, 0, 0, 0, 0, 0 };
//    //        slotMap[14] = new[] { 0, 0, 0, 0, 0, 0, 0, 0 };
//    //        slotMap[15] = new[] { 0, 0, 0, 0, 0, 0, 0, 0 };
//    //        slotMap[16] = new[] { 0, 0, 0, 0, 0, 0, 0, 0 };
//    //        slotMap[17] = new[] { 0, 0, 0, 0, 0, 0, 0, 0 };
//    //        slotMap[18] = new[] { 0, 0, 0, 0, 0, 0, 0, 0 };
//    //        slotMap[19] = new[] { 0, 0, 0, 0, 0, 0, 0, 0 };
//    //        slotMap[20] = new[] { 0, 0, 0, 0, 0, 0, 0, 0 };

//    //        break;
//    //    case Class.Barbarian:
//    //        break;
//    //    case Class.Bard:
//    //        break;
//    //    case Class.Cleric:
//    //        break;
//    //    case Class.Druid:
//    //        break;
//    //    case Class.Fighter:
//    //        break;
//    //    case Class.Monk:
//    //        break;
//    //    case Class.Paladin:
//    //        break;
//    //    case Class.Ranger:
//    //        break;
//    //    case Class.Rogue:
//    //        break;
//    //    case Class.Sorcerer:
//    //        break;
//    //    case Class.Warlock:
//    //        break;
//    //    case Class.Wizard:
//    //        break;
//    //}

//    //return slotMap;
//    //}

//}

//[System.Serializable]
//public struct Dice
//{
//    public int numDice;
//    public int numSides;
//    public int add;

//    public Dice(int numRolls, int numSides, int add)
//    {
//        this.numDice = numRolls;
//        this.numSides = numSides;
//        this.add = add;
//    }

//    public void Set(int numRolls, int numSides, int add)
//    {
//        this.numDice = numRolls;
//        this.numSides = numSides;
//        this.add = add;
//    }

//    public int Roll()
//    {
//        int result = 0;
//        for(int i = 0; i < numDice; i++)
//        {
//            result += Random.Range(1, numSides + 1);
//        }
//        return result + add;
//    }
//}