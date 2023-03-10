using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MagicSchool
{
    Abjuration,
    // Primarily designed for protection and shielding. Don’t be fooled however, some Abjuration spells can pack quite a punch.
    // Low Level: Dispel Magic, Shield
    // Mid Level: Dispel Evil and Good, Greater Restoration
    // High Level: Invulnerability, Antimagic Field

    Conjuration, // Beschwörung
    // Deals with creating objects and creatures, or making them disappear.
    // Low Level: Find Familiar, Poison Spray
    // Mid Level: Spirit Guardians, Conjure Elemental
    // High Level: Plane Shift, Wish

    Divination, // Hellsehen
    // Reveal and grant knowledge and information to the caster. Useful for reading ancient scripts,
    // identifying magical items, and seeing invisible enemies.
    // Low Level: Identify, Find Traps
    // Mid Level: Scrying, Locate Creature
    // High Level: True Seeing, Foresight

    Enchantment, // Verzauberung
    // Manipulate the mental state of a person.
    // Low Level: Hold Person, Sleep
    // Mid Level: Modify Memory, Mass Suggestion
    // High Level: Feeblemind, Power Word Kill

    Evocation, // Hervorrufung
    /* Casters within the school of evocation unleash a raw magical energy upon their enemies. Whether it be flames,
     * ice, or pure arcane energy: evocation spellcasters are here to deal damage and chew gum… and they’re all out of gum. */
    // Low Level: Burning Hands, Dancing Lights
    // Mid Level: Fireball, Freezing Sphere
    // High Level: Meteor Swarm, Prismatic Spray

    Illusion,
    // Manipulate the various senses of people and creatures.
    // This could be vision, hearing, or other various senses such as body temperature.
    // Low Level: Disguise Self, Silent Image
    // Mid Level: Invisibility, Hallucinatory Terrain
    // High Level: Project Image, Weird

    Necromancy,
    /* In general, think of spells within the School of Necromancy as manipulating the ebb and flow of different creatures’ “life energy”,
     * or the balance of energy between life and death. This can come across in the form of helping resurrection, or draining necrotic damage. */
    // Low Level: Chill Touch, Inflict Wounds
    // Mid Level: Animate Dead, Blight
    // High Level: Resurrection, Finger of Death

    Transmutation // Verwandlung
    // Manipulate the physical properties of both items and people.
    // Low Level: Shape Water, Feather Fall
    // Mid Level: Gaseous Form, Stone Shape
    // High Level: Polymorph, Etherealness
}

public enum EquipType
{
    RightHand,

    //LeftHand,
    BothHands,
    EitherHand,
    Potion,
    Shield,
    None
}

public enum CastingType
{
    FireAndForget,
    Concentration,
    ConstantEffect

    // All of the effects on a Spell must have the same casting type.
}

public enum DeliveryType
{
    None,
    Contact, // Effect is applied to the target by contact (a hit event). This only works for Weapons.
    SeekActor,
    SeekLocation,
    InstantSelf,
    InstantActor,
    InstantLocation,
    Spray,
    Beam
    // All of the effects on a Spell must have the same casting type.
}

public enum SubCategory
{
    Spell,
    Ability
    // All of the effects on a Spell must have the same casting type.
}

public enum DamageType
{
    CRUSHING,
    ACID,
    COLD,
    ELECTRICITY,
    FIRE,
    PIERCING,
    POISON,
    MAGIC,
    MISSILE,
    SLASHING,
    MAGICFIRE,
    MAGICCOLD,
    STUNNING,
    DESEASE,
    BLEEDING,
    NECROTIC,
    RADIANT,
    HEAL,
    SLEEP
}

public enum SpellAttackRollType
{
    None,
    Melee,
    Ranged
}

public enum SavingThrowType
{
    None,
    Strength,
    StrengthHalfDamage,
    Constitution,
    ConstitutionHalfDamage,
    Dexterity,
    DexterityHalfDamage,
    Intelligence,
    IntelligenceHalfDamage,
    Wisdom,
    WisdomHalfDamage,
    Charisma,
    CharismaHalfDamage,
    Reflex
}

[Flags]
public enum Keyword
{
    None = 0,
    PhysicalDamage = 1,
    FireDamage = 1 << 1,
    FrostDamage = 1 << 2,
    LightningDamage = 1 << 3,
    HolyDamage = 1 << 4,
    UnholyDamage = 1 << 5,
    PoisonDamage = 1 << 6,
    RadiantDamage = 1 << 7,
    ForceDamage = 1 << 8,
    Damage = 1 << 9,
    Heal = 1 << 10,
    HealSelf = 1 << 11,
    HealOther = 1 << 12,
    DebuffHealth = 1 << 13,
    DebuffMana = 1 << 14,
    BuffHealth = 1 << 15,
    BuffMana = 1 << 16
}

[CreateAssetMenu(menuName = "Spell System/Spell")]
public class Spell : ScriptableObject
{
    public string ID;
    public string Name;
    [TextArea]
    public string description;
    public Sprite spellIcon;

    [Range(0, 9)] //! 0 = cantrip
    public int Grade;
    internal int slotLevel;
    public Class[] targetClasses;
    public ProjectileType projectileType;
    //public SpellActivationMode activationMode;
    public MagicSchool magicSchool;
    public EquipType equipType;
    public CastingType castingType;
    public DeliveryType deliveryType;
    public SpellTargetType spellTargetType;
    public Keyword keywords;
    public List<MagicEffect> magicEffects;
    public List<EffectData> magicEffectsData;
    public SpellTargetLogic targetLogic;

    public bool attackRollRequired;
    public bool percentMagnitude;
    public int castingTime = 1;
    public float aoeRadius;
    public float travelSpeed;
    public int activationRange;
    public int spellcastMotionIndex;
    public int releaseMotionIndex;
    public float cooldownTime;
    public float recoveryTime;

    [Range(0.5f, 2f)] public float chargeTimeMult = 1f; // TODO: Get animation length
    [Range(0.5f, 2f)] public float castTimeMult = 1f; // TODO: Get animation length
    //public float cooldown = 2;
    //public float recoveryTime;

    internal float cooldownTimer;
    public System.Action<float> CooldownHook;
    internal int Cost;

    public float priority { get; set; }

    public virtual void Init()
    {
        if(deliveryType != DeliveryType.InstantSelf && targetLogic == null)
        {
            Debug.Log("<color=grey>Spell '" + (Name == "" ? GetType().ToString() : Name) +
                      "' has no targetLogic</color>");
        }
        else
        {
            targetLogic = Instantiate(targetLogic);
            //targetLogic.Init(self);
        }

        for(int i = 0; i < magicEffects.Count; i++)
        {
            //magicEffects[i] = Object.Instantiate(magicEffects[i]);
            magicEffects[i].Init(this);
        }

        switch(castingType)
        {
            case CastingType.FireAndForget:
                break;
            case CastingType.Concentration:
                break;
            case CastingType.ConstantEffect:
                break;
        }
        slotLevel = Grade;
    }

    public void StartCooldown()
    {
        AoG.Core.CoroutineRunner.Instance.StartCoroutine(CR_Cooldown());
    }

    private IEnumerator CR_Cooldown()
    {
        cooldownTimer = cooldownTime;
        while(cooldownTimer >= 0)
        {
            CooldownHook?.Invoke(cooldownTimer);
            cooldownTimer -= Time.deltaTime;
            yield return null;
        }
    }

    public bool HasKeyWord(Keyword keyword)
    {
        if((magicEffectsData[0].keywordFlags & keyword) != 0)
        {
            return true;
        }

        return false;
    }
}

public enum SpellType
{
    Arcane,
    Divine
}