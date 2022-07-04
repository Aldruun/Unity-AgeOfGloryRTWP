using System;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public class Editor_VoiceSetDatabase : Editor
{
    private static VoiceSetDatabase voiceSetDatabase;

    private void OnEnable()
    {

        CreateVoiceSetDatabase();
    }

    [MenuItem("AoG Utilities/VoiceSet Database")]
    public static void ShowVoiceSetDatabase()
    {

        CreateVoiceSetDatabase();

        EditorUtility.SetDirty(voiceSetDatabase);
    }


    [MenuItem("AoG Utilities/Create Voiceset Folder Structure")]
    private static void CreateVoicesetFolderStructure()
    {

        var path = "";
        var objects = Selection.objects;

        if(objects == null)
            return;

        foreach(var obj in objects)
        {
            path = AssetDatabase.GetAssetPath(obj.GetInstanceID());
            string[] existingFiles;
            existingFiles = Directory.GetFiles(path).Where(f => f.EndsWith(".wav")).ToArray();
            if(path.Length > 0)
            {
                if(Directory.Exists(path))
                {
                    Debug.Log("Found folder");
                    string[] fieldNames = GetFieldNames();
                    foreach(var fieldName in fieldNames)
                    {

                        string newFolderName = UppercaseFirst(fieldName);

                        if(Directory.Exists(path + "/" + newFolderName))
                        {
                            continue;
                        }
                        //Debug.Log("Adding folder '" + fieldName + "'");
                        AssetDatabase.CreateFolder(path, newFolderName);
                    }

                    foreach(var fieldName in fieldNames)
                    {
                        for(int i = 0; i < existingFiles.Length; i++)
                        {
                            if(existingFiles[i].Contains(fieldName, StringComparison.OrdinalIgnoreCase))
                            {
                                string fileName = Path.GetFileName(existingFiles[i]);
                                FileUtil.MoveFileOrDirectory(path + "/" + fileName, path + "/" + UppercaseFirst(fieldName) + "/" + fileName);
                            }
                        }
                    }


                }
                else
                {
                    //Debug.Log("File");
                }

                AssetDatabase.Refresh();
            }
            else
            {
                Debug.Log("Not in assets folder");
            } 
        }
    }


    private static void CreateVoiceSetDatabase()
    {

        voiceSetDatabase = Resources.Load("ScriptableObjects/VoiceSetDatabase", typeof(VoiceSetDatabase)) as VoiceSetDatabase;

        if(voiceSetDatabase == null)
        {
            Debug.LogError("No VoiceSet Database found");
        }

        string[] fieldNames = GetFieldNames();

        string filePath = System.IO.Path.Combine(Application.dataPath, "Resources/SFX/Actor SFX/VoiceSets");
        CharacterVoiceSet[] characterVoiceSets = ConstructVoiceSet(filePath, "Characters", fieldNames);
        CharacterVoiceSet[] creatureVoiceSets = ConstructVoiceSet(filePath, "Creatures", fieldNames);

        //voiceSetDatabase.femaleVoiceSets.Clear();
        voiceSetDatabase.characterVoiceSets.Clear();
        voiceSetDatabase.creatureVoiceSets.Clear();

        foreach(var vs in characterVoiceSets)
        {
            voiceSetDatabase.AddCharacterVoiceDataSet(new ActorVoiceSetData(vs, vs.Name));
        }

        foreach(var vs in creatureVoiceSets)
        {
            voiceSetDatabase.AddCreatureVoiceDataSet(new ActorVoiceSetData(vs, vs.Name));
        }
       
        Selection.activeObject = voiceSetDatabase;
    }

    private static CharacterVoiceSet[] ConstructVoiceSet(string voicesetMainDir, string actorTypeFolder, string[] fieldNames)
    {
        string actorTypeDir = voicesetMainDir + "/" + actorTypeFolder;
        Debug.Log("*Searching main dir '" + actorTypeDir + "' for voicesets");

        string[] voiceSetDirs = Directory.GetDirectories(actorTypeDir);

        CharacterVoiceSet[] loadedVoiceSets = new CharacterVoiceSet[voiceSetDirs.Length];

        //Foreach VoiceSet folder in this 'ActorType' folder
        for(int v = 0; v < voiceSetDirs.Length; v++)
        {
            string voiceSetFolderName = Path.GetFileName(voiceSetDirs[v]);
            //? Iterate through all "voiceset_X" folders

            //Debug.Log("*Searching folder '" + voiceSetFolderName + "' for voice categories");
            string[] categoryDirs = Directory.GetDirectories(voiceSetDirs[v]);
            loadedVoiceSets[v] = new CharacterVoiceSet(voiceSetFolderName);

            //? Iterate through all "Category" folders (i.e. "FoundLoot")
            for(int c = 0; c < categoryDirs.Length; c++)
            {

                string categoryName = Path.GetFileName(categoryDirs[c]);

                foreach(string fieldName in fieldNames)
                {

                    //Debug.Log("Field name: " + fieldName + " / Category name: " + categoryName);

                    if(string.Equals(fieldName, categoryName, StringComparison.OrdinalIgnoreCase))
                    {

                        //Debug.Log("BINGO! " + fieldName + " equals " + categoryName);

                        AudioClip[] audioClipGroup = (AudioClip[])loadedVoiceSets[v].GetType().GetField(fieldName).GetValue(loadedVoiceSets[v]);

                        audioClipGroup = Resources.LoadAll<AudioClip>("SFX/Actor SFX/VoiceSets/" + actorTypeFolder + "/" + voiceSetFolderName + "/" + categoryName);

                        //foreach(AudioClip ac in audioClipGroup)
                        //{

                        //    Debug.Log("Found clip: " + ac.name);
                        //}

                        FieldInfo fieldInfo = loadedVoiceSets[v].GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

                        fieldInfo.SetValue(loadedVoiceSets[v], Convert.ChangeType(audioClipGroup, fieldInfo.FieldType));
                    }
                }

            }
        }

        return loadedVoiceSets;
    }

    private static string[] GetFieldNames()
    {
        //CharacterVoiceSet refSource = new CharacterVoiceSet();
        FieldInfo[] fieldInfos = typeof(CharacterVoiceSet).GetFields().Where(f => f.FieldType.IsArray).ToArray();
        string[] fieldNames = new string[fieldInfos.Length];
        for(int f = 0; f < fieldNames.Length; f++)
        {

            //Debug.Log("Field name: " + fieldInfos[f].Name);
            fieldNames[f] = fieldInfos[f].Name;
        }

        return fieldNames;
    }

    private static string UppercaseFirst(string s)
    {
        if(string.IsNullOrEmpty(s))
        {
            return string.Empty;
        }
        char[] a = s.ToCharArray();
        a[0] = char.ToUpper(a[0]);
        return new string(a);
    }
}
