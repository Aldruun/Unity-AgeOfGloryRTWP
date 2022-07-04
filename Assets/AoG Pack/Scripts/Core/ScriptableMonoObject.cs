using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptableMonoObject : MonoBehaviour
{
    protected Scriptable _scriptable;

    internal virtual ScriptableMonoObject SetScriptable(Scriptable scriptable)
    {
        _scriptable = scriptable;
        //_scriptable.SetScriptable(this);
        return this;
    }

    internal virtual Scriptable GetScriptable()
    {
        return _scriptable;
    }
}
