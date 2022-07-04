using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum AffectedAttribute
{
    Health,
    Mana
}
public enum EffectType
{
    FIRE,
    ICE,
    LIGHTING,
    HOLY,
    POISON,
    HEAL
}

public enum Effects
{
    WaterBreathing = 0,
    SwiftSwim = 1,
    WaterWalking = 2,
    Shield = 3,
    FireShield = 4,
    LightningShield = 5,
    FrostShield = 6,
    Burden = 7,
    Feather = 8,
    Jump = 9,
    Levitate = 10,
    SlowFall = 11,
    Lock = 12,
    Open = 13,
    FireDamage = 14,
    ShockDamage = 15,
    FrostDamage = 16,
    DrainAttribute = 17,
    DrainHealth = 18,
    DrainMagicka = 19,
    DrainFatigue = 20,
    DrainSkill = 21,
    DamageAttribute = 22,
    DamageHealth = 23,
    DamageMagicka = 24,
    DamageFatigue = 25,
    DamageSkill = 26,
    Poison = 27,
    WeaknessToFire = 28,
    WeaknessToFrost = 29,
    WeaknessToShock = 30,
    WeaknessToMagicka = 31,
    WeaknessToCommonDisease = 32,
    WeaknessToBlightDisease = 33,
    WeaknessToCorprusDisease = 34,
    WeaknessToPoison = 35,
    WeaknessToNormalWeapons = 36,
    DisintegrateWeapon = 37,
    DisintegrateArmor = 38,
    Invisibility = 39,
    Chameleon = 40,
    Light = 41,
    Sanctuary = 42,
    NightEye = 43,
    Charm = 44,
    Paralyze = 45,
    Silence = 46,
    Blind = 47,
    Sound = 48,
    CalmHumanoid = 49,
    CalmCreature = 50,
    FrenzyHumanoid = 51,
    FrenzyCreature = 52,
    DemoralizeHumanoid = 53,
    DemoralizeCreature = 54,
    RallyHumanoid = 55,
    RallyCreature = 56,
    Dispel = 57,
    Soultrap = 58,
    Telekinesis = 59,
    Mark = 60,
    Recall = 61,
    DivineIntervention = 62,
    AlmsiviIntervention = 63,
    DetectAnimal = 64,
    DetectEnchantment = 65,
    DetectKey = 66,
    SpellAbsorption = 67,
    Reflect = 68,
    CureCommonDisease = 69,
    CureBlightDisease = 70,
    CureCorprusDisease = 71,
    CurePoison = 72,
    CureParalyzation = 73,
    RestoreAttribute = 74,
    RestoreHealth = 75,
    RestoreMagicka = 76,
    RestoreFatigue = 77,
    RestoreSkill = 78,
    FortifyAttribute = 79,
    FortifyHealth = 80,
    FortifyMagicka = 81,
    FortifyFatigue = 82,
    FortifySkill = 83,
    FortifyMaximumMagicka = 84,
    AbsorbAttribute = 85,
    AbsorbHealth = 86,
    AbsorbMagicka = 87,
    AbsorbFatigue = 88,
    AbsorbSkill = 89,
    ResistFire = 90,
    ResistFrost = 91,
    ResistShock = 92,
    ResistMagicka = 93,
    ResistCommonDisease = 94,
    ResistBlightDisease = 95,
    ResistCorprusDisease = 96,
    ResistPoison = 97,
    ResistNormalWeapons = 98,
    ResistParalysis = 99,
    RemoveCurse = 100,
    TurnUndead = 101,
    SummonScamp = 102,
    SummonClannfear = 103,
    SummonDaedroth = 104,
    SummonDremora = 105,
    SummonAncestralGhost = 106,
    SummonSkeletalMinion = 107,
    SummonBonewalker = 108,
    SummonGreaterBonewalker = 109,
    SummonBonelord = 110,
    SummonWingedTwilight = 111,
    SummonHunger = 112,
    SummonGoldenSaint = 113,
    SummonFlameAtronach = 114,
    SummonFrostAtronach = 115,
    SummonStormAtronach = 116,
    FortifyAttack = 117,
    CommandCreature = 118,
    CommandHumanoid = 119,
    BoundDagger = 120,
    BoundLongsword = 121,
    BoundMace = 122,
    BoundBattleAxe = 123,
    BoundSpear = 124,
    BoundLongbow = 125,
    ExtraSpell = 126,
    BoundCuirass = 127,
    BoundHelm = 128,
    BoundBoots = 129,
    BoundShield = 130,
    BoundGloves = 131,
    Corprus = 132,
    Vampirism = 133,
    SummonCenturionSphere = 134,
    SunDamage = 135,
    StuntedMagicka = 136,

    // Tribunal only
    SummonFabricant = 137,

    // Bloodmoon only
    SummonWolf = 138,
    SummonBear = 139,
    SummonBonewolf = 140,
    SummonCreature04 = 141,
    SummonCreature05 = 142,

    Length
}

[CreateAssetMenu(menuName = "Spell System/Magic Effect")]
public class MagicEffect : ScriptableObject
{
    //[TextArea]
    //public string description;
    public string ID;
    public string Name;

    //public ActorSkill magicSchool;
    public MagicSchool magicSchool;
    public CastingType castingType { get; private set; }
    public DeliveryType deliveryType { get; private set; }
    public string actor_skill_ref;
    public Status StatusEffect;
    public EffectType effectType;
    public Status statusEffect;
    public Keyword keywords;
    public ProjectileType projectileType;

    public bool isPermanent;
    //[HideInInspector] public float baseCost;
    public int magnitude;
    public int Rounds;
    public float projectileDiameter;
    public float aoeRadius;
    public float struckChance;
    //public List<StatusEffect> statusEffectsTarget;

    public string id_VFXHandIdle;
    public string id_VFXHandCharge;
    public string id_VFXHandRelease;
    public string id_VFXOnInflict;
    public string id_VFXOnImpact;
    public string id_VFXProjectile;
    public float projectileSpeed = 10;
    public bool destroyOnImpact = true;
    public bool clampVFXToTarget;

    [Header("Sounds")]
    public DamageType damageType;
    public AudioClip sfxDraw;
    public AudioClip sfxSheath;
    public AudioClip sfxCharge;
    public AudioClip sfxReady;
    public AudioClip sfxRelease;
    public AudioClip sfxCastLoop;
    public AudioClip[] sfxOnHit;

    public Projectile projectile { get; protected set; }

    public virtual void Init(Spell spell)
    {
        magicSchool = spell.magicSchool;
        castingType = spell.castingType;
        deliveryType = spell.deliveryType;
        //baseCost = spell.cost;

        if(sfxOnHit != null && sfxOnHit.Length == 0)
        {
            sfxOnHit = null;
        }


    }

    //protected bool ConditionsMet(AgentMonoController agent)
    //{
    //    switch(_delivery)
    //    {
    //        case DeliveryType.Self:
    //            break;
    //        case DeliveryType.Contact:
    //            break;
    //        case DeliveryType.Aimed:
    //            break;
    //        case DeliveryType.TargetActor:
    //            break;
    //        case DeliveryType.TargetLocation:
    //            break;
    //        default:
    //            break;
    //    }

    //}

    internal static EffectData GetEffectData(MagicEffect magicEffect)
    {
        EffectData effectData = new EffectData();
        effectData.magicSchool = magicEffect.magicSchool;
        effectData.ID = magicEffect.ID;
        effectData.isPermanent = magicEffect.isPermanent;
        effectData.magnitude = magicEffect.magnitude;
        effectData.Rounds = magicEffect.Rounds;
        effectData.aoeRadius = magicEffect.aoeRadius;
        effectData.statusEffect = magicEffect.statusEffect;
        effectData.damageType = magicEffect.damageType;
        effectData.castingType = magicEffect.castingType;
        effectData.deliveryType = magicEffect.deliveryType;
        effectData.keywordFlags = magicEffect.keywords;

        effectData.vfxid_impact = magicEffect.id_VFXOnImpact;
        effectData.vfxid_inflict = magicEffect.id_VFXOnInflict;
        effectData.vfxid_handrelease = magicEffect.id_VFXHandRelease;
        effectData.vfxid_handidle = magicEffect.id_VFXHandIdle;
        effectData.vfxid_handcharge = magicEffect.id_VFXHandCharge;
        effectData.vfxid_projectile = magicEffect.id_VFXProjectile;

        effectData.sfxDraw = magicEffect.sfxDraw;
        effectData.sfxCharge = magicEffect.sfxCharge;
        effectData.sfxCastLoop = magicEffect.sfxCastLoop;
        effectData.sfxOnHit = magicEffect.sfxOnHit;
        effectData.sfxReady = magicEffect.sfxReady;
        effectData.sfxRelease = magicEffect.sfxRelease;
        effectData.sfxSheath = magicEffect.sfxSheath;

        return effectData;
    }

    public void Extinguish()
    {
        //if(self.debug)
        Debug.Log("<color=orange>Extinguishing magic effect</color>");

        if(projectile != null)
        {
            projectile.StopCoroutine("CR_ClampToAndFade");
            projectile.StopAndDisable();
            projectile = null;
        }
    }
}

[System.Serializable]
public class EffectData
{
    // Magical effect, hard-coded ID
    //public short effectID;

    // Which skills/attributes are affected (for restore/drain spells
    // etc.)
    //public byte skill, attribute; // -1 if N/A
    public MagicSchool magicSchool;
    // Other spell parameters
    public string ID;
    public float aoeRadius; // 0 - self, 1 - touch, 2 - target (RangeType enum)
    public float area;
    public int Rounds;
    public float magnitude;
    //public float travelSpeed;
    public bool isPermanent;
    public Status statusEffect;
    public DamageType damageType;
    public CastingType castingType;
    public DeliveryType deliveryType;
    public Keyword keywordFlags;

    public string vfxid_handcharge;
    public string vfxid_projectile;
    public string vfxid_handidle;
    public string vfxid_handrelease;
    public string vfxid_inflict;
    public string vfxid_impact;

    public AudioClip sfxDraw;
    public AudioClip sfxSheath;
    public AudioClip sfxCharge;
    public AudioClip sfxReady;
    public AudioClip sfxRelease;
    public AudioClip sfxCastLoop;
    public AudioClip[] sfxOnHit;
}

[System.Serializable]
public struct EffectList
{
    public List<EffectData> list;

    public void Load(/*ESMReader &esm*/)
    {
        list.Clear();
        //while(esm.isNextSub("ENAM"))
        //{
        //    add(esm);
        //}
    }

    public void Add(EffectData effect/*ESMReader &esm*/)
    {
        //EffectData s;
        //esm.getHT(s, 24);
        list.Add(effect);
    }

    public void Save(/*ESMWriter &esm*/)
    {
        //for (std::vector<ENAMstruct>::const_iterator it = mList.begin(); it != mList.end(); ++it) {
        //    esm.writeHNT<ENAMstruct>("ENAM", *it, 24);
        //}
    }
}