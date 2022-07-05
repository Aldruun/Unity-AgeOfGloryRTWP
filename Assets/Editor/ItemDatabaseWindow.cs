using GenericFunctions;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public class ItemDatabaseWindow : EditorWindow
{
    enum VFXID
    {
        Projectile,
        OnHit,
        OnHandIdle,
        OnHandCharge,
        OnHandRelease,
        OnImpact,
    }
    VFXID vfxID;

    private static string[] tabs;
    private static string[] itemCategories;
    private static string[] actorCategories;
    private static string[] voicesetCategories;
    private static int selectedElementIndex;
    private static int selectedTabIndex;
    private Color colorDefault;

    private readonly Color colorselected = Color.blue;

    private GUISkin dbSkin;
    private bool foldoutState;
    private bool actorInventoryFoldoutState;
    private bool actorStatsFoldoutState;

    private Vector2 itemListScrollPosition;
    private Vector2 itemInspectorScrollPosition;
    private Vector2 actorListScrollPosition;
    private Vector2 actorInspectorScrollPosition;
    private Vector2[] voicesetListScrollPositions;

    private object selectedElement;
    private ActorConfiguration selectedActor;
    private Item selectedItem;
    private Spell selectedSpell;
    private MagicEffect selectedMagicEffect;
    private ItemDatabase itemDatabase;
    private VoiceSetDatabase voiceSetDatabase;
    private List<Spell> spells;
    private List<MagicEffect> magicEffects;

    private ActorDatabase actorDatabase;
    //private string[] _skillNames;
    private string[] vfxNames;

    private int magicSkillContextMenuIndex;
    private int vfxIndex;
    private readonly GenericMenu magicSkillSelectionMenu;
    private GenericMenu voicesetSelectionDropDown;
    private GenericMenu vfxSelectionMenu;

    #region Unity Functions
    private void OnEnable()
    {
        vfxNames = Resources.LoadAll("Prefabs/Particle Systems").Select(n => n.name).ToArray();
        vfxSelectionMenu = new GenericMenu();
        voicesetSelectionDropDown = new GenericMenu();
        foreach(string name in vfxNames)
        {
            vfxSelectionMenu.AddItem(new GUIContent(name), false, () => { SetMagicEffectVFXID(vfxID, name); });
        }

        //_skillNames = Resources.Load<ActorSkillDatabase>("ScriptableObjects/ActorSkillDatabase").actorSkills.Select(n => n.Name).ToArray();
        //_magicSkillSelectionMenu = new GenericMenu();
        //foreach(string name in _skillNames)
        //{
        //    _magicSkillSelectionMenu.AddItem(new GUIContent(name), false, () => { SetMagicSkillID(name); });
        //}

        tabs = new[] { "Items", "Actors", "Voice Sets" };
        if(EditorPrefs.HasKey("tabIndex"))
        {
            selectedTabIndex = EditorPrefs.GetInt("tabIndex", selectedTabIndex);
        }

        itemCategories = new[] { "Items", "Spells", "Magic Effects" };
        actorCategories = new[] { "Characters", "Creatures" };
        voicesetCategories = new[] { "Characters", "Creatures" };
        voicesetListScrollPositions = new Vector2[2];

        dbSkin =
            AssetDatabase.LoadAssetAtPath("Assets/Editor/Editor GUI Skins/GUISkin_IDB.guiskin", typeof(GUISkin)) as
                GUISkin;

        colorDefault = GUI.backgroundColor;

        if(actorDatabase == null)
        {
            actorDatabase = Resources.Load<ActorDatabase>("ScriptableObjects/ActorDatabase");
            if(actorDatabase == null)
            {
                Debug.LogError("ActorDatabase may not be null at this point");
            }
        }

        if(itemDatabase == null)
        {
            itemDatabase = Resources.Load("ScriptableObjects/ItemDatabase", typeof(ItemDatabase)) as ItemDatabase;
            if(itemDatabase == null)
            {
                Debug.LogError("ItemDatabase may not be null at this point");
            }
        }

        if(voiceSetDatabase == null)
        {
            voiceSetDatabase = Resources.Load("ScriptableObjects/VoiceSetDatabase", typeof(VoiceSetDatabase)) as VoiceSetDatabase;
        }

        if(voiceSetDatabase == null)
        {
            Debug.LogError("VoiceSetDatabase may not be null at this point");
        }

        spells = ScriptableObjectFactory.FindAssetsOfType<Spell>();
        magicEffects = ScriptableObjectFactory.FindAssetsOfType<MagicEffect>();

        if(itemDatabase.itemCategories == null)
        {
            Debug.LogError("itemCategories = null");
        }
        else if(itemDatabase.itemCategories.Count == 0)
        {
            Debug.LogError("itemCategories = empty");
        }

        if(itemDatabase.itemCategories == null || itemDatabase.itemCategories.Count == 0)
        {
            itemDatabase.itemCategories = new List<ItemCategory>
            {
                new ItemCategory("Ammo", ItemCategoryType.Ammo, new List<Item>()),
                new ItemCategory("Armor", ItemCategoryType.Armor, new List<Item>()),
                new ItemCategory("Weapon", ItemCategoryType.Weapon, new List<Item>()),
                new ItemCategory("Valuables", ItemCategoryType.Valuable, new List<Item>())
            };
        }

        LoadDatabase();
    }

    private void OnDisable()
    {
        EditorPrefs.SetInt("tabIndex", selectedTabIndex);
    }

    private void OnGUI()
    {
        selectedTabIndex = GUILayout.Toolbar(selectedTabIndex, tabs);

        //! Menu bar switching
        using(new GUILayout.HorizontalScope(EditorStyles.helpBox/*, GUILayout.Width(20), GUILayout.FlexibleSpace()*/))
        {
            switch(selectedTabIndex)
            {
                case 0://! Items menu bar
                    if(GUILayout.Button("S", GUILayout.Width(30)))
                    {
                        List<Item> items = new List<Item>();
                        foreach(ItemCategory item in itemDatabase.itemCategories)
                        {
                            items.AddRange(item.items);
                        }

                        ItemDatabase.WriteItemsToJSONFile(items);
                        EditorUtility.SetDirty(itemDatabase);
                        EditorUtility.SetDirty(voiceSetDatabase);
                    }

                    if(GUILayout.Button("L", GUILayout.Width(30)))
                    {
                        LoadDatabase();
                        EditorUtility.SetDirty(itemDatabase);
                    }

                    if(GUILayout.Button("LS", GUILayout.Width(30)))
                    {
                        spells = ScriptableObjectFactory.FindAssetsOfType<Spell>();
                    }

                    if(GUILayout.Button("LE", GUILayout.Width(30)))
                    {
                        magicEffects = ScriptableObjectFactory.FindAssetsOfType<MagicEffect>();
                    }
                    break;

                case 1: //! Actor menu bar
                    if(GUILayout.Button("Find Actor Configs", GUILayout.Width(120)))
                    {
                        actorDatabase.Characters = ScriptableObjectFactory.FindAssetsOfType<ActorConfiguration>();
                        EditorUtility.SetDirty(actorDatabase);
                    }

                    if(GUILayout.Button("Generate GUIDs For All Actors", GUILayout.Width(180)))
                    {
                        foreach(ActorConfiguration config in actorDatabase.Characters)
                        {
                            config.UniqueID = System.Guid.NewGuid().ToString();
                            EditorUtility.SetDirty(config);
                        }
                    }
                    break;

                case 2:
                    DrawReloadVoicesetsButton();
                    break;
            }
        }

        using(new GUILayout.HorizontalScope())
        {
            switch(selectedTabIndex)
            {
                case 0: // Items
                    DrawItemsPage();
                    break;
                case 1: // Actors
                    DrawActorsPage();
                    break;
                case 2: // Voice Sets
                    DrawVoiceSetsPage();
                    break;
            }
        }

        GUILayout.FlexibleSpace();

        //if(GUILayout.Button("Add Item"))
        //{
        //    ItemData data = new ItemData();
        //    data.Name = "New Item";
        //    database.availableItems.Add(data);
        //}

        //GUILayout.EndVertical();
    }

    public void OnInspectorUpdate()
    {
        Repaint();
    }
    #endregion End Unity Functions

    #region Draw Methods
    private void DrawActorsPage()
    {
        //using(new GUILayout.HorizontalScope())
        //{
        using(var scrollView = new GUILayout.ScrollViewScope(actorListScrollPosition, false, true, GUILayout.Width(240), GUILayout.MinHeight(200), GUILayout.MaxHeight(600)))
        {
            foreach(string objectCategory in actorCategories)
            {
                if(objectCategory == "Characters") //! -------------------------------------------------- Characters
                {
                    using(var horScope = new GUILayout.HorizontalScope(GUILayout.Width(226)))
                    {
                        foldoutState = Foldout(objectCategory, actorDatabase.Characters.Count);
                        if(GUILayout.Button("+", GUILayout.Width(20)))
                        {
                            string savedPath = EditorUtility.SaveFilePanel("Create Character Config", Application.dataPath, "New Character Config", "asset");
                            //Debug.Log(savedPath);
                            if(savedPath.Length > 0)
                            {
                                int startIndex = savedPath.IndexOf("Assets");
                                Debug.Log(savedPath.Substring(startIndex, savedPath.Length - startIndex) /*+ Path.DirectorySeparatorChar*/);
                                ActorConfiguration newCharacter = CreateInstance<ActorConfiguration>();
                                AssetDatabase.CreateAsset(newCharacter, savedPath.Substring(startIndex, savedPath.Length - startIndex));

                                actorDatabase.Characters.Add(newCharacter);
                                AssetDatabase.SaveAssets();
                                AssetDatabase.Refresh();
                                EditorUtility.FocusProjectWindow();
                                Selection.activeObject = newCharacter;
                            }
                        }
                    }

                    if(foldoutState)
                    {
                        foreach(ActorConfiguration config in actorDatabase.Characters)
                        {
                            if(selectedActor == config)
                            {
                                GUI.backgroundColor = colorselected;
                            }
                            else
                            {
                                GUI.backgroundColor = Color.clear;
                            }

                            EditorGUILayout.BeginHorizontal();
                            GUILayout.Space(30);
                            GUI.skin.button.alignment = TextAnchor.MiddleLeft;

                            if(config == null || config.InventoryTemplate == null || config.ActorPrefab == null || config.ActorFlags == 0 || config.Name == null /*|| selectedSpell.magicEffects[0] == null*/)
                            {
                                GUI.contentColor = Color.red;
                            }

                            if(GUILayout.Button(config.name == "" ? "New Character Config" : config.name, GUILayout.Width(170),
                                GUILayout.Height(15)))
                            {
                                GUI.FocusControl(null);
                                //if(_selectedElement == (object)spell)
                                selectedActor = config;
                                Selection.activeObject = config;
                                EditorGUIUtility.PingObject(config);
                                //}
                                //SelectElement(config);
                            }

                            GUI.contentColor = Color.white;

                            GUI.skin.button.alignment = TextAnchor.MiddleCenter;
                            if(GUILayout.Button("-", GUILayout.Width(20), GUILayout.Height(15)))
                            {
                                if(EditorUtility.DisplayDialog("Warning", "Do you really want to delete this CharacterConfig?",
                                                                            "Yes", "No"))
                                {
                                    actorDatabase.Characters.Remove(config);
                                    if(config == selectedActor)
                                    {
                                        selectedActor = null;
                                    }
                                }
                            }

                            EditorGUILayout.EndHorizontal();
                            GUI.backgroundColor = colorDefault;
                        }
                    }
                }
                else if(objectCategory == "Creatures") //! -------------------------------------------------- Creatures
                {
                    using(var horScope = new GUILayout.HorizontalScope(GUILayout.Width(226)))
                    {
                        foldoutState = Foldout(objectCategory, actorDatabase.Creatures.Count);
                        if(GUILayout.Button("+", GUILayout.Width(20)))
                        {
                            string savedPath = EditorUtility.SaveFilePanel("Create Creature Config", Application.dataPath, "New Creature Config", "asset");
                            //Debug.Log(savedPath);
                            if(savedPath.Length > 0)
                            {
                                int startIndex = savedPath.IndexOf("Assets");
                                Debug.Log(savedPath.Substring(startIndex, savedPath.Length - startIndex) /*+ Path.DirectorySeparatorChar*/);
                                ActorConfiguration newCreature = CreateInstance<ActorConfiguration>();
                                AssetDatabase.CreateAsset(newCreature, savedPath.Substring(startIndex, savedPath.Length - startIndex));

                                actorDatabase.Creatures.Add(newCreature);
                                AssetDatabase.SaveAssets();
                                AssetDatabase.Refresh();
                                EditorUtility.FocusProjectWindow();
                                Selection.activeObject = newCreature;
                            }
                        }
                    }

                    if(foldoutState)
                    {
                        foreach(ActorConfiguration config in actorDatabase.Creatures)
                        {
                            if(selectedActor == config)
                            {
                                GUI.backgroundColor = colorselected;
                            }
                            else
                            {
                                GUI.backgroundColor = Color.clear;
                            }

                            EditorGUILayout.BeginHorizontal();
                            GUILayout.Space(30);
                            GUI.skin.button.alignment = TextAnchor.MiddleLeft;

                            if(config == null || config.ActorPrefab == null || config.Spellbook == null /*|| selectedSpell.magicEffects[0] == null*/)
                            {
                                GUI.contentColor = Color.red;
                            }

                            if(GUILayout.Button(config.name == "" ? "New Creature Config" : config.name, GUILayout.Width(170),
                                GUILayout.Height(15)))
                            {
                                GUI.FocusControl(null);
                                //if(_selectedElement == (object)spell)
                                selectedActor = config;
                                Selection.activeObject = config;
                                EditorGUIUtility.PingObject(config);
                                //}
                                //SelectElement(config);
                            }

                            GUI.contentColor = Color.white;

                            GUI.skin.button.alignment = TextAnchor.MiddleCenter;
                            if(GUILayout.Button("-", GUILayout.Width(20), GUILayout.Height(15)))
                            {
                                if(EditorUtility.DisplayDialog("Warning", "Do you really want to delete this CreatureConfig?",
                                                                            "Yes", "No"))
                                {
                                    actorDatabase.Creatures.Remove(config);
                                    if(config == selectedActor)
                                    {
                                        selectedActor = null;
                                    }
                                }
                            }

                            EditorGUILayout.EndHorizontal();
                            GUI.backgroundColor = colorDefault;
                        }
                    }
                }
            }
        }

        if(selectedActor != null)
        {
            using(var scrollPos = new GUILayout.ScrollViewScope(actorInspectorScrollPosition, false, true, GUILayout.MinHeight(200), GUILayout.MaxHeight(600)))
            {
                SerializedObject srlActor = new SerializedObject(selectedActor);
                //EditorGUIUtility.labelWidth = 120;

                using(var check = new EditorGUI.ChangeCheckScope())
                {
                    using(new GUILayout.HorizontalScope())
                    {
                        selectedActor.UniqueID = EditorGUILayout.TextField("Unique ID:", selectedActor.UniqueID);
                        if(GUILayout.Button("↻", GUILayout.Width(30)))
                        {
                            selectedActor.UniqueID = System.Guid.NewGuid().ToString();
                        }
                    }

                    selectedActor.Name = EditorGUILayout.TextField("Name:", selectedActor.Name, GUILayout.Width(270));
                    selectedActor.portraitSprite = (Sprite)EditorGUILayout.ObjectField("Portrait:", selectedActor.portraitSprite, typeof(Sprite), false, GUILayout.ExpandWidth(false));

                    using(new GUILayout.HorizontalScope())
                    {
                        GUILayout.Label("Voice:");
                        if(GUILayout.Button(selectedActor.VoicesetID == "" ? "NONE" : selectedActor.VoicesetID))
                        {
                            RefreshVoicesetDatabaseEntries();
                            voicesetSelectionDropDown.ShowAsContext();
                        }
                    }

                    selectedActor.Level = EditorGUILayout.IntField("Level: ", Mathf.Clamp(selectedActor.Level, 1, 20), GUILayout.Width(300));
                    //selectedActor.Spellbook = (SpellBook)EditorGUILayout.ObjectField("Spellbook:", selectedActor.Spellbook, typeof(SpellBook), false, GUILayout.Width(300));

                    //if(selectedActor.Spellbook != null)
                    //{
                    //    int count = selectedActor.Spellbook.SpellData.Count;

                    //    if(count > 0)
                    //    {
                    //        actorStatsFoldoutState = Foldout("Skills", count, "actorInsp");

                    //        if(actorStatsFoldoutState)
                    //        {
                    //            GUI.contentColor = Color.gray;
                    //            using(new GUILayout.HorizontalScope())
                    //            {
                    //                GUILayout.Space(15);
                    //                using(new GUILayout.VerticalScope())
                    //                {
                    //                    for(int i = 0; i < selectedActor.Spellbook.SpellData.Count; i++)
                    //                    {
                    //                        SpellPropertyDrawer skill = selectedActor.Spellbook.SpellData[i];

                    //                        GUILayout.Label(skill.spell.name);
                    //                    }
                    //                }
                    //            }
                    //            GUI.contentColor = Color.white;
                    //        }
                    //    }
                    //}

                    selectedActor.ActorFlags = (ActorFlags)EditorGUILayout.EnumFlagsField("Actor Flags: ", selectedActor.ActorFlags, GUILayout.Width(300));
                    selectedActor.Race = (ActorRace)EditorGUILayout.EnumPopup("Race: ", selectedActor.Race, GUILayout.Width(300));
                    selectedActor.ActorClass = (Class)EditorGUILayout.EnumPopup("Class: ", selectedActor.ActorClass, GUILayout.Width(300));
                    selectedActor.Faction = (Faction)EditorGUILayout.EnumPopup("Faction: ", selectedActor.Faction, GUILayout.Width(300));
                    selectedActor.ActorPrefab = (GameObject)EditorGUILayout.ObjectField("Actor Prefab:", selectedActor.ActorPrefab, typeof(GameObject), false, GUILayout.Width(300));

                    selectedActor.ActorRadius = EditorGUILayout.FloatField("Radius: ", selectedActor.ActorRadius, GUILayout.Width(300));

                    selectedActor.InventoryTemplate = (InventoryTemplate)EditorGUILayout.ObjectField("Inventory:", selectedActor.InventoryTemplate, typeof(InventoryTemplate), false, GUILayout.Width(300));

                    if(selectedActor.InventoryTemplate != null)
                    {
                        int count = selectedActor.InventoryTemplate.items.Count;

                        if(count > 0)
                        {
                            actorInventoryFoldoutState = Foldout("Items", count, "actorInsp");

                            if(actorInventoryFoldoutState)
                            {
                                GUI.contentColor = Color.gray;
                                using(new GUILayout.HorizontalScope())
                                {
                                    GUILayout.Space(15);
                                    using(new GUILayout.VerticalScope())
                                    {
                                        for(int i = 0; i < selectedActor.InventoryTemplate.items.Count; i++)
                                        {
                                            InventoryItem item = selectedActor.InventoryTemplate.items[i];

                                            GUILayout.Label(item.stackSize + " " + item.ID);
                                        }
                                    }
                                }
                                GUI.contentColor = Color.white;
                            }
                        }
                    }

                    if(check.changed)
                    {
                        EditorUtility.SetDirty(selectedActor);
                    }
                }

                srlActor.ApplyModifiedProperties();
            }
        }
        //}
    }

    private void DrawItemsPage()
    {
        using(var scrollViewItems = new GUILayout.ScrollViewScope(itemListScrollPosition, false, true,
                                           GUILayout.Width(240), GUILayout.MinHeight(200), GUILayout.MaxHeight(600)))
        {
            itemListScrollPosition = scrollViewItems.scrollPosition;

            foreach(string objectCategory in itemCategories)
            {
                if(objectCategory == "Items")
                {
                    GUILayout.BeginHorizontal(GUILayout.Width(226));

                    foldoutState = Foldout(objectCategory);
                    GUILayout.EndHorizontal();
                    if(foldoutState)
                    {
                        DrawItemsList();
                    }
                }
                else if(objectCategory == "Spells")
                {
                    DrawSpellsList();
                }
                else if(objectCategory == "Magic Effects")
                {
                    DrawMagicEffectsList();
                }
            }
        }

        if(selectedElement != null) //! Settings Inspector
        {
            using(var scrollPos = new GUILayout.ScrollViewScope(itemInspectorScrollPosition, false, true, GUILayout.MinHeight(200), GUILayout.MaxHeight(600)))
            {
                switch(selectedElementIndex)
                {
                    case 0: // Items
                        {
                            DrawItemInspector();
                            break;
                        }
                    case 1: // Spells
                        {
                            DrawSpellInspector();
                            break;
                        }
                    case 2: // MagicEffects
                        {
                            DrawMagicEffectInspector();
                            break;
                        }

                    default:
                        Debug.LogError($"Selection index {selectedElementIndex} invalid");
                        break;
                }
            }
        }
    }

    private void DrawReloadVoicesetsButton()
    {
        if(GUILayout.Button("Reload VoiceSets", GUILayout.Width(120)))
        {
            foreach(string voicesetCategory in voicesetCategories)
            {
                int index = -1;
                if(voicesetCategory == "Characters")
                {
                    index = 0;
                }

                if(voicesetCategory == "Creatures")
                {
                    index = 1;
                }

                List<ActorVoiceSetData> set = voiceSetDatabase.AccessVoiceSetCategory(index);

                foreach(ActorVoiceSetData voiceSetData in set)
                {
                    CharacterVoiceSet refSource = new CharacterVoiceSet();
                    FieldInfo[] fieldInfos = refSource.GetType().GetFields();
                    string[] fieldNames = new string[fieldInfos.Length];
                    for(int f = 0; f < fieldNames.Length; f++) //Debug.Log("Field name: " + fieldInfos[f].Name);
                    {
                        fieldNames[f] = fieldInfos[f].Name;
                    }

                    GenericMenu gMenu = new GenericMenu();

                    string filePath = Path.Combine(Application.dataPath, "Resources/SFX/Actor SFX/VoiceSets/" + (index == 0 ? "Characters" : "Creatures"));
                    string voiceSetDir = Directory.GetDirectories(filePath)
                        .Where(d => Path.GetFileName(d).Equals(voiceSetData.voiceSet.Name, System.StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                    Debug.Log("Checking voiceset name '" + voiceSetData.voiceSet.Name + "'");
                    if(voiceSetDir == null)
                    {
                        UnityEngine.Debug.LogError("VoiceSetData folder not found");
                        continue;
                    }
                    else
                    {
                        UnityEngine.Debug.Log($"Found dir '{voiceSetDir}'");
                    }

                    string voiceSetFolderName = Path.GetFileName(voiceSetDir);

                    UnityEngine.Debug.Log($"Found file '{voiceSetFolderName}'");

                    //Debug.Log("*Searching folder '" + voiceSetFolderName + "' for voice categories");
                    string[] categoryDirs = Directory.GetDirectories(voiceSetDir);
                    //CharacterVoiceSet voiceSet = new CharacterVoiceSet(voiceSetFolderName);

                    //? Iterate through all "Category" folders (i.e. "FoundLoot")
                    for(int c = 0; c < categoryDirs.Length; c++)
                    {
                        string categoryName = Path.GetFileName(categoryDirs[c]);
                        //Debug.Log($"Testing category name '{categoryName}'");

                        foreach(string fieldName in fieldNames) //Debug.Log("Field name: " + fieldName + " / Category name: " + categoryName);
                        {
                            if(string.Equals(fieldName, categoryName, System.StringComparison.OrdinalIgnoreCase))
                            {
                                //Debug.Log("BINGO! " + fieldName + " equals " + categoryName);

                                AudioClip[] audioClipGroup =
                                    (AudioClip[])voiceSetData.voiceSet.GetType().GetField(fieldName).GetValue(voiceSetData.voiceSet);

                                audioClipGroup =
                                    Resources.LoadAll<AudioClip>(
                                        "SFX/Actor SFX/VoiceSets/" + voiceSetFolderName + "/" + categoryName);

                                //foreach(AudioClip ac in audioClipGroup) {

                                //    Debug.Log("Found clip: " + ac.name);
                                //}

                                FieldInfo fieldInfo = voiceSetData.voiceSet.GetType().GetField(fieldName,
                                    BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

                                fieldInfo.SetValue(voiceSetData.voiceSet, System.Convert.ChangeType(audioClipGroup, fieldInfo.FieldType));
                            }
                        }
                    }
                    //}
                }
            }
        }
    }

    private void DrawVoiceSetsPage()
    {
        foreach(string voicesetCategory in voicesetCategories)
        {
            int index = -1;
            if(voicesetCategory == "Characters")
            {
                index = 0;
            }

            if(voicesetCategory == "Creatures")
            {
                index = 1;
            }

            if(index == -1)
            {
                break;
            }

            List<ActorVoiceSetData> set = voiceSetDatabase.AccessVoiceSetCategory(index);

            //GUILayout.FlexibleSpace();
            using(new GUILayout.HorizontalScope(GUILayout.Width(265), GUILayout.ExpandWidth(true))) //! SetLists horizontal
            {
                using(new GUILayout.VerticalScope()) //! AddButton and SetList vertical
                {
                    using(new GUILayout.HorizontalScope())
                    {
                        GUILayout.Label(voicesetCategory/*, GUILayout.Width(60)*/);
                        if(GUILayout.Button("+", GUILayout.Width(20)))
                        {
                            set.Add(new ActorVoiceSetData());
                        }
                    }

                    using(var scopeVS1 = new GUILayout.ScrollViewScope(voicesetListScrollPositions[index], false, false, GUILayout.MinHeight(200), GUILayout.MaxHeight(600)))
                    {
                        voicesetListScrollPositions[index] = scopeVS1.scrollPosition;
                        //using(var scopeVS2 = new GUILayout.ScrollViewScope(voicesetListScrollPosition, false, false, GUILayout.MinHeight(200), GUILayout.MaxHeight(600)))
                        //{
                        for(int i = 0; i < set.Count; i++)
                        {
                            ActorVoiceSetData voiceSetData = set[i];
                            using(new GUILayout.HorizontalScope())
                            {
                                GUILayout.Label(i.ToString() + ".", GUILayout.Width(15));

                                if(voiceSetData.voiceSet == null)
                                {
                                    GUI.contentColor = Color.red;
                                }
                                else
                                {
                                    if(GUILayout.Button("<I ))", GUILayout.Width(40)))
                                    {
                                        if(voiceSetData.voiceSet.levelGained != null && voiceSetData.voiceSet.levelGained.Length > 0)
                                        {
                                            PlayClip(voiceSetData.voiceSet.levelGained[Random.Range(0, voiceSetData.voiceSet.levelGained.Length)], 0, false);
                                        }
                                        else if(voiceSetData.voiceSet.attack != null && voiceSetData.voiceSet.attack.Length > 0)
                                        {
                                            PlayClip(voiceSetData.voiceSet.attack[Random.Range(0, voiceSetData.voiceSet.attack.Length)], 0, false);
                                        }
                                        else if(voiceSetData.voiceSet.hurt != null && voiceSetData.voiceSet.hurt.Length > 0)
                                        {
                                            PlayClip(voiceSetData.voiceSet.hurt[Random.Range(0, voiceSetData.voiceSet.hurt.Length)], 0, false);
                                        }
                                    }
                                }

                                if(GUILayout.Button(voiceSetData.voiceSet != null ? voiceSetData.voiceSet.Name : "N/A"/*, GetBtnStyle()*/))
                                {
                                    //EditorUtility.FocusProjectWindow();
                                    PingFolder("Resources/SFX/Actor SFX/VoiceSets/"/* + voiceSetData.voiceSet.Name*/);
                                }
                                if(GUILayout.Button("-", GUILayout.Width(20)))
                                {
                                    if(EditorUtility.DisplayDialog("Warning", "Do you really want to delete this VoiceSet?", "Delete", "Cancel"))
                                    {
                                        set.Remove(voiceSetData);
                                    }
                                    continue;
                                }
                                GUI.contentColor = Color.white;
                            }

                            //EditorGUIUtility.labelWidth = 90;

                            GUILayout.Space(5);
                            //}

                        }
                    }
                }
            }
        }
    }

    private void DrawItemsList()
    {
        //GUILayout.BeginHorizontal();
        foreach(ItemCategory category in itemDatabase.itemCategories)
        {

            if(category == null)
            {
                Debug.LogError("category null");
                continue;
            }
            //_foldoutIndex = database.itemCategories.IndexOf(category);
            GUILayout.BeginHorizontal(GUILayout.Width(226));
            GUILayout.Space(15);
            foldoutState = Foldout(category);
            if(GUILayout.Button("+", GUILayout.Width(20)))
            {
                Item newItem = null;

                switch(category.itemCategoryType)
                {
                    case ItemCategoryType.Ammo:
                        newItem = new Ammo();
                        newItem.Name = "New Ammo";
                        break;
                    case ItemCategoryType.Armor:
                        newItem = new Armor();
                        newItem.Name = "New Armor Piece";
                        break;
                    case ItemCategoryType.Weapon:
                        newItem = new Weapon();
                        newItem.Name = "New Weapon";
                        break;
                    case ItemCategoryType.Valuable:
                        newItem = new Valuable();
                        newItem.Name = "New Valuable";
                        break;
                }

                newItem.itemCategoryType = category.itemCategoryType;

                category.items.Add(newItem);

                SelectElement(newItem);
            }

            GUILayout.EndHorizontal();
            //GUI.skin = dbSkin;
            if(foldoutState)
            {
                for(int i = 0; i < category.items.Count; i++)
                {

                    Item item = category.items[i];

                    if(selectedElement == item)
                    {
                        GUI.backgroundColor = colorselected;
                    }
                    else
                    {
                        GUI.backgroundColor = Color.clear;
                    }

                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Space(30);
                    GUI.skin.button.alignment = TextAnchor.MiddleLeft;
                    if(GUILayout.Button(item.Name == "" ? "New Item" : item.Name, GUILayout.Width(170),
                        GUILayout.Height(15)))
                    {
                        GUI.FocusControl(null);
                        if(selectedElement == item)
                        {
                            Object[] items = Resources.LoadAll("Prefabs/Items");

                            foreach(Object itemObj in items)
                            {
                                if(itemObj.name == item.identifier)
                                {
                                    Selection.activeObject = itemObj;
                                    EditorGUIUtility.PingObject(itemObj);
                                }
                            }
                        }
                        SelectElement(item);
                    }
                    GUI.skin.button.alignment = TextAnchor.MiddleCenter;
                    if(GUILayout.Button("-", GUILayout.Width(20), GUILayout.Height(15)))
                    {
                        if(EditorUtility.DisplayDialog("Warning", "Do you really want to delete this item?", "Yes", "No"))
                        {
                            category.items.Remove(item);
                            if(item == selectedElement)
                            {
                                selectedElement = null;
                            }
                        }
                    }

                    EditorGUILayout.EndHorizontal();
                    GUI.backgroundColor = colorDefault;
                }
            }

            //GUI.skin = null;
            //_foldoutIndex++;
        }
    }

    private void DrawSpellsList()
    {
        using(var horScope = new GUILayout.HorizontalScope(GUILayout.Width(226)))
        {
            foldoutState = Foldout("Spells", spells.Count);
            if(GUILayout.Button("+", GUILayout.Width(20)))
            {
                string savedPath = EditorUtility.SaveFilePanel("Create Spell Asset", Application.dataPath, "New Spell Asset", "asset");
                //Debug.Log(savedPath);
                if(savedPath.Length > 0)
                {
                    int startIndex = savedPath.IndexOf("Assets");
                    Debug.Log(savedPath.Substring(startIndex, savedPath.Length - startIndex) /*+ Path.DirectorySeparatorChar*/);
                    Spell spell = CreateInstance<Spell>();
                    AssetDatabase.CreateAsset(spell, savedPath.Substring(startIndex, savedPath.Length - startIndex));

                    spells.Add(spell);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                    EditorUtility.FocusProjectWindow();
                    Selection.activeObject = spell;
                }
            }
        }

        if(foldoutState)
        {
            foreach(Spell spell in spells)
            {
                if(selectedElement == (object)spell)
                {
                    GUI.backgroundColor = colorselected;
                }
                else
                {
                    GUI.backgroundColor = Color.clear;
                }

                using(new GUILayout.HorizontalScope())
                {
                    GUILayout.Space(30);
                    GUI.skin.button.alignment = TextAnchor.MiddleLeft;

                    if(spell == null || spell.magicEffects.Count == 0 /*|| selectedSpell.magicEffects[0] == null*/)
                    {
                        GUI.contentColor = Color.red;
                    }

                    if(GUILayout.Button(spell.name == "" ? "New Spell Asset" : spell.name, GUILayout.Width(170),
                        GUILayout.Height(15)))
                    {
                        GUI.FocusControl(null);

                        Selection.activeObject = spell;
                        EditorGUIUtility.PingObject(spell);

                        SelectElement(spell);
                    }

                    GUI.contentColor = Color.white;

                    GUI.skin.button.alignment = TextAnchor.MiddleCenter;
                    if(GUILayout.Button("-", GUILayout.Width(20), GUILayout.Height(15)))
                    {
                        if(EditorUtility.DisplayDialog("Warning", "Do you really want to delete this Spell Asset?",
                                                                    "Yes", "No"))
                        {
                            spells.Remove(spell);
                            if(spell == (Spell)selectedElement)
                            {
                                selectedElement = null;
                            }
                        }
                    }

                }
                GUI.backgroundColor = colorDefault;
            }
        }
    }

    private void DrawMagicEffectsList()
    {
        using(var horScope = new GUILayout.HorizontalScope(GUILayout.Width(226)))
        {
            foldoutState = Foldout("Magic Effects", magicEffects.Count);
            if(GUILayout.Button("+", GUILayout.Width(20)))
            {
                string savedPath = EditorUtility.SaveFilePanel("Create MagicEffect Asset", Application.dataPath, "New MagicEffect Asset", "asset");
                //Debug.Log(savedPath);
                if(savedPath.Length > 0)
                {
                    int startIndex = savedPath.IndexOf("Assets");
                    Debug.Log(savedPath.Substring(startIndex, savedPath.Length - startIndex) /*+ Path.DirectorySeparatorChar*/);
                    MagicEffect magicEffect = CreateInstance<MagicEffect>();
                    AssetDatabase.CreateAsset(magicEffect, savedPath.Substring(startIndex, savedPath.Length - startIndex));

                    magicEffects.Add(magicEffect);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                    EditorUtility.FocusProjectWindow();
                    Selection.activeObject = magicEffect;
                }
            }
        }

        if(foldoutState)
        {
            foreach(MagicEffect magicEffect in magicEffects)
            {
                if(selectedElement == (object)magicEffect)
                {
                    GUI.backgroundColor = colorselected;
                }
                else
                {
                    GUI.backgroundColor = Color.clear;
                }

                using(new GUILayout.HorizontalScope())
                {
                    GUILayout.Space(30);
                    GUI.skin.button.alignment = TextAnchor.MiddleLeft;

                    if(magicEffect == null/*|| selectedSpell.magicEffects[0] == null*/)
                    {
                        GUI.contentColor = Color.red;
                    }

                    if(GUILayout.Button(magicEffect.name == "" ? "New MagicEffect Asset" : magicEffect.name, GUILayout.Width(170),
                        GUILayout.Height(15)))
                    {
                        GUI.FocusControl(null);

                        Selection.activeObject = magicEffect;
                        EditorGUIUtility.PingObject(magicEffect);

                        SelectElement(magicEffect);
                    }

                    GUI.contentColor = Color.white;

                    GUI.skin.button.alignment = TextAnchor.MiddleCenter;
                    if(GUILayout.Button("-", GUILayout.Width(20), GUILayout.Height(15)))
                    {
                        if(EditorUtility.DisplayDialog("Warning", "Do you really want to delete this MagicEffect Asset?",
                                                                    "Yes", "No"))
                        {
                            magicEffects.Remove(magicEffect);
                            if(magicEffect == (MagicEffect)selectedElement)
                            {
                                selectedElement = null;
                            }
                        }
                    }

                }
                GUI.backgroundColor = colorDefault;
            }
        }
    }

    private void DrawItemInspector()
    {
        selectedItem = (Item)selectedElement;

        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.BeginVertical();
        EditorGUIUtility.labelWidth = 100;

        //EditorGUI.BeginChangeCheck();
        //_selectedItem.icon = (Sprite)EditorGUILayout.ObjectField("Icon:", _selectedItem.icon, typeof(Sprite), false, GUILayout.ExpandWidth(false));
        //if(EditorGUI.EndChangeCheck())
        //{
        //    if(_selectedItem.icon != null)
        //    {
        //        SerializedObject srlItem = new SerializedObject(_selectedItem.icon);
        //        srlItem.ApplyModifiedProperties();
        //    }
        //}


        selectedItem.Name = EditorGUILayout.TextField("Name:", selectedItem.Name, GUILayout.Width(270));

        EditorGUILayout.BeginHorizontal();
        selectedItem.identifier = EditorGUILayout.TextField("ID:", selectedItem.identifier, GUILayout.Width(270));
        if(GUILayout.Button(".", GUILayout.Width(15), GUILayout.Height(15)))
        {
            selectedItem.identifier = selectedItem.Name.ToLower();
        }

        EditorGUILayout.EndHorizontal();

        selectedItem.weight =
            EditorGUILayout.FloatField("Weight:", selectedItem.weight, GUILayout.Width(150));
        selectedItem.value = EditorGUILayout.IntField("Value:", selectedItem.value, GUILayout.Width(150));

        if(selectedElement is Ammo)
        {
            Ammo ammo = (Ammo)selectedElement;
            ammo.ammoType = (AmmoType)EditorGUILayout.EnumPopup("Type: ", ammo.ammoType, GUILayout.Width(270));
            ammo.damage = EditorGUILayout.FloatField("Damage:", ammo.damage, GUILayout.Width(150));
            ammo.projectileID =
                EditorGUILayout.TextField("Projectile ID:", ammo.projectileID, GUILayout.Width(300));
        }
        else if(selectedElement is Armor)
        {
            Armor armorPiece = (Armor)selectedElement;
            armorPiece.AC = EditorGUILayout.IntField("AR:", armorPiece.AC, GUILayout.Width(150));
            armorPiece.bodySlot = (BodySlot)EditorGUILayout.EnumPopup("Body Slot: ", armorPiece.bodySlot, GUILayout.Width(270));
            armorPiece.equipType = (EquipType)EditorGUILayout.EnumPopup("Equip Type: ", armorPiece.equipType, GUILayout.Width(270));
            armorPiece.armorType = (ArmorType)EditorGUILayout.EnumPopup("Armor Type: ", armorPiece.armorType, GUILayout.Width(270));
        }
        else if(selectedElement is Weapon)
        {
            Weapon weapon = (Weapon)selectedElement;

            GUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));
            EditorGUIUtility.labelWidth = 70;
            weapon.NumDice = EditorGUILayout.IntField("Attack Roll:", weapon.NumDice, GUILayout.Width(94));
            EditorGUIUtility.labelWidth = 15;
            weapon.NumDieSides = EditorGUILayout.IntField("d", weapon.NumDieSides, GUILayout.Width(150));
            EditorGUIUtility.labelWidth = 120;
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            weapon.MaxHitTargets = EditorGUILayout.IntField("Max Hit Targets:", weapon.MaxHitTargets);

            weapon.projectileIdentifier = EditorGUILayout.TextField("Projectile Identifier", weapon.projectileIdentifier, GUILayout.Width(300));

            weapon.speed = EditorGUILayout.FloatField("Speed:", weapon.speed, GUILayout.Width(150));

            GUILayout.BeginHorizontal();
            GUILayout.Label("Required Stats", GUILayout.Width(116));
            if(GUILayout.Button("+", GUILayout.Width(20)))
            {
                weapon.requiredStats.Add(new ActorStatData());
            }
            GUILayout.EndHorizontal();

            //for(int i = 0; i < weapon.requiredStats.Count; i++)
            //{
            //    GUILayout.BeginHorizontal();
            //    var rqs = weapon.requiredStats[i];
            //    rqs.stat = (Stat)EditorGUILayout.EnumPopup("", rqs.stat, GUILayout.Width(150));
            //    rqs.size = EditorGUILayout.IntField("Required:", rqs.size, GUILayout.Width(150));
            //    if(GUILayout.Button("-", GUILayout.Width(20)))
            //    {
            //        weapon.requiredStats.Remove(rqs);
            //    }
            //    GUILayout.EndHorizontal();

            //    GUILayout.Space(5);
            //}

            GUILayout.BeginHorizontal();
            GUILayout.Label("Bonus Stats", GUILayout.Width(116));
            if(GUILayout.Button("+", GUILayout.Width(20)))
            {
                weapon.bonusStats.Add(new ActorStatData());
            }
            GUILayout.EndHorizontal();

            //for(int i = 0; i < weapon.bonusStats.Count; i++)
            //{
            //    GUILayout.BeginHorizontal();
            //    ActorStatData bs = weapon.bonusStats[i];
            //    bs.stat = (Stat)EditorGUILayout.EnumPopup("", bs.stat, GUILayout.Width(150));
            //    bs.size = EditorGUILayout.IntField("Bonus:", bs.size, GUILayout.Width(150));
            //    if(GUILayout.Button("-", GUILayout.Width(20)))
            //    {
            //        weapon.bonusStats.Remove(bs);
            //    }
            //    GUILayout.EndHorizontal();

            //    GUILayout.Space(5);
            //}

            weapon.weaponCategory = (WeaponCategory)EditorGUILayout.EnumPopup("Weapon Category: ", weapon.weaponCategory, GUILayout.Width(270));
            //weapon.damageType = (DamageType)EditorGUILayout.EnumPopup("Damage Type: ", weapon.damageType, GUILayout.Width(270));
            //weapon.rangeCategory = (RangeCategory)EditorGUILayout.EnumPopup("Range: ", weapon.rangeCategory, GUILayout.Width(270));
            //weapon.equipType = (EquipType)EditorGUILayout.EnumPopup("Equip Type: ", weapon.equipType, GUILayout.Width(270));
            //weapon.impactSFXType = (WeaponImpactType)EditorGUILayout.EnumPopup("Impact SFX Type: ", weapon.impactSFXType, GUILayout.Width(270));
            //weapon.motionIndex = EditorGUILayout.IntField("Motion Index:", weapon.motionIndex, GUILayout.Width(130));
        }
        else if(selectedElement is Valuable)
        {

            //weapon.motionIndex = EditorGUILayout.IntField("Motion Index:", weapon.motionIndex, GUILayout.Width(130));
        }
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();
    }

    private void DrawSpellInspector()
    {
        selectedSpell = (Spell)selectedElement;
        SerializedObject srlSpell = new SerializedObject(selectedSpell);

        EditorGUILayout.BeginVertical();
        EditorGUIUtility.labelWidth = 120;

        EditorGUILayout.BeginHorizontal();

        //AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(selectedSpell.GetInstanceID()), EditorGUILayout.DelayedTextField("Asset Name:", selectedSpell.name, GUILayout.Width(270)));
        EditorGUI.BeginChangeCheck();
        selectedSpell.name = EditorGUILayout.TextField("Asset Name:", selectedSpell.name, GUILayout.Width(270));
        if(EditorGUI.EndChangeCheck())
        {
            AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(selectedSpell), selectedSpell.name);
        }

        //AssetDatabase.Refresh();
        if(GUILayout.Button(".", GUILayout.Width(15), GUILayout.Height(15)))
            selectedSpell.name = selectedSpell.Name.Replace(" ", "");
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        selectedSpell.Name = EditorGUILayout.TextField("Name:", selectedSpell.Name, GUILayout.Width(270));
        if(GUILayout.Button(".", GUILayout.Width(15), GUILayout.Height(15)))
            selectedSpell.Name = selectedSpell.name.SplitCamelCase();
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        selectedSpell.ID = EditorGUILayout.TextField("ID:", selectedSpell.ID, GUILayout.Width(270));
        if(GUILayout.Button(".", GUILayout.Width(15), GUILayout.Height(15)))
            selectedSpell.ID = selectedSpell.Name.ToLower();
        EditorGUILayout.EndHorizontal();
        EditorStyles.textField.wordWrap = true;
        //EditorStyles.textField.stretchHeight = true;
        GUILayout.Label("Description:", GUILayout.Width(200));
        selectedSpell.description = EditorGUILayout.TextArea(selectedSpell.description, GUILayout.ExpandHeight(true), GUILayout.Height(100));

        selectedSpell.spellIcon = (Sprite)EditorGUILayout.ObjectField("Icon:", selectedSpell.spellIcon, typeof(Sprite), false, GUILayout.ExpandWidth(false));
        //selectedSpell.projectileType = (ProjectileType)EditorGUILayout.EnumPopup("Projectile Type: ", selectedSpell.projectileType);
        //selectedSpell.activationMode = (SpellActivationMode)EditorGUILayout.EnumPopup("Activation Mode: ", selectedSpell.activationMode, GUILayout.Width(270));
        selectedSpell.subCategory = (SubCategory)EditorGUILayout.EnumPopup("Subcategory: ", selectedSpell.subCategory, GUILayout.Width(270));
        selectedSpell.attackRollType = (SpellAttackRollType)EditorGUILayout.EnumPopup("Attack Roll: ", selectedSpell.attackRollType, GUILayout.Width(270));
        selectedSpell.effectType = (DamageType)EditorGUILayout.EnumPopup("Effect Type: ", selectedSpell.effectType, GUILayout.Width(270));
        selectedSpell.spellTargetType = (SpellTargetType)EditorGUILayout.EnumPopup("Target Type: ", selectedSpell.spellTargetType, GUILayout.Width(270));
        selectedSpell.savingThrowType = (SavingThrowType)EditorGUILayout.EnumPopup("Saving Throw Type: ", selectedSpell.savingThrowType, GUILayout.Width(270));
        selectedSpell.magicSchool = (MagicSchool)EditorGUILayout.EnumPopup("Magic School: ", selectedSpell.magicSchool, GUILayout.Width(300));
        selectedSpell.equipType = (EquipType)EditorGUILayout.EnumPopup("Equip Type: ", selectedSpell.equipType, GUILayout.Width(300));
        selectedSpell.castingType = (CastingType)EditorGUILayout.EnumPopup("Casting Type: ", selectedSpell.castingType, GUILayout.Width(300));
        selectedSpell.deliveryType = (DeliveryType)EditorGUILayout.EnumPopup("Delivery Type: ", selectedSpell.deliveryType, GUILayout.Width(300));

        selectedSpell.keywords = (Keyword)EditorGUILayout.EnumFlagsField("Keywords; ", selectedSpell.keywords, GUILayout.Width(300));

        //GUILayout.BeginHorizontal();
        //GUILayout.Label("Class Requirements", GUILayout.Width(116));
        //if(GUILayout.Button("+", GUILayout.Width(20)))
        //{
        //    selectedSpell.spellRules.Add(new SpellRule());
        //}
        //GUILayout.EndHorizontal();

        //selectedSpell.targetClasses = (Class[])EditorGUILayout.EnumPopup("Target Classes", selectedSpell.targetClasses, GUILayout.Width(150));
        //foreach(SpellRule spellRule in selectedSpell.spellRules.ToArray())
        //{
        //    GUILayout.BeginHorizontal();
        //    spellRule = (Class)EditorGUILayout.EnumPopup("", spellRule.targetClass, GUILayout.Width(150));
        //    spellRule.requiredLevel = EditorGUILayout.IntField("Req. Level:", spellRule.requiredLevel, GUILayout.Width(150));
        //    if(GUILayout.Button("-", GUILayout.Width(20)))
        //    {
        //        selectedSpell.spellRules.Remove(spellRule);
        //    }
        //    GUILayout.EndHorizontal();

        //    GUILayout.Space(5);
        //}

        selectedSpell.grade = EditorGUILayout.IntField("Grade:", Mathf.Clamp(selectedSpell.grade, 0, 9), GUILayout.Width(150));
        SerializedProperty sTargetClasses = srlSpell.FindProperty("targetClasses");
        EditorGUILayout.PropertyField(sTargetClasses, true, GUILayout.Width(300));

        //if(selectedSpell.magicEffects.Count == 0 || selectedSpell.magicEffects[0] == null)
        //{
        //    GUI.contentColor = Color.red;
        //}

        SerializedProperty sMagicEffects = srlSpell.FindProperty("magicEffects");
        EditorGUILayout.PropertyField(sMagicEffects, true, GUILayout.Width(300));

        //GUI.contentColor = Color.white;

        selectedSpell.targetLogic = (SpellTargetLogic)EditorGUILayout.ObjectField("Target Logic:", selectedSpell.targetLogic, typeof(SpellTargetLogic), false, GUILayout.Width(300)/*, GUILayout.ExpandWidth(false)*/);
        selectedSpell.castingTime = EditorGUILayout.IntField("Casting Time:", selectedSpell.castingTime, GUILayout.Width(150));
        selectedSpell.duration = EditorGUILayout.IntField("Spell Duration:", selectedSpell.duration, GUILayout.Width(150));
        selectedSpell.concentrationTime = EditorGUILayout.IntField("Concentration Time:", selectedSpell.concentrationTime, GUILayout.Width(150));

        selectedSpell.attackRollRequired = EditorGUILayout.Toggle("Attack Roll Required", selectedSpell.attackRollRequired);

        GUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));
        EditorGUIUtility.labelWidth = 80;
        selectedSpell.specialRollDice = EditorGUILayout.IntField("Special Roll:", selectedSpell.specialRollDice, GUILayout.Width(100));
        EditorGUIUtility.labelWidth = 15;
        selectedSpell.numSpecialRollDieSides = EditorGUILayout.IntField("d", selectedSpell.numSpecialRollDieSides, GUILayout.Width(150));
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));
        EditorGUIUtility.labelWidth = 80;
        selectedSpell.damageRollDice = EditorGUILayout.IntField("Damage Roll:", selectedSpell.damageRollDice, GUILayout.Width(100));
        EditorGUIUtility.labelWidth = 15;
        selectedSpell.numDamageRollDieSides = EditorGUILayout.IntField("d", selectedSpell.numDamageRollDieSides, GUILayout.Width(150));
        EditorGUIUtility.labelWidth = 120;
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        selectedSpell.higherSlotBonusDice = EditorGUILayout.IntField("Die Per Higher Slot:", selectedSpell.higherSlotBonusDice, GUILayout.Width(150));
        selectedSpell.percentMagnitude = EditorGUILayout.Toggle("Percent Magnitude: ", selectedSpell.percentMagnitude);
        //selectedSpell.cost = EditorGUILayout.FloatField("Cost:", selectedSpell.cost, GUILayout.Width(150));
        selectedSpell.activationRange = EditorGUILayout.IntField("Activation Range:", selectedSpell.activationRange, GUILayout.Width(150));
        selectedSpell.aoeRadius = EditorGUILayout.FloatField("AOE Radius:", selectedSpell.aoeRadius, GUILayout.Width(150));
        selectedSpell.travelSpeed = EditorGUILayout.FloatField("Travel Speed:", selectedSpell.travelSpeed, GUILayout.Width(150));
        selectedSpell.cooldownTime = EditorGUILayout.FloatField("Cooldown:", selectedSpell.cooldownTime, GUILayout.Width(150));
        selectedSpell.recoveryTime = EditorGUILayout.FloatField("Recovery Time:", selectedSpell.recoveryTime, GUILayout.Width(150));
        selectedSpell.spellcastMotionIndex = EditorGUILayout.IntField("Cast Anim Idx:", selectedSpell.spellcastMotionIndex, GUILayout.Width(150));
        selectedSpell.releaseMotionIndex = EditorGUILayout.IntField("Release Anim Idx:", selectedSpell.releaseMotionIndex, GUILayout.Width(150));
        selectedSpell.effectDiameter = EditorGUILayout.FloatField("Effect Diameter:", selectedSpell.effectDiameter, GUILayout.Width(150));
        selectedSpell.effectRange = EditorGUILayout.FloatField("Effect Range:", selectedSpell.effectRange, GUILayout.Width(150));

        srlSpell.ApplyModifiedProperties();
        EditorGUILayout.EndVertical();
    }

    private void DrawMagicEffectInspector()
    {
        selectedMagicEffect = (MagicEffect)selectedElement;
        SerializedObject srlMagicEffect = new SerializedObject(selectedMagicEffect);

        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.BeginVertical();
        EditorGUIUtility.labelWidth = 120;

        EditorGUILayout.BeginHorizontal();

        EditorGUI.BeginChangeCheck();
        selectedMagicEffect.name = EditorGUILayout.TextField("Asset Name:", selectedMagicEffect.name, GUILayout.Width(270));
        if(EditorGUI.EndChangeCheck())
        {
            AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(selectedMagicEffect), selectedMagicEffect.name);
        }
        EditorGUILayout.EndHorizontal();

        selectedMagicEffect.projectileType = (ProjectileType)EditorGUILayout.EnumPopup("Projectile Type: ", selectedMagicEffect.projectileType, GUILayout.Width(300));
        selectedMagicEffect.affectedAttribute = (AffectedAttribute)EditorGUILayout.EnumPopup("Affected Attribute: ", selectedMagicEffect.affectedAttribute, GUILayout.Width(300));

        selectedMagicEffect.destroyOnImpact = EditorGUILayout.Toggle("Destroy On Impact: ", selectedMagicEffect.destroyOnImpact);

        GUILayout.Space(5);
        GUILayout.Label("VFX", EditorStyles.boldLabel, GUILayout.Width(200));

        EditorGUILayout.BeginHorizontal();
        selectedMagicEffect.id_VFXProjectile = EditorGUILayout.TextField("VFX Projectile ID:", selectedMagicEffect.id_VFXProjectile, GUILayout.Width(300));
        if(EditorGUILayout.DropdownButton(new GUIContent("..."), FocusType.Passive, GUILayout.Width(50)))
        {
            vfxID = VFXID.Projectile;
            vfxSelectionMenu.ShowAsContext();
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        selectedMagicEffect.id_VFXOnHit = EditorGUILayout.TextField("VFX OnHit ID:", selectedMagicEffect.id_VFXOnHit, GUILayout.Width(300));
        if(EditorGUILayout.DropdownButton(new GUIContent("..."), FocusType.Passive, GUILayout.Width(50)))
        {
            vfxID = VFXID.OnHit;
            vfxSelectionMenu.ShowAsContext();
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        selectedMagicEffect.id_VFXHandIdle = EditorGUILayout.TextField("VFX Hand Idle ID:", selectedMagicEffect.id_VFXHandIdle, GUILayout.Width(300));
        if(EditorGUILayout.DropdownButton(new GUIContent("..."), FocusType.Passive, GUILayout.Width(50)))
        {
            vfxID = VFXID.OnHandIdle;
            vfxSelectionMenu.ShowAsContext();
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        selectedMagicEffect.id_VFXHandCharge = EditorGUILayout.TextField("VFX Charge ID:", selectedMagicEffect.id_VFXHandCharge, GUILayout.Width(300));
        if(EditorGUILayout.DropdownButton(new GUIContent("..."), FocusType.Passive, GUILayout.Width(50)))
        {
            vfxID = VFXID.OnHandCharge;
            vfxSelectionMenu.ShowAsContext();
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        selectedMagicEffect.id_VFXHandRelease = EditorGUILayout.TextField("VFX Release ID:", selectedMagicEffect.id_VFXHandRelease, GUILayout.Width(300));
        if(EditorGUILayout.DropdownButton(new GUIContent("..."), FocusType.Passive, GUILayout.Width(50)))
        {
            vfxID = VFXID.OnHandRelease;
            vfxSelectionMenu.ShowAsContext();
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        selectedMagicEffect.id_VFXHandRelease = EditorGUILayout.TextField("VFX Release ID:", selectedMagicEffect.id_VFXHandRelease, GUILayout.Width(300));
        if(EditorGUILayout.DropdownButton(new GUIContent("..."), FocusType.Passive, GUILayout.Width(50)))
        {
            vfxID = VFXID.OnImpact;
            vfxSelectionMenu.ShowAsContext();
        }
        EditorGUILayout.EndHorizontal();

        GUILayout.Space(5);
        GUILayout.Label("SFX", EditorStyles.boldLabel, GUILayout.Width(200));

        selectedMagicEffect.impactSFXType = (WeaponImpactType)EditorGUILayout.EnumPopup("Impact SFX Type: ", selectedMagicEffect.impactSFXType, GUILayout.Width(300));
        SerializedProperty sOnHitSFX = srlMagicEffect.FindProperty("sfxOnHit");
        EditorGUILayout.PropertyField(sOnHitSFX, true);

        srlMagicEffect.ApplyModifiedProperties();
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();
    }

    #endregion End Draw Methods

    private void SelectElement(object selectedType)
    {
        if(selectedType is Item)
        {
            selectedElementIndex = 0;
        }
        if(selectedType is Spell)
        {
            selectedElementIndex = 1;
        }
        if(selectedType is MagicEffect)
        {
            selectedElementIndex = 2;
        }

        selectedElement = selectedType;

    }

    private void SetMagicEffectVFXID(VFXID id, string newName)
    {
        switch(id)
        {
            case VFXID.Projectile:
                selectedMagicEffect.id_VFXProjectile = newName;
                break;
            case VFXID.OnHit:
                selectedMagicEffect.id_VFXOnHit = newName;
                break;
            case VFXID.OnHandIdle:
                selectedMagicEffect.id_VFXHandIdle = newName;
                break;
            case VFXID.OnHandCharge:
                selectedMagicEffect.id_VFXHandCharge = newName;
                break;
            case VFXID.OnHandRelease:
                selectedMagicEffect.id_VFXHandRelease = newName;
                break;
            case VFXID.OnImpact:
                selectedMagicEffect.id_VFXOnImpact = newName;
                break;
        }

        AssetDatabase.Refresh();
        EditorUtility.SetDirty(selectedMagicEffect);
        AssetDatabase.SaveAssets();
    }

    private void LoadDatabase()
    {
        List<Item> items = ItemDatabase.GetItemsFromJSON();

        foreach(ItemCategory c in itemDatabase.itemCategories)
        {
            c.items = new List<Item>();
        }

        foreach(Item item in items)
        {
            switch(item.itemCategoryType)
            {
                case ItemCategoryType.Armor:
                    //Ammo ammo = (Ammo)item;
                    item.maxStackSize = 1;
                    break;
                case ItemCategoryType.Ammo:
                    //Ammo ammo = (Ammo)item;
                    item.maxStackSize = 20;
                    break;
                case ItemCategoryType.Weapon:
                    item.maxStackSize = 1;
                    break;
                case ItemCategoryType.Valuable:
                    item.maxStackSize = 10;
                    break;
            }

            itemDatabase.itemCategories.Where(c => c.itemCategoryType == item.itemCategoryType).FirstOrDefault().items
                .Add(item);
        }
        //items.AddRange(item.items);
        //AssetDatabase.Refresh();
    }

    private void RefreshVoicesetDatabaseEntries()
    {

        var items = voiceSetDatabase.characterVoiceSets.Select(c => c.voiceSet).ToList();
        items.AddRange(voiceSetDatabase.creatureVoiceSets.Select(c => c.voiceSet));
        voicesetSelectionDropDown = new GenericMenu();

        for(var i = 0; i < items.Count; i++)
        {
            var item = items[i];
            var name = item.Name;
            voicesetSelectionDropDown.AddItem(new GUIContent(name), false,
                () => selectedActor.VoicesetID = name);
        }
    }

    private bool Foldout(ItemCategory cat)
    {
        string prefKey = "IDB Custom Foldout." + cat.Name;
        bool foldoutState = EditorPrefs.GetBool(prefKey, false);
        //GUIStyle style = new GUIStyle(EditorStyles.foldout);
        EditorGUIUtility.labelWidth = 80;

        bool newFoldoutState = EditorGUILayout.Foldout(foldoutState,
            cat.Name + " (" + (cat.items == null ? 0 : cat.items.Count) + ")" /*, _dbSkin.GetStyle("foldout")*/ /*, style*/);
        if(newFoldoutState != foldoutState)
        {
            EditorPrefs.SetBool(prefKey, newFoldoutState);
        }

        return newFoldoutState;
    }

    private bool Foldout(string actorType, int count, string prefKeyAddition = "")
    {
        string prefKey = "IDB Custom Foldout." + actorType + prefKeyAddition;
        bool foldoutState = EditorPrefs.GetBool(prefKey, false);
        //GUIStyle style = new GUIStyle(EditorStyles.foldout);
        EditorGUIUtility.labelWidth = 80;

        bool newFoldoutState = EditorGUILayout.Foldout(foldoutState,
            actorType + " (" + count + ")" /*, _dbSkin.GetStyle("foldout")*/ /*, style*/);
        if(newFoldoutState != foldoutState)
        {
            EditorPrefs.SetBool(prefKey, newFoldoutState);
        }

        return newFoldoutState;
    }

    private bool Foldout(string cat)
    {
        string prefKey = "IDB Custom Foldout." + cat;
        bool foldoutState = EditorPrefs.GetBool(prefKey, false);
        //GUIStyle style = new GUIStyle(EditorStyles.foldout);
        EditorGUIUtility.labelWidth = 80;
        bool newFoldoutState = EditorGUILayout.Foldout(foldoutState,
            cat /*+ " (" + cat.items.Count + ")"*/ /*, _dbSkin.GetStyle("foldout")*/ /*, style*/);
        if(newFoldoutState != foldoutState)
        {
            EditorPrefs.SetBool(prefKey, newFoldoutState);
        }

        return newFoldoutState;
    }

    public static void PlayClip(AudioClip clip, int startSample = 0, bool loop = false)
    {
        EditorSFX.PlayClip(clip, startSample, loop);
        //System.Reflection.Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;
        //System.Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
        //System.Reflection.MethodInfo method = audioUtilClass.GetMethod(
        //    "PlayClip",
        //    System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public,
        //    null,
        //    new System.Type[] { typeof(AudioClip), typeof(int), typeof(bool) },
        //    null
        //);
        //method.Invoke(
        //    null,
        //    new object[] { clip, startSample, loop }
        //);
    }

    private void PingFolder(string path)
    {

        // Check the path has no '/' at the end, if it dose remove it,
        // Obviously in this example it doesn't but it might
        // if your getting the path some other way.

        //if(path[path.Length - 1] == '/')
        //    path = path.Substring(0, path.Length - 1);
        EditorUtility.FocusProjectWindow();
        // Load object

        // Select the object in the project folder
        Selection.activeObject = AssetDatabase.LoadAssetAtPath(path, typeof(UnityEngine.Object));
        EditorGUIUtility.PingObject(Selection.activeObject);
    }
}