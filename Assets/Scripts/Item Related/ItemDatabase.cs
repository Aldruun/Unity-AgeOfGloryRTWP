using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class ItemDatabase : ScriptableObject
{
    private static readonly List<GameObject>
        physicalItems = new List<GameObject>(); // Loading VFX obj for spells instead

    public static List<Item> itemTemplates;

    //public Dictionary<ItemCategoryType, List<Item>> itemCategoryMap;
    private static Sprite[] inventoryItemSprites;

    //public List<ItemData> availableItems;
    //public List<Ammo> ammo;
    public List<ItemCategory> itemCategories;

    //public List<Weapon> weapons;
    //public List<Valuable> valuables;

    public string[] GetAllDatabaseItemIDs()
    {
        List<string> names = new List<string>();

        foreach (ItemCategory category in itemCategories)
        {
            //if(category == null)
            //{
            //    Debug.LogError("Category null");
            //}
            if (category.items == null)
                //Debug.LogError("category.items null");
                return null;
            foreach (Item item in category.items)
                names.Add(item.identifier);
        }

        //names.AddRange(itemTemplates.Select(i => i.ID).ToList());
        //names.AddRange(ammo.Select(i => i.ID).ToList());
        return names.ToArray();
    }

    public static void WriteItemsToJSONFile(List<Item> items)
    {
        //Movie movie = new Movie
        //{
        //    Name = "Bad Boys",
        //    Year = 1995
        //};

        // serialize JSON to a string and then write string to a file

        JsonSerializerSettings settings = new JsonSerializerSettings();
        settings.TypeNameHandling = TypeNameHandling.Objects;

        string path = AssetDatabase.GetAssetPath(Resources.Load<TextAsset>("ItemDatabase"));
        Debug.Log(path);
        File.WriteAllText(path, JsonConvert.SerializeObject(items, Formatting.Indented, settings));
        AssetDatabase.ImportAsset(path);
        // serialize JSON directly to a file
        //using(StreamWriter file = File.CreateText(path))
        //{
        //    JsonSerializer serializer = new JsonSerializer();
        //    serializer.Serialize(file, items);
        //}
        //JsonConvert.SerializeObject<Item>(,);
    }

    public static void CreateItemTemplatesFromJSON(string identifier)
    {
        JsonSerializerSettings settings = new JsonSerializerSettings();
        settings.TypeNameHandling = TypeNameHandling.Objects;
        string path = AssetDatabase.GetAssetPath(Resources.Load<TextAsset>("ItemDatabase"));

        itemTemplates =
            JsonConvert.DeserializeObject<List<Item>>(Resources.Load<TextAsset>("ItemDatabase").ToString(), settings);
        Item item = null;
        foreach (Item i in itemTemplates)
            if (identifier == i.identifier)
            {
                item = i;
                item.Init();
                item.equippable = item is Armor || item is Weapon || item is Ammo;
                break;
            }
    }

    public static Item GetItemTemplate(string identifier)
    {
        foreach (Item i in itemTemplates)
            if (identifier == i.identifier)
            {
                return i;
            }

        return null;
    }

    public static List<Item> GetItemsFromJSON()
    {
        JsonSerializerSettings settings = new JsonSerializerSettings();
        settings.TypeNameHandling = TypeNameHandling.Objects;
        //string path = AssetDatabase.GetAssetPath(Resources.Load<TextAsset>("ItemDatabase"));

        return JsonConvert.DeserializeObject<List<Item>>(Resources.Load<TextAsset>("ItemDatabase").ToString(),
            settings);
    }

    public static string GetItemName(string identifier)
    {
        JsonSerializerSettings settings = new JsonSerializerSettings();
        settings.TypeNameHandling = TypeNameHandling.Objects;
        string path = AssetDatabase.GetAssetPath(Resources.Load<TextAsset>("ItemDatabase"));

        dynamic dynJson =
            JsonConvert.DeserializeObject<List<Item>>(Resources.Load<TextAsset>("ItemDatabase").ToString(), settings);
        Item item = null;
        foreach (dynamic i in dynJson)
            if (identifier == i.identifier)
            {
                item = i;
                return item.Name;
            }

        return null;
    }

    public static Item[] GetAllItemsOfType(ItemCategoryType itemType)
    {
        return itemTemplates.Where(d => d.itemCategoryType == itemType).ToArray();
        //List<InventoryItem> items = new List<InventoryItem>();

        //for(int i = 0; i < itemsOfType.Length; i++) {
        //    items.Add(new InventoryItem(itemsOfType[i], 1));
        //}
        //return items;
    }

    private static Sprite GetSpriteIconByTag(string tag)
    {
        for (int i = 0; i < inventoryItemSprites.Length; i++)
            if (inventoryItemSprites[i].name == tag)
                return inventoryItemSprites[i];

        //Debug.Log("<color=grey>Folder '<color=white>Resources/Sprites/Item Icons</color>' doesn't contain a Sprite called '<color=yellow>" + tag + "</color></color>'");

        return null;
    }

    private static void ConstructItemDatabase()
    {
        itemTemplates = new List<Item>();
        JsonSerializerSettings settings = new JsonSerializerSettings();
        settings.TypeNameHandling = TypeNameHandling.Auto;
        itemTemplates.AddRange(
            JsonConvert.DeserializeObject<List<Item>>(Resources.Load<TextAsset>("ItemDatabase").ToString(), settings));

        //foreach(Item itemData in itemTemplates)
        //{
        //    Debug.Log("Itemdata: " + itemData.ID);
        //}
    }

    public static void LoadResources()
    {
        inventoryItemSprites = Resources.LoadAll<Sprite>("Sprites/Item Icons");
        ConstructItemDatabase();

        physicalItems.AddRange(Resources.LoadAll<GameObject>("Prefabs/Items"));
        //.Where(x => itemTemplates.Find(i => i.identifier == x.name) != null));

        Debug.Log("<color=green>Item templates created</color>");
        //foreach(GameObject obj in physicalItems)
        //{
        //    Debug.Log(obj.name);
        //}
    }

    /// <summary>
    /// Instantiates a prefab found in Resources/Prefabs/Items by identifier.
    /// </summary>
    /// <param name="identifier"></param>
    /// <param name="parent"></param>
    /// <returns></returns>
    public static GameObject InstantiatePhysicalItem(string identifier, Transform parent = null)
    {
        GameObject resourceObject = physicalItems.Where(x => x.name == identifier).FirstOrDefault();
        //Item item = GetItemFromJSON(ID);
        if (resourceObject == null)
            Debug.LogError("Item template with name '" + identifier + "' not found");

        GameObject newObject = Instantiate(physicalItems.Where(x => x.name == identifier).FirstOrDefault());

        if (newObject != null)
        {
            if (parent != null)
            {
                newObject.transform.SetParent(parent);
                newObject.transform.localPosition = Vector3.zero;
                newObject.transform.localRotation = Quaternion.identity;
            }
        }

        return newObject;
    }


    public static GameObject DropPhysicalObjectAsPickupItem(string ID, int stackSize, Vector3 position, Quaternion rotation)
    {
        GameObject resourceObject = physicalItems.Where(x => x.name == ID).FirstOrDefault();

        if (resourceObject == null)
            Debug.LogError("Item template with name '" + ID + "' not found");

        GameObject newObject = Instantiate(physicalItems.Where(x => x.name == ID).FirstOrDefault());

        if (newObject != null)
        {
            newObject.transform.position = position;
            newObject.transform.rotation = rotation;

            //ContainerMonoObject pickupItem = newObject.AddComponent<ContainerMonoObject>();
            //pickupItem.gameObject.layer = LayerMask.NameToLayer("Collectables");
            //pickupItem.discarded = true;
            //pickupItem.identifier = ID;
            //pickupItem.stackSize = stackSize;

            //Rigidbody rb = pickupItem.gameObject.AddComponent<Rigidbody>();
            //rb.mass = 10;
            //Collider c = pickupItem.gameObject.GetComponent<Collider>();
            //if(c == null)
            //    c = pickupItem.gameObject.AddComponent<BoxCollider>();
            ////bc.isTrigger = true;
            //rb.AddForce(Quaternion.AngleAxis(-45, pickupItem.transform.right) * (pickupItem.transform.forward * 30), ForceMode.Impulse);
        }

        return newObject;
    }

    //public static PickupItem SpawnPickupItem(string tag, Transform parent, Vector3 position, Quaternion rotation) {
    //    GameObject pickupRoot = Object.Instantiate(Resources.Load<GameObject>("Prefabs/Pickup Root"), position, rotation);
    //    PickupItem pickup = new PickupItem(position, tag, 0);
    //    //GameObject newObject = ResourceManager.GetPoolObject(tag, ObjectPoolingCategory.ITEMS);

    //    //newObject.transform.SetParent(pickupRoot.transform);
    //    ItemData itemData = CreateItemData(tag);
    //    pickup.itemData = itemData;
    //    //Debug.Log("Creating Pickup Item of type '" + itemData.itemType.ToString() + "'");
    //    //pickup.pickupItemType = itemData.itemType;

    //    Do.WrapInBoxCollider(pickupRoot);

    //    if(parent != null)
    //        pickupRoot.transform.SetParent(parent);

    //    return pickup;
    //}

    //public static Item GetItemDataByInstanceID(GameObject objectToCheck) {
    //    for(int i = 0; i < physicalItems.Count; i++) {
    //        if(physicalItems[i].GetInstanceID() == objectToCheck.GetInstanceID()) {
    //            return CreateItemData(physicalItems[i].name);
    //        }
    //    }

    //    return null;
    //}

    //public static Item[] GetAllItemsOfType(ItemType itemType) {
    //    return itemTemplates.Where(d => d.itemType == itemType).ToArray();
    //    //List<InventoryItem> items = new List<InventoryItem>();

    //    //for(int i = 0; i < itemsOfType.Length; i++) {
    //    //    items.Add(new InventoryItem(itemsOfType[i], 1));
    //    //}
    //    //return items;
    //}

    //public static InventoryItem CreateRandomInventoryItem(bool randomQuantity, out Item rndItemData) {
    //    rndItemData = itemTemplates[(Random.Range(0, itemTemplates.Count))];
    //    return new InventoryItem(rndItemData, Random.Range(1, rndItemData.maxQuantity + 1));
    //}
}