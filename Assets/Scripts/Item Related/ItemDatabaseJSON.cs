using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

public static class ItemDatabaseJSON
{
    private static readonly List<GameObject> physicalItems = new List<GameObject>();
    private static Sprite[] inventoryItemSprites;
    public static List<Item> dataItems { get; private set; }

    public static Item GetItemFromJSONByID(string identifier)
    {
        if (dataItems == null)
        {
            var settings = new JsonSerializerSettings();
            settings.MetadataPropertyHandling = MetadataPropertyHandling.ReadAhead;
            settings.TypeNameHandling = TypeNameHandling.All;
            var items = JsonConvert.DeserializeObject<List<Item>>(Resources.Load<TextAsset>("ItemDatabase").ToString(),
                settings);

            for (var i = 0; i < items.Count; i++)
                if (items[i].identifier == identifier)
                    return items[i];

            Debug.LogError("There is no item called '" + identifier + "' inside the JSON item database.");
            return null;
        }

        for (var i = 0; i < dataItems.Count; i++)
            if (dataItems[i].identifier == identifier)
                return dataItems[i];
        Debug.LogError("There is no item called '" + identifier + "' inside the item database.");
        return null;
    }

    internal static Item CreateItemData(string identifier)
    {
        foreach (var dataItem in dataItems)
            if (dataItem.identifier == identifier)
            {
                //Debug.Log("Found item template with tag '<color=green>" + itemTag + "</color>' in the item database.");

                //bool statsNull = (dataItem.stats != null);
                //string debugResult = statsNull ?
                //            "\nStats: " + System.String.Join(", ",
                //            new List<BaseStat>(dataItem.stats)
                //            .ConvertAll(i => i.statIdentifier + " [" + i.baseValue + "]")
                //            .ToArray()) : "";

                //Debug.Log("Adding item '<color=white>" + itemTag + "</color>'" + debugResult);
                Item dItem = null;
                if (dataItem is Weapon)
                {
                    dItem = new Weapon();
                    dItem.identifier = dataItem.identifier;
                    dItem.Name = dataItem.Name;
                    dItem.itemCategoryType = dataItem.itemCategoryType;
                    dItem.description = dataItem.description;
                    dItem.maxStackSize = dataItem.maxStackSize;
                    dItem.value = dataItem.value;
                    dItem.weight = dataItem.weight;
                    return dItem;
                }

                if (dataItem is Ammo)
                {
                    dItem = new Ammo();
                    dItem.identifier = dataItem.identifier;
                    dItem.Name = dataItem.Name;
                    dItem.itemCategoryType = dataItem.itemCategoryType;
                    dItem.description = dataItem.description;
                    dItem.maxStackSize = dataItem.maxStackSize;
                    dItem.value = dataItem.value;
                    dItem.weight = dataItem.weight;
                }

                return dItem;
                //Item dItem = new Item(dataItem.itemClass, dataItem.itemType, dataItem.ID, dataItem.Name, dataItem.description,/*dataItem.stats,*/ dataItem.canStack, dataItem.value, dataItem.weight /*GetSpriteIconByTag(identifier)*/);
                //dItem.itemType = dataItem.itemType;
            }

        //Debug.Log("<color=magenta>Item with tag '<color=yellow>" + itemTag + "</color>' couldn't be found in the item database</color>");
        return null;
    }

    public static ItemComponentData GetItemDataFromJSON(string itemTag)
    {
        var items_JSON =
            JsonConvert.DeserializeObject<List<ItemComponentData>>(Resources.Load<TextAsset>("ItemDatabase")
                .ToString());

        foreach (var dataItem in items_JSON)
            if (dataItem.ID == itemTag)
            {
                //Debug.Log("Found item template with tag '<color=green>" + itemTag + "</color>' in the item database.");

                //bool statsNull = (dataItem.stats != null);
                //string debugResult = statsNull ?
                //            "\n" + System.String.Join(", ",
                //            new List<BaseStat>(dataItem.stats)
                //            .ConvertAll(i => i.statIdentifier + " [" + i.baseValue + "]")
                //            .ToArray()) : "";

                //Debug.Log("Assigning ItemComponentData '<color=white>" + itemTag + "</color>'" + debugResult);

                var dItem = new ItemComponentData(dataItem.itemClass, dataItem.itemType, dataItem.ID, dataItem.Name,
                    dataItem.description, /*dataItem.stats,*/ dataItem.canStack, dataItem.value, dataItem.weight);
                return dItem;
            }

        Debug.LogError("Item with ID '<color=yellow>" + itemTag + "</color>' couldn't be found in the item database.");
        return null;
    }

    public static string[] GetAllDatabaseItemNames()
    {
        var items = JsonConvert.DeserializeObject<List<ItemComponentData>>(Resources.Load<TextAsset>("ItemDatabase")
            .ToString());

        return items.Select(i => i.ID).ToArray();
    }

    private static Sprite GetSpriteIconByTag(string tag)
    {
        for (var i = 0; i < inventoryItemSprites.Length; i++)
            if (inventoryItemSprites[i].name == tag)
                return inventoryItemSprites[i];

        //Debug.Log("<color=grey>Folder '<color=white>Resources/Sprites/Item Icons</color>' doesn't contain a Sprite called '<color=yellow>" + tag + "</color></color>'");

        return null;
    }

    private static void ConstructItemDatabase()
    {
        dataItems = new List<Item>(
            JsonConvert.DeserializeObject<List<Item>>(Resources.Load<TextAsset>("ItemDatabase").ToString()));
    }

    public static void LoadResources()
    {
        //inventoryItemSprites = Resources.LoadAll<Sprite>("Sprites/Item Icons");
        ConstructItemDatabase();

        physicalItems.AddRange(Resources.LoadAll<GameObject>("Prefabs/Items")
            .Where(x => dataItems.Find(i => i.identifier == x.name) != null));

        //Debug.Log("<color=green>Data items created</color>");
        //foreach(GameObject obj in physicalItems) {
        //    Debug.Log(obj.name);
        //}
    }

    //public static GameObject CreatePhysicalItem(string tag, Transform parent = null) {

    //    GameObject newObject = Object.Instantiate(physicalItems.Where(x => x.name == tag).FirstOrDefault());

    //    if(newObject != null && parent != null) {

    //        newObject.transform.SetParent(parent);
    //        newObject.transform.localPosition = Vector3.zero;
    //        newObject.transform.localRotation = Quaternion.identity; 
    //    }

    //    return newObject;
    //}
    //public static PickupItem SpawnPickupItem(string tag, Transform parent, Vector3 position, Quaternion rotation) {

    //    GameObject pickupRoot = Object.Instantiate(Resources.Load<GameObject>("Prefabs/Pickup Root"), position, rotation);
    //    PickupItem pickup = new PickupItem(position, tag, 0);
    //    //GameObject newObject = ResourceManager.GetPoolObject(tag, ObjectPoolingCategory.ITEMS);

    //    //newObject.transform.SetParent(pickupRoot.transform);
    //    ItemComponentData itemData = CreateItemData(tag);
    //    pickup.itemData = itemData;
    //    //Debug.Log("Creating Pickup Item of type '" + itemData.itemType.ToString() + "'");
    //    //pickup.pickupItemType = itemData.itemType;

    //    Do.WrapInBoxCollider(pickupRoot);

    //    if(parent != null)
    //        pickupRoot.transform.SetParent(parent);

    //    return pickup;
    //}

    public static Item GetItemDataByInstanceID(GameObject objectToCheck)
    {
        for (var i = 0; i < physicalItems.Count; i++)
            if (physicalItems[i].GetInstanceID() == objectToCheck.GetInstanceID())
                return CreateItemData(physicalItems[i].name);

        return null;
    }


    //public static InventoryItem CreateRandomInventoryItem(bool randomQuantity, out ItemComponentData rndItemData) {

    //    rndItemData = dataItems[(Random.Range(0, dataItems.Count))];
    //    return new InventoryItem(rndItemData, Random.Range(1, rndItemData.maxQuantity + 1));
    //}
}

public static class JsonHelper
{
    //Usage:
    //YouObject[] objects = JsonHelper.getJsonArray<YouObject> (jsonString);
    public static T[] GetJsonArray<T>(string json)
    {
        var newJson = "{ \"array\": " + json + "}";
        var wrapper = JsonUtility.FromJson<Wrapper<T>>(newJson);
        return wrapper.array;
    }

    //Usage:
    //string jsonString = JsonHelper.arrayToJson<YouObject>(objects);
    public static string ArrayToJson<T>(T[] array)
    {
        var wrapper = new Wrapper<T>();
        wrapper.array = array;
        return JsonUtility.ToJson(wrapper);
    }

    [Serializable]
    private class Wrapper<T>
    {
        public T[] array;
    }
}