using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

public enum ScriptPriority
{
    PR_SCRIPT,
    PR_DISPLAY,
    PR_IGNORE
}

public enum AreaType
{
    OUTSIDE,
    INTERIOR
}

public class Map
{
    private float _garbageTimer = 1;
    public Dictionary<GameObject, int> garbage { get; } = new Dictionary<GameObject, int>();

    private SpawnPoint[] _spawnPoints;
    private FootstepLink[] footstepLinks;
    public string SceneName;
    public bool IsInterior;

    //public int gameTime;
    private bool pcInArea;
    public List<Actor> actors = new List<Actor>();
    //public List<Container> containers = new List<Container>();
    //public List<Door> doors = new List<Door>();

    //public List<ActorMonoController> traders { get; } = new List<ActorMonoController>();
    //public List<MonoPickupItem> pickupItems { get; private set; } = new List<MonoPickupItem>();

    //public static System.Action<ActorInput> OnActorInit;

    //HashSet<ContainerMonoObject> pickupItemCollection;

    public LoadCellTrigger[] entryPoints;

    public Map(string sceneName)
    {
        SceneName = sceneName;
    }

    //public void Load()
    //{
    //    sceneName
    //}

    public void UpdateScripts()
    {
        _garbageTimer -= Time.deltaTime;
        if(_garbageTimer <= 0)
        {
            _garbageTimer = 1;
            foreach(var key in garbage.Keys.ToArray())
            {
                garbage[key]--;

                if(garbage[key] <= 0)
                {
                    GameObject go = key;
                    garbage.Remove(key);
                    Object.Destroy(go);
                }
            }
        }

        //bool has_pcs = false;
        //foreach(ActorInput actor in actors)
        //{
        //    if(actor.ActorRecord.HasActorFlag(ActorFlags.PC))
        //    {
        //        has_pcs = true;
        //        break;
        //    }
        //}

        if(pcInArea == false && (actors.Count == 0))
        {
            return;
        }

        if(pcInArea)
        {
            //UpdateScriptableLoop();
        }
        else
        {
            //ProcessActions();
        }

        //GenerateQueues();
        //SortQueues();

        //gameTime++;
        foreach(Actor actor in actors)
        {
            //if(actor.GetCurrentArea() != this)
            //{
            //    continue;
            //}

            actor.UpdateActiveCellBehaviours();
        }
    }

    public void InitSpawns(SpawnPoint[] spawnPoints, ref List<Actor> spawnedPCs)
    {
        Debug.Log("# <color=green>Init Spawns</color>");
        _spawnPoints = spawnPoints;
        foreach(SpawnPoint sp in spawnPoints)
        {
            if(sp.SpawnType == SpawnType.ONSTART)
            {
                Actor spawnedActor = sp.Spawn();

                if(spawnedActor.ActorStats.HasActorFlag(ActorFlags.PC))
                {
                    GameEventSystem.OnPCSpawned?.Invoke(spawnedActor);
                    pcInArea = true;
                    spawnedPCs.Add(spawnedActor);
                }

                actors.Add(spawnedActor);
            }
        }
    }

    ////call this once, after area was loaded
    //internal void InitActors()
    //{
    //    // setting the map can run effects, so play on the safe side and ignore any actors that might get added
    //    foreach(ActorInput actor in actors)
    //    {
    //        //actor.SetMap(this);
    //        // make sure to bump away in case someone or something is already there
    //        //actor.navAgent.Warp(actor.transform.position + Vector3.left * 0.5f);
    //        InitActor(actor);
    //    }
    //}

    //private void InitActor(ActorInput actor)
    //{
    //}

    internal void AddActor(Actor monoAgent)
    {
        if(actors.Contains(monoAgent))
        {
            Debug.LogError("!!! Agent '<color=white>" + monoAgent.GetName() + "</color>' already in map list");
            return;
        }

        actors.Add(monoAgent);
        //switch(monoAgent.ActorRecord.faction)
        //{
        //    case Faction.Heroes:
        //        break;

        //    case Faction.Bandits:
        //        break;

        //    case Faction.Monsters:
        //        break;

        //    case Faction.Traders:
        //        //traders.Add(monoAgent);
        //        break;
        //}
        GameEventSystem.OnActorDied -= RemoveActorFromMap;
        GameEventSystem.OnActorDied += RemoveActorFromMap;
        //monoAgent.Init();
        //OnActorInit?.Invoke(monoAgent);
        //}
    }

    private void RemoveActorFromMap(Actor actor)
    {
        actors.Remove(actor);
        Debug.Log("<color=orange>Removing actor </color>'" + actor.GetName() + "'");
    }

    internal void AddFootstepLinks(FootstepLink[] footstepLinks)
    {
        this.footstepLinks = footstepLinks;
    }

    internal FootstepLink[] GetFootstepLinks()
    {
        return footstepLinks;
    }

    private bool MustSave(Actor actor)
    {
        if(actor.ActorStats.HasActorFlag(ActorFlags.ESSENTIAL))
        {
            return false;
        }

        return true;
    }


    //public void MoveToArea(string sceneName/*, string entrance, Quaternion rotation, ActorMonoController actor*/)
    //{
    //    Game game = GameStateManager.Instance.GetCurrentGame();

    //    game.LoadMap(sceneName);
    //}

    public void Save(string filePath)
    {
        //cellData.SaveData(this);

        //string json = JsonUtility.ToJson(cellData);

        //StreamWriter sw = File.CreateText(filePath);
        //sw.Close();

        //File.WriteAllText(filePath, json);
    }

    internal void SelectActors()
    {
        foreach(Actor actor in actors)
        {
            //if(actor.GetModifiedStat[Stat.ea] < EA_CONTROLLABLE)
            //{
            //SelectionManager.SelectPC(actor, true);
            //}
        }
    }

    //internal bool AnyEnemyNearPoint(Vector3 position)
    //{
    //    foreach(ActorInput actor in actors)
    //    {
    //        if(actor.dead)
    //        {
    //            continue;
    //        }

    //        if(FactionExentions.IsEnemy(actor, Faction.Heroes) == false)
    //        {
    //            continue;
    //        }

    //        if(Vector3.Distance(actor.transform.position, position) > 10)
    //        {
    //            continue;
    //        }

    //        return true;
    //    }

    //    return false;
    //}

    //public List<ActorInput> GetAllActorsInRadius(Vector3 p, ActorFlags flags, float radius, Scriptable see)
    //{

    //    List<ActorInput> neighbours = new List<ActorInput>();
        
    //    foreach(ActorInput actor in actors)
    //    {
    //        if(HelperFunctions.WithinRange(actor, p, radius) == false)
    //        {
    //            continue;
    //        }
    //        //if(actor.ValidTarget(flags, see) == false)
    //        //{
    //        //    continue;
    //        //}
    //        //if((flags & GetActorFlags.NO_LOS) == 0)
    //        //{
    //        //    //line of sight visibility
    //        //    if(IsVisibleLOS(actor.transform.position, p) == false)
    //        //    {
    //        //        continue;
    //        //    }
    //        //}
    //        neighbours.Add(actor);
    //    }
    //    return neighbours;
    //}

    private bool IsVisibleLOS(Vector3 s, Vector3 d)
    {
        if(Physics.Linecast(s, d, ~(1 << LayerMask.NameToLayer("Actors"))))
        {

            return true;
        }

        return false;
    }

    public void AddGarbage(GameObject garbage)
    {
        if(this.garbage.ContainsKey(garbage))
        {
            Debug.LogError("Garbage object already in dict");
            return;
        }

        this.garbage.Add(garbage, 10);
    }

    public void Release()
    {
        actors.Clear();
    }
}