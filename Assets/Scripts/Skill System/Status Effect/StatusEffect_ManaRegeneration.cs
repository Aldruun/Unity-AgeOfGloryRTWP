using UnityEngine;

public class StatusEffect_Slow : StatusEffect
{
    public bool permanent;

    public StatusEffect_Slow() /*: base(self, interval, duration)*/
    {
    }

    protected override void Tick(ActorInput actor)
    {
        
    }

    protected override void OnBegin(ActorInput actor)
    {
        actor.Animation.Animator.speed = 0.5f;
    }

    public override void OnEnd(ActorInput actor)
    {
        actor.Animation.Animator.speed = 1;
    }
}