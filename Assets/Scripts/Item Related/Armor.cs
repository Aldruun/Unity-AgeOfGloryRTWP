using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BodySlot
{
    Helmet,
    Necklace,
    Cape,
    Dress,
    Gloves,
    RingLeft,
    RingRight,
    Belt,
    Boots
}

public enum ArmorType
{
    Leather = 1, //AC Bonus
    Hide = 2,
    Chain = 3,
    Plate = 4

    // Leather: 11 + Dex
    // Hide: 12 + Dex (max 2)
    // Chain: 16
    // Chain + Shield: 18
    // Mage Armor: 13 + Dex
}

[System.Serializable]
public class Armor : Item
{
    public int AC = 0;
    public BodySlot bodySlot = BodySlot.Dress;
    public EquipType equipType = EquipType.None;
    public ArmorType armorType = ArmorType.Leather;

    public AudioClip[] pickupSounds;
    public AudioClip[] putdownSounds;

}
