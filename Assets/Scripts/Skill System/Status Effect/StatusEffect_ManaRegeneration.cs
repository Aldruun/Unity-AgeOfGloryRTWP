using UnityEngine;

public class StatusEffect_Slow : StatusEffect
{
    public bool permanent;

    public StatusEffect_Slow() /*: base(self, interval, duration)*/
    {
    }

    protected override void Tick(Actor actor)
    {
        
    }

    protected override void OnBegin(Actor actor)
    {
        actor.Animation.Animator.speed = 0.5f;
    }

    public override void OnEnd(Actor actor)
    {
        actor.Animation.Animator.speed = 1;
    }
}