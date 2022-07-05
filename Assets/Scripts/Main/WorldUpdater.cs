using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.SceneManagement;

public class WorldUpdater
{
    public List<Actor> actors;
    public List<GameObject> garbage;
    public List<PickupItem> pickupItems { get; private set; }
    private int extectedNumActors;
    private float _cleanUpInterval = 10;

    public WorldUpdater()
    {
        actors = new List<Actor>();
        garbage = new List<GameObject>();
      
        GameEventSystem.RequestAddGarbage = AddGarbage;
    }

    public void UpdateNPCs()
    {
        if (actors == null)
            return;

        for (int i = 0; i < actors.Count; i++)
        {
            if (actors[i] == null)
            {
                actors.RemoveAt(i);
                
                continue;
            }

            // if(actors.Count > extectedNumActors)
            //     GameEventSystem.OnNumActorsChanged?.Invoke

            Debug.Assert(actors[i] != null, "ActorInput null");
            Debug.Assert(actors[i].ActorStats != null, "ActorRecord null");
            //Debug.Log("<color=grey># Updating actor '" + actorsInCell[i].ActorRecord.Name + "' in cell [" + x + "|" + y + "]</color>");
            actors[i].UpdateActiveCellBehaviours();
        }
    }

    public void UpdateCorpseDisposalTimer()
    {
        if(garbage.Count > 0)
        {
            _cleanUpInterval -= Time.deltaTime;
            if(_cleanUpInterval <= 0)
            {
                _cleanUpInterval = 10;
                for(int i = 0; i < garbage.Count; i++)
                {
                    GameObject obj = garbage[i];

                    garbage.Remove(obj);
                    Object.Destroy(obj);

                }
            }
        }
    }

    //public void AddPlayer(ActorInput player)
    //{
        //this.player = player;
    //}

    //private void RemovePlayer()
    //{
    //    player = null;
    //}

    public void AddNPC(Actor monoAgent)
    {
        if (actors.Contains(monoAgent))
        {
            Debug.LogError("!!! Agent already in realm list");
        }
        actors.Add(monoAgent);
    }

    public void DespawnActor(Actor actorInput)
    {
        actors.Remove(actorInput);
    }
    
    private void AddGarbage(GameObject obj)
    {
        if (garbage == null)
            garbage = new List<GameObject>();
        if (this.garbage.Contains(obj))
        {
            Debug.LogError("Garbage object already in dict");
            return;
        }
        this.garbage.Add(obj);
    }
}
