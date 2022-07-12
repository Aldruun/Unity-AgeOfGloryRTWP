using UnityEngine;

public class StatusEffect_Damage : StatusEffect
{
    private DamageType effectType;
    public StatusEffect_Damage(DamageType effectType, int effectValue, bool percentage, float interval, float duration)
    {
        this.effectType = effectType;
        this.effectValue = effectValue;
        this.percentage = percentage;
        this.tickTime = interval;
        this.Duration = duration;
    }

    protected override void Tick(Actor actor)
    {
        actor.Combat.ApplyStatusEffectDamage(statusEffect, effectType, effectValue, percentage);
    }

    protected override void OnBegin(Actor actor)
    {
        
    }

    public override void OnEnd(Actor actor)
    {
        //actor.ApplyStatusEffectDamage(_effectType, _savingThrowType, (int)effectValue, _percentage);
    }
}