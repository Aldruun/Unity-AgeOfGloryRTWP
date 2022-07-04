using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Highlightable : Scriptable
{
    internal int trapDetectionDiff = 0;
    internal int trapRemovalDiff = 0;
    internal int trapped;
    internal int trapDetected;
    internal HighlightableMonoObject highlightObject; 

    public override void InitScriptable(ScriptableType type)
    {
        type = ScriptableType.DOOR;
        base.InitScriptable(type);
    }

    internal bool VisibleTrap(int see_all)
    {
        if(trapped > 0)
            return false;
        if(PossibleToSeeTrap())
            return false;
        //if(scripts[0] == null)
        //    return false;
        if(see_all > 0)
            return true;
        if(trapDetected > 0)
            return true;
        return false;
    }

    private bool PossibleToSeeTrap()
    {
        return true;
    }

    //public static void HighlightAll(bool on)
    //{
    //    outline.SetActive(on);
    //}

    //public void Highlight()
    //{
    //    _outline.SetActive(true);
    //}

    //public void Unhighlight()
    //{
    //    _outline.SetActive(false);
    //}

}
