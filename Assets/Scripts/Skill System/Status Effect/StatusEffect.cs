using System;
using UnityEngine;

[Serializable]
public abstract class StatusEffect
{
    protected float intervalTime;
    protected bool percentage;
    private float tickTimer;
    public float Duration;
    public int effectValue;
    public float tickTime;
    protected bool done;

    public Status statusEffect;

    public void Init(Actor actor, float duration)
    {
        this.Duration = duration;
        OnBegin(actor);
    }


    protected abstract void Tick(Actor actor);

    public void UpdateTicks(Actor actor)
    {
        if(tickTime <= 0)
            return;

        tickTimer += Time.deltaTime;

        if(tickTimer >= tickTime)
        {
            Tick(actor);
            tickTimer = 0;
        }
    }

    protected abstract void OnBegin(Actor actor);

    public abstract void OnEnd(Actor actor);
}

public class StatusEffect_Dummy : StatusEffect
{
    protected override void OnBegin(Actor actor)
    {

    }

    public override void OnEnd(Actor actor)
    {

    }

    protected override void Tick(Actor actor)
    {
    }
}

public class StatusEffect_Panicked : StatusEffect
{
    protected override void OnBegin(Actor actor)
    {
        actor.panicked = true;
    }

    public override void OnEnd(Actor actor)
    {
        actor.panicked = false;
    }

    protected override void Tick(Actor actor)
    {
        Actor enemy = actor.Combat.GetHostileTarget();
        if(enemy != null)
        {
            actor.SetDestination(actor.transform.position + (actor.transform.position - enemy.transform.position) * 10, 1);
        }
    }
}

public class StatusEffect_Stunned : StatusEffect
{
    protected override void OnBegin(Actor actor)
    {
        actor.stunned = true;
    }

    public override void OnEnd(Actor actor)
    {
        actor.stunned = false;
    }

    protected override void Tick(Actor actor)
    {
    }
}