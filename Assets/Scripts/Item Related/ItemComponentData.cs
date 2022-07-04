using System;

public enum ItemClass
{
    Misc,
    Potion,
    Weapon,
    Armor
}

// Is used for editor and serialization purposes and holds all possible data for all item types
[Serializable]
public class ItemComponentData
{
    public bool canStack;
    //public Sprite inventoryImage;
    //public GameObject model;

    public int damage;
    public string description;
    public string ID;
    public ItemClass itemClass;
    public ItemType itemType;
    public string Name;

    public float reach;
    public float speed;
    public float value;
    public float weight;

    public ItemComponentData(ItemClass itemClass, ItemType itemType, string ID, string name, string description,
        bool canStack, float value, float weight)
    {
        this.itemClass = itemClass;
        this.itemType = itemType;
        this.ID = ID;
        Name = name;
        this.description = description;
        this.canStack = canStack;
        this.value = value;
        this.weight = weight;
    }
}