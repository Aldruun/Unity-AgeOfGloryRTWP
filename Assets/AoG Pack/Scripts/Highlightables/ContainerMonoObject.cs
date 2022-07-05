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

    public void ApplyDamage(Actor source, SavingThrowType savingThrowType, DamageType damageType, SpellAttackRollType attackRollType, int damageRoll, bool percentage)
    {
        int damage = damageRoll;

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