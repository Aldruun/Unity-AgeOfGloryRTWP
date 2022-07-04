using UnityEditor;
using UnityEngine;

public class Editor_AoGToolbar : Editor
{
    [MenuItem("AoG Utilities/Item Database", false, 0)]
    public static void ItemDatabase()
    {
        ItemDatabaseWindow itemDatabaseWindow = EditorWindow.GetWindow(typeof(ItemDatabaseWindow)) as ItemDatabaseWindow;
        itemDatabaseWindow.minSize = new Vector2(540, 250);
        itemDatabaseWindow.titleContent = new GUIContent("Item Database");
        itemDatabaseWindow.Show();
    }
    [MenuItem("AoG Utilities/ToDoList", false, 3)]
    public static void ToDoList()
    {
        ToDoListEditor toDoWindow = EditorWindow.GetWindow(typeof(ToDoListEditor)) as ToDoListEditor;
        toDoWindow.minSize = new Vector2(200, 100);
        toDoWindow.titleContent = new GUIContent("ToDoList");
        toDoWindow.Show();
    }
    //[MenuItem("AoG Utilities/Testing/Formula Tester", false, 5)]
    //public static void DamageTester()
    //{
    //    FormulaTestingWindow damageTester = EditorWindow.GetWindow(typeof(FormulaTestingWindow)) as FormulaTestingWindow;
    //    damageTester.minSize = new Vector2(540, 250);
    //    damageTester.titleContent = new GUIContent("FormulaTester");
    //    damageTester.Show();
    //}
}