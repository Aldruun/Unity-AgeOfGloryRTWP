using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

[CustomEditor(typeof(ActorConfiguration)), CanEditMultipleObjects]
public class Inspector_NPCConfiguration : Editor
{
    private ActorConfiguration actorConfig;

    private GenericMenu voicesetSelectionDropDown;
    //

    //private List<string> namesOfAvailableVoicesets;

    private VoiceSetDatabase voiceSetDatabase;

    private void OnEnable()
    {
        if(voiceSetDatabase == null)
            voiceSetDatabase =
                Resources.Load("ScriptableObjects/VoiceSetDatabase", typeof(VoiceSetDatabase)) as VoiceSetDatabase;

        if(voiceSetDatabase == null)
            Debug.LogError("VoiceSetDatabase may not be null at this point");

        actorConfig = (ActorConfiguration)target;
    }

    public override void OnInspectorGUI()
    {
        //EditorGUI.BeginChangeCheck();
        DrawDefaultInspector();
       
        GUILayout.Label("Voiceset ID:");

        if(GUILayout.Button(actorConfig.VoicesetID == "" ? "NONE" : actorConfig.VoicesetID))
        {
            RefreshDatabaseEntries();
            voicesetSelectionDropDown.ShowAsContext();
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
        
        var items = voiceSetDatabase.characterVoiceSets.Select(c => c.voiceSet).ToList();
        items.AddRange(voiceSetDatabase.creatureVoiceSets.Select(c => c.voiceSet));
        voicesetSelectionDropDown = new GenericMenu();

        for(var i = 0; i < items.Count; i++)
        {
            var item = items[i];
            var name = item.Name;
            voicesetSelectionDropDown.AddItem(new GUIContent(name), false,
                () => actorConfig.VoicesetID = name);
        }
    }
}