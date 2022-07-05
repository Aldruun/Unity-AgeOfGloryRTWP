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

                //npcInput.set(skills);
                npcInput.FinalizeActor(config);
                npcInput.Inventory.AddItems(config.InventoryTemplate.items);
                
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
