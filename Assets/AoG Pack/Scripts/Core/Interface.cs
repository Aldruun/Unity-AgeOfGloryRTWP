//using System;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.Profiling;
//using UnityEngine.SceneManagement;

//[Flags]
//public enum QuitFlags
//{
//    NORMAL = 0,
//    QUITGAME = 1,
//    EXITGAME = 2,
//    CHANGESCRIPT = 4,
//    LOADGAME = 8,
//    ENTERGAME = 16,
//    KILL = 32
//}

//[Flags]
//public enum GameStateFlags
//{
//    TACTICALPAUSE = 1 << 0,
//    PAUSE = 1 << 1,
//    DIALOGPAUSE = 1 << 2,
//}

//public enum GameStage
//{
//    Idle,
//    ManualLoading,
//    SceneTransition
//}

//public class Interface : MonoBehaviour
//{
//    //public static MonoBehaviour coroutineRunner;

//    [Header("Debugging")] [SerializeField] bool disableConsoleLogging;
//    public static Interface core;
//    public static GameSettings gameSettings;
//    public static SpellCompendium spellCompendium;
//    //public static ActorSFXManager sfxManager;
//    //public static ActorVFXManager vfxManager;
  
//    public int targetFPS = 59;

//    uint gameStateFlags;
//    QuitFlags quitflag;

//    //public GameStage gameStage = GameStage.Idle;
//    //public static GameObject playerInstance;

//    public static Action OnInitInventories;

//    public static Dictionary<Stat, int> statDictionaryTemplate = new Dictionary<Stat, int>();
//    //public static Dictionary<Status, int> statusDictionaryTemplate = new Dictionary<Status, int>();
//    public static Dictionary<Status, StatusEffect> statusDictionaryTemplate = new Dictionary<Status, StatusEffect>();
//    public static Dictionary<WeaponProficiency, int> weaponProfMapTemplate = new Dictionary<WeaponProficiency, int>();
//    //public CellData activeCell;

//    static Game _currentGame;
//    static SelectionManager _selectionManager;
//    static WorldMap _worldMap;
//    static UIHandler _uiScript;
//    static PlayerCamera _cameraScript;
//    internal FoW.FogOfWarTeam fow;

//    void Awake()
//    {
//        //! Things that only need to be initialized once

//        if(core == null)
//        {
//            core = this;
//            if(transform.childCount == 0)
//                Instantiate(Resources.Load<GameObject>("Prefabs/Game Logic/Persistent Managers"), Vector3.zero, Quaternion.identity, transform);
//        }

//        Profiler.BeginSample("INTERFACE Load ScriptableObjects");
//        gameSettings = Resources.Load<GameSettings>("ScriptableObjects/GameSettings");
//        spellCompendium = Resources.Load<SpellCompendium>("ScriptableObjects/SpellCompendium");
//        Profiler.EndSample();

//        if(gameSettings == null)
//            Debug.LogError("GameMaster: Initilization failed -> GameSettings was null");
//        if(spellCompendium == null)
//            Debug.LogError("GameMaster: Initilization failed -> SpellCompendium was null");

//        QualitySettings.vSyncCount = 0;
//        Application.targetFrameRate = targetFPS;

//        Profiler.BeginSample("INTERFACE Set dict templates");
//        foreach(Stat item in Enum.GetValues(typeof(Stat)))
//        {
//            if(statDictionaryTemplate.ContainsKey(item) == false)
//                statDictionaryTemplate.Add(item, 0);
//        }
//        foreach(Status item in Enum.GetValues(typeof(Status)))
//        {
//            if(statusDictionaryTemplate.ContainsKey(item) == false)
//                statusDictionaryTemplate.Add(item, null);
//        }
//        foreach(WeaponProficiency item in Enum.GetValues(typeof(WeaponProficiency)))
//        {
//            if(weaponProfMapTemplate.ContainsKey(item) == false)
//                weaponProfMapTemplate.Add(item, 0);
//        }
//        Profiler.EndSample();

//        //GameEventSystem.RequestDespawn = DespawnObject;

        
//        _currentGame = new Game(SceneManager.GetActiveScene().name);

//        Profiler.BeginSample("INTERFACE ResourceManager.LoadResources()");
//        ResourceManager.LoadResources();
//        PoolSystem.LoadPoolObjects();
//        Profiler.EndSample();

//        //SceneManager.sceneLoaded -= OnSceneLoaded;
//        //SceneManager.sceneLoaded += OnSceneLoaded;
//        //SceneManager.sceneUnloaded -= OnSceneUnloaded;
//        //SceneManager.sceneUnloaded += OnSceneUnloaded;

//        _selectionManager = new SelectionManager();

//        DontDestroyOnLoad(gameObject);
//    }

//    //void OnSceneLoaded(Scene scene, LoadSceneMode arg1)
//    //{
//    //    PoolSystem.LoadPoolObjects();
//    //}

//    //void OnSceneUnloaded(Scene scene)
//    //{
//    //    Release();

//    //}

//    void Start()
//    {

//        Init();
//        //bool allOk;
//        //do
//        //{


//        //} while((quitflag & QuitFlags.QUITGAME) == 0);

//        //Application.Quit();
//    }

//    void Update()
//    {
        
//        GameLoop();
      
//        if((quitflag & QuitFlags.QUITGAME) != 0)
//        {
//#if UNITY_EDITOR
//            // Application.Quit() does not work in the editor so
//            // UnityEditor.EditorApplication.isPlaying need to be set to false to end the game
//            UnityEditor.EditorApplication.isPlaying = false;
//#else
//         Application.Quit();
//#endif
//        }

//    }

//    internal void Release()
//    {
//        //_currentGame.ReleaseCurrentAction();
//        _currentGame.Release();
//        //_cameraScript.Release();
//        _uiScript.Release();
//        _selectionManager.Release();
//    }

//    void Init()
//    {
//        Profiler.BeginSample("INTERFACE INIT new WorldMap");
//        _worldMap = new WorldMap();
//        Profiler.EndSample();
//        Profiler.BeginSample("INTERFACE INIT new PlayerCamera");
//        _cameraScript = new PlayerCamera(Camera.main);
//        Profiler.EndSample();
//        Profiler.BeginSample("INTERFACE INIT new UIHandler");
//        _uiScript = new UIHandler(this, _cameraScript.camera.transform.GetComponent<UnityEngine.Rendering.Volume>());
//        Profiler.EndSample();
//        //if(PlayerController.Instance == null)
//        //{
//        //    AgentData data = new AgentData();
//        //    data.isPlayer = true;
//        //    data.gender = Gender.Female;
//        //    data.Name = "Player";
//        //    SpawnPoint.SpawnHumanNPC(data);
//        //}

//        Debug.Log("<color=green>GameMaster:Awake</color>");
        
//        //Debug.unityLogger.logEnabled = disableConsoleLogging == false;
//        //DontDestroyOnLoad(gameObject);

        
//        Debug.Log("<color=green>GameMaster:Init Inventories</color>");
//        //OnInitInventories?.Invoke(); // Player inventory needs to be present before the first PC joins the party

//        GameObject mapSetup = GameObject.FindWithTag("MapSetup");
//        //MapInfo
//        if(mapSetup != null)
//        {
//            Profiler.BeginSample("INTERFACE INIT InitSpawns");
//            _currentGame.GetCurrentMap().InitSpawns(mapSetup.transform.Find("Spawnpoints").GetComponentsInChildren<SpawnPoint>());
//            Profiler.EndSample();
//            Profiler.BeginSample("INTERFACE INIT InitActors");
//            _currentGame.GetCurrentMap().InitActors();
//            Profiler.EndSample();
//            MapInfo mapInfo = mapSetup.GetComponent<MapInfo>();
//            fow = mapSetup.transform.Find("FOW").GetComponent<FoW.FogOfWarTeam>();
//            _currentGame.GetCurrentMap().AddFootstepLinks(mapInfo.footstepLinks);
//            //_cameraScript.SetCameraRotation(new Quaternion(mapInfo.cameraXRotation, mapInfo.cameraYRotation, 0, 0));
//        }

//        Debug.Log("<color=green>GameMaster:Init NPCs</color>");
//    }

//    void GameLoop()
//    {
//        //bool run = (quitflag & QuitFlags.QUITGAME) == 0;

//        bool uiPanelActive = false;
//        _uiScript.Update(ref uiPanelActive);

//        if(uiPanelActive == false)
//            _cameraScript.UpdateControls();

//        _selectionManager.Update();

//        if(GameStateFlagsContain(GameStateFlags.PAUSE) == false)
//            _currentGame.UpdateScripts();

//        //return run;
//    }

//    public static PlayerCamera GetCameraScript()
//    {
//        return _cameraScript;
//    }

//    public static UIHandler GetUIScript()
//    {
//        return _uiScript;
//    }

//    public static Game GetCurrentGame()
//    {
//        return _currentGame;
//    }

//    public void TogglePartyAI(bool on)
//    {
//        foreach(ActorInput pc in GetCurrentGame().PCs)
//        {
//            if(pc.selected)
//                pc.aiControlled = on;
//        }
//    }

//    void DespawnObject(GameObject go)
//    {
//        go.AddComponent<GarbageFader>();
//    }

//    public ActorInput GetFirstSelectedPC(bool forced)
//    {
//        ActorInput ret = null;
//        int slot = 0;
//        int partySize = _currentGame.GetPartySize(false);
//        if(partySize == 0)
//            return null;
//        for(int i = 0; i < partySize; i++)
//        {
//            ActorInput actor = _currentGame.GetPC(i, false);
//            if(actor.selected)
//            {
//                if(actor.InParty < slot || ret == null)
//                {
//                    ret = actor;
//                    slot = actor.InParty;
//                }
//            }
//        }

//        if(forced && ret == null)
//        {
//            return _currentGame.FindPC(1);
//        }
//        return ret;
//    }

//    //private PlayerController CreatePlayer()
//    //{
//    //    Camera cam = Camera.main;
//    //    if(cam == null)
//    //    {
//    //        cam = Instantiate(Resources.Load<Camera>("Prefabs/Managers/Persistent/_TPCamera #"));
//    //    }
//    //    if(PlayerController.Instance != null)
//    //    {
//    //        return PlayerController.Instance;
//    //        //Destroy(PlayerController.Instance);
//    //        //Debug.Log("Destroying existing player instance");
//    //    }
//    //    //if(PlayerController.Instance == null)
//    //    //{
//    //    Debug.Log("# Creating player");
//    //    ActorData ActorRecord = new ActorData();
//    //    ActorRecord.gender = Gender.Female;
//    //    ActorRecord.Name = ActorRecord.Name == "" ? "Player" : ActorRecord.Name;
//    //    Transform playerSpawnPoint = GameObject.FindWithTag("PlayerSpawnPoint").transform;
//    //    PlayerController.Instance = SpawnPoint.SpawnPlayer(ActorRecord.gender, ActorRecord.level, Vector3.zero, Quaternion.identity);
//    //    PlayerController.Instance.LoadData(ActorRecord);
//    //    PlayerController.Instance.transform.position = playerSpawnPoint.position;
//    //    PlayerController.Instance.transform.rotation = playerSpawnPoint.rotation;
//    //    PlayerController.Instance.SetLevel(25);
//    //    CellMonoUpdater.Instance.AddActor(PlayerController.Instance);


//    //    //playerObject.transform.position = CellMonoUpdater.Instance.entryPoints[0].transform.position;
//    //    //PlayerController.Instance.Init();
//    //    //}
//    //    //else
//    //    //{
//    //    //    Debug.LogError("Player already exists");
//    //    //}

//    //    cam.GetComponent<ThirdPersonCamera2>().SetPlayerTarget(PlayerController.Instance);

//    //    return PlayerController.Instance;
//    //}



//    public static int GetGameDifficulty()
//    {
//        return (int)GameStateManager.Instance.gameSettings.gameDifficulty;
//    }

//    void QuitGame()
//    {
//        quitflag ^= QuitFlags.QUITGAME;
//    }

//    public void SetGameStateFlags(GameStateFlags flags)
//    {
//        gameStateFlags |= (uint)flags;
//    }

//    public void DisableGameStateFlags(GameStateFlags flags)
//    {
//        gameStateFlags &= ~(uint)flags;
//    }

//    public void ToggleGameStateFlags(GameStateFlags flags)
//    {
//        gameStateFlags ^= (uint)flags;
//    }

//    public bool GameStateFlagsContain(GameStateFlags flags)
//    {
//        return ((gameStateFlags & (uint)flags) != 0);
//    }

//    void OnGUI()
//    {
//        GUI.Label(new Rect(1, 1, 140, 30), "Num Map Scriptables: " + Scriptable.globalScriptableCounter);
//        GUI.Label(new Rect(171, 1, 140, 30), "Selected: " + SelectionManager.selected.Count);
//        GUI.Label(new Rect(251, 1, 140, 30), "Num PCs: " + GetCurrentGame().PCs.Count);

//        if(GUI.Button(new Rect(331, 41, 130, 30), "Quit Game"))
//        {
//            //! To remove a flag: quitflag &= ~QuitFlags.QUITGAME;
//            //! To toggle a flag: quitflag ^= QuitFlags.QUITGAME;
//            QuitGame();
//            //! To set a flag: quitflag |= QuitFlags.QUITGAME;
//        }
//    }
//}