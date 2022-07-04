using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

//public class NavMeshHelper : Editor
//{
//    [MenuItem("NavMesh Helper/Bake All Surfaces")]
//    private static void BakeAllSurfaces()
//    {
//        NavMeshSurface[] surfaces = null;

//        if(Selection.activeTransform != null)
//            surfaces = Selection.activeTransform.GetComponentsInChildren<NavMeshSurface>();

//        if(Selection.activeTransform == null || surfaces.Length == 0)
//            surfaces = FindObjectsOfType<NavMeshSurface>();

//        for(int i = 0; i < surfaces.Length; i++) //surfaces[i].navMeshData.
//        {
//            surfaces[i].RemoveData();
//            surfaces[i].BuildNavMesh();
//        }
//    }
//}

public class SetWidthWindow : EditorWindow
{
    private float newWidth;

    private void OnGUI()
    {
        newWidth = EditorGUILayout.FloatField("NavMeshLink Width: ", newWidth);

        //NavMeshLink[] navmeshLinks = null;

        //if(Selection.activeTransform != null)
        //    navmeshLinks = Selection.activeTransform.GetComponentsInChildren<NavMeshLink>();

        GUILayout.FlexibleSpace();

        if(GUILayout.Button("Confirm"))
        {
            //if(navmeshLinks == null || navmeshLinks.Length == 0)
            //{
            //    ShowNotification(new GUIContent("Select one or more NavMeshLinks"));
            //    return;
            //}

            //if(newWidth <= 0)
            //{
            //    ShowNotification(new GUIContent("Width must be greater than 0"));
            //    return;
            //}

            //for(int i = 0; i < navmeshLinks.Length; i++)
            //    navmeshLinks[i].width = newWidth;
        }
    }

    [MenuItem("NavMesh Helper/Set Width Of All NavMeshLinks")]
    public static void Init()
    {
        EditorWindow window = GetWindow<SetWidthWindow>();

        window.position = new Rect(Screen.currentResolution.width / 2 - window.position.width / 2,
            Screen.currentResolution.height / 2 - window.position.height / 2, 400, 500);

        window.minSize = new Vector2(300, 50);
    }
}