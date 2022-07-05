using AoG.Controls;
using AoG.SceneManagement;
using AoG.UI;
using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

namespace AoG.Core
{
    /// <summary>
    /// The only MonoBehaviour. It's responsible for managing the high level game systems
    /// </summary>
    public class GameInterface : MonoBehaviour
    {
        public static GameInterface Instance { get; private set; }

        //////////////////////////////
        // ScriptableObjects
        //////////////////////////////
        public static int newPlaySceneIndex = 1;
        public static bool initialized;
        public string StartPlaySceneName;
        public DatabaseService DatabaseService;

        internal static UIScreenFader ScreenFader;
        private static Game currentGame;

        private UIHandler uiHandler;
        private PlayerCamera rpgCamera;
        private GameControl gameControl;
        private string currentPlaySceneName;
        private Transform mapHierarchy;

        private IDataService jsonDataService;

        private void Awake()
        {
            if(Instance != null && Instance != this)
            {
                Debug.LogError("GameInterface Duplicate");
            }
            Instance = this;


            GameEventSystem.RequestTogglePartyAI = TogglePartyAI;

            if(initialized == false)
            {
                initialized = true;

                jsonDataService = new JsonDataService();
                ResourceManager.LoadResources();
                PoolSystem.LoadPoolObjects();
                DatabaseService = new DatabaseService().InitScriptableObjectDatabases();

                Camera camera = Camera.main;
                rpgCamera = new PlayerCamera(camera);


                uiHandler = new UIHandler(camera, transform.Find("PP Volume").GetComponent<Volume>());
                LoadArea(StartPlaySceneName);

                gameControl = new GameControl(camera, uiHandler.selectionBoxVisual);


                ScreenFader = transform.Find("UIMaster/Fader").GetComponent<UIScreenFader>();
                ScreenFader.FadeIn(true, 0.5f);
            }
            Debug.Log("<color=green>// GameInterface Awake</color>");
        }

        public void LoadArea(string targetSceneName)
        {
            StartCoroutine(CR_LoadNewPlayScene(targetSceneName));
        }

        public IEnumerator CR_LoadNewPlayScene(string targetSceneName)
        {
            if(currentPlaySceneName != null)
            {
                Debug.Log("<color=cyan>GameInterface:</color> Unloading previous additive scene '" + currentPlaySceneName + "'");
                yield return SceneManager.UnloadSceneAsync(currentPlaySceneName);
            }
            currentPlaySceneName = targetSceneName;

            if(IsSceneAlreadyLoaded() == false)
            {
                yield return SceneManager.LoadSceneAsync(targetSceneName, LoadSceneMode.Additive);
                Debug.Log("<color=cyan>GameInterface:</color> Done loading additive scene '" + targetSceneName + "'");
            }
            else
            {
                Debug.Log("<color=cyan>GameInterface:</color> Scene '" + targetSceneName + "' is already loaded");
            }

            Scene targetScene = SceneManager.GetSceneByName(targetSceneName);
            //if(currentPlaySceneName < SceneManager.sceneCount) //! Scene is in hierarchy
            SceneManager.SetActiveScene(targetScene);

            if(currentGame == null)
            {
                currentGame = new Game(DatabaseService);
                GameObject[] pcObjects = currentGame.GetPCObjects();

                foreach(GameObject pcObj in pcObjects)
                {
                    SceneManager.MoveGameObjectToScene(pcObj, SceneManager.GetSceneByName("PersistentManagerScene"));
                }

                foreach(GameObject comGO in currentGame.GetPCSummonedCreatureObjects())
                {
                    SceneManager.MoveGameObjectToScene(comGO, SceneManager.GetSceneByName("PersistentManagerScene"));
                }
            }
            mapHierarchy = targetScene.GetRootGameObjects().Where(go => go.CompareTag("MapInfo")).FirstOrDefault().transform;
            currentGame.InitMap(targetSceneName, mapHierarchy);
        }
        private void Update()
        {
            GameLoop();
        }

        private void GameLoop()
        {
            //selectionManager.Update();
            //uiScript.Update();
            //player.UpdateActiveCellBehaviours();
            bool uiPanelActive = false;
            Profiler.BeginSample("uiHandler.Update");
            uiHandler.Update(ref uiPanelActive);
            Profiler.EndSample();

            rpgCamera.UpdateControls(uiPanelActive);

            gameControl.Update();
            Profiler.BeginSample("uiHandler.UpdateFloatingInfo");
            uiHandler.UpdateFloatingInfo();
            Profiler.EndSample();
            currentGame.UpdateScripts();
        }

        private bool IsSceneAlreadyLoaded()
        {
            bool sceneLoaded = false;
            int sceneCount = SceneManager.sceneCount;
            //var scenes = new Scene[sceneCount];
            for(int i = 0; i < sceneCount; i++)
            {
                Scene scene = SceneManager.GetSceneAt(i);
                if(scene.name == currentPlaySceneName && scene.isLoaded)
                {
                    sceneLoaded = true;
                    break;
                }
                //scenes[i] = SceneManager.GetSceneByBuildIndex(sceneCount);
            }

            return sceneLoaded;
        }

        private void TogglePartyAI(bool on)
        {
            foreach(Actor pc in currentGame.PCs)
            {
                if(pc.ActorUI.Selected)
                    pc.aiControlled = on;
            }
        }

        private void SerializeGameJson()
        {
            GameData data = currentGame.CollectSaveData();
            //data.Add(currentGame);
            if(jsonDataService.SaveData("/AOGSavegame.json", data, false))
            {
                //try
                //{
                //    currentGame = jsonDataService.LoadData<Game>("/AOGSavegame.json", false);
                //}
                //catch(System.Exception e)
                //{
                //    Debug.LogError($"Could not read file {e.Message}");
                //    throw;
                //}
            }
            else
            {
                Debug.LogError("Could not save file");
            }
        }

        private void DeserializeGameJson()
        {
            //data.Add(currentGame);
            try
            {
                GameData data = jsonDataService.LoadData<GameData>("/AOGSavegame.json", false);
                currentGame.Load(data);
            }
            catch(System.Exception e)
            {
                Debug.LogError($"Could not read file :: {e.Message}");
                throw;
            }
        }

        private void OnGUI()
        {
            if(GUI.Button(new Rect(100, 10, 130, 30), "Save Game"))
            {
                SerializeGameJson();
            }
            if(GUI.Button(new Rect(100, 45, 130, 30), "Load Game"))
            {
                DeserializeGameJson();
            }
            if(GUI.Button(new Rect(100, 80, 130, 30), "Create Clone"))
            {
                if(SelectionManager.selected.Count > 0)
                    ActorUtility.CreateDuplicate(SelectionManager.selected[0]);
            }
        }

        internal Game GetCurrentGame()
        {
            return currentGame;
        }

        internal UIHandler GetUIScript()
        {
            return uiHandler;
        }
    }
}