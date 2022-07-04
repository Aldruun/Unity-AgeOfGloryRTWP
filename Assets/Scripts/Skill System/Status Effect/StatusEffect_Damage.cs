using UnityEngine;

public class StatusEffect_Damage : StatusEffect
{
    private DamageType _effectType;
    public StatusEffect_Damage(DamageType effectType, int effectValue, bool percentage, float interval, int rounds)
    {
        _effectType = effectType;
        this.effectValue = effectValue;
        _percentage = percentage;
        this.tickTime = interval;
        this.rounds = rounds;
    }

    protected override void Tick(ActorInput actor)
    {
        actor.ActorStats.ApplyStatusEffectDamage(_effectType, effectValue);
    }

    protected override void OnBegin(ActorInput actor)
    {
        
    }

    public override void OnEnd(ActorInput actor)
    {
        //actor.ApplyStatusEffectDamage(_effectType, _savingThrowType, (int)effectValue, _percentage);
    }
}