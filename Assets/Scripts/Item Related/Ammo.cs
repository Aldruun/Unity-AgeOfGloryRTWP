using System;

public enum AmmoType
{
    None,
    Arrow,
    SlingStone,
    Bolt,
    Dart
}

[Serializable]
public class Ammo : Item
{
    public AmmoType ammoType;
    public float damage;
    public string projectileID;
}