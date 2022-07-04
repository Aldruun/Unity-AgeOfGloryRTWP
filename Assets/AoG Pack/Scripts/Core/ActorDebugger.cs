using AoG.Core;
using System.Collections.Generic;
using UnityEngine;

public class ActorDebugger : MonoBehaviour
{
    internal List<ActorInput> registeredRuntimeActors;

    public ActorInput observedActor { get; private set; }

    private void Start()
    {

        registeredRuntimeActors = new List<ActorInput>();
        GameEventSystem.OnActorSpawned -= AddRuntimActor;
        GameEventSystem.OnActorSpawned += AddRuntimActor;
        GameEventSystem.OnActorDespawned -= RemoveRuntimActor;
        GameEventSystem.OnActorDespawned += RemoveRuntimActor;
        GameEventSystem.OnPlayerCreated -= SetObservedPlanner;
        GameEventSystem.OnPlayerCreated += SetObservedPlanner;
    }

    private void OnDisable()
    {
        GameEventSystem.OnPlayerCreated -= SetObservedPlanner;
        GameEventSystem.OnActorSpawned -= AddRuntimActor;
        GameEventSystem.OnActorDespawned -= RemoveRuntimActor;
    }

    public void SetObservedPlanner(ActorInput observedPlanner)
    {
        this.observedActor = observedPlanner;
        GameEventSystem.ActorDebuggerOnActorChanged?.Invoke();
    }

    private void AddRuntimActor(ActorInput actor)
    {
        registeredRuntimeActors.Add(actor);
    }
    private void RemoveRuntimActor(ActorInput actor)
    {
        registeredRuntimeActors.Add(actor);
    }
}