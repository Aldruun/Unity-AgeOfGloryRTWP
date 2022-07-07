using AoG.Serialization;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Profiling;

[System.Flags]
public enum SpellCastingFlags
{
    CanHealSelf,
    CanHealOther,
    HasDamageSpell
}

public abstract class Actor : Scriptable
{
    public bool debugSpellCastStates;
    public bool debugAnimation;
    internal bool debugInput;
    internal bool debugGear;
    internal bool debugInitialization;

    public bool isCloaked;
    public bool isCasting;
    public bool isDowned;
    public bool stunned;
    public bool panicked;
    public bool sleeping;

    internal string UniqueID;

    public bool dead =>
        ActorStats.HasActorFlag(ActorFlags.ESSENTIAL) == false &&
        ActorStats.GetStat(ActorStat.HITPOINTS) <= 0;

    public float HPPercentage => ActorStats.HPPercentage;

    public SpawnPoint spawnpoint;
    public ActorStats ActorStats;
    public SpellBook Spellbook;
    public ActorCombat Combat;
    public ActorEquipment Equipment;
    public Inventory Inventory;
    public ActorAnimation Animation;
    public ActorHeadTracking HeadTracking;
    public ConversationData conversationData;
    public AoGRoundSystem RoundSystem;

    internal List<NPCInput> companions { get; private set; }
    internal List<NPCInput> summonedCreatures { get; private set; }
    public NavMeshAgent NavAgent { get; protected set; }
    public CharacterController cc { get; protected set; }
    public int EscortIndex { get; protected set; }
    public CharacterVoiceSet CharacterVoiceSet { get; set; }
    public ActorUI ActorUI { get; set; }

    public HighlightPlus.HighlightEffect HighlightEffect { get; set; }

    protected MovementSpeed CurrentMovementSpeed;

    protected Vector3 colliderRoot;
    protected float lowestSpellMagickaCost;
    internal bool HasSpells { get; private set; }
    public int PartySlot { get; internal set; }

    private readonly SpellCastingFlags spellCastingFlags;

    public bool IsPlayer => ActorStats.HasActorFlag(ActorFlags.PC);
    public bool IsAlly => ActorStats.HasActorFlag(ActorFlags.ALLY);
    public bool IsSummon => ActorStats.HasActorFlag(ActorFlags.SUMMONED);

    public bool IsRanged { get; set; }
    internal bool aiControlled;
    internal bool hasMovementOrder;
    protected Action OnMovementOrderDone;
    protected Vector3 desiredTargetReachedDirection;
    internal bool inWater;
    private Vector3 currentDestination;
    private StatusEffectSystem statusEffectSystem;

    public virtual void FinalizeActor(ActorConfiguration config)
    {
        InitScriptable(ScriptableType.ACTOR);

        ActorStats = new ActorStats();
        statusEffectSystem = new StatusEffectSystem(this);
        RoundSystem = new AoGRoundSystem(this, statusEffectSystem);
        UniqueID = config.UniqueID;
        InitializeStats(config);

        InitializeEquipment(ActorStats);

        Animation = GetComponent<ActorAnimation>();
        if(Animation == null)
        {
            Animation = gameObject.AddComponent<ActorAnimation>();
        }

        Animation.Initialize(this, GetComponent<Animator>());
        Animation.ChangeForm(AnimationSet.DEFAULT);

        Combat = GetComponent<ActorCombat>();
        if(Combat == null)
        {
            Combat = gameObject.AddComponent<ActorCombat>();
        }

        AudioSource voiceAudioSource = transform.Find("Audio/AS Voice").GetComponent<AudioSource>();
        CharacterVoiceSet = ResourceManager.voiceSetDatabase.GetVoiceSetByID(ActorStats.voicesetID);

        Combat.Initialize(this, ActorStats, Equipment, Animation, CharacterVoiceSet, voiceAudioSource);

        HighlightEffect = GetComponent<HighlightPlus.HighlightEffect>();

        ActorUI = new ActorUI(this, voiceAudioSource, HighlightEffect);
        ActorUI.ChangeRelationColor(ActorStats.GetActorFlags());

        InitializeHeadTracking();

        InitializeCharacterController();

        if(IsPlayer)
        {
            Instantiate(Resources.Load<GameObject>("Prefabs/GFX/unitfow"), transform, false);
        }

        companions = new List<NPCInput>();
        summonedCreatures = new List<NPCInput>();


        ChangeMovementSpeed(MovementSpeed.Run);

        Debug.Log(GetName() + ": Initialization done");
    }

    internal bool ValidTarget(ActorFlags actorFlags)
    {
        return dead == false;
    }

    private void InitializeStats(ActorConfiguration config)
    {
        config.InitializeStats(ActorStats);
    }

    private void InitializeHeadTracking()
    {
        HeadTracking = GetComponent<ActorHeadTracking>();
        if(HeadTracking == null)
        {
            HeadTracking = gameObject.AddComponent<ActorHeadTracking>();
        }
    }

    private void InitializeEquipment(ActorStats stats)
    {
        Inventory = new Inventory();
        Profiler.BeginSample("InventoryTemplate inventory.Init");
        Inventory.Init(null, InventoryType.Actor);
        Profiler.EndSample();

        Equipment = new ActorEquipment(this, stats, Inventory);
        Profiler.BeginSample("INVENTORY InitEquipment");
        Equipment.InitEquipment(); // Creates equipment slots
        Profiler.EndSample();

        Equipment.EquipBestArmor();
    }

    private void InitializeCharacterController()
    {
        cc = GetComponent<CharacterController>();
        if(cc == null)
        {
            cc = gameObject.AddComponent<CharacterController>();
        }
        cc.height = 1.82f;
        cc.center = new Vector3(0, 0.91f, 0);
        cc.radius = 0.2f;
        cc.stepOffset = 0.01f;
        colliderRoot = cc.center - (Vector3.up * (cc.height * 0.5f));
    }

    internal void ApplyStatusEffect(Status status, int rounds)
    {
        switch(status)
        {
            case Status.BERSERK:
            case Status.CONFUSED:
            case Status.ENTANGLED:
            case Status.FEEBLEMINDED:
            case Status.HELD:
            case Status.PANIC:
            case Status.PETRIFIED:
            case Status.POLYMORPHED:
            case Status.UNCONSCIOUS:
            case Status.STUN:
            case Status.SLEEP:
            case Status.WEBBED:
                ClearActions();
                break;
        }

        statusEffectSystem.ApplyStatusEffect(status, rounds);
    }

    internal bool HasStatusEffect(Status status)
    {
        return statusEffectSystem.HasStatusEffect(status);
    }

    public List<StatusEffect> GetAppliedStatusEffects()
    {
        return statusEffectSystem.GetAppliedStatusEffects();
    }

    public override void UpdateScriptTicks()
    {
        if(Immobile())
        {
            return;
        }

        int scriptDepth = 1;

        if(IsPlayer && AoG.Core.GameInterface.Instance.GetCurrentGame().controlStatus.partyAIEnabled == 0)
        {
            scriptDepth = 0;
        }

        ExecuteScript(scriptDepth);

        base.UpdateScriptTicks();
    }

    /// <summary>
    /// Commanding a PC will in most cases clear all other actions.
    /// One exception would be drinking a potion, which can be queued once.
    /// </summary>
    /// <param name="action"></param>
    public void CommandActor(GameAction action)
    {
        if(dead)
        {
            return;
        }

        FormationController.ClearFormationVisual(PartySlot);
        ClearActions();
        isCasting = false;
        //SetPortraitActionIcon?.Invoke(action);

        AddAction(action, true);

        //TODO Handle verbal constants and their probability
    }

    /// <summary>
    /// Commanding a PC will in most cases clear all other actions.
    /// One exception would be drinking a potion, which can be queued once.
    /// </summary>
    /// <param name="action"></param>
    public void MoveCommand(MoveAction action)
    {
        if(dead)
        {
            return;
        }
        CancelAnimations();
        //SetAttackTarget(null);
        hasMovementOrder = true;
        CommandActor(action);
        //TODO Handle verbal constants and their probability
    }

    public bool Immobile()
    {
        return isDowned;
    }

    public bool AtDestination()
    {
        return NavAgent.pathPending == false && AgentStopping();
    }

    private bool AgentStopping()
    {
        return NavAgent.remainingDistance <= NavAgent.stoppingDistance;
    }

    /// <summary>
    /// Call BEFORE activating a new skill, since AttackTarget will be nullified with this method.
    /// </summary>
    public void CancelActions()
    {
        if(dead)
        {
            return;
        }
        CancelAnimations();
        Debug.Log(GetName() + ":<color=orange>[partyindex: " + PartySlot + "] Canceling actions</color>");
        FormationController.ClearFormationVisual(PartySlot);
        //ClearActions();
        Combat.SetHostileTarget(null);
        isCasting = false;
        hasMovementOrder = false;
        //SetPortraitActionIcon?.Invoke(null);

        //AddAction(action, true);

        //TODO Handle verbal constants and their probability
    }

    internal void CancelAnimations()
    {
        //animator.Play("Cancel");
        Animation.Animator.Play("Cancel", 1);
        //animator.SetTrigger("tCancelAim");
        if(Animation.Animator.GetCurrentAnimatorStateInfo(2).IsName("New State") == false)
        {
            Animation.Animator.Play("Cancel", 2);
        }

        Animation.Animator.Play("Cancel", 3);
        //animator.SetLayerWeight(3, 0);

        //ResetAnimatorTriggers();
    }

    public virtual bool SetDestination(Vector3 destination, float stoppingDistance, string debug = "", int priority = 0, bool resetPriority = false, int pathType = -1)
    {
        if(debug != "")
        {
            Debug.Log("SetDestination debug: " + debug);
        }

        if(NavAgent.enabled == false || isDowned || sleeping)
        {
            return false;
        }

        if(currentDestination == destination)
        {
            return true;
        }

        currentDestination = destination;

        //if(priority < _currMovePriority)
        //{
        //    return false;
        //}

        //if(resetPriority)
        //{
        //    _currMovePriority = 0;
        //}
        //else
        //    _currMovePriority = priority;


        //if(navAgent == null)
        //{
        //    Debug.LogError(Name + ": No navagent found");
        //}

        NavAgent.isStopped = false;
        ////Callback_ChangeMovementState(movementState);
        NavAgent.stoppingDistance = stoppingDistance;

        if(NavAgent.SetDestination(destination) == false)
        {

            NavAgent.SetDestination(HelperFunctions.SnapToNavMesh(destination, "Humanoid"));
        }

        //OnNavMeshPathRequest?.Invoke(NavAgent, pathType);

        return true;
    }

    internal void HoldPosition()
    {
        SetDestination(transform.position, 0.1f, "", 0);
    }

    internal void Unroot()
    {
        NavAgent.isStopped = false;
    }

    internal void InititializeSpellbook(List<Spell> spells)
    {
        Profiler.BeginSample("Spellbook.Init");
        Spellbook = new SpellBook();
        Spellbook.SetSpells(spells);

        HasSpells = spells.Count > 0;
        Profiler.EndSample();
    }

    public void ChangeMovementSpeed(MovementSpeed movementState)
    {
        CurrentMovementSpeed = movementState;
        //OnChangedMovementState?.Invoke(movementState);
    }

    public abstract void UpdateActiveCellBehaviours();

    //! The following methods should probably go into a CharacterPhysics class
    public abstract Vector3 GetMovementVelocity();
    public abstract Vector3 GetLookAtPoint();
    public abstract void UpdateLookAtIK();
    public abstract void Signal_ProjectileIncoming(Transform projectile, float aoeRadius);

    internal abstract float GetCharacterRadius();

    internal void AddCompanion(NPCInput companion)
    {
        if(companions.Contains(companion))
        {
            Debug.LogError("Companion already in player's companion list");
        }
        companions.Add(companion);
    }

    internal void RemoveCompanion(NPCInput companion)
    {
        if(companions.Contains(companion) == false)
        {
            Debug.LogError("Companion not found in player's companion list");
        }
        companions.Remove(companion);
    }

    internal void UpdateSummonedCreatureStates()
    {
        foreach(NPCInput creature in summonedCreatures.ToArray())
        {
            if(creature == null || creature.dead)
            {
                summonedCreatures.Remove(creature);
                continue;
            }

            creature.UpdateActiveCellBehaviours();
        }
    }

    internal void AddSummonedCreature(NPCInput summonedCreature, bool isHostile)
    {
        if(isHostile)
        {
            summonedCreature.ActorStats.OverrideActorFlags(ActorFlags.SUMMONED | ActorFlags.HOSTILE);
        }
        else
        {
            summonedCreature.ActorStats.OverrideActorFlags(ActorFlags.SUMMONED | ActorFlags.PC);
        }

        summonedCreature.ActorUI.ChangeRelationColor(ActorStats.GetActorFlags());
        summonedCreatures.Add(summonedCreature);
    }

    internal void RemoveSummonedCreature(NPCInput summonedCreature)
    {
        summonedCreatures.Remove(summonedCreature);
    }

    public string GetName()
    {
        return ActorStats.Name;
    }

    #region Serialization
    public ActorData CollectData()
    {
        ActorData savedata = new ActorData(ActorStats, transform.position, transform.eulerAngles, Inventory.inventoryItems, Equipment.EquipmentSlots);
        savedata.UniqueID = UniqueID;
        Dictionary<string, ActorData> serializeData = new Dictionary<string, ActorData>();
        serializeData.Add("ActorData", savedata);

        return savedata;
    }

    public void ApplyData(ActorData savedata)
    {
        ActorData data = savedata;

        UniqueID = data.UniqueID;
        ActorStats.Level = data.Level;
        ActorStats.StatsBase = data.StatsBase;
        ActorStats.StatsModified = data.StatsModified;

        Inventory = new Inventory();
        Inventory.Init(data.InventoryItems, InventoryType.Actor);
        Equipment.EquipArmorFromEquipmentData(data.Equipment);

        transform.position = data.WorldPosition.ToVector();
        transform.eulerAngles = data.WorldEulerAngles.ToVector();
    }
    #endregion Serialization End
}

[System.Serializable]
public class ActorData
{
    public string UniqueID;
    public int Level;
    public ActorConfiguration ActorConfiguration;
    public Dictionary<ActorStat, int> StatsBase;
    public Dictionary<ActorStat, int> StatsModified;
    public SerializableVector3 WorldPosition;
    public SerializableVector3 WorldEulerAngles;
    public List<InventoryItem> InventoryItems;
    public Equipment Equipment;

    public ActorData(ActorStats record, Vector3 worldPosition, Vector3 worldEulerAngles, List<InventoryItem> inventoryItems, Equipment equipment)
    {
        Level = record.Level;
        StatsBase = record.StatsBase;
        StatsModified = record.StatsModified;
        WorldPosition = new SerializableVector3(worldPosition);
        WorldEulerAngles = new SerializableVector3(worldEulerAngles);
        InventoryItems = inventoryItems;
        Equipment = equipment;
    }

    public ActorData()
    {

    }
}