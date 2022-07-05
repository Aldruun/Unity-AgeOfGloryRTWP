using AoG.Core;
using AoG.UI;
using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Profiling;
using FoW;

namespace AoG.Controls
{
    public enum ActorSelectionProcedure
    {
        NORMAL = 0x00,
        REPLACE = 0x01,
        QUIET = 0x02
    }

    public enum SpellActivationMode
    {
        None,
        InstantSelf,
        AimActor,
        AimLocation
    }

    public enum TargetMode
    {
        NONE,
        TALK,
        ATTACK,
        CAST,
        DEFEND,
        PICK,
        GRAB
    }

    [Flags]
    public enum ClickActionType
    {
        NONE = 100,
        STEALTH = 0,
        THIEVING = 1,
        CAST = 2,
        TALK = 7,
        DEFEND = 14,
        ATTACK = 15,
        SEARCH = 22,
    }

    public enum KeyMod
    {
        NONE,
        ALT,
        CTRLLEFT,
        CTRLRIGHT,
        SHIFTLEFT,
        SHIFTRIGHT
    }

    [Flags]
    public enum ScreenFlags
    {
        DISABLEMOUSE = 1,  //no mouse cursor
        CENTERONACTOR = 2, //
        ALWAYSCENTER = 4,
        GUIENABLED = 8,    //
        LOCKSCROLL = 16,   //don't scroll
        CUTSCENE = 32      //don't push new actions onto the action queue
    }

    public class GameControl
    {
        public bool debug;

        private DeliveryType _spellActivationMode;
        private SpellTargetProjector _aoeProjector;
        private SpellTargetProjector _aimProjector;
        private SelectionManager selectionManager;

        private Actor _caster;
        private Actor _target;
        private Spell _clickedSpell;
        private Collider[] _highlightedTargets;

        private ScreenFlags _screenFlags;

        private TargetMode _targetMode;
        private ClickActionType _clickActionType;
        private CursorType _currCursorType;

        private Actor portraitActorUnderCursor;
        public static int lastActorID;
        public static bool mouseDisabled;

        private bool MouseIsDown;
        Container overContainer;
        Door overDoor;
        private bool overInfoPoint;
        private bool overUI;

        private UISpellButton _lastPressedSpellButton;

        private Camera cameraMain;

        // Start is called before the first frame update
        public GameControl(Camera camera, RectTransform selectionBox)
        {
            cameraMain = camera;
            selectionManager = new SelectionManager(camera, selectionBox);
            _aoeProjector = UnityEngine.Object.Instantiate(ResourceManager.indicator_aoeprojector).GetComponent<SpellTargetProjector>();
            _aimProjector = UnityEngine.Object.Instantiate(ResourceManager.indicator_aimprojector).GetComponent<SpellTargetProjector>();
            _aoeProjector.Init();
            _aimProjector.Init();

            GameEventSystem.OnPlayerSpellButtonClicked = HandleSpellButtonPress;
            GameEventSystem.OnSetPCUnderCursorForGameControl = SetPortraitActorUnderCursor;
            _highlightedTargets = new Collider[10];
        }

        public void Release()
        {
            GameEventSystem.OnPlayerSpellButtonClicked = null;
        }

        public void Update()
        {
            overUI = UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject(-1);

            selectionManager.Update();

            if(GameEventSystem.isAimingSpell && (Input.GetMouseButtonUp(1) || Input.GetKeyDown(KeyCode.Escape) || mouseDisabled || UIHandler.activePanel != null))
            {
                FinishTargeting();
                return;
            }

            if(GameEventSystem.isAimingSpell)
            {
                HandleSpellTargeting(portraitActorUnderCursor != null ? portraitActorUnderCursor : SelectionManager.actorUnderCursor);
            }
            else if(SelectionManager.actorUnderCursor != null)
            {

                if(SelectionManager.actorUnderCursor.PartySlot > 0)
                {
                    SetCursor(CursorType.NORMAL);
                    _targetMode = TargetMode.NONE;
                    return;
                }
                else if(FactionExentions.IsEnemy(SelectionManager.actorUnderCursor.ActorStats, Faction.Heroes))
                {
                    SetCursor(CursorType.ATTACK);
                    _targetMode = TargetMode.ATTACK;
                }
                else if(FactionExentions.IsNeutral(SelectionManager.actorUnderCursor.ActorStats, Faction.Heroes))
                {
                    SetCursor(CursorType.TALK);
                    _targetMode = TargetMode.TALK;
                }

                if(Input.GetMouseButtonUp(0))
                {
                    PerformActionOn(SelectionManager.actorUnderCursor);
                }
            }
            else
            {
                HandleMouseOver();
            }
        }

        private void HandleMouseOver()
        {
            Profiler.BeginSample("SCAN_FOR_IMPASSABLE");
            GameEventSystem.areaBlocked = false;
            if((overUI && GameEventSystem.isAimingSpell == false) || GameEventSystem.drawingSelectionRect)
            {
                SetCursor(CursorType.NORMAL);
                return;
            }

            if(GameEventSystem.FormationRotation == false)
            {
                overDoor = selectionManager.GetDoorAtMousePosition();
                if(overDoor == null)
                {
                    overContainer = selectionManager.GetContainerAtMousePosition();
                }
                if(overDoor != null)
                {
                    Debug.Log("________Checking door visibility");
                    if(overDoor.Visible())
                    {
                        Debug.Log("________Over door");
                        SetCursor(CursorType.DOOR);
                    }
                    else
                        overDoor = null;
                }
                else if(overContainer != null)
                {
                    Debug.Log("________Over cursor");
                    SetCursor(GetCursorOverContainer(overContainer));
                }
                else
                {
                    Ray ray = cameraMain.ScreenPointToRay(Input.mousePosition);
                    RaycastHit physicsHit;
                    if(Physics.Raycast(ray, out physicsHit, Mathf.Infinity))
                    {
                        if(physicsHit.collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
                        {
                            NavMeshHit navmeshHit;
                            if(/*physicsHit.point.y > 0.1f || */GameEventSystem.RequestFogOfWarValueAtPosition?.Invoke(physicsHit.point) > 128 || NavMesh.SamplePosition(physicsHit.point, out navmeshHit, 1, 1 << NavMesh.GetAreaFromName("Walkable")) == false)
                            {
                                GameEventSystem.areaBlocked = true;
                                SetCursor(CursorType.BLOCKED);
                                return;
                            }
                            else
                            {
                                GameEventSystem.areaBlocked = false;
                                SetCursor(CursorType.WALK);
                            }
                        }
                        else if(physicsHit.collider.gameObject.layer == LayerMask.NameToLayer("Obstacles"))
                        {
                            GameEventSystem.areaBlocked = true;
                            SetCursor(CursorType.BLOCKED);
                            return;
                        }
                    }
                    else
                    {
                        SetCursor(CursorType.NORMAL);
                        _targetMode = TargetMode.NONE;
                    }
                }
            }

            Profiler.EndSample();

        }

        private void HandleSpellTargeting(Actor actorUnderMouse) // Can be null in case of target location
        {
            Vector3 targetPosition = Vector3.one; // Using Vector3.one as false condition since ground y is always 0
            Ray cursorGroundRay = cameraMain.ScreenPointToRay(Input.mousePosition);

            Actor targetActor = null;

            switch(_spellActivationMode)
            {
                case DeliveryType.None:

                    break;
                case DeliveryType.InstantSelf:
                case DeliveryType.InstantActor:

                    if(actorUnderMouse == _caster && _clickedSpell.effectType != DamageType.HEAL)
                        return;
                    targetActor = actorUnderMouse;
                    break;
                case DeliveryType.SeekActor:

                    if(actorUnderMouse == _caster)
                        return;
                    targetActor = actorUnderMouse;
                    break;
                case DeliveryType.InstantLocation:
                case DeliveryType.SeekLocation:

                    if(actorUnderMouse == _caster)
                        return;

                    if(_aoeProjector.gameObject.activeInHierarchy)
                    {
                        if(portraitActorUnderCursor == null && overUI)
                        {
                            _aoeProjector.Toggle(false);
                            return;
                        }

                        _aoeProjector.Toggle(true);

                        RaycastHit hit;

                        Vector3 aoeLocation = cursorGroundRay.origin;
                        if(actorUnderMouse != null)
                        {
                            aoeLocation = actorUnderMouse.transform.position;
                        }
                        else if(Physics.Raycast(cursorGroundRay, out hit, Mathf.Infinity, 1 << LayerMask.NameToLayer("Ground")))
                        {
                            aoeLocation = hit.point;
                        }
                        _aoeProjector.transform.position = aoeLocation;
                        //int numTargetHits = Physics.OverlapSphereNonAlloc(aoeLocation, _aoeProjector.radius, _highlightedTargets, 1 << LayerMask.NameToLayer("Actors"));
                        //for(int i = 0; i < numTargetHits; i++)
                        //{
                        //ActorInput targetActor = (ActorInput)_highlightedTargets[i].GetComponent<ActorInput>();
                        //targetActor.Highlight();
                        //}

                        targetPosition = aoeLocation;
                    }
                    break;
                case DeliveryType.Spray: //TODO Implement effect type 'Line' that connects caster with target
                    {
                        //if(_aimProjector.enabled)
                        //{
                        if(overUI)
                        {
                            _aimProjector.Toggle(false);
                            return;
                        }

                        //_aimProjector.Toggle(true);

                        RaycastHit hit;

                        //Vector3 coneAimDir = Vector3.zero;
                        if(actorUnderMouse != null)
                        {
                            targetPosition = actorUnderMouse.transform.position;
                            //if(actorUnderMouse == _caster)
                            //    coneAimDir = _caster.transform.forward;
                            //else
                            //    coneAimDir = actorUnderMouse.transform.position - _caster.transform.position;
                        }
                        else if(Physics.Raycast(cursorGroundRay, out hit, Mathf.Infinity, 1 << LayerMask.NameToLayer("Ground")))
                        {
                            targetPosition = hit.point/* - _caster.transform.position*/;
                        }

                        //coneAimDir.y = 0;
                        //_aimProjector.transform.position = _caster.transform.position;
                        //_aimProjector.transform.rotation = Quaternion.Slerp(_aimProjector.transform.rotation, Quaternion.LookRotation(coneAimDir), Time.unscaledDeltaTime * 15);

                        //targetPosition = _caster.transform.position + coneAimDir.normalized;
                        //}
                        break;
                    }
                case DeliveryType.Beam:
                    {
                        if(overUI)
                        {
                            return;
                        }

                        if(actorUnderMouse == null || actorUnderMouse == _caster/* && _clickedSpell.effectType != DamageType.HEAL*/)
                            return;
                        targetActor = actorUnderMouse;
                        break;
                    }
            }

            if(Input.GetMouseButtonUp(0))
            {
                //Debug.Log("_____Placed cone target");

                if(targetActor != null)
                {
                    TryToCast(_caster, targetActor);
                }
                else if(targetPosition != Vector3.one)
                {
                    TryToCast(_caster, targetPosition);
                }

                FinishTargeting();
            }
        }

        private void PerformActionOn(Actor actor)
        {
            if(FactionExentions.IsEnemy(actor.ActorStats, Faction.Heroes))
            {
                _clickActionType = ClickActionType.ATTACK;
            }
            else if(FactionExentions.IsNeutral(actor.ActorStats, Faction.Heroes))
            {
                _clickActionType = ClickActionType.TALK;
            }
            else
            {
                _clickActionType = ClickActionType.NONE;
            }

            switch(_targetMode)
            {
                case TargetMode.ATTACK:
                    _clickActionType = ClickActionType.ATTACK;
                    break;
                case TargetMode.TALK:
                    _clickActionType = ClickActionType.TALK;
                    break;
                case TargetMode.CAST:
                    _clickActionType = ClickActionType.CAST;
                    break;
                case TargetMode.DEFEND:
                    _clickActionType = ClickActionType.DEFEND;
                    break;
                case TargetMode.PICK:
                    _clickActionType = ClickActionType.THIEVING;
                    break;
            }

            //if(_targetMode != TargetMode.CAST)
            //{
            //    SetTargetMode(TargetMode.NONE);
            //}

            switch(_clickActionType)
            {
                case ClickActionType.NONE:

                    //if(actor.ValidTarget(GetActorFlags.SELECT, null) == false)
                    //{
                    //    if(debug)
                    //        Debug.Log("<color=yellow>TargetMode.NONE: Invalid Target ActorInput</color>");
                    //    return;
                    //}

                    //if(actor.InParty > 0)
                    //{
                    //    SelectActor(actor.InParty);
                    //}
                    //else if(ActorIsSummoned)
                    //{
                    //    SelectionManager.current.SelectActor(actor, true);
                    //}


                    break;
                case ClickActionType.TALK:

                    //if(actor.ValidTarget(ActorFlags.TALK, null) == false)
                    //    return;
                    Actor pc = null;
                    if(SelectionManager.selected.Count > 0)
                        pc = SelectionManager.selected[0];

                    if(pc != null)
                    {
                        TryToTalk(pc, actor);
                    }

                    break;
                case ClickActionType.ATTACK:

                    _clickActionType = ClickActionType.ATTACK;

                    foreach(Actor selectedActor in SelectionManager.selected)
                    {
                        TryToAttack(selectedActor, actor);
                    }

                    break;
                case ClickActionType.CAST:

                    if(SelectionManager.selected.Count == 1)
                    {
                        Actor cstr = SelectionManager.selected[0];
                        if(cstr != null)
                        {
                            TryToCast(cstr, actor);
                        }
                    }

                    break;
                case ClickActionType.DEFEND:
                    break;
                case ClickActionType.THIEVING:
                    break;
            }
        }

        private void HandleSpellButtonPress(Actor caster, Spell spell, UISpellButton _activeSpellButton)
        {
            FinishTargeting();
            if(spell.cooldownTimer > 0)
            {
                return;
            }
            _lastPressedSpellButton = _activeSpellButton;
            _lastPressedSpellButton.ToggleSelected(true);

            _caster = caster;
            _clickedSpell = spell;
            GameEventSystem.isAimingSpell = true;
            _aoeProjector.Toggle(false);
            _aoeProjector.enabled = false;
            _aimProjector.Toggle(false);
            _aimProjector.enabled = false;
            _spellActivationMode = spell.deliveryType;
            SetTargetMode(TargetMode.CAST);
            switch(_spellActivationMode)
            {
                case DeliveryType.None:
                    Debug.LogError("Spell delivery type = none");
                    GameEventSystem.isAimingSpell = false;
                    break;
                case DeliveryType.InstantSelf: // AoE possible, i.e. bards songs

                    TryToCast(_caster, _caster);
                    FinishTargeting();

                    break;
                case DeliveryType.InstantActor:
                case DeliveryType.SeekActor:
                //if(spell.aoeRadius >= 1)
                //{
                //    targetAOEProjector.transform.position = _camera.ScreenPointToRay(Input.mousePosition).origin;
                //    targetAOEProjector.enabled = true;
                //    targetAOEProjector.SetAoE(caster, spell.aoeRadius, SpellTargetType.Foe);
                //    _spellActivationMode = SpellActivationMode.AimLocation;
                //}
                //break;
                case DeliveryType.InstantLocation:
                case DeliveryType.SeekLocation:


                    if(spell.aoeRadius >= 1)
                    {
                        _aoeProjector.transform.position = cameraMain.ScreenPointToRay(Input.mousePosition).origin;
                        _aoeProjector.enabled = true;
                        _aoeProjector.SetAoE(caster, spell.aoeRadius, spell.spellTargetType);
                    }

                    break;
                case DeliveryType.Spray:
                    {
                        _aimProjector.transform.position = caster.transform.position;
                        _aimProjector.enabled = true;
                        _aimProjector.SetBeam(caster, spell.effectRange, spell.effectDiameter, spell.spellTargetType);
                        break;
                    }
                case DeliveryType.Beam:
                    {
                        //_aimProjector.transform.position = caster.transform.position;
                        //_aimProjector.enabled = true;
                        //_aimProjector.SetBeam(caster, spell.effectRange, spell.effectDiameter, spell.spellTargetType);
                        break;
                    }
            }
        }

        private void FinishTargeting()
        {
            if(GameEventSystem.isAimingSpell == false)
                return;
            _lastPressedSpellButton.ToggleSelected(false);
            //_caster = null;
            _aoeProjector.Toggle(false);
            _aoeProjector.enabled = false;
            _aimProjector.Toggle(false);
            _aimProjector.enabled = false;
            GameEventSystem.isAimingSpell = false;
            _spellActivationMode = DeliveryType.None;
            SetTargetMode(TargetMode.NONE);
        }

        private void SetTargetMode(TargetMode targetMode)
        {
            switch(targetMode)
            {
                case TargetMode.NONE:
                    SetCursor(CursorType.NORMAL);
                    break;
                case TargetMode.TALK:
                    SetCursor(CursorType.TALK);
                    break;
                case TargetMode.ATTACK:
                    SetCursor(CursorType.ATTACK);
                    break;
                case TargetMode.CAST:
                    SetCursor(CursorType.CAST);
                    break;
                case TargetMode.DEFEND:
                    SetCursor(CursorType.DEFEND);
                    break;
                case TargetMode.PICK:
                    SetCursor(CursorType.PICK);
                    break;
                case TargetMode.GRAB:
                    SetCursor(CursorType.GRAB);
                    break;
            }
        }

        //! TryTo***
        #region TryTo*** Code

        void TryToAttack(Actor attacker, Actor target)
        {
            //FormationController.ClearFormationVisual(attacker.InParty);
            attacker.Combat.SetHostileTarget(target);
            AIActions.Action_Attack act = new AIActions.Action_Attack(attacker);
            act.Set(target);
            attacker.CommandActor(act);
        }

        void TryToCast(Actor caster, Actor target)
        {
            if(caster.Animation.Animator.GetCurrentAnimatorStateInfo(2).IsName("New State") == false)
            {
                return;
            }
            caster.Animation.Animator.Play("CancelAttack", 1);
            //FormationController.ClearFormationVisual(caster.InParty);

            AIActions.Action_CastSpellAtActor action = new AIActions.Action_CastSpellAtActor(caster);
            caster.CommandActor(action.Set(target, _clickedSpell));
            if(FactionExentions.IsEnemy(target.ActorStats, Faction.Heroes))
            {
                caster.Combat.SetHostileTarget(target);
            }
        }

        void TryToCast(Actor caster, Vector3 point)
        {
            if(caster.Animation.Animator.GetCurrentAnimatorStateInfo(2).IsName("New State") == false)
            {
                return;
            }
            caster.Combat.SetHostileTarget(null);
            //FormationController.ClearFormationVisual(caster.InParty);

            AIActions.Action_CastSpellAtLocation action = new AIActions.Action_CastSpellAtLocation(caster);
            caster.CommandActor(action.Set(point, _clickedSpell));
        }

        void TryToTalk(Actor talkingPC, Actor dialogOwner)
        {
            FormationController.ClearFormationVisual(talkingPC.PartySlot);
            //TODO Implement dialog stuff
            talkingPC.CommandActor(new AIActions.Action_PCTalkTo(talkingPC, dialogOwner));
            //Interface.GetUIScript().
        }

        void HandleContainer(Container container, Actor actor)
        {
            //if(actor->GetStat(IE_SEX) == SEX_ILLUSION)
            //    return;
            //container is disabled, it should not react
            if(container.Flags.HasFlag(ContainerFlags.DISABLED))
            {
                return;
            }

            if((_targetMode == TargetMode.CAST) /*&& spellCount*/)
            {
                //we'll get the container back from the coordinates
                TryToCast(actor, container.transform.position);
                //Do not reset target_mode, TryToCast does it for us!!
                return;
            }

            //core->SetEventFlag(EF_RESETTARGET);

            //if(_targetMode == TargetMode.ATTACK)
            //{
            //    //actor.SetHostileTarget(container);
            //    actor.CommandActor(new MoveAction(actor).Set(container.transform.position, container.transform.position - actor.transform.position, 2,
            //    () =>
            //    {
            //        actor.Execute_Attack();
            //    }));
            //    return;
            //}

            if(_targetMode == TargetMode.PICK)
            {
                TryToPick(actor, container);
                return;
            }

            //container.AddTrigger(TriggerEntry(trigger_clicked, actor->GetGlobalID()));
            //core->SetCurrentContainer(actor, container);
            //actor->CommandActor(GenerateAction("UseContainer()"));
            //actor.CommandActor(new MoveAction(actor).Set(container.transform.position, container.transform.position - actor.transform.position, 2,
            //    () =>
            //    {
            //        actor.Execute_PickUpItem(null, true, true);
            //    }));
        }

        // generate action code for source actor to try to pick pockets of a target (if an actor)
        // else if door/container try to pick a lock/disable trap
        // The -1 flag is a placeholder for dynamic target IDs
        private void TryToPick(Actor source, Scriptable tgt)
        {

            //source.SetModal(MS_NONE);
            //    const char* cmdString = NULL;
            //	switch (tgt->Type) {
            //		case ST_ACTOR:
            //			cmdString = "PickPockets([-1])";
            //			break;
            //		case ST_DOOR:
            //		case ST_CONTAINER:
            //			if (((const Highlightable*) tgt)->Trapped && ((const Highlightable*) tgt)->TrapDetected) {
            //				cmdString = "RemoveTraps([-1])";
            //			} else
            //{
            //    cmdString = "PickLock([-1])";
            //}
            //break;
            //default:
            //			Log(ERROR, "GameControl", "Invalid pick target of type %d", tgt->Type);
            //return;
            //	}
            //	source->CommandActor(GenerateActionDirect(cmdString, tgt));
        }

        //generate action code for source actor to try to disable trap (only trap type active regions)
        //void TryToDisarm(ActorInput source, InfoPoint tgt)
        //{
        //    if(tgt->Type != ST_PROXIMITY)
        //        return;

        //    source->SetModal(MS_NONE);
        //    source->CommandActor(GenerateActionDirect("RemoveTraps([-1])", tgt));
        //}
        #endregion TryTo*** Code


        //! Utility Functions
        #region Utility Functions

        private void SetPortraitActorUnderCursor(int partyIndex)
        {
            portraitActorUnderCursor = GameEventSystem.RequestGetPCByPartyIndex?.Invoke(partyIndex);
        }

        private void SetCursor(CursorType cursorType)
        {

            if(_currCursorType == cursorType)
            {
                return;
            }

            _currCursorType = cursorType;

            //GameStateManager.Instance.uiScript.set(cursorType);
            //! TODO -----------------------------------
        }

        CursorType GetCursorOverContainer(Container overContainer)
        {
            if((overContainer.Flags.HasFlag(ContainerFlags.DISABLED)))
            {
                return _currCursorType;
            }

            if(_targetMode == TargetMode.PICK)
            {
                if(overContainer.VisibleTrap(0))
                {
                    return CursorType.TRAP;
                }
                if(overContainer.Flags.HasFlag(ContainerFlags.LOCKED))
                {
                    return CursorType.LOCK2;
                }

                return CursorType.STEALTH | CursorType.GRAY;
            }
            return CursorType.TAKE;
        }

        #endregion Utility Functions

        //void OnGUI()
        //{
        //    GUI.Label(new Rect(Screen.width / 2, 2, 130, 300),
        //        "Over door? " + (overDoor != null ? true : false) + "\n"+
        //        "Over container? " + (overContainer != null ? true : false) + "\n" +
        //        "Cursor type: " + (_currCursorType));

        //}
    }
}