using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class QuestStage_KillContract : QuestStage
{
    //List<AgentMonoController> _entities;
    private List<GameObject> _entitiesToKill;
    private Transform _questTarget;

    [MinMax(1, 50)]
    public int levelRange = 1;
    public Faction requiredFaction;
    public Faction newFaction;
    public int numTargets;

    public string[] prefabNames;
    private SpawnPoint spawnPoint;

    public override void Init()
    {
        _entitiesToKill = new List<GameObject>();

        for(var i = 0; i < numTargets; i++)
        {
            var agentObj = PoolSystem.GetPoolObject(prefabNames[Random.Range(0, prefabNames.Length)],
                ObjectPoolingCategory.CHARACTER);
            _entitiesToKill.Add(agentObj);
        }

        //spawnPoint = PoolSystem.GetPoolObject("Spawn Point", ObjectPoolingCategory.GAMELOGIC)
        //    .GetComponent<SpawnPoint>();

        var randDirection = Random.insideUnitSphere * 1000;

        NavMeshHit navHit;

        NavMesh.SamplePosition(randDirection, out navHit, 1000, NavMesh.GetAreaFromName("Humanoid"));

        spawnPoint.transform.position = navHit.position;
        _questTarget = spawnPoint.transform;

        //spawnPoint.SpawnEntitiesForQuest(_entitiesToKill.ToArray(), numTargets, Random.Range(1, levelRange), newFaction);
    }

    public override bool Complete()
    {
        var done = true /*spawnPoint.spawnedEntities.Count == 0*/;

        //for (var i = 0; i < spawnPoint.spawnedEntities.Count; i++)
        //    if (spawnPoint.spawnedEntities[i] != null && spawnPoint.spawnedEntities[i].dead == false)
        //    {
        //        done = false;
        //        break;
        //    }

        //if (done)
        //{
        //    spawnPoint.spawnedEntities.Clear();
        //    _entitiesToKill.Clear();
        //    Destroy(spawnPoint.gameObject);
        //}

        //    Debug.Log("<color=white>QuestStage_KillContract: Quest stage complete</color>");
        return done;
    }

    public override float3 GetLocation()
    {
        //if(spawnPoint.spawnedEntities.Count > 0)
        //{
        //    return spawnPoint.spawnedEntities[0].transform.position;
        //}
        return _questTarget.position;
    }
}