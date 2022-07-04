using GenericFunctions;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class Editor_AoGUtilities : Editor
{
    [MenuItem("AoG Utilities/Agent Setup")]
    public static void SetupHero()
    {
        Object obj = Selection.activeObject;
        string szPath = AssetDatabase.GetAssetPath(obj);
        ModelImporter modelImporter = AssetImporter.GetAtPath(szPath) as ModelImporter;


        Debug.Log("Processing model " + modelImporter.name);

        GameObject npcRootPrefab = Object.Instantiate(Resources.Load("Prefabs/Characters/ref_actor")) as GameObject;
        GameObject fbx = Object.Instantiate(obj) as GameObject;

        fbx.transform.SetParent(npcRootPrefab.transform);
        fbx.transform.localPosition = Vector3.zero;
        fbx.transform.localRotation = Quaternion.identity;
        //characterRoot.SetParent(npcRootPrefab.transform);
        ////npcRootPrefab.transform.position = characterRoot.position;
        //characterRoot = npcRootPrefab.transform;
        Animator animator = npcRootPrefab.ForceGetComponent<Animator>();
        animator.avatar = fbx.GetComponent<Animator>().avatar;
        Object.DestroyImmediate(fbx.GetComponent<Animator>());
        szPath = Path.GetDirectoryName(szPath) + "\\" + fbx.name + ".prefab";
        //szPath += "\\";
        PrefabUtility.SaveAsPrefabAsset(npcRootPrefab, szPath);
        Object.DestroyImmediate(npcRootPrefab);
    }

    [MenuItem("AoG Utilities/Agent Setup", true)]
    private static bool SingleSelectionValidation()
    {

        return Selection.activeObject != null;
    }

    //[MenuItem("AoG Utilities/NPC Configuration")]
    //public static void NPCConfiguration()
    //{

    //    NPCConfigWindow configWindow = EditorWindow.GetWindow<NPCConfigWindow>();
    //    configWindow.Show();
    //}

    [MenuItem("AoG Utilities/Spawn Actor Template In Scene")]
    public static void CreateActorTemplateInScene()
    {
        GameObject templatePrefab = Instantiate(Resources.Load("Prefabs/Characters/ref_actor")) as GameObject;

        var view = (SceneView)SceneView.sceneViews[0];
        if(view != null)
        {
            RaycastHit hit;

            if(Physics.Raycast(view.camera.transform.position, view.camera.transform.forward, out hit, Mathf.Infinity))
            {
                templatePrefab.transform.position = hit.point;

                Selection.activeGameObject = templatePrefab;
            }
        }
    }

    [MenuItem("AoG Utilities/Create Actor From Scene Object", true)]
    //[MenuItem("AoG Utilities/Setup NPC Mountpoints", true)]
    private static bool CreateActorFromSceneObjectValidation()
    {
        return Selection.activeTransform != null;
    }

    [MenuItem("AoG Utilities/Create Actor From Scene Object")]
    public static void CreateActorFromSceneObject()
    {
        GameObject templatePrefab = Instantiate(Resources.Load("Prefabs/Characters/ref_actor"), Selection.activeTransform.position, Selection.activeTransform.rotation) as GameObject;

        templatePrefab.name = "ref_" + Selection.activeTransform.name.ToLower();
        Selection.activeTransform.SetParent(templatePrefab.transform);
        Animator originalAnimator = Selection.activeTransform.GetComponent<Animator>();
        Avatar originalAvatar = originalAnimator.avatar;
        templatePrefab.GetComponent<Animator>().avatar = originalAvatar;
        DestroyImmediate(originalAnimator);
    }

    //[UnityEditor.Callbacks.DidReloadScripts]
    //private static void OnScriptsReloaded()
    //{
    //    GameTimeManager gtm = FindObjectOfType<GameTimeManager>();

    //    if(gtm != null)
    //        gtm.UpdateTimeComponents();
    //}

    [MenuItem("AoG Utilities/Unity Specific/Fit BoxCollider")]
    private static void FitToChildren()
    {
        foreach(GameObject rootGameObject in Selection.gameObjects)
        {
            if(!(rootGameObject.GetComponent<Collider>() is BoxCollider))
                continue;

            bool hasBounds = false;
            Bounds bounds = new Bounds(Vector3.zero, Vector3.zero);

            for(int i = 0; i < rootGameObject.transform.childCount; ++i)
            {
                Renderer childRenderer = rootGameObject.transform.GetChild(i).GetComponent<Renderer>();
                if(childRenderer != null)
                {
                    if(hasBounds)
                    {
                        bounds.Encapsulate(childRenderer.bounds);
                    }
                    else
                    {
                        bounds = childRenderer.bounds;
                        hasBounds = true;
                    }
                }
            }

            BoxCollider collider = (BoxCollider)rootGameObject.GetComponent<Collider>();
            collider.center = bounds.center - rootGameObject.transform.position;
            collider.size = bounds.size;
        }
    }

    [MenuItem("AoG Utilities/Mesh Info/Get Bounds Height")]
    private static void LogSkinnedMeshBoundsSize()
    {
        GameObject go = Selection.activeGameObject;
        if(go == null)
        {
            return;
        }

        SkinnedMeshRenderer smr = go.GetComponentInChildren<SkinnedMeshRenderer>();
        Bounds bounds = smr.bounds;
        Debug.Log("X: " + bounds.size.x * go.transform.localScale.x +
            " Y: " + bounds.size.y * go.transform.localScale.y +
            " Z: " + bounds.size.z * go.transform.localScale.z);
        //Debug.Log("X: " + bounds.size.x + " Y " + bounds.size.y + " Z: " + bounds.size.z);

    }

    [MenuItem("AoG Utilities/Action Selection Debugger", false, 2)]
    public static void ActionSelectionDebugger()
    {
        EditorWindow window = EditorWindow.GetWindow<Editor_ActorDebugger>();

        window.minSize = new Vector2(325, 200);
        window.Show();
    }

    [MenuItem("AoG Utilities/AudioSource/Apply Top-Down Settings To Selected AudioSources")]
    public static void TopDownRolloff(MenuCommand command)
    {
        foreach(GameObject obj in Selection.gameObjects)
        {
            AudioSource[] aSources = obj.GetComponentsInChildren<AudioSource>();

            foreach(AudioSource audiso in aSources)
            {
                audiso.TopDownRPGRolloff();
            }
        }
    }

    public class ProjectUtilities : MonoBehaviour
    {

        [MenuItem("CONTEXT/AudioSource/Realistic")]
        public static void RalisticRolloff(MenuCommand command)
        {
            Undo.RecordObject(command.context, "AudioSource Realistic Setup");
            ((AudioSource)command.context).RealisticRolloff();
            EditorUtility.SetDirty(command.context);
        }

        [MenuItem("CONTEXT/AudioSource/Top-Down Games")]
        public static void TopDownRolloff(MenuCommand command)
        {
            Undo.RecordObject(command.context, "AudioSource TopDown Setup");
            ((AudioSource)command.context).TopDownRPGRolloff();
            EditorUtility.SetDirty(command.context);
        }
    }

}

public class NPCConfigWindow : EditorWindow
{
    private static List<ActorInput> _agents;
    private int _currAgentIndex;

    private Gender _gender;
    private Faction _faction;
    private bool _useLegacyAI;
    private bool _isBeast;
    private bool _isCloaked;
    private bool _essential;

    private void OnEnable()
    {

        titleContent = new GUIContent("NPC Config");

        _agents = new List<ActorInput>();

        if(Selection.transforms.Length > 0)
        {
            foreach(Transform t in Selection.transforms)
            {

                ActorInput agent = t.GetComponentInChildren<ActorInput>();

                if(agent == null)
                {
                    continue;
                }

                _agents.Add(agent);
            }
        }
    }

    private void OnInspectorUpdate()
    {
        Repaint();
    }

    private void OnGUI()
    {
        if(_agents.Count == 0)
        {
            return;
        }

        ActorInput currentAgent = _agents[_currAgentIndex];

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label(currentAgent.transform.gameObject.name);
     
        if(currentAgent.NavAgent != null)
        {
            currentAgent.NavAgent.radius = EditorGUILayout.FloatField("NavAgent Radius", currentAgent.NavAgent.radius);
            currentAgent.NavAgent.height = EditorGUILayout.FloatField("NavAgent Radius", currentAgent.NavAgent.height);
        }

        EditorGUILayout.EndHorizontal();

        if(GUILayout.Button("Apply", GUILayout.Width(80)))
        {
            if(_currAgentIndex < _agents.Count)
            {
                _currAgentIndex++;
            }
            else
            {
                Close();
            }
        }

        EditorGUILayout.BeginHorizontal(/*new Rect((Screen.width / 2) - 50, (Screen.height / 2), 100, 100)*/);
        GUILayout.FlexibleSpace();
        EditorGUILayout.BeginVertical();
        GUILayout.FlexibleSpace();
        if(GUILayout.Button("Close", GUILayout.Width(80)))
        {

            //selectedActor.currentFaction = faction;

            //foreach(AgentMonoController agent in _agents)
            //{

            //    if(agent == null)
            //    {
            //        continue;
            //    }

            //    agent.agentData.gender = _gender;
            //    agent.agentData.faction = _faction;
            //    agent.useLegacyAI = _useLegacyAI;
            //    agent.isBeast = _isBeast;
            //    agent.isCloaked = _isCloaked;
            //    agent.noDeath = _essential;

            //}

            Close();
        }
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();
    }
}

public static class AudioSourceExtensions
{
    public static void TopDownRPGRolloff(this AudioSource AS)
    {
        AS.maxDistance = 40;
        AS.minDistance = 3;
        var animCurve = new AnimationCurve(
         new Keyframe(AS.minDistance, 1f),
         //new Keyframe(AS.minDistance + (AS.maxDistance - AS.minDistance) / 2f, .35f),
         new Keyframe(AS.maxDistance, 0f));

        AS.rolloffMode = AudioRolloffMode.Custom;
        //animCurve.SmoothTangents(1, .06f);
        //animCurve.SmoothTangents(0, .05f);
        AS.SetCustomCurve(AudioSourceCurveType.CustomRolloff, animCurve);

        AS.spatialBlend = 1f;
        AS.dopplerLevel = 0f;
        AS.spread = 0f;
    }

    public static void RealisticRolloff(this AudioSource AS)
    {
        AS.maxDistance = 100;
        var animCurve = new AnimationCurve(
         new Keyframe(AS.minDistance, 1f),
         new Keyframe(AS.minDistance + (AS.maxDistance - AS.minDistance) / 4f, .35f),
         new Keyframe(AS.maxDistance, 0f));

        AS.rolloffMode = AudioRolloffMode.Custom;
        animCurve.SmoothTangents(1, .025f);
        AS.SetCustomCurve(AudioSourceCurveType.CustomRolloff, animCurve);

        AS.spatialBlend = 1f;
        AS.dopplerLevel = 0f;
        AS.spread = 70f;
    }
}