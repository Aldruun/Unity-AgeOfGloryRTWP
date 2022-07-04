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

    internal ActorInput InstantiateAndSetUpActor(string uniqueID, Vector3 position, Quaternion rotation)
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
              

                List<Skill> skills = new List<Skill>();
                config.Skillbook.Init(ref skills);
                npcInput.SetSkills(skills);
                npcInput.FinalizeActor(config);
                npcInput.Inventory.AddItems(config.InventoryTemplate.items);
                //FinalizeActor(npcInput, inventory);

                return npcInput;
            }
        }

        Debug.LogError("ActorConfig not found");
        return null;
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
