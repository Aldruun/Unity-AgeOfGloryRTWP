//using cakeslice;
using UnityEngine;

public class ContainerMonoObject : HighlightableMonoObject, IAttackable
{
    Container _containerData;
    public ContainerType containerType;

    public bool healthDepleted => _containerData.Health <= 0;

    public AttackableType AttackableType { get; private set; }

    public bool IgnorePlayerAttack => throw new System.NotImplementedException();

    protected void Start()
    {
        _containerData = new Container(this, containerType, lockDifficulty, locked, trapDetectDifficulty, trapRemovalDifficulty, trapped, 0);
        //base.Start();
    }

    public void InitializeAttackableInterface()
    {
        AttackableType = AttackableType.CONTAINER;
    }

    public void ApplyDamage(ActorInput source, Weapon weapon, EffectData magicEffect, bool isProjectile, bool hitSuccess = true)
    {
        int damage = weapon != null ? weapon.damage : magicEffect != null ? (int)magicEffect.magnitude : 0;

        _containerData.Health -= damage;
    }

    public Transform GetTransform()
    {
        return transform;
    }

    public float GetObjectRadius()
    {
        return 2f;
    }
}