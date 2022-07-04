using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighlightableMonoObject : MonoBehaviour, IHighlight
{
    public int lockDifficulty = 0;
    public bool locked;
    public int trapDetectDifficulty = 0;
    public int trapRemovalDifficulty = 0;
    public int trapped;

    protected Outline _outline;
    private Highlightable highlightable;

    public Transform rootTransform => throw new NotImplementedException();

    private void Awake()
    {
        _outline = GetComponent<Outline>();
    }

    private void OnMouseEnter()
    {
        Highlight();
        //OnCursorEnter?.Invoke(true);
    }

    private void OnMouseExit()
    {
        Unhighlight();
        //OnCursorEnter?.Invoke(false);
    }

    public Transform GetAttackPoint()
    {
        return transform;
    }

    public string GetName()
    {
        return gameObject.name;
    }

    public void Highlight()
    {
        _outline.SetActive(true);
    }

    public void Unhighlight()
    {
        _outline.SetActive(false);
    }

    internal Highlightable GetHighlightable()
    {
        return highlightable;
    }

    //internal static void HighlightAll(bool on)
    //{
    //    outline.SetActive(on);
    //}
}
