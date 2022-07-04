using System.Collections;
using UnityEngine;

public class StatusEffect_Sleep : StatusEffect
{

    public StatusEffect_Sleep()
    {
    }

    protected override void Tick(ActorInput actor)
    {
        
    }

    protected override void OnBegin(ActorInput actor)
    {
        actor.Combat.Execute_KnockDown(Vector3.zero, rounds);
    }

    public override void OnEnd(ActorInput actor)
    {
        actor.Combat.Execute_StandUp();
    }
}