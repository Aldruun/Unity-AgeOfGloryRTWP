﻿using AoG.Core;
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
    public List<ActorInput> PCsInSelRect;
    public static List<ActorInput> selected;

    public static ActorInput actorUnderCursor;

    public static ActorInput selectedNPC;
    public static ActorInput lastSelectedPC;

    public bool playSelectSound;
    public int agentOrderMouseKey = 1;
    public KeyCode addToSelectionKey = KeyCode.LeftShift;
    public KeyCode removeFromSelectionKey = KeyCode.LeftControl;

    public GameObject clickParticles;

    public float leftMouseButtonDownTime;

    private bool _startedOverGUI;
    //float _leftButtonDownTime;
    private float _lastClickTime = 0;
    private bool _doubleClicked;
    public float doubleClickCatchTime = 0.25f;

    //public static int numSelected;

    private Rect selectionRect;

    private Vector2 squareStartPos;
    private Vector2 squareEndPos;
    private Camera cameraMain;
    private RectTransform selectionBoxVisual;
    public SelectionManager(Camera camera, RectTransform selectionBoxVisual)
    {
        cameraMain = camera;
        this.selectionBoxVisual = selectionBoxVisual;
        //if(current != null && current != this)
        //{

        //    Debug.LogError("Only one instance allowed");
        //    Destroy(this);
        //}
        //else
        //    current = this;

        //selectedUnits = new List<ActorInput>();
        PCsInSelRect = new List<ActorInput>();
        selected = new List<ActorInput>();
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
            _startedOverGUI = UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject(-1);

        //_holdingShift = Input.GetKey(KeyCode.LeftShift);

        // If we are above a GUI element, bail out
        if(_startedOverGUI)
        {

            if(Input.GetMouseButtonUp(0))
            {

                _startedOverGUI = false;
            }
            //Debug.LogError("Started over UI");
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
                ActorInput clickedAgent = hitUnitClick.collider.GetComponentInParent<ActorInput>();

                // Calculate double click
                //if(Time.unscaledTime - _lastClickTime < doubleClickCatchTime)
                //{

                //SelectHeroes(GetVisibleAgents(), (Input.GetKey(addToSelectionKey) == false));
                //foreach(Agent agent in ResourceManager.agents) {

                //    if(agent.GetComponentInChildren<Renderer>().IsVisibleFrom(Camera.main)) {

                //        SelectAgent(clickedAgent);
                //    }
                //}
                //}

                //if(_doubleClicked == false)
                //{

                //DeselectAll();
                if(clickedAgent is ActorInput ac)
                {
                    if(ac.IsPlayer)
                    {
                        //UnitInfoPanel.ShowInfo(clickedAgent);
                        SelectPC(ac, /*ac.ActorRecord.faction == Faction.Bandits || ac.ActorRecord.faction == Faction.Monsters,*/ (Input.GetKey(KeyCode.LeftShift) == false));
                    }
                }
                //}
            }

            _lastClickTime = Time.unscaledTime;

            leftMouseButtonDownTime = 0;
        }
        if(Input.GetMouseButton(0))
        {
            //if((squareStartPos - Input.mousePosition).magnitude > 10)
            //{

            //}

            leftMouseButtonDownTime += Time.deltaTime;
        }

    }

    private List<ActorInput> GetSelectedPCs()
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
            ActorInput newUnitUnderCursor = selectableHit.collider.GetComponentInParent<ActorInput>();

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

    public void HighlightUnit(ActorInput newUnitUnderCursor)
    {
        if(newUnitUnderCursor == null)
        {
            if(actorUnderCursor != null)
                actorUnderCursor.ActorUI.Unhighlight();
            actorUnderCursor = null;
            return;
        }

        if(actorUnderCursor != null)
        {
            if(actorUnderCursor != newUnitUnderCursor)
            {
                //UIHandler.SetCursor(0);
                actorUnderCursor.ActorUI.Unhighlight();
                //    actorUnderCursor.Highlight();
            }
        }

        actorUnderCursor = newUnitUnderCursor;
        newUnitUnderCursor.ActorUI.Highlight();
    }

    private void SelectNPC(ActorInput npc)
    {
        selectedNPC = npc;
    }

    public static void SelectPC(ActorInput actor, bool deselectOthers)
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
            //Debug.Log("* Selecting agent '" + agent.gameObject.name + "'");
            //UnitInfoPanel.ShowInfo(actor);
            selectedNPC = null;
            actor.ActorUI.Select();
            selected.Add(actor);

            if(selected.Count == 1)
            {
                //actor.PlaySelectionSound();
            }

            GameEventSystem.OnPartyMemberSelected?.Invoke(actor);
            GameEventSystem.RequestHighlightPCPortrait?.Invoke(actor.PartyIndex, true);
            lastSelectedPC = actor;
        }
    }

    private void SelectPCs(List<ActorInput> actors, bool deselectOthers = false)
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

    internal static void DeselectPC(ActorInput actor)
    {
        if(lastSelectedPC == actor)
        {
            lastSelectedPC = null;
        }

        if(selected.Contains(actor))
        {
            //agent.Unhighlight();
            actor.ActorUI.Deselect();
            selected.Remove(actor);
            //Debug.Log("* Deselecting agent '" + agent.gameObject.name + "'");
            GameEventSystem.OnPartyMemberDeselected?.Invoke(actor);
            //UIHandler.HighlightPartymemberPortrait(actor.PartyIndex, false);
        }
        //else
        //{

        //    Debug.LogError("Can't deselect agent, because it was not selected");
        //}

        if(selected.Count == 0)
        {

            //UnitInfoPanel.Hide();
        }
    }

    private void DeselectPCs(List<ActorInput> agents)
    {

        for(int i = 0; i < agents.Count; i++)
        {

            DeselectPC(agents[i]);
        }
    }

    public static void DeselectAllPCs()
    {

        //Debug.Log("* Deselecting all");
        //UnitInfoPanel.Hide();

        foreach(ActorInput actor in selected.ToArray())
        {

            DeselectPC(actor);
        }
    }

    private void UnhighlightAllPCs()
    {
        foreach(ActorInput actor in selected)
        {
            actor.ActorUI.Unhighlight();
        }
    }

    private void TogglePCSelectionState(int partySlot, bool select)
    {
        ActorInput actor = GameEventSystem.RequestGetPCByPartyIndex?.Invoke(partySlot);
        if(select)
        {
            SelectPC(actor, Input.GetKey(KeyCode.LeftShift) == false);
        }
        else
            DeselectPC(actor);

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
    //Debug.Log("# Setting move state on selected");
    //actor.Combat.Execute_SetMovementSpeed((MovementSpeed)speedIndex);
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


            PCsInSelRect = new List<ActorInput>(GetPCsInsideSelRect());

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

    ActorInput[] GetPCsInsideSelRect()
    {
        List<ActorInput> pcsInsideSelRect = new List<ActorInput>();
        //Looping through all the selectables in our world (automatically added/removed through the Selectable OnEnable/OnDisable)
        foreach(ActorInput selectable in GameInterface.Instance.GetCurrentGame().PCs)
        {
            //If the screenPosition of the worldobject is within our selection bounds, we can add it to our selection
            Vector3 screenPos = cameraMain.WorldToScreenPoint(selectable.transform.position);
            screenPos.z = 0;

            if(selectionRect.Contains(screenPos))
                pcsInsideSelRect.Add(selectable);

        }

        return pcsInsideSelRect.ToArray();
        //}
    }

    private int numInSelRect;
    void HighlightPCsInSelectionRect()
    {
        int currNumInSelRect = 0;
        List<ActorUI> pcsInsideSelRect = new List<ActorUI>();
        foreach(ActorInput selectable in GameInterface.Instance.GetCurrentGame().PCs)
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
        foreach(ActorInput pc in GameInterface.Instance.GetCurrentGame().PCs)
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