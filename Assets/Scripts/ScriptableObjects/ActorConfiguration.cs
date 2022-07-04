using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Profiling;

[System.Serializable]
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
    public int levelMin = 1;
    public int levelMax = 1;
    public Sprite portraitSprite;
    public SkillBook Skillbook;

    public float ActorRadius = 0.3f;

    [Header("Stats")]
    public int Health = 100;

    public string VoicesetID;

    public int Level { get; set; }
    public Gender Gender { get; set; }
    public ActorRace Race { get; set; }
    public Class ActorClass { get; set; }
    public Faction Faction { get; set; }

    public void InitializeStats(ActorStats stats)
    {
        if(InventoryTemplate == null)
        {
            InventoryTemplate = Resources.Load<InventoryTemplate>("ScriptableObjects/InventoryTemplateDummy");
        }
        
        stats.Execute_ModifyLevel(Random.Range(levelMin, levelMax), ModType.ABSOLUTE);
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
