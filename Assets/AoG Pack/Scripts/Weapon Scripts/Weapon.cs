using System;
using System.Collections.Generic;
using UnityEngine;

//using System;

public enum WeaponImpactType
{
    None,
    Blade,
    Blunt,
    Arrow,
    Unarmed,
    Fire,
    Fireball,
    Frost,
    Lightning,
    Holy,
    Unholy,
    Poison,
    Creature
}

public enum WeaponCategory
{
    Unarmed = 0,
    Shortbow = 1,
    Longbow = 2,
    XBow = 3,
    Sling = 4,
    Dart = 5,
    ThrowingKnife = 6,
    Blunt = 7,
    Spiked = 8,
    Dagger = 9,
    ShortSword = 10,
    LongSword = 11,
    GreatSword = 13,
    Club = 14,
    Hammer = 15,
    Morningstar = 17,
    Axe = 18,
    BastardSword = 19,
    Flail = 20,
    Spear = 21
}

public enum RangeCategory
{
    MELEE1H = 1,
    MELEE2H = 2,
    SLING = 10,
    BOW = 22,
    XBOW = 30
}

public enum CombatType
{
    MELEE,
    RANGED
}

public enum AnimationSet
{
    NONE = 0,
    DEFAULT = 1,
    UNARMED = 2,
    DAGGER = 3,
    ONEHANDED = 4,
    TWOHANDED = 5,
    BOW = 6,
    XBOW = 7,
    MAGIC = 8
}

public enum WeaponType
{
    None,
    Unarmed,
    Dagger,
    SwordAndShield,
    Dual,
    TwoHanded,
    OneHanded,
    Crossbow,
    Bow,
    Magic
}

public enum WeaponProficiency
{
    NONE,
    BOWS,
    SPIKEDWEAPONS,
    SMALLSWORDS,
    LARGESWORDS,
    AXES,
    BLUNTWEAPONS,
    MISSILEWEAPONS,
    SPEARS
}

[Serializable]
public class Weapon : Item
{
    public int NumDice = 1;
    public int NumDieSides = 3;
    public int BonusAPR;
    public int BaseDamageRoll => DnD.Roll(NumDice, NumDieSides);

    public float Range;
    public int MaxHitTargets = 1;

    public WeaponCategory weaponCategory;

    public List<ActorStatData> requiredStats = new List<ActorStatData>();
    public List<ActorStatData> bonusStats = new List<ActorStatData>();
    //public Stat[] bonusStats;
    public AnimationSet AnimationPack;
    public CombatType CombatType;
    public DamageType damageType;
    public WeaponType weaponType;
    public EquipType equipType;
    public WeaponProficiency weaponProficiency;
    public string projectileIdentifier;
    
    [Tooltip("The animator will use this number to\n" +
             "choose the correct Draw, Sheath and Attack animations")]
  
    public WeaponImpactType impactSFXType;

    public float speed = 1f;

    public AmmoType ammoType;
    internal bool IsRanged { get; private set; }

    public override void Init()
    {
        InitWeaponValues();
    }

    public void InitWeaponValues()
    {
        weaponProficiency = GetProficiencyType();
        BonusAPR = GetAPRBonus();
        AnimationPack = GetAnimationPackageType();
        ammoType = GetAmmoType();
        Range = GetRange();
        CombatType = GetCombatType();
        impactSFXType = GetImpactType();
        damageType = GetDamageType();
    }

    WeaponProficiency GetProficiencyType()
    {
        switch(weaponCategory)
        {
            case WeaponCategory.Shortbow:
            case WeaponCategory.Longbow:
                return WeaponProficiency.BOWS;
            case WeaponCategory.XBow:
            case WeaponCategory.Sling:
            case WeaponCategory.Dart:
            case WeaponCategory.ThrowingKnife:
                return WeaponProficiency.MISSILEWEAPONS;
            case WeaponCategory.Dagger:
            case WeaponCategory.ShortSword:
                return WeaponProficiency.SMALLSWORDS;
            case WeaponCategory.LongSword:
            case WeaponCategory.BastardSword:
            case WeaponCategory.GreatSword:
                return WeaponProficiency.LARGESWORDS;
            case WeaponCategory.Club:
            case WeaponCategory.Hammer:
                return WeaponProficiency.BLUNTWEAPONS;
            case WeaponCategory.Flail:
            case WeaponCategory.Morningstar:
                return WeaponProficiency.SPIKEDWEAPONS;
            case WeaponCategory.Axe:
                return WeaponProficiency.AXES;
            case WeaponCategory.Spear:
                return WeaponProficiency.SPEARS;
        }
        return WeaponProficiency.NONE;
    }

    int GetAPRBonus()
    {
        switch(weaponCategory)
        {
            case WeaponCategory.ThrowingKnife:
            case WeaponCategory.Shortbow:
            case WeaponCategory.Longbow:
                return 1;
            //case WeaponCategory.XBow:
            //case WeaponCategory.Sling:
            case WeaponCategory.Dart:
                return 2;
                //case WeaponCategory.Dagger:
                //case WeaponCategory.ShortSword:
                //    return WeaponProficiency.SMALLSWORDS;
                //case WeaponCategory.LongSword:
                //case WeaponCategory.BastardSword:
                //case WeaponCategory.GreatSword:
                //    return WeaponProficiency.LARGESWORDS;
                //case WeaponCategory.Club:
                //case WeaponCategory.Hammer:
                //    return WeaponProficiency.BLUNTWEAPONS;
                //case WeaponCategory.Flail:
                //case WeaponCategory.Morningstar:
                //    return WeaponProficiency.SPIKEDWEAPONS;
                //case WeaponCategory.Axe:
                //    return WeaponProficiency.AXES;
                //case WeaponCategory.Spear:
                //    return WeaponProficiency.SPEARS;
        }
        return 0;
    }

    private AmmoType GetAmmoType()
    {
        switch(weaponCategory)
        {
            case WeaponCategory.Shortbow:
            case WeaponCategory.Longbow:
                return AmmoType.Arrow;
            case WeaponCategory.XBow:
                return AmmoType.Bolt;
            case WeaponCategory.Sling:
                return AmmoType.SlingStone;
        }

        return AmmoType.None;
    }

    private float GetRange()
    {
        switch(weaponCategory)
        {
            case WeaponCategory.Shortbow:
            case WeaponCategory.Longbow:
            case WeaponCategory.XBow:
            case WeaponCategory.Sling:
            case WeaponCategory.Dart:
            case WeaponCategory.ThrowingKnife:
                IsRanged = true;
                return 10;
            case WeaponCategory.Unarmed:
            case WeaponCategory.Dagger:
                return 1.5f;
            case WeaponCategory.ShortSword:
            case WeaponCategory.LongSword:
            case WeaponCategory.Club:
            case WeaponCategory.Hammer:
            case WeaponCategory.Morningstar:
            case WeaponCategory.Axe:
                return 2f;
            case WeaponCategory.GreatSword:
                return 3;
        }

        Debug.LogError("Unsafe range::WeaponCategory not found");
        return 1;
    }


    private DamageType GetDamageType()
    {
        switch(weaponCategory)
        {
            case WeaponCategory.ThrowingKnife:
            case WeaponCategory.Shortbow:
            case WeaponCategory.Longbow:
            case WeaponCategory.Dart:
            case WeaponCategory.XBow:
                return DamageType.PIERCING;
            case WeaponCategory.Dagger:
            case WeaponCategory.ShortSword:
            case WeaponCategory.LongSword:
            case WeaponCategory.GreatSword:
            case WeaponCategory.Axe:
                return DamageType.SLASHING;
            case WeaponCategory.Club:
            case WeaponCategory.Hammer:
            case WeaponCategory.Sling:
            case WeaponCategory.Morningstar:
                return DamageType.CRUSHING;
        }
        return DamageType.CRUSHING;
    }

    private CombatType GetCombatType()
    {
        switch(weaponCategory)
        {
            case WeaponCategory.Shortbow:
            case WeaponCategory.Longbow:
            case WeaponCategory.XBow:
            case WeaponCategory.Sling:
            case WeaponCategory.Dart:
            case WeaponCategory.ThrowingKnife:
                return CombatType.RANGED;
        }
        return CombatType.MELEE;
    }

    private AnimationSet GetAnimationPackageType()
    {
        switch(weaponCategory)
        {
            case WeaponCategory.Unarmed:
                return AnimationSet.UNARMED;
            case WeaponCategory.Shortbow:
            case WeaponCategory.Longbow:
                return AnimationSet.BOW;
            case WeaponCategory.XBow:
                return AnimationSet.XBOW;
            case WeaponCategory.Sling:
            case WeaponCategory.Dart:
            case WeaponCategory.ThrowingKnife:
            case WeaponCategory.Dagger:
            case WeaponCategory.ShortSword:
            case WeaponCategory.LongSword:
            case WeaponCategory.Club:
            case WeaponCategory.Hammer:
            case WeaponCategory.Axe:
                return AnimationSet.ONEHANDED;
            case WeaponCategory.GreatSword:
                return AnimationSet.TWOHANDED;
        }
        return AnimationSet.DEFAULT;
    }

    private WeaponImpactType GetImpactType()
    {
        switch(weaponCategory)
        {
            case WeaponCategory.Unarmed:
                return WeaponImpactType.Unarmed;
            case WeaponCategory.Shortbow:
            case WeaponCategory.Longbow:
            case WeaponCategory.XBow:
            case WeaponCategory.Sling:
            case WeaponCategory.Dart:
            case WeaponCategory.ThrowingKnife:
                return WeaponImpactType.Arrow;
            case WeaponCategory.Spiked:
            case WeaponCategory.Dagger:
            case WeaponCategory.ShortSword:
            case WeaponCategory.LongSword:
            case WeaponCategory.GreatSword:
            case WeaponCategory.Axe:
                return WeaponImpactType.Blade;
            case WeaponCategory.Hammer:
            case WeaponCategory.Morningstar:
            case WeaponCategory.Club:
                return WeaponImpactType.Blunt;
        }

        return WeaponImpactType.Creature;
    }
}

[System.Serializable]
public struct ActorStatData
{
    public Stat stat;
    public int size;

    public ActorStatData(Stat stat, int size)
    {
        this.stat = stat;
        this.size = size;
    }
}

public struct ImpactData
{
    public AudioClip[] impactSounds;
    public string vfxIdentifier;

    public ImpactData(AudioClip[] impactSounds, string vfxIdentifier)
    {
        this.impactSounds = impactSounds;
        this.vfxIdentifier = vfxIdentifier;
    }
}