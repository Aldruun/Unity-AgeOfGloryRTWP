using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Race
{
    public ActorRace race;
    public int baseSkillBonus;

    public Race(ActorRace actorRace, int baseSkillBonus)
    {
        this.race = actorRace;
        this.baseSkillBonus = baseSkillBonus;
    }
}
