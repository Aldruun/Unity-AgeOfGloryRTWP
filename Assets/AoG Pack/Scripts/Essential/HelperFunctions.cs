using System.Collections.Generic;
using System.Linq;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public static class HelperFunctions
{
    public static float GetCurrentLightLevel()
    {
        Color c = RenderSettings.ambientLight;

        return (c.r + c.g + c.b) / 3;
    }

    public static bool IsPositionOccupiedByOtherActor()
    {
        return false;
    }

    public static float GetLinearFalloff(float currentValue, float minValue, float maxValue)
    {
        float normalizedValue = Mathf.Clamp((currentValue - minValue) / (maxValue - minValue), 0, 1);
        return normalizedValue;
    }

    public static float GetLinearDistanceAttenuation(Vector3 pos1, Vector3 pos2, float minDist, float maxDist)
    {
        float dist = Vector3.Distance(pos1, pos2);
        return GetLinearFalloff(dist, minDist, maxDist);
    }

    public static void RotateTo(Transform rotator, Vector3 targetPosition, float speed, string debug = "")
    {
        Vector3 dir = targetPosition - rotator.position;

        if(dir == Vector3.zero)
        {
            return;
        }

        Quaternion targetRot = Quaternion.LookRotation(dir);
        targetRot.x = 0;
        targetRot.z = 0;

        rotator.rotation = Quaternion.RotateTowards(rotator.rotation, targetRot,
            speed * Time.deltaTime);
    }

    public static Actor GetClosestActor_WithJobs(Actor actor, float range, ActorFlags enemyFlags)
    {
        Actor closestActor = null;

        Collider[] agents = new Collider[5];
        int numHit = Physics.OverlapSphereNonAlloc(actor.transform.position, range, agents,
            1 << LayerMask.NameToLayer("Actors"));

        List<Actor> agentList = new List<Actor>();

        for(int i = 0; i < numHit; i++)
        {
            Actor a = agents[i].GetComponent<Actor>();
            if(a == actor)
            {
                continue;
            }

            if(a.ActorStats.GetActorFlags().HasFlag(enemyFlags) && /*a.isDowned == false &&*/ a.dead == false)
            {
                agentList.Add(a);
            }
        }

        float3[] enemyPositions = agentList.Select(e => (float3)e.transform.position).ToArray();

        NativeArray<float3> positions = new NativeArray<float3>(enemyPositions, Allocator.TempJob);
        NativeArray<float> distances = new NativeArray<float>(enemyPositions.Length, Allocator.TempJob);

        if(actor != null)
        {
            DistanceCheckJob job = new DistanceCheckJob
            {
                positions = positions,
                selfPosition = actor.transform.position,
                distances = distances
            };
            JobHandle jobHandle = job.Schedule(distances.Length, 1);
            jobHandle.Complete();

            float currDist = Mathf.Infinity;
            for(int i = 0; i < distances.Length; i++)
            {
                if(distances[i] < currDist)
                {
                    currDist = distances[i];
                    closestActor = agentList[i];
                }
            }
        }

        distances.Dispose();
        positions.Dispose();

        if(actor.debug)
        {
            Debug.Log(actor.GetName() + ": " + (closestActor != null ? "Found closest actor '" + closestActor.GetName() + "'" : "Closest actor = null"));
        }
        return closestActor;
    }

    public static Actor GetClosestEnemy_WithJobs(Actor agent, float range)
    {
        Actor closestEnemy = null;

        Collider[] agents = new Collider[5];
        int numHit = Physics.OverlapSphereNonAlloc(agent.transform.position, range, agents, 1 << LayerMask.NameToLayer("Actors"));

        List<Actor> agentList = new List<Actor>();

        for(int i = 0; i < numHit; i++)
        {
            //Profiler.BeginSample("AggroCtrl: GetComponent");
            Actor a = agents[i].GetComponent<Actor>();
            //Profiler.EndSample();
            if(a != agent && /*agent.dead && */agent.ActorStats.IsEnemy(a.ActorStats))
            {
                agentList.Add(a);
            }
            //Profiler.EndSample();
        }

        float3[] enemyPositions = agentList.Select(e => (float3)e.transform.position).ToArray();

        NativeArray<float3> positions = new NativeArray<float3>(enemyPositions, Allocator.TempJob);
        NativeArray<float> distances = new NativeArray<float>(enemyPositions.Length, Allocator.TempJob);

        if(agent != null)
        {
            DistanceCheckJob job = new DistanceCheckJob()
            {
                positions = positions,
                selfPosition = agent.transform.position,
                distances = distances
            };
            JobHandle jobHandle = job.Schedule(distances.Length, 1);
            jobHandle.Complete();

            float currDist = Mathf.Infinity;
            for(int i = 0; i < distances.Length; i++)
            {
                if(distances[i] < currDist)
                {

                    currDist = distances[i];
                    closestEnemy = agentList[i];
                }
            }
        }

        distances.Dispose();
        positions.Dispose();
        return closestEnemy;
    }

    public static Actor[] GetEnemiesInRangeNonAlloc(Actor self, float range, bool requireLOS)
    {
        Collider[] agents = new Collider[5];
        int numHit = Physics.OverlapSphereNonAlloc(self.transform.position, range, agents, 1 << LayerMask.NameToLayer("Actors"));

        List<Actor> agentList = new List<Actor>();

        for(int i = 0; i < numHit; i++)
        {
            Actor agent = agents[i].GetComponent<Actor>();

            if(agent.transform != self.transform && agent.isCloaked == false && self.ActorStats.IsEnemy(agent.ActorStats))
            {
                agentList.Add(agent);
            }
        }

        List<Actor> enemiesInRange = new List<Actor>();
        float3[] enemyPositions = agentList.Select(e => (float3)e.transform.position).ToArray();
        NativeArray<float3> positions = new NativeArray<float3>(enemyPositions, Allocator.TempJob);
        NativeArray<float> distances = new NativeArray<float>(enemyPositions.Length, Allocator.TempJob);
        if(self != null)
        {
            DistanceCheckJob job = new DistanceCheckJob
            {
                positions = positions,
                selfPosition = self.transform.position,
                distances = distances
            };
            JobHandle jobHandle = job.Schedule(distances.Length, 1);
            jobHandle.Complete();

            float currDist = Mathf.Infinity;
            for(int i = 0; i < distances.Length; i++)
            {
                if(distances[i] <= range)
                {
                    currDist = distances[i];
                    enemiesInRange.Add(agentList[i]);
                }
            }
        }
        distances.Dispose();
        positions.Dispose();
        return enemiesInRange.ToArray();
    }

    public static Actor GetMostWoundedInRangeNonAlloc(Actor caller, float healthThreshold,
      float range, Collider[] populatedArray)
    {
        int numHit = Physics.OverlapSphereNonAlloc(caller.transform.position, range, populatedArray,
            1 << LayerMask.NameToLayer("Actors"));

        if(numHit == 0)
        {
            return null;
        }

        List<Actor> agentList = new List<Actor>();

        for(int i = 0; i < numHit; i++)
        {
            Actor actor = populatedArray[i].GetComponent<Actor>();

            if(caller != actor && actor.HPPercentage <= healthThreshold && actor.ActorStats.isBeingHealed == false)
            {
                //if(actor != skillTarget)
                //    Debug.Log($"<color=cyan>Found wounded friend ({actor.m_agentData.Name})</color>");

                if(actor.ActorStats.IsEnemy(caller.ActorStats) == false)
                {
                    //if(actor != skillTarget)
                    //    Debug.Log($"<color=cyan>Found wounded friend ({actor.m_agentData.Name})</color>");
                    agentList.Add(actor);
                }
            }
        }

        return agentList.OrderBy(a => a.HPPercentage).FirstOrDefault();
    }

    public static Vector3 GetCursorWorldPosition()
    {
        return Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    public static bool GetTerrainMouseRaycastHit(out RaycastHit hit, Camera camera)
    {
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << LayerMask.NameToLayer("Ground")))
        {
            return true;
        }

        return false;
    }

    public static Vector3 GetTerrainCursorHitPoint(Camera camera)
    {
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << LayerMask.NameToLayer("Ground")))
        {
            //if(hit.point.y <= 0)
            //{
            return hit.point;
            //}

        }

        return Vector3.zero;
    }

    public static Vector3 GetTerrainHitPointCameraForward(Camera camera)
    {
        Ray ray = new Ray(camera.transform.position, camera.transform.forward);
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << LayerMask.NameToLayer("Ground"), QueryTriggerInteraction.Ignore))
        {
            return hit.point;
        }

        return Vector3.zero;
    }

    public static Vector3 GetGridObjectWorldPosition(int x, int z, float cellSize, Vector3 origin)
    {
        return (new Vector3(x, 0, z) * cellSize) + origin;
    }

    public static Vector3 SampledTerrainPosition(Terrain terrain, Vector3 position)
    {
        float groundLevel = terrain.SampleHeight(position);

        return new Vector3(position.x, groundLevel, position.z);

    }

    public static Vector3 SnapToTerrain(Terrain terrain, Vector3 position)
    {

        float groundLevel = terrain.SampleHeight(position);

        return SnapToNavMesh(new Vector3(position.x, groundLevel, position.z), "Walkable");

    }

    public static Vector3 SnapToNavMesh(Vector3 position, string navMeshLayerName)
    {

        float groundLevel = Terrain.activeTerrain.SampleHeight(position);

        UnityEngine.AI.NavMeshHit navHit;

        UnityEngine.AI.NavMesh.SamplePosition(position, out navHit, 50, UnityEngine.AI.NavMesh.GetAreaFromName(navMeshLayerName));

        return navHit.position;
    }

    public static Vector3 GetSampledNavMeshPosition(Vector3 targetPosition)
    {
        NavMeshHit navHit;
        if(NavMesh.SamplePosition(targetPosition, out navHit, float.PositiveInfinity, NavMesh.AllAreas))
        {
            targetPosition = navHit.position;
        }

        return targetPosition;
    }

    public static Vector3 GetSampledNavMeshPositionAroundPoint(Vector3 targetPosition, int searchCycles, float minRadiusAroundPoint, float maxRadiusAroundPoint)
    {
        List<Vector3> availablePositions = new List<Vector3>();

        Vector3 rndPoint = targetPosition + (UnityEngine.Random.insideUnitSphere * UnityEngine.Random.Range(minRadiusAroundPoint, maxRadiusAroundPoint));

        NavMeshHit navHit;
        if(NavMesh.SamplePosition(rndPoint, out navHit, float.PositiveInfinity, NavMesh.AllAreas))
        {
            targetPosition = navHit.position;
        }

        return targetPosition;
    }

    public static Vector3 GetRandomNavMeshLocation()
    {
        NavMeshTriangulation navMeshData = NavMesh.CalculateTriangulation();

        int maxIndices = navMeshData.indices.Length - 3;
        // Pick the first indice of a random triangle in the nav mesh
        int firstVertexSelected = UnityEngine.Random.Range(0, maxIndices);
        int secondVertexSelected = UnityEngine.Random.Range(0, maxIndices);
        //Spawn on Verticies
        Vector3 point = navMeshData.vertices[navMeshData.indices[firstVertexSelected]];

        Vector3 firstVertexPosition = navMeshData.vertices[navMeshData.indices[firstVertexSelected]];
        Vector3 secondVertexPosition = navMeshData.vertices[navMeshData.indices[secondVertexSelected]];
        //Eliminate points that share a similar X or Z position to stop spawining in square grid line formations
        if((int)firstVertexPosition.x == (int)secondVertexPosition.x ||
            (int)firstVertexPosition.z == (int)secondVertexPosition.z
            )
        {
            point = GetRandomNavMeshLocation(); //Re-Roll a position - I'm not happy with this recursion it could be better
        }
        else
        {
            // Select a random point on it
            point = Vector3.Lerp(
                                            firstVertexPosition,
                                            secondVertexPosition, //[t + 1]],
                                            UnityEngine.Random.Range(0.05f, 0.95f) // Not using Random.value as clumps form around Verticies 
                                        );
        }

        return point;
    }

    public static T FindClosestTarget<T>(List<T> list, Vector3 origin) where T : MonoBehaviour
    {
        return list.OrderBy(o => (o.transform.position - origin).sqrMagnitude)
            .FirstOrDefault();
    }

    //! Remember: Casts won't detect object if they start inside of it
    public static bool LineOfSightBoxcast(Transform origin, Transform target, Vector3 boxExtends,
        float startYOffset = 0, float endYOffset = 0)
    {
        Vector3 start = origin.position + (Vector3.up * startYOffset);
        Vector3 dir = target.position + (Vector3.up * endYOffset) - start;
        float dist = dir.magnitude;

        if(Physics.BoxCast(start, boxExtends, dir.normalized, out RaycastHit hit, origin.rotation, dist,
                (1 << LayerMask.NameToLayer("Actors")) | (1 << LayerMask.NameToLayer("Player")) |
                (1 << LayerMask.NameToLayer("Obstacles"))))
        {
            if(hit.collider.TryGetComponent(out IAttackable attackable))
            {
                return attackable.GetTransform() == target;
            }
        }

        return false;
    }

    public static bool LineOfSight(Vector3 start, Vector3 end)
    {
        Vector3 _start = start + (Vector3.up * 1.5f);
        Vector3 _end = end + (Vector3.up * 1.5f);
        if(Physics.Linecast(_start, _end, /*out hit,*/ ~(1 << LayerMask.NameToLayer("Actors"))))
        {
            return false;
        }
        return true;
    }

    public static bool LineOfSightH2H(Transform origin, Vector3 targetPoint, float startYOffset = 0, float endYOffset = 0)
    {
        Vector3 start = origin.position + (Vector3.up * startYOffset);
        bool allTrue = true;
        const float width = 0.2f;
        const int count = 3;
        const float spacing = width / (count - 1);
        RaycastHit hit;
        for(float i = -(width / 2); i < width; i += spacing)
        {
            Vector3 finalOffset = origin.right * i;
            Vector3 end = targetPoint + finalOffset + (Vector3.up * endYOffset);

            if(UnityEngine.Physics.Linecast(start + finalOffset, end + finalOffset, out hit,
                   (1 << LayerMask.NameToLayer("Obstacles")) | (1 << LayerMask.NameToLayer("Ground"))))
            {
                Debug.DrawLine(start, end, new Color(0.8f, 0, 0.2f));

                allTrue = false;
            }
            else
            {
                Debug.DrawLine(start, end, Color.green);
            }
        }
        return allTrue;
    }

    public static Vector3 GetPredictedPosition(Vector3 targetPosition, Vector3 shooterPosition, Vector3 targetVelocity, float projectileSpeed)
    {
        Vector3 displacement = targetPosition - shooterPosition;
        float targetMoveAngle = Vector3.Angle(-displacement, targetVelocity) * Mathf.Deg2Rad;

        if(targetVelocity.magnitude == 0 || (targetVelocity.magnitude > projectileSpeed && Mathf.Sin(targetMoveAngle) / projectileSpeed > Mathf.Cos(targetMoveAngle) / targetVelocity.magnitude))
        {
            //Debug.Log("Position prediction is not feasible.");
            return targetPosition;
        }
        //also Sine Formula
        float shootAngle = Mathf.Asin(Mathf.Sin(targetMoveAngle) * targetVelocity.magnitude / projectileSpeed);
        return targetPosition + (targetVelocity * displacement.magnitude / Mathf.Sin(Mathf.PI - targetMoveAngle - shootAngle) * Mathf.Sin(shootAngle) / targetVelocity.magnitude);
    }

    public static float GetPathLength(NavMeshAgent navAgent, Vector3 targetPosition)
    {

        NavMeshPath path = new NavMeshPath();

        if(navAgent.enabled)
        {

            navAgent.CalculatePath(targetPosition, path);
        }

        Vector3[] pathPoints = new Vector3[path.corners.Length + 2];
        pathPoints[0] = navAgent.transform.position;
        pathPoints[pathPoints.Length - 1] = targetPosition;

        for(int i = 0; i < path.corners.Length; i++)
        {

            pathPoints[i + 1] = path.corners[i];
        }

        float pathLength = 0;

        for(int i = 0; i < pathPoints.Length - 1; i++)
        {

            pathLength += Vector3.Distance(pathPoints[i], pathPoints[i + 1]);
        }

        return pathLength;
    }

    public static Vector3 GetRandomPointOnCircle3D(Vector3 center, float radius)
    {
        var ang = Random.value * 360;
        Vector3 pos;
        pos.x = center.x + (radius * Mathf.Sin(ang * Mathf.Deg2Rad));
        pos.y = center.y + (radius * Mathf.Cos(ang * Mathf.Deg2Rad));
        pos.z = center.z;
        return pos;
    }
}


[BurstCompile]
internal struct DistanceCheckJob : IJobParallelFor
{

    public NativeArray<float3> positions;
    public NativeArray<float> distances;
    public Vector3 selfPosition;

    public void Execute(int i)
    {
        distances[i] = math.distancesq(selfPosition, positions[i]);
    }
}