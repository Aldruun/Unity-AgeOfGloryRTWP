using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SkillBook : ScriptableObject
{
    public List<SkillPropertyDrawer> skills = new List<SkillPropertyDrawer>();

    public void Init(ref List<Skill> skillList)
    {
        //float meleeAttackRange = 2;

        //if(agent.isSpellCaster == false)
        //{
        Skill defaultAttack = Instantiate(Resources.Load<Skill_DefaultAttack>("Skills/Skill_DefaultAttack"));
        defaultAttack.hidden = true;
        //Skill defaultAttack = ScriptableObject.CreateInstance<Skill_DefaultAttack>();
        defaultAttack.Init();
        defaultAttack.priority = 999;
        //defaultAttack.skillName = "Default Attack";
        //defaultAttack.cooldown = 2;
        //defaultAttack.activationRange = agent.isSpellCaster ? 10 : meleeAttackRange;
        //defaultAttack.skillIcon = Resources.Load<Sprite>("Images/Sprites/Icons/Skill Icons/defaultattack");
        skillList.Add(defaultAttack);
        //}


        //Skill takeMPPot = Instantiate(Resources.Load<Skill_DrinkManaPotion>("Skills/Skill_DrinkManaPotion"));
        ////Skill takeMPPot = ScriptableObject.CreateInstance<Skill_DrinkManaPotion>();
        //takeMPPot.hidden = true;
        //takeMPPot.Init();
        //takeMPPot.priority = -1;
        //takeMPPot.skillName = "Drink Mana Potion";
        //skillList.Add(takeMPPot);


        Skill takeHPPot = Instantiate(Resources.Load<Skill_DrinkHealthPotion>("Skills/Skill_DrinkHealthPotion"));
        //Skill takeHPPot = ScriptableObject.CreateInstance<Skill_DrinkHealthPotion>();
        takeHPPot.hidden = true;
        takeHPPot.Init();
        takeHPPot.priority = -2;
        takeHPPot.skillName = "Drink Health Potion";
        skillList.Add(takeHPPot);

        foreach(var skillPropertyDrawer in skills)
        {
            Skill newSkill = Instantiate(skillPropertyDrawer.skill);
            newSkill.priority = skillPropertyDrawer.priority;
            newSkill.Init();
            //newSkill.vfxPoint = vfxPoint;
            //newSkill.onlyManualUse = skillPropertyDrawer.onlyManualUse;
            skillList.Add(newSkill);

            //if(agent.debugAnimation)
            //    Debug.Log($"{agent.GetName()}: <color=cyan>Adding skill '{newSkill.skillName}'</color>");
        }

        skillList.OrderBy(s => s.priority);
    }
}