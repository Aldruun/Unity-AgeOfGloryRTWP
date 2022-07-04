using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Profiling;

public enum InventoryType
{
    Actor,
    Loot,
    Container
}

[CreateAssetMenu(fileName = "InventoryTemplate", menuName = "ScriptableObjects/InventoryTemplate")]
public class InventoryTemplate : ScriptableObject
{
    [HideInInspector] public List<InventoryItem> items;
    public int maxSlots = 20;

    //! Used by editor
    public void AddItemReference(string identifier, ItemCategoryType itemType, int amount)
    {
        if(items == null)
            items = new List<InventoryItem>();
        InventoryItem i = new InventoryItem();
        //i.item = ItemDatabase.GetItemFromJSON(identifier);
        i.stackSize = amount;
        i.itemType = itemType;
        i.ID = identifier;
        items.Add(i);
    }
}
