using AoG.Core;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AoG.SceneManagement
{
    public class AreaTransition : MonoBehaviour
    {
        enum PortalIdentifier
        {
            NONE,
            Entrance,
            Entrance2,
            Entrance3,
            Exit,
            Exit2,
            Exit3,
            North,
            NorthWest,
            MorthEast,
            East,
            South,
            SouthEast,
            SouthWest,
            West
        }

        [SerializeField] private PortalIdentifier Identifier;
        public string TargetSceneName;
        private Transform playerSpawnpoint;

        private void Awake()
        {
            playerSpawnpoint = transform.Find("player spawnpoint");
        }

        private void OnTriggerEnter(Collider col)
        {
            if(col.CompareTag("Player"))
            {
                if(Identifier == PortalIdentifier.NONE)
                {
                    Debug.LogError("Departure portal with destination scene '" + TargetSceneName + "' with enum identifier 'NONE'. Scene transition canceled");
                    return;
                }
                //col.transform.SetParent(GameObject.FindWithTag("PersistentManagers").transform, true);
                StartCoroutine(CR_SceneLoadAsynch());
            }
        }

        private IEnumerator CR_SceneLoadAsynch()
        {
            GameInterface.ScreenFader.FadeIn(false, 0.5f);
            yield return new WaitForSeconds(0.5f);

            DontDestroyOnLoad(transform.parent.gameObject);

            //yield return SceneManager.LoadSceneAsync(TargetSceneName);
            yield return GameInterface.Instance.CR_LoadNewPlayScene(TargetSceneName);
            Debug.Log("<color=#00c1ff>AreaTransition:</color> Loaded scene '" + TargetSceneName + "'");

            //GameInitializer.newPlaySceneIndex = SceneIndexFromName(TargetSceneName);
            //GameEventSystem.OnAreaTransition?.Invoke(SceneIndexFromName(TargetSceneName));

            AreaTransition arrival = GetLinkedPortal();

            if(arrival.Identifier == PortalIdentifier.NONE)
                Debug.LogError("<color=#00c1ff>AreaTransition:</color> Destination portal with scene '" + arrival.TargetSceneName + "' with enum identifier 'NONE'");

            Debug.Log("<color=#00c1ff>AreaTransition:</color> Teleporting player to transition waypoint");
            UpdatePlayerPosition(arrival);


            GameInterface.ScreenFader.FadeIn(true, 0.5f);
            yield return new WaitForSeconds(0.5f);

            Destroy(transform.parent.gameObject);
            yield return new WaitForEndOfFrame();
        }

        private AreaTransition GetLinkedPortal()
        {
            MapInfo mapInfo = GameObject.FindWithTag("MapInfo").GetComponent<MapInfo>();
            foreach(AreaTransition transition in mapInfo.AreaTransitions)
            {
                if(transition == this)
                    continue;

                if(transition.Identifier == Identifier)
                {
                    Debug.Log("<color=#00c1ff>AreaTransition:</color> Found target portal in scene '" + TargetSceneName + "' with identifier '" + Identifier + "'");
                    return transition;
                }
            }

            Debug.LogError("<color=#00c1ff>AreaTransition:</color> No portal found in scene '" + TargetSceneName + "' with identifier '" + Identifier + "'");
            return null;
        }

        private void UpdatePlayerPosition(AreaTransition portal)
        {

            //GameStateManager.player.transform.SetParent(null);
            GameObject[] pcObjects = GameInterface.Instance.GetCurrentGame().GetPCObjects();

            foreach(GameObject pcObj in pcObjects)
            {
                SceneManager.MoveGameObjectToScene(pcObj, SceneManager.GetSceneByName("PersistentManagerScene"));
                pcObj.transform.position = portal.playerSpawnpoint.position;
                pcObj.transform.rotation = portal.playerSpawnpoint.rotation;
            }

            foreach(GameObject sumGO in GameInterface.Instance.GetCurrentGame().GetPCSummonedCreatureObjects())
            {
                SceneManager.MoveGameObjectToScene(sumGO, SceneManager.GetSceneByName("PersistentManagerScene"));
                sumGO.transform.position = portal.playerSpawnpoint.position;
                sumGO.transform.rotation = portal.playerSpawnpoint.rotation;
            }


            //Scene loadedScene = SceneManager.GetSceneByName(TargetSceneName);
            //SceneManager.MoveGameObjectToScene(playerObj.gameObject, loadedScene);
            
        }
    }
}
