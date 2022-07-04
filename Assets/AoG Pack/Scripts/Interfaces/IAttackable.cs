using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AttackableType
{
    NPC,
    PC,
    WORLDOBJECT,
    DOOR,
    CONTAINER
}

public interface IAttackable
{
    AttackableType AttackableType { get; }
    void InitializeAttackableInterface();
    void ApplyDamage(ActorInput source, Weapon weapon, EffectData magicEffect, bool isProjectile, bool hitSuccess = true);
    Transform GetTransform();
    bool IgnorePlayerAttack { get; }
    float GetObjectRadius();
}
