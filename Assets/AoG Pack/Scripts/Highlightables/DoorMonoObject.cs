using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorMonoObject : HighlightableMonoObject, IAttackable
{
    Door _doorData;
    
    public bool healthDepleted => _doorData.Health <= 0;

    public AttackableType AttackableType { get; private set; }

    public bool IgnorePlayerAttack => false;

    protected void Start()
    {
        
        _doorData = new Door(this, lockDifficulty, locked, trapDetectDifficulty, trapRemovalDifficulty, trapped, 0);
        //base.Start();
    }

    public void Execute_ModifyHealth(int value, ModType modType)
    {
        //throw new System.NotImplementedException();
    }

    public void InitializeAttackableInterface()
    {
        AttackableType = AttackableType.DOOR;
    }

    public void ApplyDamage(Actor source, SavingThrowType savingThrowType, DamageType damageType, SpellAttackRollType attackRollType, int damageRoll, bool percentage)
    {
       
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
