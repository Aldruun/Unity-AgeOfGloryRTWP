using AoG.Core;
using AoG.UI;
using GenericFunctions;
using System.Collections.Generic;
using UnityEngine;
using Debug = UnityEngine.Debug;

/// <summary>
///Camera Raycasting based functionality, which enables mouse interaction with actors
/// </summary>
public class SelectionManager
{
    public bool debug;

    //public static SelectionManager current;

    //public static List<ActorInput> selectedUnits;
    public List<Actor> PCsInSelRect;
    public static List<Actor> selected;

    public static Actor actorUnderCursor;

    public static Actor selectedNPC;
    public static Actor lastSelectedPC;

    public bool playSelectSound;
    public int agentOrderMouseKey = 1;
    public KeyCode addToSelectionKey = KeyCode.LeftShift;
    public KeyCode removeFromSelectionKey = KeyCode.LeftControl;

    public GameObject clickParticles;

    public float leftMouseButtonDownTime;

    private bool startedOverGUI;
    private Rect selectionRect;

    private Vector2 squareStartPos;
    private Vector2 squareEndPos;
    private Camera cameraMain;
    private RectTransform selectionBoxVisual;
    public SelectionManager(Camera camera, RectTransform selectionBoxVisual)
    {
        cameraMain = camera;
        this.selectionBoxVisual = selectionBoxVisual;
      
        PCsInSelRect = new List<Actor>();
        selected = new List<Actor>();
        selectionBoxVisual.gameObject.SetActive(false);
        GameEventSystem.RequestSelectedPCs = GetSelectedPCs;
        GameEventSystem.OnRequest_GetNumSelectedHeroes -= NumSelectedPCs;
        GameEventSystem.OnRequest_GetNumSelectedHeroes += NumSelectedPCs;
        //GameEventSystem.OnRequestChangeMovementSpeed -= SetMovementSpeedOfSelected;
        //GameEventSystem.OnRequestChangeMovementSpeed += SetMovementSpeedOfSelected;
        GameEventSystem.OnHeroPortraitClicked -= TogglePCSelectionState;
        GameEventSystem.OnHeroPortraitClicked += TogglePCSelectionState;
        GameEventSystem.OnHeroGameObjectDestroyed -= DeselectPC;
        GameEventSystem.OnHeroGameObjectDestroyed += DeselectPC;
        GameEventSystem.OnPartyMemberRemoved -= DeselectPC;
        GameEventSystem.OnPartyMemberRemoved += DeselectPC;
    }

    internal void Release()
    {
        GameEventSystem.OnRequest_GetNumSelectedHeroes -= NumSelectedPCs;
        GameEventSystem.OnHeroPortraitClicked -= TogglePCSelectionState;
        //GameEventSystem.OnRequestChangeMovementSpeed -= SetMovementSpeedOfSelected;
        GameEventSystem.OnHeroGameObjectDestroyed -= DeselectPC;
        GameEventSystem.OnPartyMemberRemoved -= DeselectPC;
    }

    internal void Update()
    {
        HandleSingleUnitHighlighting();

        if(GameEventSystem.isAimingSpell)
        {
            return;
        }

        if(Input.GetMouseButtonDown(0))
            startedOverGUI = UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject(-1);

        // If we are above a GUI element, bail out
        if(startedOverGUI)
        {
            if(Input.GetMouseButtonUp(0))
            {

                startedOverGUI = false;
            }

            return;
        }

        if(Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.A))
        {
            SelectPCs(GameInterface.Instance.GetCurrentGame().PCs);
            return;
        }

        HandleMarqueeSelection();

        // Manage agent selection
        if(Input.GetMouseButtonUp(0))
        {
            RaycastHit hitUnitClick;
            if(Physics.Raycast(cameraMain.ScreenPointToRay(Input.mousePosition), out hitUnitClick, 100, 1 << LayerMask.NameToLayer("Actors")))
            {
                if(debug)
                    Debug.Log("<color=orange>Click on actor</color>");
                Actor clickedAgent = hitUnitClick.collider.GetComponentInParent<Actor>();

                if(clickedAgent is Actor ac)
                {
                    if(ac.IsPlayer)
                    {
                        SelectPC(ac, /*ac.ActorRecord.faction == Faction.Bandits || ac.ActorRecord.faction == Faction.Monsters,*/ (Input.GetKey(KeyCode.LeftShift) == false));
                    }
                }
            }
        }
    }

    private List<Actor> GetSelectedPCs()
    {
        return selected;
    }

    //MonoPickupItem _lootUnderCursor
    //void HandleLootHighlighting()
    //{
    //    RaycastHit selectableHit;
    //    if(Physics.Raycast(GameStateManager.Instance.GetCameraScript().camera.ScreenPointToRay(Input.mousePosition), out selectableHit, maxSelectDistance, 1 << LayerMask.NameToLayer("Loot")))
    //    {
    //        MonoPickupItem loot = selectableHit.collider.GetComponentInParent<MonoPickupItem>();

    //        if(actorUnderCursor != null)
    //        {
    //            if(actorUnderCursor != newUnitUnderCursor)
    //            {
    //                actorUnderCursor.Unhighlight();
    //                //    actorUnderCursor.Highlight();
    //            }
    //        }
    //        actorUnderCursor = newUnitUnderCursor;
    //        //else
    //        newUnitUnderCursor.Highlight();
    //    }
    //    else
    //    {
    //        if(actorUnderCursor != null)
    //        {
    //            actorUnderCursor.Unhighlight();
    //        }

    //        actorUnderCursor = null;
    //    }

    //}

    private void HandleSingleUnitHighlighting()
    {
        RaycastHit selectableHit;
        if(Physics.Raycast(cameraMain.ScreenPointToRay(Input.mousePosition), out selectableHit, 100, 1 << LayerMask.NameToLayer("Actors")))
        {
            Actor newUnitUnderCursor = selectableHit.collider.GetComponentInParent<Actor>();

            HighlightUnit(newUnitUnderCursor);
        }
        else
        {
            HighlightUnit(null);
        }
    }

    public Container GetContainerAtMousePosition()
    {
        Container container = null;
        RaycastHit selectableHit;
        //Debug.Log("<color=orange>- - - - raycasting container</color>");
        if(Physics.Raycast(cameraMain.ScreenPointToRay(Input.mousePosition), out selectableHit, 100, 1 << LayerMask.NameToLayer("Containers")))
        {
            container = (Container)selectableHit.collider.GetComponent<HighlightableMonoObject>().GetHighlightable();
            //Debug.Log("<color=orange>- - - - hitting container</color>");
        }
        return container;
    }

    public Door GetDoorAtMousePosition()
    {
        Door door = null;
        RaycastHit selectableHit;
        //Debug.Log("<color=orange>- - - - raycasting door</color>");
        if(Physics.Raycast(cameraMain.ScreenPointToRay(Input.mousePosition), out selectableHit, 100, 1 << LayerMask.NameToLayer("Doors")))
        {
            door = (Door)selectableHit.collider.GetComponent<HighlightableMonoObject>().GetHighlightable();
            //Debug.Log("<color=orange>- - - - hitting door</color>");
        }
        return door;
    }

    public void HighlightUnit(Actor newUnitUnderCursor)
    {
        if(newUnitUnderCursor == null)
        {
            if(actorUnderCursor != null)
            {
                actorUnderCursor.ActorUI.Unhighlight();
                actorUnderCursor = null;
            }

            return;
        }

        if(actorUnderCursor != null)
        {
            if(actorUnderCursor != newUnitUnderCursor)
            {
                actorUnderCursor.ActorUI.Unhighlight();
            }
        }

        actorUnderCursor = newUnitUnderCursor;
        newUnitUnderCursor.ActorUI.Highlight();
    }

    public static void SelectPC(Actor actor, bool deselectOthers)
    {
        if(actor.dead)
        {
            Debug.LogError("Cannot select, actor dead");
            return;
        }

        if(deselectOthers == true)
        {
            DeselectAllPCs();
        }

        // If the clicked thing is not selected yet ...
        if(selected.Contains(actor) == false)
        {
            selectedNPC = null;
            actor.ActorUI.Select();
            selected.Add(actor);

            if(selected.Count == 1)
            {
                //actor.PlaySelectionSound();
            }

            GameEventSystem.OnPartyMemberSelected?.Invoke(actor);
            GameEventSystem.RequestHighlightPCPortrait?.Invoke(actor.PartySlot, true);
            lastSelectedPC = actor;
        }
    }

    private void SelectPCs(List<Actor> actors, bool deselectOthers = false)
    {

        if(actors.Count > 0 && deselectOthers)
        {

            DeselectAllPCs();
        }

        for(int i = 0; i < actors.Count; i++)
        {

            SelectPC(actors[i], false);
        }

        if(actors.Count > 0)
            GameEventSystem.UIRequestShowAIToggleAsOn(actors[actors.Count - 1].aiControlled); //TODO Update complete ui info of this actor
    }

    internal static void DeselectPC(Actor actor)
    {
        if(lastSelectedPC == actor)
        {
            lastSelectedPC = null;
        }

        if(selected.Contains(actor))
        {
            actor.ActorUI.Deselect();
            selected.Remove(actor);
        }
    }

    private void DeselectPCs(List<Actor> agents)
    {

        for(int i = 0; i < agents.Count; i++)
        {

            DeselectPC(agents[i]);
        }
    }

    public static void DeselectAllPCs()
    {
        foreach(Actor actor in selected.ToArray())
        {
            DeselectPC(actor);
        }
    }

    private void UnhighlightAllPCs()
    {
        foreach(Actor actor in selected)
        {
            actor.ActorUI.Unhighlight();
        }
    }

    private void TogglePCSelectionState(int partySlot, bool select)
    {
        Actor actor = GameEventSystem.RequestGetPCByPartyIndex?.Invoke(partySlot);
        if(select)
        {
            SelectPC(actor, Input.GetKey(KeyCode.LeftShift) == false);
        }
        else
        {
            DeselectPC(actor); 
        }

        GameEventSystem.OnPCSelectionStateChanged?.Invoke(actor, actor.aiControlled); //TODO Update all ui info of this actor
    }

    //List<ActorInput> GetVisibleUnits()
    //{

    //    List<ActorInput> actors = new List<ActorInput>();

    //    foreach(ActorInput actor in UIScript.RosterMembers())
    //    {

    //        if(actor.GetComponentInChildren<Renderer>().IsVisibleFrom(Camera.main))
    //        {

    //            actors.Add(actor);
    //        }
    //    }

    //    return actors;
    //}

    //public void SetMovementSpeedOfSelected(int speedIndex)
    //{
    //    foreach(ActorInput actor in selected)
    //    {
    //        Debug.Log("# Setting move state on selected");
    //        actor.Combat.Execute_SetMovementSpeed((MovementSpeed)speedIndex);
    //    }
    //}

    void HandleMarqueeSelection()
    {
        if(Input.GetMouseButtonDown(0))
        {
            squareStartPos = Input.mousePosition;
            selectionRect = new Rect();
        }

        if(Input.GetMouseButton(0))
        {
            squareEndPos = Input.mousePosition;
            DrawSelectionRectVisual();
            GetSelectionRectContent();
            HighlightPCsInSelectionRect();
        }

        if(Input.GetMouseButtonUp(0))
        {
            GameEventSystem.drawingSelectionRect = false;
            squareStartPos = Vector2.zero;
            squareEndPos = Vector2.zero;
            DrawSelectionRectVisual();
            if(Vector2.Distance(Input.mousePosition, squareStartPos) < 5)
            {
                return;
            }

            PCsInSelRect = new List<Actor>(GetPCsInsideSelRect());

            if(PCsInSelRect.Count > 0)
            {
                if(Input.GetKey(addToSelectionKey))
                {

                    SelectPCs(PCsInSelRect);
                }
                else if(Input.GetKey(removeFromSelectionKey))
                {

                    DeselectPCs(PCsInSelRect);
                }
                else
                {
                    SelectPCs(PCsInSelRect, true);
                }
                UnhighlightAllPCs();
            }
        }
    }

    void DrawSelectionRectVisual()
    {
        selectionBoxVisual.gameObject.SetActive(true);
        Vector2 boxStart = squareStartPos;
        Vector2 boxEnd = squareEndPos;

        Vector2 boxCenter = (boxStart + boxEnd) / 2;

        selectionBoxVisual.position = boxCenter;

        Vector2 boxSize = new Vector2(Mathf.Abs(boxStart.x - boxEnd.x), Mathf.Abs(boxStart.y - boxEnd.y));

        if(boxSize.x > 5 || boxSize.y > 5)
        {
            selectionBoxVisual.sizeDelta = boxSize;
            GameEventSystem.drawingSelectionRect = true;
        }
        else
        {
            selectionBoxVisual.sizeDelta = Vector2.zero;
        }
    }

    private void GetSelectionRectContent()
    {
        if(Input.mousePosition.x < squareStartPos.x) //! left
        {
            selectionRect.xMin = Input.mousePosition.x;
            selectionRect.xMax = squareStartPos.x;
        }
        else //! right
        {
            selectionRect.xMin = squareStartPos.x;
            selectionRect.xMax = Input.mousePosition.x;
        }

        if(Input.mousePosition.y < squareStartPos.y) //! down
        {
            selectionRect.yMin = Input.mousePosition.y;
            selectionRect.yMax = squareStartPos.y;
        }
        else //! up
        {
            selectionRect.yMin = squareStartPos.y;
            selectionRect.yMax = Input.mousePosition.y;
        }
    }

    Actor[] GetPCsInsideSelRect()
    {
        List<Actor> pcsInsideSelRect = new List<Actor>();

        foreach(Actor selectable in GameInterface.Instance.GetCurrentGame().PCs)
        {
            Vector3 screenPos = cameraMain.WorldToScreenPoint(selectable.transform.position);
            screenPos.z = 0;

            if(selectionRect.Contains(screenPos))
                pcsInsideSelRect.Add(selectable);
        }

        return pcsInsideSelRect.ToArray();
    }

    private int numInSelRect;
    void HighlightPCsInSelectionRect()
    {
        int currNumInSelRect = 0;
        List<ActorUI> pcsInsideSelRect = new List<ActorUI>();
        foreach(Actor selectable in GameInterface.Instance.GetCurrentGame().PCs)
        {
            Vector3 screenPos = cameraMain.WorldToScreenPoint(selectable.transform.position);
            screenPos.z = 0;

            if(selectionRect.Contains(screenPos))
            {
                currNumInSelRect++;
                pcsInsideSelRect.Add(selectable.ActorUI);
                selectable.ActorUI.Highlight();
            }
            else
            {
                selectable.ActorUI.Unhighlight();
            }
        }

        if(numInSelRect != currNumInSelRect)
        {
            numInSelRect = currNumInSelRect;
            foreach(ActorUI actorUI in pcsInsideSelRect)
            {
                actorUI.ResetHighlighting();
            }
        }
    }

    internal static int NumSelectedPCs()
    {
        if(selected.Count == 0)
        {
            return 0;
        }

        int count = 0;
        foreach(Actor pc in GameInterface.Instance.GetCurrentGame().PCs)
        {
            if(pc.dead)
            {
                continue;
            }

            if(pc.ActorUI.Selected)
                count++;
        }

        return count;
    }
}