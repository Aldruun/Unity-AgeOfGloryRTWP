using AoG.SceneManagement;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

[CustomEditor(typeof(AreaTransition))]
public class Inspector_Portal : Editor
{
    private AreaTransition portal;

    private GenericMenu scenesSelectionDropDown;
  
    private void OnEnable()
    {
        //if(voiceSetDatabase == null)
        //    voiceSetDatabase =
        //        Resources.Load("ScriptableObjects/VoiceSetDatabase", typeof(VoiceSetDatabase)) as VoiceSetDatabase;

        //if(voiceSetDatabase == null)
        //    Debug.LogError("VoiceSetDatabase may not be null at this point");

        portal = (AreaTransition)target;
    }

    public override void OnInspectorGUI()
    {
        //EditorGUI.BeginChangeCheck();
        DrawDefaultInspector();

        GUILayout.Label("Target Scene:");

        if(GUILayout.Button(portal.TargetSceneName == "" ? "NONE" : portal.TargetSceneName))
        {
            RefreshDatabaseEntries();
            scenesSelectionDropDown.ShowAsContext();
        }

        if(GUI.changed)
        {
            //Debug.Log("GUI changed"); // Works!
            EditorUtility.SetDirty(target);
            //prop_ItemList.ApplyModifiedProperties();
            if(Application.isPlaying == false)
                EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        }
    }

    private void RefreshDatabaseEntries()
    {
        int numScenes = SceneManager.sceneCountInBuildSettings;

        scenesSelectionDropDown = new GenericMenu();

        for(var i = 0; i < numScenes; i++)
        {
            var name = System.IO.Path.GetFileNameWithoutExtension(UnityEngine.SceneManagement.SceneUtility.GetScenePathByBuildIndex(i));
            scenesSelectionDropDown.AddItem(new GUIContent(name), false,
                () => portal.TargetSceneName = name);
        }
    }
}