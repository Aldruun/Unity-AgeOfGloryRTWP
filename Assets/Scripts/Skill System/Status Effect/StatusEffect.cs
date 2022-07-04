using System;
using UnityEngine;

[Serializable]
public abstract class StatusEffect
{
    protected float _intervalTime;
    protected bool _percentage;
    private float _tickTimer;
    public int rounds;
    public int effectValue;
    public float tickTime;
    protected bool _done;

    public Status statusEffect;

    public void Init(ActorInput actor, int rounds)
    {
        this.rounds = rounds;
        OnBegin(actor);
    }


    protected abstract void Tick(ActorInput actor);

    public void UpdateTicks(ActorInput actor)
    {
        if(tickTime <= 0)
            return;

        _tickTimer += Time.deltaTime;

        if(_tickTimer >= tickTime)
        {
            Tick(actor);
            _tickTimer = 0;
        }
    }

    protected abstract void OnBegin(ActorInput actor);

    public abstract void OnEnd(ActorInput actor);
}

public class StatusEffect_Dummy : StatusEffect
{
    protected override void OnBegin(ActorInput actor)
    {

    }

    public override void OnEnd(ActorInput actor)
    {

    }

    protected override void Tick(ActorInput actor)
    {
    }
}

public class StatusEffect_Panicked : StatusEffect
{
    protected override void OnBegin(ActorInput actor)
    {
        actor.panicked = true;
    }

    public override void OnEnd(ActorInput actor)
    {
        actor.panicked = false;
    }

    protected override void Tick(ActorInput actor)
    {
        ActorInput enemy = actor.Combat.GetHostileTarget();
        if(enemy != null)
        {
            //actor.SetDestination(actor.transform.position + (actor.transform.position - enemy.transform.position) * 10, 1);
        }
    }
}

public class StatusEffect_Stunned : StatusEffect
{
    protected override void OnBegin(ActorInput actor)
    {
        actor.stunned = true;
    }

    public override void OnEnd(ActorInput actor)
    {
        actor.stunned = false;
    }

    protected override void Tick(ActorInput actor)
    {
    }
}