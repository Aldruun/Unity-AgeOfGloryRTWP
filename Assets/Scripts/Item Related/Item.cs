using System;
using UnityEngine;

public enum ItemType
{
    None,
    WeapTypeDagger,
    WeapTypeSword,
    WeapTypeMace,
    WeapTypeGreatSword,
    WeapTypeBattleAxe,
    WeapTypeBow,
    WeapTypeCrossbow,
    Ammo
}

public enum ItemRarity
{
   COMMON,
   RARE,
   EPIC,
   LEGENDARY
}


[Serializable]
public abstract class Item
{
    public int maxStackSize = 1;
    public string description;
    public string identifier;
    public ItemCategoryType itemCategoryType;
    public string Name;
    public int value;
    public ItemRarity rarity;

    public float weight;
    public bool equippable;
    //public GameObject model;

    //public ItemComponentData itemData;

    //public void SetItemData(ItemComponentData itemData)
    //{
    //    this.itemData = itemData;
    //}
    
    public virtual void Init()
    {

    }

    public void PrintStats()
    {
        //string debugResult = " of type <color=white>" + ItemType.ToString() + "</color>\n" +
        //    "Name: <color=white>" + Name + "</color>\n" +
        //    "Desc: <color=white>" + description + "</color>\n" +
        //    "Stackable: <color=white>" + (canStack == true) + "</color>\n" +
        //    "Value: <color=white>" + value + "</color>\n" +
        //    "Weight: <color=white>" + weight + "</color>\n" +
        //    "InvImg: <color=white>" + (inventoryImage == null ? "NULL" : inventoryImage.name) + "</color>\n" +
        //    "Model: <color=white>" + (model == null ? "NULL" : model.name) + "</color>";

        Debug.Log("Item Info: Item '<color=green>" + identifier + "</color>'" /* + debugResult*/);
    }
}