using System;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public struct StatusEffectData
{
    public int rounds;
    public Status statusType;

    public StatusEffectData(int rounds, Status statusType)
    {
        this.rounds = rounds;
        this.statusType = statusType;
    }
}

public class StatusEffectSystem
{
    private ActorInput self;

    private Dictionary<Status, StatusEffect> _statusEffects;

    public bool immobilized;

    public StatusEffectSystem(ActorInput agent)
    {
        self = agent;
        Init(self);
    }

    private void Init(ActorInput agent)
    {
        self = agent;

        _statusEffects = new Dictionary<Status, StatusEffect>();

        //if(_agent.isSpellCaster)
        //    ApplyStatusEffect(new StatusEffect_ManaRegeneration(
        //        (0.02f + _agent.level * 0.01f) * GameMaster.Instance.gameSettings.globalManaRegenMult, true, 1, -1));

    }

    public void UpdateStatusEffectTicks()
    {
        //Debug.Log($"<color=cyan>{_agent.name}:</color> Updating StatusEffectSystem");

        //Profiler.BeginSample("StatusEffectSystem: StatusEffectLoop");

        foreach(KeyValuePair<Status, StatusEffect> kvp in _statusEffects)
        {
            kvp.Value.UpdateTicks(self);
        }

        //Profiler.EndSample();
    }

    public void ApplyStatusEffect(Status status, int rounds)
    {
        if(_statusEffects.ContainsKey(status))
        {
            if(_statusEffects[status].rounds < rounds)
            {
                _statusEffects[status].rounds = rounds;
            }
            return;
        }

        StatusEffect statusEffect = null;

        switch(status)
        {
            case Status.BARKSKIN:
                break;
            case Status.BLESSED:
                break;
            case Status.CHANT:
                break;
            case Status.HASTE:
                break;
            case Status.MAGICARMOR:
                break;
            case Status.PROTFIRE:
                break;
            case Status.PROTCOLD:
                break;
            case Status.PROTEVIL:
                break;
            case Status.PROTPETRIFICATION:
                break;
            case Status.REGENERATING:
                break;
            case Status.RESISTFEAR:
                break;
            case Status.STRENGTH:
                break;
            case Status.ABILITYDRAIN:
                break;
            case Status.ACID:
                break;
            case Status.BERSERK:
                break;
            case Status.BLINDED:
                break;
            case Status.CONFUSED:
                break;
            case Status.CURSED:
                break;
            case Status.DEAF:
                break;
            case Status.DIRECHARM:
                break;
            case Status.DYING:
                break;
            case Status.ENERGYDRAIN:
                break;
            case Status.ENFEEBLED:
                break;
            case Status.ENTANGLED:
                break;
            case Status.FATIGUED:
                break;
            case Status.FEEBLEMINDED:
                break;
            case Status.GREASE:
                break;
            case Status.HELD:
                break;
            case Status.INTOXICATED:
                break;
            case Status.LEVELDRAIN:
                break;
            case Status.MISCASTMAGIC:
                break;
            case Status.NAUSEATED:
                break;
            case Status.PANIC:
                statusEffect = new StatusEffect_Panicked();

                break;
            case Status.PETRIFIED:
                break;
            case Status.POISENED:
                break;
            case Status.POLYMORPHED:
                break;
            case Status.RIGIDTHINKING:
                break;
            case Status.SILENCED:
                break;
            case Status.SLEEP:
                statusEffect = new StatusEffect_Sleep();
                break;
            case Status.SLOW:
                statusEffect = new StatusEffect_Slow();
                break;
            case Status.SPELLFAILURE:
                break;
            case Status.STUN:
                statusEffect = new StatusEffect_Stunned();
                break;
            case Status.UNCONSCIOUS:
                break;
            case Status.WEBBED:
                break;
            default:
                break;
        }

        if(statusEffect == null)
            statusEffect = new StatusEffect_Dummy();

        statusEffect.Init(self, rounds);

        _statusEffects.Add(status, statusEffect);
    }

    public void Update()
    {
        foreach(KeyValuePair<Status, StatusEffect> kvp in _statusEffects.ToArray())
        {
            _statusEffects[kvp.Key].rounds--;
            if(kvp.Value.rounds == 0)
            {
                _statusEffects[kvp.Key].OnEnd(self);

                _statusEffects.Remove(kvp.Key);
            }
        }
    }

    internal bool HasStatusEffect(Status status)
    {
        return _statusEffects.ContainsKey(status);
    }

    public void TerminateAllStatusEffects()
    {
        foreach(var kvp in _statusEffects)
        {
            kvp.Value.OnEnd(self);
            _statusEffects[kvp.Key] = null;
        }
        self.isDowned = false;
        self.stunned = false;
        self.panicked = false;
        self.sleeping = false;

    }

    internal List<StatusEffect> GetAppliedStatusEffects()
    {
        return _statusEffects.Values.ToList();
    }
}