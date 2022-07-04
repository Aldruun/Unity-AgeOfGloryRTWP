using GenericFunctions;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class ResourceManager
{
    private const string PATH_SFX_UISFX = "SFX/UI SFX/";

    private const string PATH_SFX_AGENTSFX = "SFX/Actor SFX/";

    //const string PATH_PREFABS_CHARACTERS = "Prefabs/Characters/";
    private const string PATH_PREFABS_TOOLS = "Prefabs/Tools/";
    private const string PATH_PREFABS_UI = "Prefabs/UI/";

    ///////////////////////////////////////////////////
    // Shared ScriptableObjects
    ///////////////////////////////////////////////////
    
    //public static ActorSkillDatabase actorSkillDatabase;
    public static VoiceSetDatabase voiceSetDatabase;
    public static Quest[] questRefs;

    ///////////////////////////////////////////////////
    // Art Resources
    ///////////////////////////////////////////////////
    private static Sprite[] _potraitSprites;
    public static Sprite cursor_default;
    public static Sprite cursor_grab;
    public static Sprite cursor_blocked;
    public static Sprite cursor_attack;
    public static Sprite cursor_castspell;
    public static Sprite cursor_use;
    public static Sprite cursor_pickup;
    public static Sprite cursor_resizevertical;
    public static Sprite cursor_resizehorizontal;
    private static Sprite[] itemSprites;

    ///////////////////////////////////////////////////
    // SFX
    ///////////////////////////////////////////////////
    private static List<Item> _items = new();

    // GameObject Prefabs
    public static GameObject agentRef_f;
    public static GameObject agentRef_m;

    public static GameObject prefab_pickaxe;
    public static GameObject prefab_axe;
    public static GameObject prefab_fishingrod;

    // Text files
    public static string[] nicknames_female;

    public static string[] nicknames_male;
    public static string[] nicknames;

    public static GameObject[] actorRefs;
  
    // Dialog
    public static GameObject prefab_dialogchoice;

    //3D Indicatiors
    public static GameObject indicator_aoeprojector;
    public static GameObject indicator_aimprojector;

    // UI Prefabs
    public static GameObject prefab_uitextpopup;
    public static GameObject prefab_uispritepopup;
    public static GameObject prefab_agentInfo;
    public static GameObject prefab_actorportrait;

    ///////////////////////////////////////////////////
    // SFX
    ///////////////////////////////////////////////////
    // UI Sounds
    public static AudioClip sfx_ui_inventory_show;

    public static AudioClip sfx_ui_inventory_hide;
    public static AudioClip sfx_ui_inventory_liftitem;

    // Notification Sounds
    public static AudioClip sfx_notify_queststarted;

    public static AudioClip sfx_notify_questcompleted;

    // Weapon SFX
    //public static AudioClip sfx_SoftImpact;
    public static AudioClip sfx_sword_draw;

    public static AudioClip sfx_sword_sheath;
    public static AudioClip[] sfx_list_bow_draw;
    public static AudioClip[] sfx_bow_arrowNock;
    public static AudioClip[] sfx_bow_drawString;
    public static AudioClip[] sfx_bow_releasestring;
    public static AudioClip[] sfx_list_bow_holster;

    // Spell SFX
    public static AudioClip sfx_spell_charge_default;
    public static AudioClip sfx_spell_draw_default;
    public static AudioClip[] sfx_list_spellchants_fire_f;
    public static AudioClip[] sfx_list_spellchants_fire_m;
    public static AudioClip[] sfx_list_spellchants_heal_f;
    public static AudioClip[] sfx_list_spellchants_heal_m;

    // Inventory SFX
    public static AudioClip[] sfx_list_dropitem;

    public static AudioClip sfx_default_storeitem;
    public static AudioClip[] sfx_list_watersplashes;

    // Agent Interact SFX
    public static AudioClip sfx_notify_levelup;

    public static AudioClip sfx_notify_collectcoin;
    public static AudioClip sfx_container_locked;
    public static AudioClip sfx_container_open;
    public static AudioClip sfx_container_close;
    public static AudioClip sfx_default_operate;
    public static AudioClip[] sfx_list_mining;
    public static AudioClip[] sfx_list_woodcutting;
    public static AudioClip[] sfx_list_equiplightarmor;
    public static AudioClip[] sfx_list_equipclothing;
    public static AudioClip[] sfx_jumpGruntSounds_m;
    public static AudioClip[] sfx_jumpGruntSounds_f;
    public static AudioClip[] sfx_swallowSounds;
    public static AudioClip[] sfx_chewSounds;
    public static AudioClip[] sfx_healSounds;
    public static AudioClip[] sfx_list_hits_blade;
    public static AudioClip[] sfx_list_hits_arrow;
    public static AudioClip[] sfx_list_hits_magic;
    public static AudioClip[] sfx_list_hits_fire;
    public static AudioClip[] sfx_list_hits_frost;
    public static AudioClip[] sfx_list_hits_lightning;
    public static AudioClip[] sfx_list_hits_holy;
    public static AudioClip[] sfx_list_hits_unholy;
    public static AudioClip[] sfx_list_hits_poison;
    public static AudioClip[] sfx_list_unarmedAttackSounds;
    public static AudioClip[] sfx_list_hits_creature;
    public static AudioClip[] sfx_list_swings_sword;
    public static AudioClip[] sfx_list_blocks_sword;
    public static AudioClip sfx_blocked_critical;
    public static AudioClip[] swordHitsOnFleshSounds;

    public static AudioClip[] FootstepSoundsStone;
    public static AudioClip[] FootstepSoundsStoneFemale;
    public static AudioClip[] FootstepSoundsWood;
    public static AudioClip[] FootstepSoundsDirt;
    public static AudioClip[] FootstepSoundsGrass;
    public static AudioClip[] FootstepSoundsGlass;
    public static AudioClip[] FootstepSoundsCarpet;
    public static AudioClip[] FootstepSoundsWater;
    public static AudioClip[] FootstepSoundsBarefoot;

    public static AudioClip[] FootstepSoundsMetalRustling;

    public static void LoadResources()
    {
        //actorSkillDatabase = Resources.Load<ActorSkillDatabase>("ScriptableObjects/ActorSkillDatabase");
        //if(actorSkillDatabase == null)
        //{
        //    Debug.LogError("ActorSkillDatabase: Initilization failed -> ActorSkillDatabase was null");
        //    return;
        //}

        //Debug.Log("<color=green>// ActorSkillDatabase Loaded</color>");

        //actorSkillDatabase.CreateSkillTemplates(
        //    actorSkillDatabase.GetActorSkillsBySpecialization(Specialization.COMBAT),
        //    actorSkillDatabase.GetActorSkillsBySpecialization(Specialization.MAGIC),
        //    actorSkillDatabase.GetActorSkillsBySpecialization(Specialization.STEALTH)
        //    );

        

        voiceSetDatabase = Resources.Load<VoiceSetDatabase>("ScriptableObjects/VoiceSetDatabase");
        if(voiceSetDatabase == null)
        {
            UnityEngine.Debug.LogError("No VoiceSet Database ScriptableObject found");
            return;
        }

        // Actors
        // actorRefs = Resources.LoadAll<GameObject>(PATH_PREFABS_CHARACTERS);
        //agentRef_f = Resources.Load<GameObject>(PATH_PREFABS_CHARACTERS + "ref_human_female");
        //agentRef_m = Resources.Load<GameObject>(PATH_PREFABS_CHARACTERS + "ref_human_male");

        // ScriptableObjects
        questRefs = Resources.LoadAll<Quest>("ScriptableObjects/Quests");
        //actionSetRefs = Resources.LoadAll<ActionSet>("ScriptableObjects/Action Sets");

        // Tools
        prefab_pickaxe = Resources.Load<GameObject>(PATH_PREFABS_TOOLS + "pickaxe");
        prefab_axe = Resources.Load<GameObject>(PATH_PREFABS_TOOLS + "axe");
        prefab_fishingrod = Resources.Load<GameObject>(PATH_PREFABS_TOOLS + "fishingrod");

        // UI
        prefab_dialogchoice = Resources.Load<GameObject>("Prefabs/Dialog Choice");
        prefab_uitextpopup = Resources.Load<GameObject>("Prefabs/UI Popup Text");
        prefab_uispritepopup = Resources.Load<GameObject>("Prefabs/UI Popup Sprite");
        prefab_agentInfo = Resources.Load<GameObject>(PATH_PREFABS_UI + "agentinfo");
        prefab_actorportrait = Resources.Load<GameObject>("Prefabs/GFX/actorportrait");

        //actors_Rebels = Resources.LoadAll<GameObject>("Prefabs/Actors/Rebels");
        //actors_Monsters = Resources.LoadAll<GameObject>("Prefabs/Actors/Monsters");
        //actors_Police = Resources.LoadAll<GameObject>("Prefabs/Actors/Police");
        //actors_Underground = Resources.LoadAll<GameObject>("Prefabs/Actors/Underground");

        // UI Sounds

        sfx_ui_inventory_show = Resources.Load<AudioClip>(PATH_SFX_UISFX + "sfx_ui_inventory_show");
        sfx_ui_inventory_hide = Resources.Load<AudioClip>(PATH_SFX_UISFX + "sfx_ui_inventory_hide");
        sfx_ui_inventory_liftitem = Resources.Load<AudioClip>(PATH_SFX_UISFX + "sfx_ui_inventory_liftitem");
        sfx_list_dropitem = Resources.LoadAll<AudioClip>(PATH_SFX_AGENTSFX + "Drop");

        // Notification Sounds
        sfx_notify_queststarted = Resources.Load<AudioClip>("SFX/Notification SFX/sfx_notify_queststarted");
        sfx_notify_questcompleted = Resources.Load<AudioClip>("SFX/Notification SFX/sfx_notify_questcompleted");

        // Weapon SFX
        sfx_sword_draw = Resources.Load<AudioClip>("SFX/Weapon SFX/Sword/Draw");
        sfx_sword_sheath = Resources.Load<AudioClip>("SFX/Weapon SFX/Sword/Sheath");
        sfx_bow_arrowNock = Resources.LoadAll<AudioClip>("SFX/Weapon SFX/Bow/Arrow Nock");
        sfx_bow_drawString = Resources.LoadAll<AudioClip>("SFX/Weapon SFX/Bow/Draw String");
        sfx_bow_releasestring = Resources.LoadAll<AudioClip>("SFX/Weapon SFX/Bow/Release String");
        sfx_list_bow_draw = Resources.LoadAll<AudioClip>("SFX/Weapon SFX/Bow/Equip Bow");
        sfx_list_bow_holster = Resources.LoadAll<AudioClip>("SFX/Weapon SFX/Bow/Unequip Bow");
        sfx_list_equiplightarmor = Resources.LoadAll<AudioClip>(PATH_SFX_AGENTSFX + "Equip/Light Armor");
        sfx_list_equipclothing = Resources.LoadAll<AudioClip>(PATH_SFX_AGENTSFX + "Equip/Clothing");

        // Spell SFX
        sfx_spell_charge_default = Resources.Load<AudioClip>("SFX/Spell SFX/Charge/sfx_spell_charge_default");
        sfx_spell_draw_default = Resources.Load<AudioClip>("SFX/Spell SFX/Draw/spelldrawdefault");
        sfx_list_spellchants_fire_f = Resources.LoadAll<AudioClip>("SFX/Chants/F/Fire/");
        sfx_list_spellchants_fire_m = Resources.LoadAll<AudioClip>("SFX/Chants/M/Fire/");
        sfx_list_spellchants_heal_f = Resources.LoadAll<AudioClip>("SFX/Chants/F/Heal/");
        sfx_list_spellchants_heal_m = Resources.LoadAll<AudioClip>("SFX/Chants/M/Heal/");

        // Agent Interaction Sounds
        sfx_notify_levelup = Resources.Load<AudioClip>("SFX/Notifications/sfx_notify_levelup");
        sfx_notify_collectcoin = Resources.Load<AudioClip>("SFX/Notifications/sfx_notify_collectcoin");
        sfx_container_open = Resources.Load<AudioClip>("SFX/Interactable SFX/Container/sfx_container_open");
        sfx_container_close = Resources.Load<AudioClip>("SFX/Interactable SFX/Container/sfx_container_close");
        sfx_container_locked = Resources.Load<AudioClip>("SFX/Interactable SFX/Container/sfx_container_locked");
        sfx_default_storeitem = Resources.Load<AudioClip>("SFX/Item Sounds/generic_storeItem");
        sfx_list_watersplashes = Resources.LoadAll<AudioClip>(PATH_SFX_AGENTSFX + "Water Splashes");
        sfx_default_operate = Resources.Load<AudioClip>(PATH_SFX_AGENTSFX + "Operating/sfx_default_operate");
        sfx_list_mining = Resources.LoadAll<AudioClip>(PATH_SFX_AGENTSFX + "Operating/Mining");
        sfx_list_woodcutting = Resources.LoadAll<AudioClip>(PATH_SFX_AGENTSFX + "Operating/Wood Cutting");
        sfx_jumpGruntSounds_f = Resources.LoadAll<AudioClip>("SFX/Grunt Sounds/f/Jump Grunts");
        sfx_jumpGruntSounds_m = Resources.LoadAll<AudioClip>("SFX/Grunt Sounds/m/Jump Grunts");
        //sfx_swallowSounds = Resources.LoadAll<AudioClip>("SFX/Swallow Sounds");
        //sfx_chewSounds = Resources.LoadAll<AudioClip>("SFX/Chew Sounds");
        sfx_healSounds = Resources.LoadAll<AudioClip>("SFX/Heal Sounds");

        sfx_list_hits_blade = Resources.LoadAll<AudioClip>(PATH_SFX_AGENTSFX + "Hits/Blade");
        sfx_list_hits_arrow = Resources.LoadAll<AudioClip>(PATH_SFX_AGENTSFX + "Hits/Arrow");
        sfx_list_hits_magic = Resources.LoadAll<AudioClip>(PATH_SFX_AGENTSFX + "Hits/Magic");
        sfx_list_hits_fire = Resources.LoadAll<AudioClip>(PATH_SFX_AGENTSFX + "Hits/Fire");
        sfx_list_hits_frost = Resources.LoadAll<AudioClip>(PATH_SFX_AGENTSFX + "Hits/Frost");
        sfx_list_hits_lightning = Resources.LoadAll<AudioClip>(PATH_SFX_AGENTSFX + "Hits/Lightning");
        sfx_list_hits_holy = Resources.LoadAll<AudioClip>(PATH_SFX_AGENTSFX + "Hits/Fire");
        sfx_list_hits_unholy = Resources.LoadAll<AudioClip>(PATH_SFX_AGENTSFX + "Hits/Unholy");
        sfx_list_hits_poison = Resources.LoadAll<AudioClip>(PATH_SFX_AGENTSFX + "Hits/Poison");
        sfx_list_hits_creature = Resources.LoadAll<AudioClip>(PATH_SFX_AGENTSFX + "Hits/Creature");
        sfx_list_unarmedAttackSounds = Resources.LoadAll<AudioClip>(PATH_SFX_AGENTSFX + "Hits/Unarmed");
        sfx_list_swings_sword = Resources.LoadAll<AudioClip>(PATH_SFX_AGENTSFX + "Cafofo Swings/Sword");
        sfx_list_blocks_sword = Resources.LoadAll<AudioClip>(PATH_SFX_AGENTSFX + "Blocks");
        //sfx_blocked_critical = Resources.Load<AudioClip>(PATH_SFX_AGENTSFX + "Blocks/sfx_blocked_critical");
        //sfx_swordHitsOnFleshSounds = Resources.LoadAll<AudioClip>("SFX/Attack Sounds/Sword Hits/On Flesh");

        FootstepSoundsStone = Resources.LoadAll<AudioClip>(PATH_SFX_AGENTSFX + "Footsteps/Concrete Set 3");
        FootstepSoundsStoneFemale = Resources.LoadAll<AudioClip>(PATH_SFX_AGENTSFX + "Footsteps/Concrete Set 3");
        FootstepSoundsWood = Resources.LoadAll<AudioClip>(PATH_SFX_AGENTSFX + "Footsteps/Wood Set 1");
        FootstepSoundsDirt = Resources.LoadAll<AudioClip>(PATH_SFX_AGENTSFX + "Footsteps/Dirt");
        FootstepSoundsGrass = Resources.LoadAll<AudioClip>(PATH_SFX_AGENTSFX + "Footsteps/Grass");
        FootstepSoundsGlass = Resources.LoadAll<AudioClip>(PATH_SFX_AGENTSFX + "Footsteps/Glass");
        //FootstepSoundsCarpet = Resources.LoadAll<AudioClip>(PATH_SFX_AgentSFX + "Footsteps/Carpet");
        FootstepSoundsWater = Resources.LoadAll<AudioClip>(PATH_SFX_AGENTSFX + "Footsteps/Water Set 2");
        FootstepSoundsBarefoot = Resources.LoadAll<AudioClip>(PATH_SFX_AGENTSFX + "Footsteps/Barefoot");
        FootstepSoundsMetalRustling =
            Resources.LoadAll<AudioClip>(PATH_SFX_AGENTSFX + "Footsteps/Metal Armor Rustling");

        ItemDatabase.LoadResources();
        _items = ItemDatabase.itemTemplates;
        //SpellDatabase.LoadResources();

        //voiceSets = AudioDatabase.LoadVoiceSets();

        itemSprites = Resources.LoadAll<Sprite>("Images/UI Sprites/Icons");
        //portraitsprites = new Dictionary<Race, Sprite[]>() {
        //    {Race.HUMAN, Resources.LoadAll<Sprite>("Images/UI Sprites/Portraits/POE/Human")}
        //{Race.ELF, Resources.LoadAll<Sprite>("Images/UI Sprites/Portraits/Elf")}
        //};
        //3D Indicators
        indicator_aoeprojector = Resources.Load<GameObject>("Prefabs/Projectors/Projector AOE");
        indicator_aimprojector = Resources.Load<GameObject>("Prefabs/Projectors/Projector Aim");

        cursor_default = Resources.Load<Sprite>("Images/UI Sprites/Cursors/cursor_default");
        cursor_blocked = Resources.Load<Sprite>("Images/UI Sprites/Cursors/cursor_blocked");
        cursor_grab = Resources.Load<Sprite>("Images/UI Sprites/Cursors/cursor_grab");
        cursor_castspell = Resources.Load<Sprite>("Images/UI Sprites/Cursors/cursor_castspell");
        cursor_attack = Resources.Load<Sprite>("Images/UI Sprites/Cursors/cursor_attack");
        cursor_use = Resources.Load<Sprite>("Images/UI Sprites/Cursors/cursor_use");
        cursor_pickup = Resources.Load<Sprite>("Images/UI Sprites/Cursors/cursor_pickup");
        //Sprite[] talkCursors = Resources.LoadAll<Sprite>("Images/UI Sprites/Cursors/cursor_talk_sheet");
        //cursor_talk = talkCursors[0];
        //cursor_talk_click = talkCursors[1];
        cursor_resizevertical = Resources.Load<Sprite>("Images/UI Sprites/Cursors/cursor_resizevertical");
        cursor_resizehorizontal = Resources.Load<Sprite>("Images/UI Sprites/Cursors/cursor_resizehorizontal");

        // _potraitSprites = Resources.LoadAll<Sprite>("Images/UI Sprites/Portraits/POE/").Where(s => s.name[s.name.Length - 1] == 'm').ToArray();

        nicknames_female = Get.AllLinesFromTextfile("nicknames_female");
        nicknames_male = Get.AllLinesFromTextfile("nicknames_male");
        nicknames = Get.AllLinesFromTextfile("nicknames");
    }

    public static Item GetItem(string ID)
    {
        foreach(var item in _items)
            if(item.identifier == ID)
                return item;
        Debug.LogError("Item with ID '<color=yellow>" + ID + "</color>' not found in item database.");
        return null;
    }

    public static List<Item> GetItemOfType(ItemCategoryType itemType)
    {
        return _items.Where(i => i.itemCategoryType == itemType).ToList();
    }

    public static Sprite GetItemSprite(string spriteName)
    {
        for(var i = 0; i < itemSprites.Length; i++)
            if(itemSprites[i].name == spriteName)
                return itemSprites[i];

        Debug.LogError("ResourceManager: Icon sprite with name '" + spriteName + "' could not be found in 'Images/UI Sprites/Icons'");
        return null;
    }

    public static Sprite GetPortraitSprite(string spriteName)
    {
        return _potraitSprites.Where(p => p.name == spriteName).FirstOrDefault();
    }

    public static Sprite GetRandomPortraitSprite(Gender gender, Race race, Class actorClass)
    {
        Sprite[] sprites = Resources.LoadAll<Sprite>("Images/UI Sprites/Portraits/POE/" + gender + "/" + race + "/" + actorClass).Where(s => s.name[s.name.Length - 1] == 'm').ToArray();

        //foreach(Sprite sprite in sprites)
        //{
        //    if(sprite.name[sprite.name.Length - 1] != 'm') // 'm' in "sm" in the filename = small
        //    {
        //        continue;
        //    }
        //}

        if(sprites.Length == 0)
        {
            Debug.LogError("ResourceManager: No random portrait sprite found for '<color=white>" + gender + "</color>' '<color=white>" + race + "</color>' '<color=white>" + actorClass + "</color>'");
            return null;
        }

        return sprites[Random.Range(0, sprites.Length)];
    }

    public static string GetRandomSurname()
    {
        return nicknames[Random.Range(0, nicknames.Length)];
    }

    public static string GetRandomName(Gender gender)
    {
        return GetRandomNickname(gender) /*+ " " + GetRandomSurname()*/;
    }

    private static string GetRandomNickname(Gender gender)
    {
        var name = "";

        switch(gender)
        {
            case Gender.Female:
                name = nicknames_female[Random.Range(0, nicknames_female.Length)];
                break;

            case Gender.Male:
                name = nicknames_male[Random.Range(0, nicknames_male.Length)];
                break;
        }

        return name;
    }
}