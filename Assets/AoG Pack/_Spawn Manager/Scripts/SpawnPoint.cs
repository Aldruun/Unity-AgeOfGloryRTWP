using GenericFunctions;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using AoG.Serialization;

public enum SpawnType
{
    ONSTART,
    TRIGGERDISABLE,
    TRIGGERDESTROY,
    QUEST
}

public class SpawnPoint : MonoBehaviour
{
    public SpawnType SpawnType;

    public bool debugAnimation;
    public bool debugInitialization;
    public bool debugActorGear;
    public bool debugActions;

    public string UniqueID => actorConfiguration.UniqueID;
    [SerializeField] private ActorConfiguration actorConfiguration;

    [SerializeField] private bool canRespawn;
    [SerializeField] private float respawnTimer = 5f;
    [SerializeField] private bool randomFacing;

    //TODO: Set up actors within a dedicated class
    public Actor Spawn()
    {
        //if(actorConfiguration == null)
        //{
        //    Debug.LogError("Spawnpoint: Actor Configuration missing");
        //    return null;
        //}

        //if(actorConfiguration.ActorPrefab == null)
        //{
        //    Debug.LogError("Spawnpoint: Actor Configuration '" + actorConfiguration.name + "' has no actor prefab assigned");
        //    return null;
        //}

        //GameObject spawnedActorObj = Instantiate(actorConfiguration.ActorPrefab, transform.position, randomFacing ? Quaternion.Euler(0, Random.Range(-360, 360), 0) : transform.rotation);

        //CharacterController characterController = spawnedActorObj.ForceGetComponent<CharacterController>();
        //characterController.radius = 0.3f;

        //ActorInput spawnedActor;
        ////bool isPlayer = false;
        //NavMeshAgent navAgent = spawnedActorObj.GetComponent<NavMeshAgent>();
        //bool isPlayer = actorConfiguration.ActorFlags.HasFlag(ActorFlags.PC);

        //spawnedActor = spawnedActorObj.ForceGetComponent<NPCInput>();

        //if(TryGetComponent<UniqueIDGenerator>(out UniqueIDGenerator uID))
        //{
        //    spawnedActor.UniqueID = uID.ID;
        //}

        //((NPCInput)spawnedActor).AIProfile = actorConfiguration.AICombatProfile.AIProfile;

        //spawnedActor.ActorConfiguration = actorConfiguration;

        ////spawnedActor.transform.eulerAngles = new Vector3(spawnedActor.transform.eulerAngles.x, transform.eulerAngles.y, spawnedActor.transform.eulerAngles.z);

        //spawnedActor.spawnpoint = this;
        //spawnedActor.debugAnimation = debugAnimation;
        //spawnedActor.debugInput = debugActorInput;
        //spawnedActor.debugGear = debugActorGear;
        //spawnedActor.debugInitialization = debugInitialization;

        //if(gameObject.CompareTag("PlayerSpawnpoint"))
        //{
        //    enabled = false;
        //}

        return null;
    }

    internal Actor SpawnSummonedCreature()
    {
        canRespawn = false;
        return Spawn();
    }

    public void SetActorConfig(ActorConfiguration actorConfiguration)
    {
        this.actorConfiguration = actorConfiguration;
    }

    public void StartRespawnCountdown()
    {
        if(canRespawn == false)
            return;

        StartCoroutine(CR_StartRespawnCountdown());
    }

    private IEnumerator CR_StartRespawnCountdown()
    {
        yield return new WaitForSeconds(respawnTimer);

        Spawn();
    }
}
