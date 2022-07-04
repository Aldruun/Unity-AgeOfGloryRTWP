using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ToDoList : ScriptableObject
{
    public List<TodoList> todoLists;
}

[Serializable]
public class TodoList
{
    public string title = "LIST TITLE";
    public List<TextObject> textList;

    public TodoList()
    {
        title = "NEW LIST";
        textList = new List<TextObject>();
    }

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
    public string s = "empty content item";

    public Color color;

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