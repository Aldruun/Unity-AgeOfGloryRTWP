using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using GenericFunctions;

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
