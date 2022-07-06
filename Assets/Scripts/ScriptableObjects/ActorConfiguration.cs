using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Profiling;

[CreateAssetMenu(fileName = "NPCConfiguration", menuName = "ScriptableObjects/NPC Configuration/NPCConfiguration")]
public class ActorConfiguration : ScriptableObject
{
    public AICombatProfile AICombatProfile;

    public GameObject ActorPrefab;
    public bool BakedWeapon;

    [Header("Actor Data")]
    public string UniqueID;
    public string Name;
    public InventoryTemplate InventoryTemplate;
    public ActorFlags ActorFlags;
    public Sprite portraitSprite;
    public SpellBook Spellbook;

    public float ActorRadius = 0.3f;

    public string VoicesetID;

    public int Level;
    public Gender Gender;
    public ActorRace Race;
    public Class ActorClass;
    public Faction Faction;

    public void InitializeStats(ActorStats stats)
    {
        if(InventoryTemplate == null)
        {
            InventoryTemplate = Resources.Load<InventoryTemplate>("ScriptableObjects/InventoryTemplateDummy");
        }
        
        stats.SetActorFlags(ActorFlags);
        stats.Gender = Gender;
        stats.Faction = Faction;
        stats.Race = Race;
        stats.Class = ActorClass;
        stats.Name = Name;
        stats.voicesetID = VoicesetID;
       
        stats.ActorRadius = ActorRadius;
    }

    public void ConfigureNavAgent(NavMeshAgent navAgent)
    {
        navAgent.updateRotation = false;
        AICombatProfile.ConfigureNPC(navAgent);
    }
}
