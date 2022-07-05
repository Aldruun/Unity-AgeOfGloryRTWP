using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Profiling;

namespace AoG.Core
{
    [System.Serializable]
    public class ControlStatus
    {
        public int partyAIEnabled = 1;
    }

    [System.Serializable]
    public class Game /*: Scriptable*/
    {
        private bool doQuit;
        internal List<Actor> PCs = new List<Actor>();
        internal List<Actor> NPCs = new List<Actor>();

        private Dictionary<string, Map> maps;
        private Map currentMap;
        internal ControlStatus controlStatus;
        private DatabaseService databaseService;

        public bool PartyAttack { get; internal set; }

        public Game(DatabaseService databaseService) // Called in Awake
        {
            this.databaseService = databaseService;
            controlStatus = new ControlStatus();

            Debug.Log("# <color=green>Setting initial game</color>");
            //InitScriptable(ScriptableType.GLOBAL);
            maps = new Dictionary<string, Map>();

            GameEventSystem.RequestGetPCByPartyIndex = GetPCByPartyIndex;
        }

        private void CreatePartyMember(Actor pc) // Called in Start of SpawnPoint.cs
        {
            SpawnPoint playerSpawnpoint = GameObject.FindWithTag("PlayerSpawnpoint").GetComponent<SpawnPoint>();

            pc.gameObject.tag = "Player";
            pc.PartySlot = PCs.Count + 1;
            if(pc.IsSummon == false)
            {
                Debug.Log("Requesting portrait");
                UIActorPortrait portrait = GameEventSystem.RequestCreatePortrait?.Invoke(pc.PartySlot, pc.transform);
                pc.Combat.UnregisterCallback_OnHealthChanged(portrait.UpdatePortraitHealthbar);
                pc.Combat.RegisterCallback_OnHealthChanged(portrait.UpdatePortraitHealthbar);
                pc.ActorUI.SetPortrait(portrait, databaseService.ActorDatabase.GetActorByUniqueID(pc.UniqueID).portraitSprite);
                pc.Combat.Callback_OnHealthChanged();

            }
            PCs.Add(pc);
            if(PCs.Count == 1)
            {
                //! Select first actor
                //GameEventSystem.OnHeroPortraitClicked?.Invoke(pc, true);
                GameEventSystem.RequestCameraJumpToPosition?.Invoke(pc.transform.position);
                GameEventSystem.OnPCSelectionStateChanged?.Invoke(pc, true);
            }
        }

        public GameObject[] GetPCObjects()
        {
            GameObject[] pcObjects = PCs.Select(c => c.gameObject).ToArray();
            return pcObjects;
        }

        public GameObject[] GetPCSummonedCreatureObjects()
        {
            List<GameObject> summonedList = new List<GameObject>();
            for(int i = 0; i < PCs.Count; i++)
            {
                foreach(Actor creature in PCs[i].summonedCreatures)
                {
                    summonedList.Add(creature.gameObject);
                }
            }
            return summonedList.ToArray();
        }

        private void SpawnActors(Map map, Transform mapHierarchy)
        {
            SpawnPoint[] spawnpoints = mapHierarchy.Find("Spawnpoints").GetComponentsInChildren<SpawnPoint>();

            for(int i = 0; i < spawnpoints.Length; i++)
            {
                SpawnPoint spawnpoint = spawnpoints[i];
                Actor spawned = databaseService.ActorDatabase.InstantiateAndSetUpActor(spawnpoint.UniqueID, spawnpoint.transform.position, spawnpoint.transform.rotation);
                ActorUtility.Initialization.CalculateCharacterStats(spawned.ActorStats, 1);
                spawned.InititializeSpellbook(databaseService.SpellCompendium.GetSpellsForClassAtLevel(spawned.ActorStats.Class, 1));

                spawned.aiControlled = true;

                Debug.Assert(spawned != null, "Spawn from spawnpoint '" + spawnpoint.name + "' null");

                if(spawned.IsPlayer)
                {
                    // Needs to be done before actor.Initialize()
                    // because here the partyIndex gets set
                    CreatePartyMember(spawned);
                }

                map.AddActor(spawned);
                //worldUpdater.AddNPC(spawned);
            }
        }

        //public void OnTransitionDone()
        //{
        //    Debug.Log(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>> Transition done");

        //    ResourceManager.LoadResources();
        //    PoolSystem.ClearPools();
        //    //PoolSystem.LoadPoolObjects();
        //    //mapInfo = GameObject.FindWithTag("MapInfo").GetComponent<MapInfo>();
        //    //uiHandler = new UIHandler(Camera.main, mapInfo.transform.Find("PP Volume").GetComponent<Volume>());

        //    GameEventSystem.OnSceneLoaded?.Invoke();
        //    //CreatePartyMember(Camera.main);
        //}

        public void UpdateScripts()
        {
            Profiler.BeginSample("Game.currentMap.UpdateScripts");
            currentMap.UpdateScripts();
            Profiler.EndSample();
        }

        public void AddMap(string sceneIdentifier)
        {
            if(maps.ContainsKey(sceneIdentifier) == false)
            {
                Map newMap = new Map(sceneIdentifier);
                //newMap.sceneIndex = sceneIdentifier;
                maps.Add(sceneIdentifier, newMap);
            }
        }

        public Map FindMap(string sceneIdentifier)
        {
            Map map = maps[sceneIdentifier];
            return map;
        }

        public Map GetCurrentMap()
        {
            return currentMap;
        }

        public void InitMap(string sceneIdentifier, Transform mapHierarchy)
        {
            if(maps.ContainsKey(sceneIdentifier) == false)
            {
                AddMap(sceneIdentifier);
            }

            currentMap = maps[sceneIdentifier];

            MapInfo mapInfo = mapHierarchy.GetComponent<MapInfo>();
            GetCurrentMap().AddFootstepLinks(mapInfo.footstepLinks);
            Profiler.BeginSample("GameInterface: InitSpawns");
            //GetCurrentMap().InitSpawns(mapHierarchy.transform.Find("Spawnpoints").GetComponentsInChildren<SpawnPoint>(), ref PCs);
            SpawnActors(currentMap, mapHierarchy);
            Profiler.EndSample();

            //SetMap(currentMap);
            //currentMap.InitScriptable(ScriptableType.AREA);
        }

        private int InParty(Actor pc)
        {
            for(int i = 0; i < PCs.Count; i++)
            {
                if(PCs[i] == pc)
                {
                    return i;
                }
            }
            return -1;
        }

        internal Actor GetPCByPartyIndex(int partyIndex)
        {
            foreach(var a in PCs)
            {
                if(a.PartySlot == partyIndex)
                {
                    return a;
                }
            }

            return null;
        }

        //public class GameScene
        //{
        //    public string sceneName;
        //    public bool isInterior;
        //    public SpawnPoint[] spawnpoints;

        //    public GameScene(string sceneName, bool isInterior)
        //    {
        //        this.sceneName = sceneName;
        //        this.isInterior = isInterior;
        //    }
        //}


        #region Serialization
        public GameData CollectSaveData()
        {
            //List<object> playerData = new List<object>();
            //Dictionary<string, ActorData> actorData = new Dictionary<string, ActorData>();
            List<ActorData> actors = new List<ActorData>();
            foreach(Actor actor in PCs)
            {
                actors.Add(actor.CollectData());
            }
            //foreach(ActorInput actor in NPCs)
            //{
            //    actors.Add(actor.CollectData());
            //}
            //actorData.Add("GameActorData", actors);

            GameData gamedata = new GameData(currentMap.SceneName, actors);
            return gamedata;
        }

        internal void Load(GameData data)
        {
            foreach(Actor actor in PCs)
            {
                UnityEngine.Object.Destroy(actor.gameObject);
            }

            foreach(ActorData actorData in data.PCs)
            {
                Actor spawnedActor = databaseService.ActorDatabase.InstantiateAndSetUpActor(actorData.UniqueID, actorData.WorldPosition.ToVector(), Quaternion.Euler(actorData.WorldEulerAngles.ToVector()));
                ActorUtility.Initialization.CalculateCharacterStats(spawnedActor.ActorStats, actorData.Level);
                spawnedActor.InititializeSpellbook(databaseService.SpellCompendium.GetSpellsForClassAtLevel(spawnedActor.ActorStats.Class, actorData.Level));
            }
        }
        #endregion Serialization End
    }

    [System.Serializable]
    public class GameData
    {
        public string CurrentSceneName;
        public List<ActorData> PCs;

        public GameData(string currentSceneName, List<ActorData> pCs)
        {
            CurrentSceneName = currentSceneName;
            PCs = pCs;
        }

        public GameData()
        {

        }
    }
}