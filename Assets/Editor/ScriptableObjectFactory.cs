using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

/// <summary>
/// A helper class for instantiating ScriptableObjects in the editor.
/// </summary>
public static class ScriptableObjectFactory
{

    [MenuItem("Assets/Create ScriptableObject From Selected")]
    public static void CreateScriptableObjectFromSelected()
    {

        UnityEngine.Object selObj = Selection.activeObject;

        // Get all classes derived from ScriptableObject
        CreateScriptableObject(selObj);

    }

    private static void CreateScriptableObject(UnityEngine.Object selObj)
    {
        var scriptableObject = GetAssembly().GetTypes().Where(t => t.IsSubclassOf(typeof(ScriptableObject)) && t.Name == selObj.name).FirstOrDefault();

        if(selObj == null)
            return;

        Debug.Log("Attempting to create ScriptableObject from " + scriptableObject.Name);

        ScriptableObject scrObj = ScriptableObject.CreateInstance(scriptableObject.Name/*obj.GetType()*/);
        ProjectWindowUtil.StartNameEditingIfProjectWindowExists(
                scrObj.GetInstanceID(),
                ScriptableObject.CreateInstance<EndNameEdit>(),
                string.Format("{0}.asset", AssetDatabase.GetAssetPath(selObj.GetInstanceID()).Split('.')),
                AssetPreview.GetMiniThumbnail(scrObj),
                null);
    }

    [MenuItem("Assets/Create ScriptableObject From Selected", true)]
    private static bool ValidateIsScriptableObject() {

        return SelectedIsFolder() == false && IsSubclassOf<ScriptableObject>(Selection.activeObject.name) == true;
    }

    [MenuItem("Assets/Create/ScriptableObject")]
    public static void CreateScriptableObject()
    {
        var assembly = GetAssembly();

        // Get all classes derived from ScriptableObject
        var allScriptableObjects = (from t in assembly.GetTypes()
                                    where t.IsSubclassOf(typeof(ScriptableObject)) /*&& t.IsSubclassOf(typeof(GoapAction))*/
                                    select t).ToArray();

        // Show the selection window.
        ScriptableObjectWindow.Init(allScriptableObjects);
    }
    [MenuItem("Assets/ScriptableObject Factory/Batch Create In Folder")]
    public static void BatchCreateScriptableObjectsInFolder() {

        if(SelectedIsFolder() == false) {

            return;
        }
        string selectedPath = AssetDatabase.GetAssetPath(Selection.activeObject.GetInstanceID());
        Debug.Log("Searching path: " + selectedPath);
        //foreach(var item in /*(ScriptableObject[])*/FindAssetsOfType(selectedPath)) {

            
            //if(item.GetType() == typeof(ScriptableObject)) {

                //Debug.Log("Found asset: " + item.name + " of type " + item.GetType().Name);
            //}
        //}
    }
    /// <summary>
    /// Returns the assembly that contains the script code for this project (currently hard coded)
    /// </summary>
    private static Assembly GetAssembly()
    {
        return Assembly.Load(new AssemblyName("Assembly-CSharp"));
    }

    private static bool IsSubclassOf<T>(string typeName) where T : UnityEngine.Object {

        return GetAssembly().GetTypes().Where(t => t.IsSubclassOf(typeof(T)) && t.Name == typeName).FirstOrDefault() != null;
    }

    private static bool SelectedIsFolder() {

        var path = "";
        var obj = Selection.activeObject;
        if(obj == null) path = "Assets";
        else path = AssetDatabase.GetAssetPath(obj.GetInstanceID());
        if(path.Length > 0) {

            if(Directory.Exists(path)) {
                //Debug.Log("Folder");
                return true;
            }
            //else {
            //    Debug.Log("File");
                
            //}
        }
        //else {
        //    Debug.Log("Not in assets folder");
        //}
        return false;
    }

    public static List<T> FindAssetsOfType<T>() where T : UnityEngine.Object {

        List<T> assets = new List<T>();
        string[] guids = AssetDatabase.FindAssets(string.Format("t:{0}", typeof(T)));
        for(int i = 0; i < guids.Length; i++) {
            string assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
            T asset = AssetDatabase.LoadAssetAtPath<T>(assetPath);
            if(asset != null) {
                assets.Add(asset);
            }
        }
        return assets;
    }
    //static T[] FindAssetsWithExtension<T>(string fileExtension) where T : UnityEngine.Object {
    //    var paths = FindAssetPathsWithExtension(fileExtension);
    //    if(paths == null || paths.Length == 0)
    //        return null;

    //    List<T> assetsOfType = new List<T>();
    //    for(int i = 0; i < paths.Length; i++) {
    //        var asset = AssetDatabase.LoadAssetAtPath(paths[i], typeof(T)) as T;
    //        if(asset == null || (asset is T) == false)
    //            continue;

    //        assetsOfType.Add(asset);
    //    }

    //    return assetsOfType.ToArray();
    //}


    //static string[] FindAssetPathsWithExtension(string fileExtension) {
    //    if(string.IsNullOrEmpty(fileExtension))
    //        return null;

    //    if(fileExtension[0] == '.')
    //        fileExtension = fileExtension.Substring(1);

    //    DirectoryInfo directoryInfo = new DirectoryInfo(Application.dataPath);
    //    FileInfo[] fileInfos = directoryInfo.GetFiles("*." + fileExtension, SearchOption.AllDirectories);

    //    List<string> assetPaths = new List<string>();
    //    foreach(var file in fileInfos) {
    //        var assetPath = file.FullName.Replace(@"\", "/").Replace(Application.dataPath, "Assets");
    //        assetPaths.Add(assetPath);
    //    }

    //    return assetPaths.ToArray();
    //}
}