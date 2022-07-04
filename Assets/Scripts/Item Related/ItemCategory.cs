using System;
using System.Collections.Generic;

[Serializable]
public class ItemCategory
{
    public ItemCategoryType itemCategoryType;
    public List<Item> items;
    public string Name;

    public ItemCategory(string name, ItemCategoryType itemCategoryType, List<Item> items)
    {
        Name = name;

        this.itemCategoryType = itemCategoryType;
        this.items = items;
    }
}