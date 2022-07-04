using AoG.Core;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerEditor : EditorWindow
{
    static List<SceneAsset> Scenes = new List<SceneAsset>();
    private GenericMenu scenesSelectionDropDown;
    private string[] sceneNames;
    void OnGUI()
    {
        //EditorUtility.scene.sce
        //// Use the Object Picker to select the start SceneAsset
        //EditorGUI.BeginChangeCheck();
        //GUILayout.Label("StartScene: " + EditorSceneManager.playModeStartScene == null && EditorSceneManager.playModeStartScene.name != null ? "NONE" : EditorSceneManager.playModeStartScene.name);
        //if(EditorGUI.EndChangeCheck())
        //{
        //    EditorUtility.SetDirty(EditorSceneManager.playModeStartScene);
        //}

        //GUILayout.Label("Target Scene:");

        //if(GUILayout.Button(startSceneName == "" ? "NONE" : startSceneName))
        //{

        //    scenesSelectionDropDown.ShowAsContext();
        //}
        // Or set the start Scene from code
        if(GUILayout.Button("Set Persistent Scene Active"))
            SetPlayModeStartScene("Assets/Scenes/PersistentManagerScene.unity");
        if(GUILayout.Button("Load Additive Scene"))
        {
            SceneAsset[] sceneAssets = TryGetUnityObjectsOfTypeFromPath<SceneAsset>("Assets/Scenes", "*.unity").ToArray();
            scenesSelectionDropDown = new GenericMenu();
            //sceneNames = new string[SceneManager.sceneCountInBuildSettings];
            for(int i = 0; i < sceneAssets.Length; i++)
            {
                
                //sceneAsset.pa
                var scene = sceneAssets[i];
                //Debug.Log(scene.path);
                string path = AssetDatabase.GetAssetPath(scene);
                string name = scene.name;
                scenesSelectionDropDown.AddItem(new GUIContent(name), false, () => LoadAdditiveScene(path));
            }
            scenesSelectionDropDown.ShowAsContext();
            //SetPlayModeStartScene("Assets/Scenes/PersistentManagerScene.unity"); 
        }
    }

    void SetPlayModeStartScene(string scenePath)
    {
        SceneAsset myWantedStartScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath);
        if(myWantedStartScene != null)
            EditorSceneManager.playModeStartScene = myWantedStartScene;
        else
            Debug.Log("Could not find Scene " + scenePath);
    }

    void LoadAdditiveScene(string scenePath)
    {
        Scene scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Additive);

        if(scene.name != "PersistentManagerScene")
        {
            if(EditorUtility.DisplayDialog("Loading Additive Scene", "Set As Game Start Scene?", "Yes", "No"))
            {
                EditorSceneManager.SetActiveScene(scene);
                GameInterface gi = GameObject.FindWithTag("PersistentManagers").GetComponent<GameInterface>();
                gi.StartPlaySceneName = scene.name;
                EditorUtility.SetDirty(gi);
            } 
        }
    }



    [MenuItem("AoG Utilities/Scene Manager")]
    static void Open()
    {
        GetWindow<SceneManagerEditor>();
    }

    public static List<T> TryGetUnityObjectsOfTypeFromPath<T>(string path, string searchpattern) where T : UnityEngine.Object
    {
        string[] filePaths = System.IO.Directory.GetFiles(path, searchpattern, System.IO.SearchOption.AllDirectories);
        List<T> assetsFound = new List<T>();

        int countFound = 0;

        //Debug.Log(filePaths.Length);
        //AssetDatabase.getsub
        if(filePaths != null && filePaths.Length > 0)
        {
            for(int i = 0; i < filePaths.Length; i++)
            {
                UnityEngine.Object obj = UnityEditor.AssetDatabase.LoadAssetAtPath(filePaths[i], typeof(T));
                if(obj is T asset)
                {
                    countFound++;
                    if(!assetsFound.Contains(asset))
                    {
                        assetsFound.Add(asset);
                    }
                }
            }
        }

        return assetsFound;
    }
}