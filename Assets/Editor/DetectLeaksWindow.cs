using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class DetectLeaksWindow : EditorWindow
{
    private GUIStyle _missingFoldoutStyle;

    // To have rich text
    private GUIStyle _missingLabelStyle;
    private bool _projectFoldout;
    private bool _projectHasMissingScripts;
    private List<GameObject> _projectList;

    private bool _sceneFoldout;

    // To know the color of the foldout label
    private bool _sceneHasMissingScripts;

    private List<GameObject> _sceneList;
    private Vector2 _scroll;

    // Add menu named "DetectLeaks Window" to the Window menu
    [MenuItem("Tools/DetectLeaks Window")]
    private static void Init()
    {
        // Get existing open window or if none, make a new one:
        var window = (DetectLeaksWindow) GetWindow(typeof(DetectLeaksWindow));
        window.name = "DetectLeaks";
    }

    private void OnGUI()
    {
        if (GUILayout.Button("SCAN", GUILayout.Height(30f)))
            Scan();


        _scroll = GUILayout.BeginScrollView(_scroll);
        {
            _missingLabelStyle = _missingLabelStyle ?? new GUIStyle(GUI.skin.label);
            _missingLabelStyle.richText = true;
            _missingFoldoutStyle = _missingFoldoutStyle ?? new GUIStyle(EditorStyles.foldout);
            _missingFoldoutStyle.richText = true;

            if (_sceneList != null && _sceneList.Count > 0 || _projectList != null && _projectList.Count > 0)
            {
                _sceneFoldout = EditorGUILayout.Foldout(_sceneFoldout,
                    "<color=" + (_sceneHasMissingScripts ? "red" : "green") + ">Scene GameObjects</color>",
                    _missingFoldoutStyle);
                if (_sceneFoldout)
                    DrawGameObjectList(_sceneList);

                _projectFoldout = EditorGUILayout.Foldout(_projectFoldout,
                    "<color=" + (_projectHasMissingScripts ? "red" : "green") + ">Project GameObjects</color>",
                    _missingFoldoutStyle);
                if (_projectFoldout)
                    DrawGameObjectList(_projectList);
            }
            else
            {
                GUILayout.Label("Hit scan");
            }
        }
        GUILayout.EndScrollView();
    }

    /// <summary>
    ///     Scan for every game object inside the scene AND the project folder
    /// </summary>
    private void Scan()
    {
        _sceneList = (FindObjectsOfType(typeof(GameObject)) as GameObject[]).ToList();
        _sceneHasMissingScripts = false;
        foreach (var go in _sceneList)
            if (CheckForMissingScripts(go))
                _sceneHasMissingScripts = true;

        _projectList = new List<GameObject>();
        LoadAllPrefabs(ref _projectList, Application.dataPath);
        _projectHasMissingScripts = false;
        foreach (var go in _projectList)
            if (CheckForMissingScripts(go))
            {
                _projectHasMissingScripts = true;
            }
    }

    /// <summary>
    ///     Load every prefabs recursively from the asset folder
    /// </summary>
    /// <param name="prefabs">List of prefab being updated by the recursive function</param>
    /// <param name="path">Current path</param>
    private void LoadAllPrefabs(ref List<GameObject> prefabs, string path)
    {
        var directories = Directory.GetDirectories(path);
        foreach (var directorie in directories)
            LoadAllPrefabs(ref prefabs, directorie);

        path = path.Replace('\\', '/');
        path = "Assets" + path.Substring(Application.dataPath.Length) + "/";
        var assetsPath = Directory.GetFiles(path, "*.prefab");
        foreach (var assetPath in assetsPath)
        {
            var asset = AssetDatabase.LoadAssetAtPath(assetPath, typeof(GameObject)) as GameObject;
            if (asset != null && PrefabUtility.GetPrefabAssetType(asset) != PrefabAssetType.NotAPrefab)
                prefabs.Add(asset);
        }
    }

    /// <summary>
    ///     Display the list of game objects inside the window vertically. If a component is missing,
    ///     the label [Missing script] is drawn in red in front of it.
    /// </summary>
    /// <param name="list">List of game object to display</param>
    private void DrawGameObjectList(List<GameObject> list)
    {
        if (list != null)
        {
            GameObject destroyGO = null;

            foreach (var obj in list)
            {
                if (obj == null)
                    continue;

                GUILayout.BeginHorizontal();
                {
                    GUILayout.Space(20f);

                    var isMissing = CheckForMissingScripts(obj);
                    if (isMissing)
                        GUILayout.Label("<color=red>[Missing script]</color>", _missingLabelStyle,
                            GUILayout.ExpandWidth(false));

                    // Selectable, read-only field
#pragma warning disable CS0618 // 'EditorGUILayout.ObjectField(Object, Type, params GUILayoutOption[])' is obsolete: 'Check the docs for the usage of the new parameter 'allowSceneObjects'.'
                    EditorGUILayout.ObjectField(obj, obj.GetType(), GUILayout.ExpandWidth(true));
#pragma warning restore CS0618 // 'EditorGUILayout.ObjectField(Object, Type, params GUILayoutOption[])' is obsolete: 'Check the docs for the usage of the new parameter 'allowSceneObjects'.'

                    if (isMissing)
                    {
                        // Delete obj, but only after the loop
                        if (GUILayout.Button(new GUIContent("X", "Delete"), GUILayout.Width(20f)))
                            if (EditorUtility.DisplayDialog("Exterminate !",
                                "Are you sure you want to delete " + obj.name + " ?", "Yes", "No"))
                                destroyGO = obj;

                        // Select obj in hierarchy
                        if (GUILayout.Button(new GUIContent("S", "Select"), GUILayout.Width(20f)))
                            Selection.activeGameObject = obj;

                        if (GUILayout.Button(new GUIContent("C", "Clear missing scripts"), GUILayout.Width(20f)))
                            if (!EditorUtility.DisplayDialog("You wish",
                                "Ha ! you thought it would be that easy didn't ya ? You'll have to remove it yourself pal !",
                                "Ok :'(", "More info on UA"))
                                Application.OpenURL(
                                    "http://answers.unity3d.com/questions/15225/how-do-i-remove-null-components-ie-missingmono-scr.html");
                    }
                }
                GUILayout.EndHorizontal();
            }

            if (destroyGO != null)
                DestroyImmediate(destroyGO);
        }
    }

    /// <summary>
    ///     Check if a component as a missing script
    /// </summary>
    /// <param name="obj">Object to check</param>
    /// <returns>True if there is at least one missing component</returns>
    private bool CheckForMissingScripts(GameObject obj)
    {
        if (obj == null)
            return false;

        var components = obj.GetComponents<Component>();
        return components != null ? components.Any(c => c == null) : false;
    }
}