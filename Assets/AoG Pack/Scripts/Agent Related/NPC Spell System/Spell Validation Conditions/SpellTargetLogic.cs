using UnityEngine;

public abstract class SpellTargetLogic : ScriptableObject
{
    public ActorInput owner { get; protected set; }
    public ActorInput spellTarget { get; protected set; }
    public Vector3 targetPosition { get; protected set; }
    public Spell spell { get; protected set; }
    public Projectile projectile { get; protected set; }

    public string[] vfxMainIdentifier;

    public virtual SpellTargetLogic Init(ActorInput owner, ActorInput target, Spell spell, Projectile projectile)
    {
        return this;
    }
    public virtual SpellTargetLogic Init(ActorInput owner, Vector3 targetPosition, Spell spell, Projectile projectile)
    {
        return this;
    }
    //public virtual SpellTargetLogic Init(ActorInput owner, Spell spell, Projectile projectile)
    //{
    //    return this;
    //}

    public abstract bool Done();
}