using System.Collections;
using UnityEngine;

public enum AffectedAttribute
{
    Health,
    Mana
}

[CreateAssetMenu(menuName = "Spell System/Magic Effect")]
public class MagicEffect : ScriptableObject
{
    public Keyword keywords;
    SpellTargetType targetType;
    public AffectedAttribute affectedAttribute;
    protected DamageType damageType;
    protected SavingThrowType savingThrowType;
    protected CastingType castingType;
    protected DeliveryType deliveryType;
    public Status statusEffect;
    
    private bool attackRollRequired;
    private SpellAttackRollType attackRollType;
    private bool percentMagnitude;

    public float duration;
    public float aoeRadius;
    public float effectDiameter;
    public float effectRange;

    public string id_VFXProjectile;
    public string id_VFXOnHit;
    public string id_VFXHandIdle;
    public string id_VFXHandCharge;
    public string id_VFXHandRelease;
    public ProjectileType projectileType;
    public float projectileSpeed = 10;
    public bool destroyOnImpact = true;
    public bool clampVFXToTarget;

    [Header("Sounds")]
    public WeaponImpactType impactSFXType;
    //public AudioClip sfxDraw;
    //public AudioClip sfxSheath;
    //public AudioClip sfxCharge;
    //public AudioClip sfxReady;
    public AudioClip[] sfxOnRelease;
    //public AudioClip sfxCastLoop;
    public AudioClip[] sfxOnHit;
    public AudioClip sfxChargeSpell;

    //private Collider[] _friends;
    public Projectile projectile { get; protected set; }

    public string id_VFXOnInflict;
    public string id_VFXOnImpact;
    private bool IsPermanent;
    private float Magnitude;

    public virtual void Init(Spell spell)
    {
        targetType = spell.spellTargetType;
        castingType = spell.castingType;
        deliveryType = spell.deliveryType;
 
        if(sfxOnHit != null && sfxOnHit.Length == 0)
            sfxOnHit = null;
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

    public void SpawnEffect(Actor self, Actor target, Transform spellAnchor)
    {
        if(id_VFXHandRelease != "")
            VFXPlayer.TriggerVFX(PoolSystem.GetPoolObject(id_VFXHandRelease, ObjectPoolingCategory.VFX), spellAnchor.position, Quaternion.LookRotation(self.transform.forward, Vector3.up));
        if(id_VFXHandIdle != "")
            VFXPlayer.TriggerVFX(PoolSystem.GetPoolObject(id_VFXHandIdle, ObjectPoolingCategory.VFX), spellAnchor, Vector3.zero, Quaternion.identity, false, 5);

        if(sfxOnRelease != null && sfxOnRelease.Length > 0)
        {
            SFXPlayer.TriggerSFX(sfxOnRelease[Random.Range(0, sfxOnRelease.Length)], spellAnchor.position);
        }

        // Special cases
        if(projectileType == ProjectileType.MagicMissile)
        {
            Vector3 dir = target.transform.position - self.transform.position;
            int lvl = self.ActorStats.Level;
            int numProjectiles = lvl < 3 ? 1 : lvl < 5 ? 2 : lvl < 7 ? 3 : lvl < 9 ? 4 : 5;

            Vector3 startPos = self.transform.position; // umm, start position !
            Vector3 targetPos = Vector3.zero; // variable for calculated end position

            float angle = dir.magnitude * 10;
            //float startAngle = -angle * 0.5f; // half the angle to the Left of the forward
            //float finishAngle = angle * 0.5f; // half the angle to the Right of the forward

            // the gap between each ray (increment)
            int inc = Mathf.CeilToInt(angle / numProjectiles);
            angle *= 0.5f;
            for(float i = -angle; i < angle; i += inc) // Angle from forward
            {
                targetPos = (Quaternion.Euler(0, i, 0) * self.transform.forward).normalized;

                GameObject vfxObjSpecial = PoolSystem.GetPoolObject(id_VFXProjectile, ObjectPoolingCategory.VFX);
                Projectile projectileSpecial = vfxObjSpecial.GetComponent<Projectile>();
                projectileSpecial.transform.rotation = Quaternion.LookRotation(targetPos); // UnityEngine.Random.Range(0.5f, -0.5f)
                projectileSpecial.Launch(self, spellAnchor.position, target, deliveryType, projectileType, damageType, projectileSpeed, aoeRadius, effectDiameter, effectRange);

                projectileSpecial.OnImpact = () =>
                {
                    if(sfxOnHit != null)
                        SFXPlayer.TriggerSFX(sfxOnHit[Random.Range(0, sfxOnHit.Length)], projectileSpecial.transform.position);
                    if(id_VFXOnHit != "")
                        VFXPlayer.TriggerVFX(PoolSystem.GetPoolObject(id_VFXOnHit, ObjectPoolingCategory.VFX), projectileSpecial.transform.position, Quaternion.identity);
                };
            }


            return;
        }

        //if(_projectile == null)
        //{
        GameObject vfxObj = PoolSystem.GetPoolObject(id_VFXProjectile, ObjectPoolingCategory.VFX);
        //Debug.Log(spellAnchor.name);
        VFXPlayer.TriggerVFX(vfxObj, spellAnchor.position, Quaternion.identity);

        projectile = vfxObj.GetComponent<Projectile>();
        if(projectile != null)
        {



            projectile.Launch(self, spellAnchor.position, target, deliveryType, projectileType, damageType, projectileSpeed, aoeRadius, effectDiameter, effectRange);
            projectile.OnImpact = () =>
            {
                if(sfxOnHit != null)
                    SFXPlayer.TriggerSFX(sfxOnHit[Random.Range(0, sfxOnHit.Length)], projectile.transform.position);
                if(id_VFXOnHit != "")
                    VFXPlayer.TriggerVFX(PoolSystem.GetPoolObject(id_VFXOnHit, ObjectPoolingCategory.VFX), projectile.transform.position, Quaternion.identity);
                //if(duration > 1)
                //    spellTarget.Execute_ApplyStatusEffect(new StatusEffect_Damage(_effectType, _savingThrowType, 1, false, 1, duration, 1));
            };
        }
        else
        {
            Debug.LogError("'" + id_VFXProjectile + "' projectile null");
        }
    }

    public void SpawnEffectLocation(Actor self, Vector3 target, Transform spellAnchor)
    {
        if(id_VFXHandRelease != "")
            VFXPlayer.TriggerVFX(PoolSystem.GetPoolObject(id_VFXHandRelease, ObjectPoolingCategory.VFX), spellAnchor.position, Quaternion.LookRotation(self.transform.forward, Vector3.up));
        if(id_VFXHandIdle != "")
            VFXPlayer.TriggerVFX(PoolSystem.GetPoolObject(id_VFXHandIdle, ObjectPoolingCategory.VFX), spellAnchor, Vector3.zero, Quaternion.identity, false, 5);

        if(sfxOnRelease.Length > 0)
        {
            SFXPlayer.TriggerSFX(sfxOnRelease[Random.Range(0, sfxOnRelease.Length)], spellAnchor.position);
        }

        //if(_projectile == null)
        //{
        GameObject vfxObj = PoolSystem.GetPoolObject(id_VFXProjectile, ObjectPoolingCategory.VFX);
        projectile = vfxObj.GetComponent<Projectile>();

        //Debug.Log(spellAnchor.name);
        //! Create the projectile
        VFXPlayer.TriggerVFX(vfxObj, spellAnchor.position, Quaternion.LookRotation(self.transform.forward, Vector3.up));


        if(projectile != null)
        {
            Debug.Log("Launching projectile at position '" + target + "'");
            projectile.Launch(self, spellAnchor.position, target, deliveryType, projectileType, damageType, projectileSpeed, aoeRadius, effectDiameter, effectRange);
            projectile.OnImpact = () =>
            {
                if(sfxOnHit != null)
                    SFXPlayer.TriggerSFX(sfxOnHit[Random.Range(0, sfxOnHit.Length)],
                        projectile.transform.position);
                //! Create this effect when the projectile arrives at its target position
                if(id_VFXOnHit != "")
                    VFXPlayer.TriggerVFX(PoolSystem.GetPoolObject(id_VFXOnHit, ObjectPoolingCategory.VFX), projectile.transform.position, Quaternion.identity);
                //if(duration > 1)
                //    spellTarget.Execute_ApplyStatusEffect(new StatusEffect_Damage(1, false, 1, duration, 1));
            };
            //Debug.LogError("No projectile component on '" + vfxObj.name + "'");
        }
        else
        {
            Debug.LogError("Projectile null");
        }
    }


    public void Extinguish()
    {
        //if(self.debug)
        Debug.Log("<color=orange>Extinguishing magic effect</color>");

        if(projectile != null)
        {
            projectile.StopCoroutine("CR_ClampToAndFade");
            projectile.StopAndDisable("MagicEffect -> Extinguish");
            projectile = null;
        }
    }

    internal static EffectData GetEffectData(MagicEffect magicEffect)
    {
        EffectData effectData = new EffectData();
        effectData.isPermanent = magicEffect.IsPermanent;
        effectData.magnitude = magicEffect.Magnitude;
        effectData.duration = magicEffect.duration;
        effectData.aoeRadius = magicEffect.aoeRadius;
        effectData.effect = magicEffect.statusEffect;
        effectData.damageType = magicEffect.damageType;
        effectData.castingType = magicEffect.castingType;
        effectData.deliveryType = magicEffect.deliveryType;
        effectData.keywordFlags = magicEffect.keywords;
        effectData.effectDiameter = magicEffect.effectDiameter;

        effectData.vfxid_impact = magicEffect.id_VFXOnImpact;
        effectData.vfxid_inflict = magicEffect.id_VFXOnInflict;
        effectData.vfxid_handrelease = magicEffect.id_VFXHandRelease;
        effectData.vfxid_handidle = magicEffect.id_VFXHandIdle;
        effectData.vfxid_handcharge = magicEffect.id_VFXHandCharge;
        effectData.vfxid_projectile = magicEffect.id_VFXProjectile;

        effectData.sfxCharge = magicEffect.sfxChargeSpell;
        effectData.sfxCastLoop = magicEffect.sfxChargeSpell;
        effectData.sfxOnHit = magicEffect.sfxOnHit;
        effectData.sfxRelease = magicEffect.sfxOnRelease;

        return effectData;
    }

    protected void ClampTo(Actor self, Transform obj, Transform target, float duration)
    {
        self.StartCoroutine(CR_ClampToAndFade(self, obj, target, duration));
        //SceneManagment.FadeTo(obj.gameObject, 0, 0, 1, SceneManagment.OnFadeDoneAction.Disable);
    }

    private IEnumerator CR_ClampToAndFade(Actor self, Transform obj, Transform target, float duration)
    {
        while(duration > 0 && target != null && self != null && self.dead == false)
        {
            obj.position = target.position;
            obj.rotation = Quaternion.LookRotation(self.transform.forward);
            duration -= Time.deltaTime;
            yield return null;
        }

        obj.gameObject.SetActive(false);
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
    public float duration;
    public float magnitude;
    //public float travelSpeed;
    public bool isPermanent;
    public Status effect;
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

    public AudioClip sfxCharge;
    public AudioClip[] sfxRelease;
    public AudioClip sfxCastLoop;
    public AudioClip[] sfxOnHit;
    internal float effectDiameter;
}

public enum Effects
{
    Shield = 3,
    FireShield = 4,
    LightningShield = 5,
    FrostShield = 6,
    Burden = 7,
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