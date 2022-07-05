using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

[CreateAssetMenu(fileName = "ActorDatabase", menuName = "ScriptableObjectsActorDatabase")]
public class ActorDatabase : ScriptableObject
{
    public List<ActorConfiguration> Characters;
    public List<ActorConfiguration> Creatures;

    static Dictionary<ActorStat, int> actorAttributesTemplate;

    internal Actor InstantiateAndSetUpActor(string uniqueID, Vector3 position, Quaternion rotation)
    {
        for(int i = 0; i < Characters.Count; i++)
        {
            if(Characters[i].UniqueID == uniqueID)
            {
                ActorConfiguration config = Characters[i];
                GameObject actorObj = Instantiate(config.ActorPrefab, position, rotation);
                NPCInput npcInput = actorObj.AddComponent<NPCInput>();

                //actor.ActorStats.StatsBase[ActorStat.HEALTH] = Health;
                //actor.ActorStats.StatsBase[ActorStat.MAXHEALTH] = Health;
                //actor.ActorStats.StatsModified = new Dictionary<ActorStat, int>(actor.ActorStats.StatsBase);
              

                List<Spell> skills = new List<Spell>();
                config.Spellbook.Init(npcInput);
                //npcInput.set(skills);
                npcInput.FinalizeActor(config);
                
                npcInput.ActorStats.InitializeStats(CreateActorStatsDictionaryTemplate());
                npcInput.Inventory.AddItems(config.InventoryTemplate.items);
                npcInput.Equipment.EquipBestArmor();
                //FinalizeActor(npcInput, inventory);

                return npcInput;
            }
        }

        Debug.LogError("ActorConfig not found");
        return null;
    }

    //public void UpdateSpellbook(SpellBook spellbook, int characterLevel)
    //{
    //    if(spellbook == null)
    //    {
    //        spellbook = new SpellBook();
    //        spellbook.Init(this);
    //    }
    //}

    private Dictionary<ActorStat, int> CreateActorStatsDictionaryTemplate()
    {
        if(actorAttributesTemplate == null)
        {
            actorAttributesTemplate = new Dictionary<ActorStat, int>();
            foreach(ActorStat attr in System.Enum.GetValues(typeof(ActorStat)))
                actorAttributesTemplate.Add(attr, 0); 
        }

        return actorAttributesTemplate;
    }

    internal ActorConfiguration GetActorByUniqueID(string uniqueID)
    {
        for(int i = 0; i < Characters.Count; i++)
        {
            if(Characters[i].UniqueID == uniqueID)
            {
                return Characters[i];
            }
        }

        Debug.LogError("ActorConfig not found");
        return null;
    }
}
