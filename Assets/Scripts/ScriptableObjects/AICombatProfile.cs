using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(fileName = "NPCCombatSettings", menuName = "ScriptableObjects/NPC Configuration/NPCCombatSettings")]
public class AICombatProfile : ScriptableObject
{
    [Header("Combat Settings")]
    public AIProfile AIProfile;
    public bool IsRanged;
   
    public float AggroRange = 20f;
    public float AttackRange = 10f;
    public float AttackInterval = 2f;
    public float MinDamage = 2f;
    public float MaxDamage = 4;

    [Header("NavMesh Settings")]
    public float AIUpdateInterval = 0.1f;

    public float Acceleration = 12f;
    public float AngularSpeed = 120f;
    public int AreaMask = -1;
    public int AvoidancePriority = 50;
    public float BaseOffset = 0f;
    public ObstacleAvoidanceType ObstacleAvoidanceType = ObstacleAvoidanceType.LowQualityObstacleAvoidance;

    public float Speed = 3;
    internal void ConfigureNPC(NavMeshAgent navAgent)
    {
        navAgent.updateRotation = false;
        navAgent.acceleration = Acceleration;
        navAgent.speed = Speed;
        navAgent.angularSpeed = AngularSpeed;
        navAgent.avoidancePriority = AvoidancePriority;
        navAgent.baseOffset = BaseOffset;
        navAgent.obstacleAvoidanceType = ObstacleAvoidanceType;
        navAgent.avoidancePriority = AvoidancePriority;
        navAgent.areaMask = AreaMask;
        navAgent.radius = 0.3f;
    }
}