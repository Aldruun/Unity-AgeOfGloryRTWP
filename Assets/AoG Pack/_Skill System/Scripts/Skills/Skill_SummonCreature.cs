using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(fileName = "Skill_SummonCreature", menuName = "ScriptableObjects/Skills/SummonCreatureSkill")]
public class Skill_SummonCreature : Skill
{
    [SerializeField] private ActorConfiguration ActorConfiguration;
    [SerializeField] private float creatureLifeTime = 10;
    [SerializeField] private bool useStartVFXForDespawn = true;
    [SerializeField] private string onDespawnVFX;

    private NPCInput summonedCreature;

    public override bool ConditionsMetAI(NPCInput actor)
    {
        if((summonedCreature != null && summonedCreature.dead == false) || actor.Combat.GetHostileTarget() == null)
        {
            return false;
        }

        skillTarget = actor;
        return true;
    }

    public override bool ConditionsMetPlayer(ActorInput actor)
    {
        return true;
    }

    public override void IndividualSetup(ActorInput actor)
    {
        DeliveryType = DeliveryType.InstantLocation;
        creatureLifeTime = 12 + (actor.ActorStats.Level * 6);
    }

    public override void SpawnVFX(ActorInput self, ActorInput target, Vector3 targetPosition)
    {
        // Find a free spot to spawn at.
        float radius = 2f;
        List<Vector3> spawnPositionCandidates = new List<Vector3>();
        for(int i = 0; i < 4; i++)
        {
            float angle = i * Mathf.PI * 2f / 4;
            Vector3 newPos = new Vector3(Mathf.Cos(angle) * radius, 0f, Mathf.Sin(angle) * radius);

            if(Physics.CheckSphere(newPos, 1f, 1 << LayerMask.NameToLayer("Actors") | 1 << LayerMask.NameToLayer("Obstacles")) == false)
            {
                spawnPositionCandidates.Add(newPos);
            }
        }

        Vector3 creaturePos = self.transform.position;
        if(spawnPositionCandidates.Count > 0)
        {
            creaturePos = spawnPositionCandidates[Random.Range(0, spawnPositionCandidates.Count)];
        }
        else
        {
            creaturePos = HelperFunctions.GetRandomPointOnCircle3D(creaturePos, 2f);
        }
        creaturePos = HelperFunctions.GetSampledNavMeshPosition(creaturePos);
        VFXPlayer.TriggerVFX(PoolSystem.GetPoolObject(vfxIdentifier, ObjectPoolingCategory.VFX), creaturePos, Quaternion.identity);
        SpawnCreature(self, creaturePos);
    }

    private void SpawnCreature(ActorInput self, Vector3 creaturePos)
    {
        if(ActorConfiguration == null)
        {
            Debug.LogError("ActorConfiguration null");
            return;
        }
        else if(ActorConfiguration.ActorPrefab == null)
        {
            Debug.LogError("ActorPrefab in ActorConfiguration null");
            return;
        }

        summonedCreature = (NPCInput)AoG.Core.GameInterface.Instance.DatabaseService.ActorDatabase.InstantiateAndSetUpActor(ActorConfiguration.UniqueID, creaturePos, self.transform.rotation);
        
        summonedCreature.SetEscortTarget(self);
        summonedCreature.FinalizeActor(ActorConfiguration);
        self.AddSummonedCreature(summonedCreature, self.ActorStats.HasActorFlag(ActorFlags.HOSTILE));
        //self.Combat.UnregisterCallback_OnDeath(DespawnCreatureOnDeath);
        //self.Combat.RegisterCallback_OnDeath(DespawnCreatureOnDeath);

        summonedCreature.StartCoroutine(CR_DespawnTimer(self, creatureLifeTime));
    }

    private void DespawnCreature()
    {
        Destroy(summonedCreature.gameObject);
        summonedCreature = null;
    }

    private void DespawnCreatureOnDeath(ActorInput creatureOwner)
    {
        creatureOwner.RemoveSummonedCreature(summonedCreature);
        summonedCreature.StartCoroutine(CR_DespawnTimer(creatureOwner, 0.2f));
    }

    private IEnumerator CR_DespawnTimer(ActorInput owner, float countdown)
    {
        yield return new WaitForSeconds(countdown);

        if(useStartVFXForDespawn)
        {
            VFXPlayer.TriggerVFX(PoolSystem.GetPoolObject(vfxIdentifier, ObjectPoolingCategory.VFX), summonedCreature.transform.position, Quaternion.identity);
        }
        else if(onDespawnVFX != "")
        {
            VFXPlayer.TriggerVFX(PoolSystem.GetPoolObject(onDespawnVFX, ObjectPoolingCategory.VFX), summonedCreature.transform.position, Quaternion.identity);
        }

        yield return new WaitForSeconds(0.3f);

        if(owner != null)
            owner.Combat.UnregisterCallback_OnDeath(DespawnCreatureOnDeath);
        summonedCreature.ClearEscortTarget();
        DespawnCreature();
    }
}
