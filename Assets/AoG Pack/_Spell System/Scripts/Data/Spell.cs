using AoG.Core;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum SpellType // A spell's Type determines which subcategory it appears in under Spell in the Object Window. It also determines some of the spell's secondary effects or use restrictions:
{
    Ability, // are always-on, constant-effect spells, such as the Wood Elf's racial resistance to poison and disease.
    Disease, // represent diseases the player can acquire. Disease Resistance offers protection from these spells.
    LesserPower, // are classified as Powers in the Magic Menu, and can be used multiple times a day, such as the Khajiit's Nighteye Power.
    Poison, // represents poisons the player can use or acquire.Poison Resistance offers protection from these spells.
    Power, // can be used by the player once per day and show up in a separate section of the magic menu.
    Spell  //represents standard spells. These will be sorted by school in the Magic Menu.
}

public enum MagicSchool
{
    DESTRUCTION,
    RESTORATION,
    CONJURATION,
    ALTERATION,
    ENCHANTING,
    ILLUSION
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
    SeekActor,
    InstantSelf,
    InstantActor,
    InstantLocation,
    SeekLocation,
    Spray,
    Beam,
    Contact

    // All of the effects on a Spell must have the same casting type.
}

[System.Flags]
public enum Keyword
{
    None = 0,
    FireDamage = 1,
    FrostDamage = 1 << 1,
    LightningDamage = 1 << 2,
    DamageHealth = 1 << 3,
    DamageFatigue = 1 << 4,
    PoisonDamage = 1 << 5,
    HealSelf = 1 << 6,
    HealOther = 1 << 7,
    DebuffHealth = 1 << 8,
    DebuffMana = 1 << 9,
    BuffHealth = 1 << 10,
    BuffMana = 1 << 11
}

[CreateAssetMenu(menuName = "Spell System/Spell")]
public class Spell : ScriptableObject
{
    //public SpellData spellData;

    public string ID;
    public string Name;
    [TextArea]
    public string description;
    public Sprite spellIcon;

    public SpellType spellType;
    public MagicSchool magicSchool;
    public EquipType equipType;
    public CastingType castingType;
    public DeliveryType deliveryType;

    //public EffectList effectList;

    public List<MagicEffect> magicEffects;
    public List<EffectData> magicEffectsData;
    public SpellTargetLogic targetLogic;

    public int spellcastMotionIndex;
    public int releaseMotionIndex;

    public System.Action<float> OnPriorityChangedHook { get; set; }
    public bool needsPreparation;
    public bool needsToStopMoving;
    public int cost;
    public float activationRange;

    [Range(0.5f, 2f)] public float chargeTimeMult = 1f; // TODO: Get animation length
    [Range(0.5f, 2f)] public float castTimeMult = 1f; // TODO: Get animation length
    //public float cooldown = 2;
    //public float recoveryTime;

    [Tooltip("Determines how long this skill will be unavailable after it has been activated")]
    public float cooldownTime = 1;
    internal float cooldownTimer;
    public System.Action<float> CooldownHook;
    [Tooltip("Determines how long it takes until a new skill can be activated after this skill has been activated")]
    public float recoveryTime = 1;
    public float travelSpeed;
    public int grade;
    public float castingTime;

    public ActorStats spellTarget { get; protected set; }

    public virtual void Init(ActorStats self)
    {
        if(deliveryType != DeliveryType.InstantSelf && targetLogic == null)
        {
            Debug.Log("<color=grey>Spell '" + (Name == "" ? GetType().ToString() : Name) + "' has no targetLogic</color>");
        }
        else
        {
            //targetLogic = Instantiate(targetLogic);
            //targetLogic.Init(self);
        }

        for(int i = 0; i < magicEffects.Count; i++)
        {
            //magicEffects[i] = Object.Instantiate(magicEffects[i]);
            magicEffects[i].Init(this);
        }

        magicEffectsData = new List<EffectData>();
        foreach(MagicEffect eff in magicEffects)
        {
            magicEffectsData.Add(MagicEffect.GetEffectData(eff));
        }
    }

    //internal Actor GetAITarget(Actor self)
    //{
    //    switch(deliveryType)
    //    {
    //        case DeliveryType.Self:
    //            spellTarget = self;
    //            break;
    //        case DeliveryType.Contact:
    //            break;
    //        case DeliveryType.Aimed:
    //        //break;
    //        case DeliveryType.TargetActor:

    //            if(targetLogic != null)
    //                spellTarget = targetLogic.GetTarget(self);
    //            else
    //                spellTarget = HelperFunctions.GetClosestEnemy_WithJobs(self, 20);

    //            break;
    //        case DeliveryType.TargetLocation:
    //            break;
    //    }

    //    return spellTarget;
    //}

    public void Stop()
    {
        foreach(var effect in magicEffects)
        {
            effect.Extinguish();
        }
    }

    public float GetEffectDiameter()
    {
        return magicEffects[0].projectileDiameter;
    }


    public void StartCooldown()
    {
        CoroutineRunner.Instance.StartCoroutine(CR_Cooldown());
    }

    private IEnumerator CR_Cooldown()
    {
        cooldownTimer = cooldownTime;
        while(cooldownTimer > 0)
        {
            CooldownHook?.Invoke(cooldownTimer);
            cooldownTimer -= Time.deltaTime;
            yield return null;
        }
    }
}
