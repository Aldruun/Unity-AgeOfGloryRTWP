using System.Linq;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Profiling;

public static class FactionManager
{
    [ReadOnly] private static NativeArray<RelationData> _relationDataCollection;
    private static RelationData[] data;

    //public static FactionManager current;
    public static FactionRelationManager factionRelationManager =
        Resources.Load<FactionRelationManager>("ScriptableObjects/FactionRelationManager");

    public static int GetRelations(Faction faction1, Faction faction2)
    {
        if (faction1 == faction2) return 256;

        if (factionRelationManager == null)
        {
            Debug.LogError("FactionRelationManager is null");
            return 0;
        }

        if (data == null) data = factionRelationManager.relations.Select(d => d.relationData).ToArray();
        Profiler.BeginSample("GetRelations: Job");

        for (var i = 0; i < data.Length; i++)
        {
            var rd = data[i];
            //if(rd.faction1 != faction1)
            //{
            //    continue;
            //}
            //else
            //{
            if (rd.faction2 == faction2 && rd.faction1 == faction1 ||
                rd.faction2 == faction1 && rd.faction1 == faction2)
            {
                Profiler.EndSample();
                return rd.relations;
            }

            //}
        }

        Profiler.EndSample();
        return 0;
    }
}