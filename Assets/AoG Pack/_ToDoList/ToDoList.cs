using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ToDoList : ScriptableObject
{

    public List<TodoList> todoLists;

    //void Awake()
    //{
    //    if(textList.Count == 0)
    //    {
    //        TextObject t = new TextObject();
    //        textList.Add(t);
    //    }
    //}

   
}

[Serializable]
public class TodoList
{
    //[GUIColor("GetColor")]
    public string title = "LIST TITLE";
    public List<TextObject> textList;
    //public Color color;

    public TodoList()
    {
        title = "NEW LIST";
        textList = new List<TextObject>();
    }

    //private Color GetColor()
    //{
    //    return color;
    //}

    public void CreateNewText()
    {
        textList.Insert(0, new TextObject(Color.white));
    }

    public void DeleteText(int id)
    {
        textList.RemoveAt(id);
    }

    public void MoveUp(int id)
    {
        TextObject swap = textList[id];
        textList[id] = textList[id - 1];
        textList[id - 1] = swap;
    }

    public void MoveDown(int id)
    {
        TextObject swap = textList[id];
        textList[id] = textList[id + 1];
        textList[id + 1] = swap;
    }
}

[Serializable]
public class TextObject
{
    [TextArea(1, 10)]
    //[GUIColor("GetColor")]
    public string s = "empty content item";

    //[TableColumnWidth(40, Resizable = false)]
    public Color color;
    //public bool show = true;

    private Color GetColor()
    {
        return color;
    }

    public TextObject()
    {

    }

    public TextObject(Color color)
    {
        this.color = color;
    }
}