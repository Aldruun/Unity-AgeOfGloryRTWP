using UnityEngine;

public abstract class SpellTargetLogic : ScriptableObject
{
    public Actor owner { get; protected set; }
    public Actor spellTarget { get; protected set; }
    public Vector3 targetPosition { get; protected set; }
    public Spell spell { get; protected set; }
    public Projectile projectile { get; protected set; }

    public string[] vfxMainIdentifier;

    public virtual SpellTargetLogic Init(Actor owner, Actor target, Spell spell, Projectile projectile)
    {
        return this;
    }
    public virtual SpellTargetLogic Init(Actor owner, Vector3 targetPosition, Spell spell, Projectile projectile)
    {
        return this;
    }
    //public virtual SpellTargetLogic Init(ActorInput owner, Spell spell, Projectile projectile)
    //{
    //    return this;
    //}

    public abstract bool Done();
}