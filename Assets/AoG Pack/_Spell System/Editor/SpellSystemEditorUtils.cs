using UnityEditor;

public class Editor_DnDUtils : Editor
{
    static SpellCompendium _spellCompendium;

    [MenuItem("AoG Utilities/Spell System Utils/Create Spell Slot Table")]
    static void SpellSystemEditorUtils()
    {
        //Dictionary<Class, Dictionary<int, int[]>> classSlotMap = new Dictionary<Class, Dictionary<int, int[]>>();
        //Dictionary<int, int[]> slotMap = new Dictionary<int, int[]>();


        //for(int i = 1; i <= 20; i++)
        //{
        //    slotMap.Add(i, new int[9]);
        //}

        SpellSlotTableData slotData = new SpellSlotTableData();
        _spellCompendium.spellSlotTable.Clear();
        //switch(characterClass)
        //{
        //case Class.Alchemist:
        slotData.characterClass = Class.ALCHEMIST;

        slotData.levels = new Grades[20];
        slotData.levels[0].slots = new[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        slotData.levels[1].slots = new[] { 2, 2, 0, 0, 0, 0, 0, 0, 0, 0, 4 };
        slotData.levels[2].slots = new[] { 2, 3, 0, 0, 0, 0, 0, 0, 0, 0, 5 };
        slotData.levels[3].slots = new[] { 2, 3, 0, 0, 0, 0, 0, 0, 0, 0, 6 };
        slotData.levels[4].slots = new[] { 2, 4, 2, 0, 0, 0, 0, 0, 0, 0, 7 };
        slotData.levels[5].slots = new[] { 3, 4, 2, 0, 0, 0, 0, 0, 0, 0, 8 };
        slotData.levels[6].slots = new[] { 3, 4, 3, 0, 0, 0, 0, 0, 0, 0, 9 };
        slotData.levels[7].slots = new[] { 3, 4, 3, 0, 0, 0, 0, 0, 0, 0, 10 };
        slotData.levels[8].slots = new[] { 3, 4, 3, 2, 0, 0, 0, 0, 0, 0, 11 };
        slotData.levels[9].slots = new[] { 4, 4, 3, 2, 0, 0, 0, 0, 0, 0, 12 };
        slotData.levels[10].slots = new[] { 4, 4, 3, 3, 0, 0, 0, 0, 0, 0, 13 };
        slotData.levels[11].slots = new[] { 4, 4, 3, 3, 0, 0, 0, 0, 0, 0, 14 };
        slotData.levels[12].slots = new[] { 4, 4, 3, 3, 1, 0, 0, 0, 0, 0, 15 };
        slotData.levels[13].slots = new[] { 5, 4, 3, 3, 1, 0, 0, 0, 0, 0, 16 };
        slotData.levels[14].slots = new[] { 5, 4, 3, 3, 2, 0, 0, 0, 0, 0, 17 };
        slotData.levels[15].slots = new[] { 6, 4, 3, 3, 2, 0, 0, 0, 0, 0, 18 };
        slotData.levels[16].slots = new[] { 6, 4, 3, 3, 3, 1, 0, 0, 0, 0, 19 };
        slotData.levels[17].slots = new[] { 7, 4, 3, 3, 3, 1, 0, 0, 0, 0, 20 };
        slotData.levels[18].slots = new[] { 7, 4, 3, 3, 3, 2, 0, 0, 0, 0, 21 };
        slotData.levels[19].slots = new[] { 8, 4, 3, 3, 3, 2, 0, 0, 0, 0, 22 };

        _spellCompendium.spellSlotTable.Add(slotData);



        //slotData.characterClass = Class.Barbarian;
        //slotData.levels = new Grades[0];
        //_spellSlotTable.spellSlotTable.Add(slotData);


        slotData = new SpellSlotTableData();
        slotData.characterClass = Class.BARD;

        slotData.levels = new Grades[20];
        slotData.levels[0].slots = new[] { 2, 2, 0, 0, 0, 0, 0, 0, 0, 0, 4 };
        slotData.levels[1].slots = new[] { 2, 3, 0, 0, 0, 0, 0, 0, 0, 0, 5 };
        slotData.levels[2].slots = new[] { 2, 4, 2, 0, 0, 0, 0, 0, 0, 0, 6 };
        slotData.levels[3].slots = new[] { 3, 4, 3, 0, 0, 0, 0, 0, 0, 0, 7 };
        slotData.levels[4].slots = new[] { 3, 4, 3, 2, 0, 0, 0, 0, 0, 0, 8 };
        slotData.levels[5].slots = new[] { 3, 4, 3, 3, 0, 0, 0, 0, 0, 0, 9 };
        slotData.levels[6].slots = new[] { 3, 4, 3, 3, 1, 0, 0, 0, 0, 0, 10 };
        slotData.levels[7].slots = new[] { 3, 4, 3, 3, 2, 0, 0, 0, 0, 0, 11 };
        slotData.levels[8].slots = new[] { 3, 4, 3, 3, 3, 1, 0, 0, 0, 0, 12 };
        slotData.levels[9].slots = new[] { 4, 4, 3, 3, 3, 2, 0, 0, 0, 0, 14 };
        slotData.levels[10].slots = new[] { 4, 4, 3, 3, 3, 2, 1, 0, 0, 0, 15 };
        slotData.levels[11].slots = new[] { 4, 4, 3, 3, 3, 2, 1, 0, 0, 0, 15 };
        slotData.levels[12].slots = new[] { 4, 4, 3, 3, 3, 2, 1, 1, 0, 0, 16 };
        slotData.levels[13].slots = new[] { 4, 4, 3, 3, 3, 2, 1, 1, 0, 0, 18 };
        slotData.levels[14].slots = new[] { 4, 4, 3, 3, 3, 2, 1, 1, 1, 0, 19 };
        slotData.levels[15].slots = new[] { 4, 4, 3, 3, 3, 2, 1, 1, 1, 0, 19 };
        slotData.levels[16].slots = new[] { 4, 4, 3, 3, 3, 2, 1, 1, 1, 1, 20 };
        slotData.levels[17].slots = new[] { 4, 4, 3, 3, 3, 3, 1, 1, 1, 1, 22 };
        slotData.levels[18].slots = new[] { 4, 4, 3, 3, 3, 3, 2, 1, 1, 1, 22 };
        slotData.levels[19].slots = new[] { 4, 4, 3, 3, 3, 3, 2, 2, 1, 1, 22 };

        _spellCompendium.spellSlotTable.Add(slotData);


        slotData = new SpellSlotTableData();
        slotData.characterClass = Class.CLERIC;

        slotData.levels = new Grades[20];
        slotData.levels[0].slots = new[] { 3, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        slotData.levels[1].slots = new[] { 3, 3, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        slotData.levels[2].slots = new[] { 3, 4, 2, 0, 0, 0, 0, 0, 0, 0, 0 };
        slotData.levels[3].slots = new[] { 4, 4, 3, 0, 0, 0, 0, 0, 0, 0, 0 };
        slotData.levels[4].slots = new[] { 4, 4, 3, 2, 0, 0, 0, 0, 0, 0, 0 };
        slotData.levels[5].slots = new[] { 4, 4, 3, 3, 0, 0, 0, 0, 0, 0, 0 };
        slotData.levels[6].slots = new[] { 4, 4, 3, 3, 1, 0, 0, 0, 0, 0, 0 };
        slotData.levels[7].slots = new[] { 4, 4, 3, 3, 2, 0, 0, 0, 0, 0, 0 };
        slotData.levels[8].slots = new[] { 4, 4, 3, 3, 3, 1, 0, 0, 0, 0, 0 };
        slotData.levels[9].slots = new[] { 5, 4, 3, 3, 3, 2, 0, 0, 0, 0, 0 };
        slotData.levels[10].slots = new[] { 5, 4, 3, 3, 3, 2, 1, 0, 0, 0, 0 };
        slotData.levels[11].slots = new[] { 5, 4, 3, 3, 3, 2, 1, 0, 0, 0, 0 };
        slotData.levels[12].slots = new[] { 5, 4, 3, 3, 3, 2, 1, 1, 0, 0, 0 };
        slotData.levels[13].slots = new[] { 5, 4, 3, 3, 3, 2, 1, 1, 0, 0, 0 };
        slotData.levels[14].slots = new[] { 5, 4, 3, 3, 3, 2, 1, 1, 1, 0, 0 };
        slotData.levels[15].slots = new[] { 5, 4, 3, 3, 3, 2, 1, 1, 1, 0, 0 };
        slotData.levels[16].slots = new[] { 5, 4, 3, 3, 3, 2, 1, 1, 1, 1, 0 };
        slotData.levels[17].slots = new[] { 5, 4, 3, 3, 3, 3, 1, 1, 1, 1, 0 };
        slotData.levels[18].slots = new[] { 5, 4, 3, 3, 3, 3, 2, 1, 1, 1, 0 };
        slotData.levels[19].slots = new[] { 5, 4, 3, 3, 3, 3, 2, 2, 1, 1, 0 };

        _spellCompendium.spellSlotTable.Add(slotData);


        slotData = new SpellSlotTableData();
        slotData.characterClass = Class.DRUID;

        slotData.levels = new Grades[20];
        slotData.levels[0].slots = new[] { 2, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        slotData.levels[1].slots = new[] { 2, 3, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        slotData.levels[2].slots = new[] { 2, 4, 2, 0, 0, 0, 0, 0, 0, 0, 0 };
        slotData.levels[3].slots = new[] { 3, 4, 3, 0, 0, 0, 0, 0, 0, 0, 0 };
        slotData.levels[4].slots = new[] { 3, 4, 3, 2, 0, 0, 0, 0, 0, 0, 0 };
        slotData.levels[5].slots = new[] { 3, 4, 3, 3, 0, 0, 0, 0, 0, 0, 0 };
        slotData.levels[6].slots = new[] { 3, 4, 3, 3, 1, 0, 0, 0, 0, 0, 0 };
        slotData.levels[7].slots = new[] { 3, 4, 3, 3, 2, 0, 0, 0, 0, 0, 0 };
        slotData.levels[8].slots = new[] { 3, 4, 3, 3, 3, 1, 0, 0, 0, 0, 0 };
        slotData.levels[9].slots = new[] { 4, 4, 3, 3, 3, 2, 0, 0, 0, 0, 0 };
        slotData.levels[10].slots = new[] { 4, 4, 3, 3, 3, 2, 1, 0, 0, 0, 0 };
        slotData.levels[11].slots = new[] { 4, 4, 3, 3, 3, 2, 1, 0, 0, 0, 0 };
        slotData.levels[12].slots = new[] { 4, 4, 3, 3, 3, 2, 1, 1, 0, 0, 0 };
        slotData.levels[13].slots = new[] { 4, 4, 3, 3, 3, 2, 1, 1, 0, 0, 0 };
        slotData.levels[14].slots = new[] { 4, 4, 3, 3, 3, 2, 1, 1, 1, 0, 0 };
        slotData.levels[15].slots = new[] { 4, 4, 3, 3, 3, 2, 1, 1, 1, 0, 0 };
        slotData.levels[16].slots = new[] { 4, 4, 3, 3, 3, 2, 1, 1, 1, 1, 0 };
        slotData.levels[17].slots = new[] { 4, 4, 3, 3, 3, 3, 1, 1, 1, 1, 0 };
        slotData.levels[18].slots = new[] { 4, 4, 3, 3, 3, 3, 2, 1, 1, 1, 0 };
        slotData.levels[19].slots = new[] { 4, 4, 3, 3, 3, 3, 2, 2, 1, 1, 0 };

        _spellCompendium.spellSlotTable.Add(slotData);


        slotData = new SpellSlotTableData();
        slotData.characterClass = Class.PALADIN;

        slotData.levels = new Grades[20];
        slotData.levels[0].slots = new[] { -1, 0, 0, 0, 0, 0, 0, 0, 0, 0, -1 };
        slotData.levels[1].slots = new[] { -1, 2, 0, 0, 0, 0, 0, 0, 0, 0, -1 };
        slotData.levels[2].slots = new[] { -1, 3, 0, 0, 0, 0, 0, 0, 0, 0, -1 };
        slotData.levels[3].slots = new[] { -1, 3, 0, 0, 0, 0, 0, 0, 0, 0, -1 };
        slotData.levels[4].slots = new[] { -1, 4, 2, 0, 0, 0, 0, 0, 0, 0, -1 };
        slotData.levels[5].slots = new[] { -1, 4, 2, 0, 0, 0, 0, 0, 0, 0, -1 };
        slotData.levels[6].slots = new[] { -1, 4, 3, 0, 0, 0, 0, 0, 0, 0, -1 };
        slotData.levels[7].slots = new[] { -1, 4, 3, 0, 0, 0, 0, 0, 0, 0, -1 };
        slotData.levels[8].slots = new[] { -1, 4, 3, 2, 0, 0, 0, 0, 0, 0, -1 };
        slotData.levels[9].slots = new[] { -1, 4, 3, 2, 0, 0, 0, 0, 0, 0, -1 };
        slotData.levels[10].slots = new[] { -1, 4, 3, 3, 0, 0, 0, 0, 0, 0, -1 };
        slotData.levels[11].slots = new[] { -1, 4, 3, 3, 0, 0, 0, 0, 0, 0, -1 };
        slotData.levels[12].slots = new[] { -1, 4, 3, 3, 1, 0, 0, 0, 0, 0, -1 };
        slotData.levels[13].slots = new[] { -1, 4, 3, 3, 1, 0, 0, 0, 0, 0, -1 };
        slotData.levels[14].slots = new[] { -1, 4, 3, 3, 2, 0, 0, 0, 0, 0, -1 };
        slotData.levels[15].slots = new[] { -1, 4, 3, 3, 2, 0, 0, 0, 0, 0, -1 };
        slotData.levels[16].slots = new[] { -1, 4, 3, 3, 3, 1, 0, 0, 0, 0, -1 };
        slotData.levels[17].slots = new[] { -1, 4, 3, 3, 3, 1, 0, 0, 0, 0, -1 };
        slotData.levels[18].slots = new[] { -1, 4, 3, 3, 3, 2, 0, 0, 0, 0, -1 };
        slotData.levels[19].slots = new[] { -1, 4, 3, 3, 3, 2, 0, 0, 0, 0, -1 };

        _spellCompendium.spellSlotTable.Add(slotData);


        slotData = new SpellSlotTableData();
        slotData.characterClass = Class.SORCERER;

        slotData.levels = new Grades[20];
        slotData.levels[0].slots = new[] { 4, 2, 0, 0, 0, 0, 0, 0, 0, 0, 2 };
        slotData.levels[1].slots = new[] { 4, 3, 0, 0, 0, 0, 0, 0, 0, 0, 3 };
        slotData.levels[2].slots = new[] { 4, 4, 2, 0, 0, 0, 0, 0, 0, 0, 4 };
        slotData.levels[3].slots = new[] { 5, 4, 3, 0, 0, 0, 0, 0, 0, 0, 5 };
        slotData.levels[4].slots = new[] { 5, 4, 3, 2, 0, 0, 0, 0, 0, 0, 6 };
        slotData.levels[5].slots = new[] { 5, 4, 3, 3, 0, 0, 0, 0, 0, 0, 7 };
        slotData.levels[6].slots = new[] { 5, 4, 3, 3, 1, 0, 0, 0, 0, 0, 8 };
        slotData.levels[7].slots = new[] { 5, 4, 3, 3, 2, 0, 0, 0, 0, 0, 9 };
        slotData.levels[8].slots = new[] { 5, 4, 3, 3, 3, 1, 0, 0, 0, 0, 10 };
        slotData.levels[9].slots = new[] { 6, 4, 3, 3, 3, 2, 0, 0, 0, 0, 11 };
        slotData.levels[10].slots = new[] { 6, 4, 3, 3, 3, 2, 1, 0, 0, 0, 12 };
        slotData.levels[11].slots = new[] { 6, 4, 3, 3, 3, 2, 1, 0, 0, 0, 12 };
        slotData.levels[12].slots = new[] { 6, 4, 3, 3, 3, 2, 1, 1, 0, 0, 13 };
        slotData.levels[13].slots = new[] { 6, 4, 3, 3, 3, 2, 1, 1, 0, 0, 13 };
        slotData.levels[14].slots = new[] { 6, 4, 3, 3, 3, 2, 1, 1, 1, 0, 14 };
        slotData.levels[15].slots = new[] { 6, 4, 3, 3, 3, 2, 1, 1, 1, 0, 14 };
        slotData.levels[16].slots = new[] { 6, 4, 3, 3, 3, 2, 1, 1, 1, 1, 15 };
        slotData.levels[17].slots = new[] { 6, 4, 3, 3, 3, 3, 1, 1, 1, 1, 15 };
        slotData.levels[18].slots = new[] { 6, 4, 3, 3, 3, 3, 2, 1, 1, 1, 15 };
        slotData.levels[19].slots = new[] { 6, 4, 3, 3, 3, 3, 2, 2, 1, 1, 15 };

        _spellCompendium.spellSlotTable.Add(slotData);


        slotData = new SpellSlotTableData();
        slotData.characterClass = Class.SHAMAN;
        // 0 = cantrips known, 1 = invokations known, 2 = spell slots, 3 = slot level, 4 = spells known
        slotData.levels = new Grades[20];
        slotData.levels[0].slots = new[] { 2, 0, 1, 1, 2 };
        slotData.levels[1].slots = new[] { 2, 2, 2, 1, 3 };
        slotData.levels[2].slots = new[] { 2, 2, 2, 2, 4 };
        slotData.levels[3].slots = new[] { 3, 2, 2, 2, 5 };
        slotData.levels[4].slots = new[] { 3, 3, 2, 3, 6 };
        slotData.levels[5].slots = new[] { 3, 3, 2, 3, 7 };
        slotData.levels[6].slots = new[] { 3, 4, 2, 4, 8 };
        slotData.levels[7].slots = new[] { 3, 4, 2, 4, 9 };
        slotData.levels[8].slots = new[] { 3, 5, 2, 5, 10 };
        slotData.levels[9].slots = new[] { 4, 5, 2, 5, 10 };
        slotData.levels[10].slots = new[] { 4, 5, 3, 5, 11 };
        slotData.levels[11].slots = new[] { 4, 6, 3, 5, 11 };
        slotData.levels[12].slots = new[] { 4, 6, 3, 5, 12 };
        slotData.levels[13].slots = new[] { 4, 6, 3, 5, 12 };
        slotData.levels[14].slots = new[] { 4, 7, 3, 5, 13 };
        slotData.levels[15].slots = new[] { 4, 7, 3, 5, 13 };
        slotData.levels[16].slots = new[] { 4, 7, 4, 5, 14 };
        slotData.levels[17].slots = new[] { 4, 8, 4, 5, 14 };
        slotData.levels[18].slots = new[] { 4, 8, 4, 5, 15 };
        slotData.levels[19].slots = new[] { 4, 8, 4, 5, 15 };

        _spellCompendium.spellSlotTable.Add(slotData);


        slotData = new SpellSlotTableData();
        slotData.characterClass = Class.MAGE;

        slotData.levels = new Grades[20];
        slotData.levels[0].slots = new[] { 3, 2, 0, 0, 0, 0, 0, 0, 0, 0 };
        slotData.levels[1].slots = new[] { 3, 3, 0, 0, 0, 0, 0, 0, 0, 0 };
        slotData.levels[2].slots = new[] { 3, 4, 2, 0, 0, 0, 0, 0, 0, 0 };
        slotData.levels[3].slots = new[] { 4, 4, 3, 0, 0, 0, 0, 0, 0, 0 };
        slotData.levels[4].slots = new[] { 4, 4, 3, 2, 0, 0, 0, 0, 0, 0 };
        slotData.levels[5].slots = new[] { 4, 4, 3, 3, 0, 0, 0, 0, 0, 0 };
        slotData.levels[6].slots = new[] { 4, 4, 3, 3, 1, 0, 0, 0, 0, 0 };
        slotData.levels[7].slots = new[] { 4, 4, 3, 3, 2, 0, 0, 0, 0, 0 };
        slotData.levels[8].slots = new[] { 4, 4, 3, 3, 3, 1, 0, 0, 0, 0 };
        slotData.levels[9].slots = new[] { 5, 4, 3, 3, 3, 2, 0, 0, 0, 0 };
        slotData.levels[10].slots = new[] { 5, 4, 3, 3, 3, 2, 1, 0, 0, 0 };
        slotData.levels[11].slots = new[] { 5, 4, 3, 3, 3, 2, 1, 0, 0, 0 };
        slotData.levels[12].slots = new[] { 5, 4, 3, 3, 3, 2, 1, 1, 0, 0 };
        slotData.levels[13].slots = new[] { 5, 4, 3, 3, 3, 2, 1, 1, 0, 0 };
        slotData.levels[14].slots = new[] { 5, 4, 3, 3, 3, 2, 1, 1, 1, 0 };
        slotData.levels[15].slots = new[] { 5, 4, 3, 3, 3, 2, 1, 1, 1, 0 };
        slotData.levels[16].slots = new[] { 5, 4, 3, 3, 3, 2, 1, 1, 1, 1 };
        slotData.levels[17].slots = new[] { 5, 4, 3, 3, 3, 3, 1, 1, 1, 1 };
        slotData.levels[18].slots = new[] { 5, 4, 3, 3, 3, 3, 2, 1, 1, 1 };
        slotData.levels[19].slots = new[] { 5, 4, 3, 3, 3, 3, 2, 2, 1, 1 };

        _spellCompendium.spellSlotTable.Add(slotData);

        EditorUtility.SetDirty(_spellCompendium);
    }

    [MenuItem("AoG Utilities/Spell System Utils/Create Spell Slot Table", true)]
    static bool ValidateIsSpellBook()
    {
        if(Selection.activeObject == null)
        {
            return false;
        }

        _spellCompendium = (SpellCompendium)Selection.activeObject;

        return _spellCompendium != null;
    }
}
