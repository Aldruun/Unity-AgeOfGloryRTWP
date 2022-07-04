using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StatType {

    HealthCurrent = 0,
    HealthMax = 1,
    ManaCurrent = 2,
    ManaMax = 3,
    PhysAttack = 4,
    MagAttack = 5,
    PhysDefense = 6,
    MagDefense = 7
}
[System.Serializable]
public class Stat {
    private string _name;
    public StatType statType;

    public string Name {

        get { return _name; }
        set { _name = value; }
    }

    private int _baseValue;
    public virtual int BaseValue {

        get { return _baseValue; }
        set { _baseValue = value; }
    }
    public virtual int Value {

        get { return _baseValue; }
    }

    public Stat() {

        this.Name = string.Empty;
        this.BaseValue = 0;
    }

    public Stat(string name, StatType statType, int baseValue) {

        _name = name;
        this.statType = statType;
        BaseValue = baseValue;
    }

    public StatType GetStatType() {

        return statType;
    }
}
