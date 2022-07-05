using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public static class GameEventSystem
{
    ////////////////////////////////
    // Player Control
    ////////////////////////////////
    public static Action<bool> RequestTogglePartyAI;
    public static Action<FormationType> SetFormation;
    public static Func<Vector3, byte> RequestFogOfWarValueAtPosition;

    ////////////////////////////////
    // Camera
    ////////////////////////////////
    public static Action<Vector3, float, float> RequestCameraShake; // triggerPosition, amount, duration

    ////////////////////////////////
    // PC Portrait UI
    ////////////////////////////////
    public static Action<int, GameAction> SetPortraitActionIcon;
    public static Action<int, bool> RequestHighlightPCPortrait;
    public static Action<Vector3> RequestCameraJumpToPosition;
    public static Action<Transform> RequestCameraFollowPC;
    public static Func<int, Transform, UIActorPortrait> RequestCreatePortrait; // PartyMemberIndex (1 - 6), Portrait Sprite, Actor Transform (For potrait click camera control)
    public static Action<int, Image> OnHeroPortraitAdded;
    public static Action<int> OnSetPCUnderCursorForGameControl; // PartyMemberIndex (1 - 6)
    public static Func<int, Actor> RequestGetPCByPartyIndex;

    ////////////////////////////////
    // UI
    ////////////////////////////////
    public static Action<bool> UIRequestShowAIToggleAsOn;
    public static Action<string> RequestShowTooltipAtMousePosition;
    public static Action<Vector3, Vector3, string> RequestShowTooltip;
    public static Action RequestHideTooltip;
    public static Action RequestCloseDialogueBox;
    public static Action<bool> OnSetTacticalPause;
    public static Func<List<Actor>> RequestSelectedPCs;
    public static Action<Actor, bool> OnPCSelectionStateChanged;
    ////////////////////////////////
    // GC
    ////////////////////////////////
    public static Action<GameObject> RequestAddGarbage;

    ////////////////////////////////
    // VFX
    ////////////////////////////////
    //internal static Action<Vector3, Vector3, DamageType> RequestPlayVFXOnHit; // Position, Direction, Type

    ////////////////////////////////
    // Audio
    ////////////////////////////////
    //internal static Action<Vector3, DamageType> RequestPlaySoundOnHit; // Position, Type

    ////////////////////////////////
    // SceneManagement
    ////////////////////////////////
    public static Action<int> OnAreaTransition; // SceneIndex to give to the GameInitializer to load



    public static Action<Vector3> OnCombatBegin; // Center of battle start
    public static Action OnCombatEnd;
    public static Action OnRoundBegin;
    public static Action<int> OnRoundEnd;
    public static Action<float> RoundProgressHook;

    public static Action<GameObject> RequestDespawn;

    public static bool drawingSelectionRect;
    public static bool areaBlocked;
    public static bool FormationRotation;
    public static bool isAimingSpell;
    public static bool showFloatingInfo;
    //public static Action OnSpellAimingDone;
    //public static Action OnSpellAimingDone;
    public static System.Action<Actor> OnActorSpawned;
    public static System.Action<Actor> OnActorDied;
    public static System.Action<Actor> OnActorDespawned;
    public static System.Action<Actor> OnNumActorsChanged;
    public static System.Action<Actor> OnPlayerCreated;
    public static System.Action<Actor, Spell, UISpellButton> OnPlayerSpellButtonClicked;
    public static System.Action<Actor, QuestBoard> OnPlayerInteraction_Questboard;

    public static System.Action<PickupItem> OnPickUpItemSpawned;
    public static System.Action<PickupItem> OnPickUpItemCollected;

    public static System.Action<Quest> OnQuestAdded;
    public static System.Action<Quest> OnQuestStarted;
    public static System.Action<Quest> OnQuestRemoved;
    public static System.Action<List<Quest>> OnQuestReminder;
    
    public static System.Action ActorDebuggerOnActorChanged;

    //public static Action<bool, Transform> OnDialogueChoiceSelected;
    public static Action<bool> OnInventoryToggled;
    public static Action<string, int> OnPlayerItemAdded; // Item, quantity
    public static Action<string, int> OnPlayerItemRemoved; // Item identifier, quantity
    public static Action<string, int> OnPlayerDropItem; // Item identifier, quantity
    public static Action<string, int> RefreshUI_PlayerPickedUpItem;

    public static Action<Actor> OnHeroGameObjectDestroyed;
    // Popup
    public static Action<Vector3, string, float, float, float, Color?> OnRequestTextPopup;

    
    public static Action<Actor> OnPCSpawned;
    public static Action<int, bool> OnHeroPortraitClicked;
    public static Func<int> OnRequest_GetNumSelectedHeroes;
    public static Action<Actor> OnCharacterUIDeselected;
    public static Action<Actor> OnPCDied;

    public static Action<Actor> OnPartyMemberSelected; // PartyIndex
    public static Action<Actor> OnPartyMemberDeselected;
    public static Action<int> OnRequestChangeMovementSpeed; // movement speed index
    public static Action<Actor> RequestAddPlayerCompanion;
    public static Action<int> RequestRemovePortrait; // PartyMemberIndex (1 - 6)
    
    public static Action<Actor, int, DamageType> OnHeroHit;
    public static Action<Actor, int> OnHeroLevelUp;
    public static Action<Actor, int, int> OnGoldReceived; // receiver, gold, gems

    public static Action<Actor, Actor> OnMobDefeated; // victim, killer
    public static Action<Actor> OnCharacterObjectDestroyed;

    public static Action<Actor> OnPartyMemberAdded;
    public static Action<Actor> OnPartyMemberRemoved;

    internal static Action<string> RequestShowInteractionPopup;

    public static bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }
}