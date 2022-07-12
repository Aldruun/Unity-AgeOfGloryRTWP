using System.Collections;
using UnityEngine;

public class StatusEffect_Sleep : StatusEffect
{

    public StatusEffect_Sleep()
    {
    }

    protected override void Tick(Actor actor)
    {
        
    }

    protected override void OnBegin(Actor actor)
    {
        actor.Combat.Execute_KnockDown(Vector3.zero, Duration);
    }

    public override void OnEnd(Actor actor)
    {
        actor.Combat.Execute_StandUp();
    }
}