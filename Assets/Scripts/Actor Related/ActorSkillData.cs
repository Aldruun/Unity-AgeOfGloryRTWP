using System.Collections;
using System.Collections.Generic;
using UnityEngine;



//public enum SecondaryAttribute
//{
    
//}

//[System.Flags]
public enum Skills
{
    Alchemy,
    Alteration,
    Archery,
    Block,
    Conjuration,
    Destruction,
    Enchanting,
    HeavyArmor,
    Illusion,
    LightArmor,
    Lockpicking,
    OneHanded,
    Pickpocket,
    Restoration,
    Smithing,
    Sneak,
    Speech,
    TwoHanded
}

public enum SkillFocus
{
    MISC,
    MINOR,
    MAJOR
}

[System.Serializable]
public class ActorSkill
{
    public ActorSkillData data;
    public SkillFocus skillFocus;
    public int level;
    public float progress;

    public ActorSkill(ActorSkillData data)
    {
        this.data = data;
    }
}

[System.Serializable]
public class ActorSkillData
{
    public string Name;
    public string Description;
    //public int raceLevel;
    public Skills skill;
    public Specialization specialization;
    //public ActorAttribute attribute;
    

    public float baseProgressionMultiplicator = 1;
}
