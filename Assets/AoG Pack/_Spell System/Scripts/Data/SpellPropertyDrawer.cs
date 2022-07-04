using System;
using UnityEngine;

[Serializable]
public class SpellPropertyDrawer
{
    [Range(0f, 1f)] public float priority;
    public Spell spell;
    public int grade;
}