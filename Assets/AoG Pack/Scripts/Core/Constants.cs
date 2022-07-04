public static class Constants
{
    public const int TGT_MAGE_ALL = 3;
    public const int TGT_CLERIC_ALL = 4;
    public const int TGT_DRUID_ALL = 5;
    public const int TGT_BARD_ALL = 6;
    public const int TGT_SORCERER = 7;

    public const int EQUIP_ANY = 0;
    public const int EQUIP_MELEE = 1;
    public const int EQUIP_RANGED = 2;
    public const int REQUIREDRANGE_MELEE = 3;
    public const int REQUIREDRANGE_RANGED = 10;

    //alignment values
    public const int AL_GE_MASK = 3;//good / evil
    public const int AL_GOOD = 1;
    public const int AL_GE_NEUTRAL = 2;
    public const int AL_EVIL = 3;
    public const int AL_LC_MASK = 0x30; //lawful / chaotic
    public const int AL_LAWFUL = 0x10;
    public const int AL_LC_NEUTRAL = 0x20;
    public const int AL_CHAOTIC = 0x30;
    public const int AL_LAWFUL_GOOD = (AL_LAWFUL | AL_GOOD);
    public const int AL_NEUTRAL_GOOD = (AL_LC_NEUTRAL | AL_GOOD);
    public const int AL_CHAOTIC_GOOD = (AL_CHAOTIC | AL_GOOD);
    public const int AL_LAWFUL_NEUTRAL = (AL_LAWFUL | AL_GE_NEUTRAL);
    public const int AL_TRUE_NEUTRAL = (AL_LC_NEUTRAL | AL_GE_NEUTRAL);
    public const int AL_CHAOTIC_NEUTRAL = (AL_CHAOTIC | AL_GE_NEUTRAL);
    public const int AL_LAWFUL_EVIL = (AL_LAWFUL | AL_EVIL);
    public const int AL_NEUTRAL_EVIL = (AL_LC_NEUTRAL | AL_EVIL);
    public const int AL_CHAOTIC_EVIL = (AL_CHAOTIC | AL_EVIL);

    public const int MAX_TRAVELING_DISTANCE = 200;

    public const int GF_HAS_KAPUTZ = 0; //pst
    public const int GF_ALL_STRINGS_TAGGED = 1; //bg1, pst, iwd1
    public const int GF_HAS_SONGLIST = 2; //bg2
    public const int GF_TEAM_MOVEMENT = 3; //pst
    public const int GF_UPPER_BUTTON_TEXT = 4; //bg2
    public const int GF_LOWER_LABEL_TEXT = 5; //bg2
    public const int GF_HAS_PARTY_INI = 6; //iwd2
    public const int GF_SOUNDFOLDERS = 7; //iwd2
    public const int GF_IGNORE_BUTTON_FRAMES = 8; // all?
    public const int GF_ONE_BYTE_ANIMID = 9; // pst
    public const int GF_HAS_DPLAYER = 10; // not pst
    public const int GF_HAS_EXPTABLE = 11; // iwd, iwd2
    public const int GF_HAS_BEASTS_INI = 12; //pst; also for quests.ini
    public const int GF_HAS_DESC_ICON = 13; //bg
    public const int GF_HAS_PICK_SOUND = 14; //pst
    public const int GF_IWD_MAP_DIMENSIONS = 15; //iwd, iwd2
    public const int GF_AUTOMAP_INI = 16; //pst
    public const int GF_SMALL_FOG = 17; //bg1, pst
    public const int GF_REVERSE_DOOR = 18; //pst
    public const int GF_PROTAGONIST_TALKS = 19; //pst
    public const int GF_HAS_SPELLLIST = 20; //iwd2
    public const int GF_IWD2_SCRIPTNAME = 21; //iwd2, iwd, how
    public const int GF_DIALOGUE_SCROLLS = 22; //pst
    public const int GF_KNOW_WORLD = 23; //iwd2
    public const int GF_REVERSE_TOHIT = 24; //all except iwd2
    public const int GF_SAVE_FOR_HALF = 25; //pst
    public const int GF_CHARNAMEISGABBER = 26; //iwd2
    public const int GF_MAGICBIT = 27; //iwd, iwd2
    public const int GF_CHECK_ABILITIES = 28; //bg2 (others?)
    public const int GF_CHALLENGERATING = 29; //iwd2
    public const int GF_SPELLBOOKICONHACK = 30; //bg2
    public const int GF_ENHANCED_EFFECTS = 31; //iwd2 (maybe iwd/how too)
    public const int GF_DEATH_ON_ZERO_STAT = 32; //not in iwd2
    public const int GF_SPAWN_INI = 33; //pst, iwd, iwd2
    public const int GF_IWD2_DEATHVARFORMAT = 34; //iwd branch (maybe pst)
    public const int GF_RESDATA_INI = 35; //pst
    public const int GF_OVERRIDE_CURSORPOS = 36; //pst, iwd2
    public const int GF_BREAKABLE_WEAPONS = 37; //only bg1
    public const int GF_3ED_RULES = 38; //iwd2
    public const int GF_LEVELSLOT_PER_CLASS = 39; //iwd2
    public const int GF_SELECTIVE_MAGIC_RES = 40; //bg2, iwd2, (how)
    public const int GF_HAS_HIDE_IN_SHADOWS = 41; // bg2, iwd2
    public const int GF_AREA_VISITED_VAR = 42; //iwd, iwd2
    public const int GF_PROPER_BACKSTAB = 43; //bg2, iwd2, how?
    public const int GF_ONSCREEN_TEXT = 44; //pst
    public const int GF_SPECIFIC_DMG_BONUS = 45; //how, iwd2
    public const int GF_STRREF_SAVEGAME = 46; //iwd2
    public const int GF_SIMPLE_DISRUPTION = 47; // ToBEx: simplified disruption
    public const int GF_BIOGRAPHY_RES = 48; //iwd branch
    public const int GF_NO_BIOGRAPHY = 49; //pst
    public const int GF_STEAL_IS_ATTACK = 50; //bg2 for sure
    public const int GF_CUTSCENE_AREASCRIPTS = 51; //bg1, maybe more
    public const int GF_FLEXIBLE_WMAP = 52; //iwd
    public const int GF_AUTOSEARCH_HIDDEN = 53; //all except iwd2
    public const int GF_PST_STATE_FLAGS = 54; //pst complicates this
    public const int GF_NO_DROP_CAN_MOVE = 55; //bg1
    public const int GF_JOURNAL_HAS_SECTIONS = 56; //bg2
    public const int GF_CASTING_SOUNDS = 57; //all except pst and bg1
    public const int GF_CASTING_SOUNDS2 = 58; //bg2
    public const int GF_FORCE_AREA_SCRIPT = 59; //how and iwd2 (maybe iwd1)
    public const int GF_AREA_OVERRIDE = 60; //pst maze and other hardcode
    public const int GF_NO_NEW_VARIABLES = 61; //pst
    public const int GF_SOUNDS_INI = 62; //iwd/how/iwd2
    public const int GF_USEPOINT_400 = 63; //all except pst and iwd2
    public const int GF_USEPOINT_200 = 64; //iwd2
    public const int GF_HAS_FLOAT_MENU = 65; //pst
    public const int GF_RARE_ACTION_VB = 66; //pst
    public const int GF_NO_UNDROPPABLE = 67; //iwd,how
    public const int GF_START_ACTIVE = 68; //bg1
    public const int GF_INFOPOINT_DIALOGS = 69; //pst, but only bg1 has garbage there
    public const int GF_IMPLICIT_AREAANIM_BACKGROUND = 70; //idw,how,iwd2
    public const int GF_HEAL_ON_100PLUS = 71; //bg1, bg2, pst
    public const int GF_IN_PARTY_ALLOWS_DEAD = 72; //all except bg2
    public const int GF_ZERO_TIMER_IS_VALID = 73; // how, not bg2, other unknown
    public const int GF_SHOP_RECHARGE = 74; // all?
    public const int GF_MELEEHEADER_USESPROJECTILE = 75; // minimally bg2
    public const int GF_FORCE_DIALOGPAUSE = 76; // all except if using v1.04 DLG files (bg2, special)
    public const int GF_RANDOM_BANTER_DIALOGS = 77; // bg1
    public const int GF_FIXED_MORALE_OPCODE = 78; // bg2
    public const int GF_HAPPINESS = 79; // all except pst and iwd2
    public const int GF_EFFICIENT_OR = 80; // does the OR trigger shortcircuit on success or not? Only in iwd2
}

[System.Flags]
public enum InternalFlags
{
    GIVEXP = 1,                           //give xp for this death
    JUSTDIED = 2,                           //Died() will return true
    FROMGAME = 4,                           //this is an NPC or PC
    REALLYDIED = 8,                          //real death happened, actor will be set to
    NORETICLE = 16,                           //draw reticle (target mark)
    NOINT = 32,                          //cannot interrupt the actions of this acto
    CLEANUP = 64,                           //actor died chunky death, or other total d
    RUNNING = 128,                          //actor is running
    RUNFLAGS = (RUNNING | NORETICLE | NOINT),
    BECAMEVISIBLE = 0x100,                      //actor just became visible (trigger ev
    INITIALIZED = 0x200,
    USEDSAVE = 0x400,                    //actor needed saving throws
    GOTAREA = 0x800,                          //actor already moved to an area
    USEEXIT = 0x1000,                     //
    INTRAP = 0x2000,                     //actor is currently in a trap (intrap 
    PST_WMAPPING = 0x8000,                      // trying to use the worldmap for trave

    ACTIVE = 0x10000,
    VISIBLE = 0x40000,
    ONCREATION = 0x80000,
    IDLE = 0x100000,
    PARTYRESTED = 0x200000,                    //party rested trigger event
    FORCEUPDATE = 0x400000,
    TRIGGER_AP = 0x800000,
    STOPATTACK = (JUSTDIED | REALLYDIED | CLEANUP | IDLE)
}