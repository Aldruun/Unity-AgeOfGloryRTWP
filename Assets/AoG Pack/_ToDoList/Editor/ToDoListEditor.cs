using UnityEngine;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;

public class ToDoListEditor : EditorWindow
{
    private GUIStyle tlStyle = new GUIStyle();

    //[TableList(ShowPaging = true)]
    //[ListDrawerSettings(CustomAddFunction = "InsertValueAtFront")]
    //public List<TextObject> stringTableData;

    private ToDoList _todoList;
    //SerializedObject _serObjTodoList;

    private Vector2 _itemListScrollPosition;
    private bool _showSettings;
    private string _toggleArrow = ">";

    private static string[] tabs;
    private static int selectedTabIndex;

    private Color _tabColor;
    //[HorizontalGroup]
    //[LabelText(GUIColor())]

    private void OnEnable()
    {
        //base.OnEnable();


        tlStyle.normal.background = MakeTex(1, 1, new Color(0.1f, 0.1f, 0.11f, 1f));
        tlStyle.wordWrap = true;

        tlStyle.alignment = TextAnchor.MiddleLeft;
        if(_todoList == null)
            _todoList = Resources.Load("ScriptableObjects/ToDoList", typeof(ToDoList)) as ToDoList;

        //stringTableData = new List<TextObject>();
        if(_todoList == null)
            Debug.LogError("ToDoList may not be null at this point");

        tabs = _todoList.todoLists.Select(t => t.title).ToArray();

        //else
        //{
        //    foreach(TextObject s in _todoList.textList)
        //    {
        //        stringTableData.Add(s);

        //    }
        //}


        //_serObjTodoList = new SerializedObject(_todoList);

    }

    //int InsertValueAtFront()
    //{
    //if(stringTableData.Count > 0)
    //    stringTableData.Insert(stringTableData.Count - 1, new TextObject());
    //return stringTableData.Count;
    //}

    //protected override void OnGUI()
    //{
    //    base.OnGUI();

    //    foreach(TextObject s in stringTableData)
    //    {
    //        GUI.color = s.color;
    //    }
    //    GUI.color = Color.white;
    //}
    protected void OnGUI()
    {
        //base.OnGUI();

        GUI.color = Color.white;
        using(var scope1 = new GUILayout.HorizontalScope("scope1"))
        {
            //EditorGUILayout.BeginHorizontal();
            if(GUILayout.Button("+", GUILayout.Width(20)))
            {
                _todoList.todoLists.Add(new TodoList());
                tabs = _todoList.todoLists.Select(t => t.title).ToArray();
            }
            //EditorGUILayout.EndHorizontal();
            EditorGUI.BeginChangeCheck();
            selectedTabIndex = GUILayout.Toolbar(selectedTabIndex, tabs/*cSkin.GetStyle("Goap Runtime Editor Tab")*/);
            if(EditorGUI.EndChangeCheck())
            {
                //GUI.FocusControl(null);
                EditorGUI.FocusTextInControl(null);
                //Repaint();
                //GUI.UnfocusWindow();
                //GUIUtility.keyboardControl = 0;
            }
        }
        //GUI.contentColor = _tabColor;
        
        //GUI.contentColor = Color.white;

        if(_todoList.todoLists.Count == 0)
        {
            return;
        }

        TodoList todoList = _todoList.todoLists[selectedTabIndex];

        //using(var horScope02 = new GUILayout.HorizontalScope("HorScope02"))
        //{
        using(var scope2 = new GUILayout.HorizontalScope("scope2"))
        {
            
            EditorGUI.BeginChangeCheck();
            todoList.title = GUILayout.TextField(todoList.title);
            if(EditorGUI.EndChangeCheck())
            {
                tabs[selectedTabIndex] = todoList.title;
            }
            //GUILayout.Space(10);
            //todoList.color = EditorGUILayout.ColorField(todoList.color, GUILayout.Width(10));
            //if(EditorGUI.EndChangeCheck())
            //{
            //    _tabColor = todoList.color;
            //}

            //EditorGUILayout.BeginHorizontal();
            if(GUILayout.Button("Add Note", GUILayout.Width(80)))
            {
                GUI.FocusControl(null);
                _todoList.todoLists[selectedTabIndex].CreateNewText();
               
            }
            GUILayout.FlexibleSpace();
            if(GUILayout.Button("Remove List", GUILayout.Width(100)))
            {
                _todoList.todoLists.Remove(todoList);
                tabs = _todoList.todoLists.Select(t => t.title).ToArray();
       
                return;
            }

            //GUILayout.FlexibleSpace();
            //EditorGUILayout.EndHorizontal();
        }

        _itemListScrollPosition = EditorGUILayout.BeginScrollView(_itemListScrollPosition, true, true, GUI.skin.horizontalScrollbar, GUI.skin.verticalScrollbar, tlStyle,
                       GUILayout.Width(position.width), GUILayout.MinHeight(100), GUILayout.MaxHeight(600));

        EditorGUILayout.BeginHorizontal();

        using(var scope3 = new GUILayout.VerticalScope("scope3"))
        {
            //Hack ------------------------------------------------------------- Settings Toggle

            if(GUILayout.Button(_toggleArrow, GUILayout.Width(15), GUILayout.ExpandHeight(true)))
            {
                _showSettings = !_showSettings;
                _toggleArrow = _showSettings ? "<" : ">";
            }
        }
        //GUI.skin.button.fontSize = 12;
        EditorGUILayout.BeginVertical();

        for(int i = 0; i < todoList.textList.Count; i++)
        {
            TextObject t = todoList.textList[i];
            //SerializedObject _serTextObj = new SerializedObject(t);


            if(t.color.a == 0)
            {
                t.color.a = 255;
            }

            //GUI.contentColor = t.color;
            EditorGUILayout.BeginHorizontal();


            if(_showSettings)
            {
                EditorGUILayout.BeginVertical(/*GUILayout.Height(10)*/);
                EditorGUI.BeginDisabledGroup(i < 1);
                if(GUILayout.Button("˄", GUILayout.Width(15), GUILayout.Height(8)))
                {
                    todoList.MoveUp(i);
                }
                EditorGUI.EndDisabledGroup();

                EditorGUI.BeginDisabledGroup(i > todoList.textList.Count - 2);
                if(GUILayout.Button("˅", GUILayout.Width(15), GUILayout.Height(8)))
                {
                    todoList.MoveDown(i);
                }
                EditorGUI.EndDisabledGroup();
                EditorGUILayout.EndVertical();

                GUILayout.Space(8);

                t.color = EditorGUILayout.ColorField(t.color, GUILayout.Width(10));

                GUI.color = (Color.red + Color.white) / 2;
                if(GUILayout.Button("X", new GUIStyle(EditorStyles.miniButtonRight), GUILayout.Width(10)))
                {
                    if(EditorUtility.DisplayDialog("Warning", "Do you really want to delete this element?", "Yes", "No"))
                    {
                        EditorGUI.FocusTextInControl(null);
                        todoList.textList.RemoveAt(i);
                    }
                }
                GUI.color = Color.white;
            }
            GUI.skin.textField.wordWrap = true;
            GUI.skin.textField.fixedHeight = 0;
            GUI.skin.textField.fixedHeight = GUI.skin.textField.CalcHeight(new GUIContent(t.s), position.width - 80);
            GUI.contentColor = t.color;
            //GUI.SetNextControlName("TextArea" + i);
            EditorGUI.BeginChangeCheck();
            t.s = EditorGUILayout.TextArea(t.s, GUI.skin.textField, GUILayout.Height(GUI.skin.textField.fixedHeight), GUILayout.Width(position.width - 70));
            if(EditorGUI.EndChangeCheck())
            {
                if(t.s.StartsWith("fix:", System.StringComparison.OrdinalIgnoreCase))
                {
                    t.color = Color.red;
                }
                else if(t.s.StartsWith("add:", System.StringComparison.OrdinalIgnoreCase))
                {
                    t.color = Color.white;
                }
                else if(t.s.StartsWith("!", System.StringComparison.OrdinalIgnoreCase))
                {
                    t.color = Color.yellow;
                }
                else if(t.s.StartsWith("note:", System.StringComparison.OrdinalIgnoreCase))
                {
                    t.color = Color.green;
                }
            }
            //t.s = EditorGUILayout.TextArea(t.s, GUI.skin.textField, GUILayout.Height(GUI.skin.textField.fixedHeight), GUILayout.Width(position.width - 70));
            GUI.skin.textField.wordWrap = false;
            GUI.skin.textField.fixedHeight = 0;
            GUI.contentColor = Color.white;

            EditorGUILayout.EndHorizontal();

            GUILayout.FlexibleSpace();

            //EditorStyles.helpBox.normal.textColor = Color.white;

        }
            EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndScrollView();

        if(GUI.changed)
        {
            //_serObjTodoList.ApplyModifiedProperties();
            //AssetDatabase.Refresh();
            //AssetDatabase.SaveAssets();
            EditorUtility.SetDirty(_todoList);

        }
        //GUILayout.FlexibleSpace();
    }

    private void SaveScriptableObject(Object obj)
    {
        AssetDatabase.Refresh();
        EditorUtility.SetDirty(obj);
        AssetDatabase.SaveAssets();
    }

    public static bool Foldout(bool foldout, GUIContent content, bool toggleOnLabelClick)
    {
        //Rect position = GUILayoutUtility.GetRect(40f, 40f, 16f, 16f, style);
        // EditorGUI.kNumberW == 40f but is internal
        EditorGUIUtility.labelWidth = 5;
        foldout = EditorGUILayout.Foldout(foldout, content, toggleOnLabelClick);

        return foldout;
    }

    public static bool Foldout(bool foldout, string content, bool toggleOnLabelClick)
    {
        return Foldout(foldout, new GUIContent(content), toggleOnLabelClick);
    }

    private Texture2D MakeTex(int width, int height, Color col)
    {
        Color[] pix = new Color[width * height];

        for(int i = 0; i < pix.Length; i++)
            pix[i] = col;

        Texture2D result = new Texture2D(width, height);
        result.SetPixels(pix);
        result.Apply();

        return result;
    }
}
#endif