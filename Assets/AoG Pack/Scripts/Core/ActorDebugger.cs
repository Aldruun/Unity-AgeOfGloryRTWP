using AoG.Core;
using System.Collections.Generic;
using UnityEngine;

public class ActorDebugger : MonoBehaviour
{
    public List<Actor> registeredRuntimeActors;

    public Actor observedActor { get; private set; }

    private void Start()
    {

        registeredRuntimeActors = new List<Actor>();
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

    public void SetObservedPlanner(Actor observedPlanner)
    {
        this.observedActor = observedPlanner;
        GameEventSystem.ActorDebuggerOnActorChanged?.Invoke();
    }

    private void AddRuntimActor(Actor actor)
    {
        registeredRuntimeActors.Add(actor);
    }
    private void RemoveRuntimActor(Actor actor)
    {
        registeredRuntimeActors.Add(actor);
    }
}