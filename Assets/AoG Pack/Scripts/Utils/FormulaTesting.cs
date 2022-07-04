using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FormulaTesting : ScriptableObject
{
    public Class profession;
    public int agentLevel = 1;
    public int enemyLevel = 1;
    public float currentExp;
    public int expNeeded = 75;
    public float expMultiplier = 1;


    public string Name;

    public bool isMagicAttack;
    public float rawDamage;

    //public int finalDamage;
    public int pAttack;
    public int pDef;
    public int mAttack;
    public int mDef;
    public float currentHealth = 100;
    public float maxHealth = 100;


    public string Name_opp;

    public bool isMagicAttack_opp;
    public float rawDamage_opp;

    public int pAttack_opp;
    public int pDef_opp;
    public int mAttack_opp;
    public int mDef_opp;
    public float currentHealth_opp = 100;
    public float maxHealth_opp = 100;
}
