using System;
using System.Collections.Generic;
using UnityEngine;

public class FactionRelationManager : ScriptableObject
{
    public List<FactionRelation> relations;
}

[Serializable]
public class FactionRelation
{
    //public Faction faction1;
    //public Faction faction2;

    //public int relations;
    public RelationData relationData;

    public FactionRelation(Faction faction1, Faction faction2, int relations)
    {
        relationData = new RelationData(faction1, faction2, relations);
        //this.faction1 = faction1;
        //this.faction2 = faction2;
        //this.relations = relations;
    }
}

[Serializable]
public struct RelationData
{
    public Faction faction1;
    public Faction faction2;

    public int relations;

    public RelationData(Faction faction1, Faction faction2, int relations)
    {
        this.faction1 = faction1;
        this.faction2 = faction2;
        this.relations = relations;
    }
}