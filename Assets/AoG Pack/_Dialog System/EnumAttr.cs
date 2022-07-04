using System;
using UnityEngine;

public class EnumAttr : PropertyAttribute
{
    public Type enumType;

    public EnumAttr(Type enumType)
    {
        this.enumType = enumType;
    }
}