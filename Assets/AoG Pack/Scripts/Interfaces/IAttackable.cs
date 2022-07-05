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
    void ApplyDamage(Actor source, SavingThrowType savingThrowType, DamageType damageType, SpellAttackRollType attackRollType, int damageRoll, bool percentage);
    Transform GetTransform();
    bool IgnorePlayerAttack { get; }
    float GetObjectRadius();
}
