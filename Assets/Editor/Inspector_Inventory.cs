using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

[CustomEditor(typeof(InventoryTemplate))]
public class Inspector_Inventory : Editor
{
    private bool addMaxQuant;

    //GUISkin cSkin;

    // For testing
    private string identifier;

    private InventoryTemplate inventory;
    private ItemDatabase itemDatabase;

    private GenericMenu itemSelectionDropDown;
    //

    private string[] namesOfAvailableItems;
    private int quantity;

    private int taken;
    //SerializedObject prop_ItemList;

    private void OnEnable()
    {
        if(itemDatabase == null)
            itemDatabase =
                Resources.Load("ScriptableObjects/ItemDatabase", typeof(ItemDatabase)) as ItemDatabase;

        if(itemDatabase == null)
            Debug.LogError("Item database may not be null at this point");
        inventory = (InventoryTemplate)target;

        if(inventory.items == null)
        {
            Debug.LogError("inventory.items = null");
            inventory.items = new List<InventoryItem>();
        }

        //prop_ItemList = serializedObject.FindProperty("");
        //cSkin = (GUISkin)(AssetDatabase.LoadAssetAtPath("Assets/GUI Skins/CI_Skin.guiskin", typeof(GUISkin)));
        //RefreshDatabaseEntries();
        namesOfAvailableItems = itemDatabase.GetAllDatabaseItemIDs();
    }

    public override void OnInspectorGUI()
    {
        //EditorGUI.BeginChangeCheck();

        GUILayout.BeginVertical();
        GUILayout.BeginHorizontal();
        GUILayout.Space(3);

        GUI.skin.button.fontSize = 20;
        if(GUILayout.Button(new GUIContent("↻", "Refresh Item Database Entries"), GUILayout.Height(22),
            GUILayout.Width(24)))
            RefreshDatabaseEntries();
        GUI.skin.button.fontSize = 12;

        GUILayout.FlexibleSpace();

        GUILayout.Label("Items Count: " + inventory.items.Count);

        GUILayout.EndHorizontal();

        if(Application.isPlaying == false)
            inventory.maxSlots =
                EditorGUILayout.IntField("Num Slots", Mathf.Clamp(inventory.maxSlots, inventory.items.Count, 100));

        //inventory.generateRandomContent = EditorGUILayout.Toggle("Generate Random Items", inventory.generateRandomContent);

        //if(inventory.generateRandomContent) {
        //    inventory.lootTable = (LootTable)EditorGUILayout.ObjectField("Loot Table", inventory.lootTable, typeof(LootTable), false);
        //    int freeSlots = inventory.overrideExistingItems ? inventory.maxSlots : inventory.maxSlots - inventory.items.Count;

        //    if(freeSlots == 0) {
        //        GUI.contentColor = Color.red;
        //        GUILayout.Label("Warning! No space for random items.");
        //        GUI.contentColor = Color.white;
        //    }
        //    else {
        //        inventory.numRandomItems = EditorGUILayout.IntField("Num Slots To Fill", Mathf.Clamp(inventory.numRandomItems, 1, 1 + freeSlots - 1));
        //    }

        //    inventory.overrideExistingItems = EditorGUILayout.Toggle("Override Existing Items", inventory.overrideExistingItems);
        //}

        if(Application.isPlaying == false)
        {
            for(var i = 0; i < inventory.items.Count; i++)
            {
                var itemRef = inventory.items[i];

                if(inventory.items == null)
                    Debug.LogError("ai_Inventory.items = null");

                GUILayout.BeginHorizontal();

                if(GUILayout.Button("-", GUILayout.Width(20)))
                    inventory.items.Remove(itemRef);
                //if(GUILayout.Button("s", GUILayout.Width(20))) {
                //    itemRef.item.PrintStats();
                //}

                GUILayout.Label(itemRef.ID);

                EditorGUIUtility.fieldWidth = 25;
                GUILayout.FlexibleSpace();
                EditorGUIUtility.labelWidth = 30;

                itemRef.stackSize = EditorGUILayout.IntField("Qty: ", Mathf.Clamp(itemRef.stackSize, 1, 999));
                GUILayout.EndHorizontal();
            }
        }
        //else
        //{
        //    if(inventory.inventoryItems == null)
        //    {
        //        Debug.LogError("ai_Inventory.items = null");
        //    }
        //    else
        //        foreach(var kvp in inventory.inventoryItems)
        //        {
        //            var itemRef = kvp;

        //            GUILayout.BeginHorizontal();
        //            GUILayout.Label(kvp.ToString(), GUILayout.Width(20));

        //            if(itemRef.equipped)
        //            {
        //                GUI.color = Colors.Salmon;
        //                GUILayout.Label(">", GUILayout.Width(10));
        //                GUI.color = Color.white;
        //            }

        //            //if(GUILayout.Button("s", GUILayout.Width(20))) {
        //            //    itemRef.item.PrintStats();
        //            //}

        //            GUILayout.Label(itemRef.itemData != null ? itemRef.itemData.identifier : " - ");

        //            EditorGUIUtility.fieldWidth = 25;
        //            GUILayout.FlexibleSpace();
        //            EditorGUIUtility.labelWidth = 30;

        //            itemRef.stackSize = EditorGUILayout.IntField("Qty: ", itemRef.itemData != null ? itemRef.stackSize : 0);

        //            GUILayout.EndHorizontal();
        //        }
        //}

        GUILayout.EndVertical();

        GUILayout.BeginHorizontal();

        //identifier = EditorGUILayout.TextField("Identifier: ", identifier);

        GUILayout.EndHorizontal();

        if(GUILayout.Button("+"))
        {
            addMaxQuant = false;
            RefreshDatabaseEntries();
            itemSelectionDropDown.ShowAsContext();
        }

        if(GUILayout.Button("+ (Max Quantity)"))
        {
            addMaxQuant = true;
            RefreshDatabaseEntries();
            itemSelectionDropDown.ShowAsContext();
        }

        if(GUI.changed)
        {
            //Debug.Log("GUI changed"); // Works!
            EditorUtility.SetDirty(target);
            //prop_ItemList.ApplyModifiedProperties();
            if(Application.isPlaying == false)
                EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        }

        //Test_TakeQuantity();
    }

    private void AddItemFromDatabase(string identifier, ItemCategoryType itemType)
    {
        //Item item = ItemDatabase.GetItemByID(identifier);

        //inventory.items.Add(new InventoryItem(item, addMaxQuant ? (item.canStack ? 999 : 1) : 1));
        inventory.AddItemReference(identifier, itemType, 1);
        EditorUtility.SetDirty(target);
    }

    private void RefreshDatabaseEntries()
    {
        var items = itemDatabase.itemCategories.SelectMany(c => c.items).ToList();

        itemSelectionDropDown = new GenericMenu();

        for(var i = 0; i < items.Count; i++)
        {
            var item = items[i];
            var name = item.identifier;
            itemSelectionDropDown.AddItem(new GUIContent(name), false,
                () => AddItemFromDatabase(name, item.itemCategoryType));
        }
    }

    private void Test_TakeQuantity()
    {
        GUI.skin.label.fontSize = 18;
        GUILayout.Label("Debugging");
        GUI.skin.label.fontSize = 12;
        GUILayout.BeginHorizontal();

        //if(GUILayout.Button("Take"))
        //    taken = inventory.TakeQuantity(identifier, quantity);

        EditorGUIUtility.fieldWidth = 60;
        GUILayout.FlexibleSpace();
        EditorGUIUtility.labelWidth = 60;

        identifier = EditorGUILayout.TextField("Identifier: ", identifier);

        EditorGUIUtility.fieldWidth = 25;
        GUILayout.FlexibleSpace();
        EditorGUIUtility.labelWidth = 30;

        quantity = EditorGUILayout.IntField("Qty: ", Mathf.Clamp(quantity, 1, quantity));

        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        GUILayout.Label("Taken: " + taken);
        if(GUILayout.Button("Clear Taken"))
            taken = 0;
        GUILayout.EndHorizontal();
    }
}