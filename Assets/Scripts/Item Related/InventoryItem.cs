using System;
using UnityEngine;

public enum ItemCategoryType
{
    Ammo,
    Armor,
    Weapon,
    Valuable,
}

// This is a data wrapper for the actual item class
[Serializable]
public class InventoryItem
{
    public string ID;
    public Item itemData;
    public int slotIndex;
    public ItemCategoryType itemType;
    public bool equipped;
    public int stackSize;
    public InventoryItemFlags flags;

    public InventoryItem()
    {
    }

    public InventoryItem(Item itemData)
    {
        this.itemData = itemData;
        ID = itemData.identifier;
        itemType = itemData.itemCategoryType;
    }

    public InventoryItem(Item itemData, int stackSize, int slotIndex)
    {
        this.itemData = itemData;
        ID = itemData.identifier;
        itemType = itemData.itemCategoryType;
        this.slotIndex = slotIndex;
        this.stackSize = stackSize;
    }

    public InventoryItem(InventoryItem copyFrom)
    {
        this.itemData = copyFrom.itemData;
        ID = copyFrom.ID;
        itemType = copyFrom.itemType;
        this.slotIndex = copyFrom.slotIndex;
        this.stackSize = copyFrom.stackSize;
        flags = copyFrom.flags;
    }

    public void ClearData()
    {
        ID = "";
        itemData = null;
        equipped = false;
        stackSize = 0;
        flags = 0;
    }
}