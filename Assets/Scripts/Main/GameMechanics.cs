using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameMechanics
{
    public static int GetArmorRating(ActorStats stats)
    {
        return stats.GetStat(ActorStat.AC);
    }

}
