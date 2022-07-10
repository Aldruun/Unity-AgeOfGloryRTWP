using AoG.Core;
using System.Collections.Generic;
using UnityEngine;

public enum FormationType
{
    SPEAR,
    SPEARHEAD,
    CIRCLE,
    TEE,
    LINE,
    WALL,
    PENTAGON,
    SQUARELONG,
    SQUAREWIDE,
    SQUARELONGSHIFTED
}

/// <summary>
/// The sole purpose of this monobehaviour is the setup and rotation of
/// the formation as well as giving the movement orders on button up.
/// </summary>
public class FormationController : MonoBehaviour
{
    //! Formation Related Variables
    #region Formation Variables
    public bool debugFormation;
    private static FormationType _currFormationType;
    //public Transform formationParent;
    //Transform _formationCenter;
    private static List<GameObject> _cachedTargetReticles;
    private static Dictionary<int, Vector3> formation;
    private Vector3 _draggedDirection;
    private int _numSelected;
    private Vector3 _terrainClickPoint;
    private Vector3 _formationApplicationPoint;
    private const int FORMATIONSIZE = 10;
    private Camera cameraMain;

    #endregion Formation Variables

    void Start()
    {
        cameraMain = Camera.main;
        SetupFormationControl();

        GameEventSystem.SetFormation -= SetFormation;
        GameEventSystem.SetFormation += SetFormation;
        ConstuctFormation(FormationType.SQUARELONG);
    }

    void OnDisable()
    {
        GameEventSystem.SetFormation -= SetFormation;
    }

    void Update()
    {
        UpdateReticles(); //TODO Should not be called every frame

        if(GameEventSystem.isAimingSpell || GameEventSystem.areaBlocked)
        {
            return;
        }

        HandleFormation();
    }

    void DrawTargetReticle(int idx, Vector3 position, bool animate, bool flash = false, bool actorSelected = false)
    {
        //Debug.Log("Idx: " + idx);
        if(_cachedTargetReticles[idx].activeInHierarchy == false)
            _cachedTargetReticles[idx].SetActive(true);
        _cachedTargetReticles[idx].transform.rotation = Quaternion.identity;
        _cachedTargetReticles[idx].transform.position = position;
    }

    void UpdateReticles()
    {
        foreach(Actor ac in GameInterface.Instance.GetCurrentGame().PCs)
        {
            //int selIndex = -1;

            if(ac.hasMovementOrder)
            {
                bool underCursor = ac == SelectionManager.actorUnderCursor;
                if(ac.ActorUI.Selected)
                {
                    foreach(GameObject reticle in _cachedTargetReticles)
                    {
                        if(reticle.transform.position == ac.NavAgent.destination)
                        {
                            reticle.SetActive(true);
                            reticle.GetComponentInChildren<MeshRenderer>().enabled = true;
                            reticle.GetComponentInChildren<MeshRenderer>().material.color = new Color(0.144f, 1, 0, 1);
                            break;
                        }
                    }
                }
                else if(underCursor)
                {
                    foreach(GameObject reticle in _cachedTargetReticles)
                    {
                        if(reticle.transform.position == ac.NavAgent.destination)
                        {
                            reticle.SetActive(true);
                            reticle.GetComponentInChildren<MeshRenderer>().enabled = true;
                            reticle.GetComponentInChildren<MeshRenderer>().material.color = new Color(0.144f, 1, 0, 0.6f);
                            break;
                        }
                    }
                }
                else
                {
                    foreach(GameObject reticle in _cachedTargetReticles)
                    {
                        if(reticle.transform.position == ac.NavAgent.destination)
                        {
                            reticle.SetActive(false);
                            reticle.GetComponentInChildren<MeshRenderer>().enabled = false;
                            break;
                        }
                    }
                }
            }
        }
    }

    Vector3 GetFormationPoint(int posIndex, Vector3 applicationPoint, Vector3 clickPoint)
    {
        if(posIndex >= FORMATIONSIZE)
        {
            posIndex = FORMATIONSIZE - 1;
        }

        // calculate angle
        float angle;
        float xdiff = applicationPoint.x - clickPoint.x;
        float zdiff = applicationPoint.z - clickPoint.z;
        if(zdiff == 0)
        {
            if(xdiff > 0)
            {
                angle = Mathf.PI / 2;
            }
            else
            {
                angle = -(Mathf.PI / 2);
            }
        }
        else
        {
            angle = Mathf.Atan(xdiff / zdiff);
            if(zdiff < 0)
                angle += Mathf.PI;
        }
        //Debug.Log(posIndex);

        // calculate new coordinates by rotating formation around (0,0)
        float newx = -formation[posIndex].x * Mathf.Cos(angle) + formation[posIndex].z * Mathf.Sin(angle);
        float newz = formation[posIndex].x * Mathf.Sin(angle) + formation[posIndex].z * Mathf.Cos(angle);
        clickPoint.x += newx;
        clickPoint.z += newz;

        return clickPoint;
    }

    void HandleFormation()
    {
        if(Input.GetMouseButtonDown(1))
        {
            _numSelected = 0;

            // Only check for cursor over UI before formation creation
            if(UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject(-1))
            {
                return;
            }

            _numSelected = SelectionManager.selected.Count;
            _terrainClickPoint = HelperFunctions.GetTerrainCursorHitPoint(cameraMain);
        }

        if(_numSelected == 0)
            return;

        if(_terrainClickPoint == Vector3.zero)
        {
            ClearFormationVisuals();
            return;
        }

        if(Input.GetMouseButton(1)) //! Set destination and drag mouse to rotation target
        {
            if(_numSelected == 0)
            {
                if(debugFormation)
                {
                    Debug.Log("<color=yellow>///</color> Cannot draw formation -> No pc selected");
                }
                return;
            }

            RaycastHit hit;
            Ray ray = cameraMain.ScreenPointToRay(Input.mousePosition);

            if(Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << LayerMask.NameToLayer("Ground")))
            {
                if(debugFormation)
                {
                    Debug.Log("<color=green>///</color> Ground ray hit");
                }

                Vector3 direction = Vector3.zero;
                //formationParent.transform.position = _terrainClickPoint;

                // Only rotate formation if the player intentionally drags the mouse
                if((hit.point - _terrainClickPoint).magnitude > 0.02f)
                {
                    _formationApplicationPoint = hit.point;
                    direction = hit.point - _terrainClickPoint;
                    GameEventSystem.FormationRotation = true;
                }
                else
                {
                    // Else it's just a simple click. Best to make the formation point away from the party center
                    Vector3 sum = Vector3.zero;
                    foreach(Actor actor in SelectionManager.selected)
                    {
                        sum += actor.transform.position;
                    }
                    direction = hit.point - sum / _numSelected;
                    _formationApplicationPoint = _terrainClickPoint + direction;
                }

                _draggedDirection = direction.normalized;

                if(debugFormation)
                {
                    Debug.DrawLine(_terrainClickPoint, hit.point);
                    Debug.DrawRay(_terrainClickPoint + Vector3.up * 1, _draggedDirection * 5, Colors.Acajou);
                }

                //float frontPoint = 0;



                if(_numSelected > 1)
                {
                    // Draw reticles while rotating formation. At this point we only care about the selected PCs count
                    // so we can pick the formation keys from index 0 upwards

                    int count = SelectionManager.selected.Count;

                    for(int i = 1; i <= count; i++)
                    {
                        //! Every frame
                        Vector3 formationPosition = HelperFunctions.GetSampledNavMeshPosition(GetFormationPoint(i, _formationApplicationPoint, _terrainClickPoint));
                        GameObject visual = _cachedTargetReticles[i - 1];
                        visual.SetActive(true);
                        visual.transform.position = formationPosition;
                        visual.transform.rotation = Quaternion.identity;
                        DrawTargetReticle(i - 1, formationPosition, false);

                        if(debugFormation)
                            GenericFunctions.DebugDraw.Circle(formationPosition, Vector3.up, 0.5f, 6, Color.green);
                    }
                }
                else
                {
                    int idx = SelectionManager.selected[0].PartySlot;
                    Vector3 sampledPosition = HelperFunctions.GetSampledNavMeshPosition(_terrainClickPoint);
                    DrawTargetReticle(idx - 1, sampledPosition, false);

                    if(debugFormation)
                        GenericFunctions.DebugDraw.Circle(sampledPosition, Vector3.up, 0.5f, 6, Color.green);
                }
            }
        }

        if(Input.GetMouseButtonUp(1))
        {
            GameEventSystem.FormationRotation = false;
            if(SelectionManager.actorUnderCursor != null)
            {
                ClearFormationVisuals();
                return;
            }

            if(_numSelected == 0)
            {
                return;
            }

            _draggedDirection.y = 0;

            Vector3 move = _terrainClickPoint;

            for(int i = 1; i <= _numSelected; i++)
            {
                Actor actor = SelectionManager.selected[i - 1];
                actor.HoldPosition();

                if(_numSelected > 1)
                {
                    move = GetFormationPoint(i, _formationApplicationPoint, _terrainClickPoint);
                }

                if(i == 1)
                {
                    int frequencySetting = 2;
                    bool chatter = false;

                    switch(frequencySetting)
                    {
                        case 1:
                            chatter = DnD.RollBG(1, 100, 0) < 10;
                            break;
                        case 2:
                            chatter = DnD.RollBG(1, 100, 0) < 20;
                            break;
                        case 3:
                            chatter = DnD.RollBG(1, 100, 0) < 50;
                            break;
                        case 4:
                            chatter = DnD.RollBG(1, 100, 0) < 60;
                            break;
                        default:
                            break;
                    }

                    if(chatter)
                        actor.VerbalConstant(VerbalConstantType.MOVECOMMANDYES);
                }

                GenericFunctions.DebugDraw.Circle(move, Vector3.up, 0.5f, 6, Colors.White, 5);
                int retIndex = i - 1;
                actor.MoveCommand(new MoveAction(actor).Set(HelperFunctions.GetSampledNavMeshPosition(move), _draggedDirection, 0.1f, () => { _cachedTargetReticles[retIndex].SetActive(false); }));
            }
        }
    }

    public static void ClearFormationVisual(int partySlot)
    {
        _cachedTargetReticles[partySlot - 1].SetActive(false);
    }

    void ClearFormationVisuals()
    {
        foreach(GameObject go in _cachedTargetReticles)
        {
            go.SetActive(false);
        }
    }


    void SetupFormationControl()
    {
        //! Set up circle as default formation for now

        formation = new Dictionary<int, Vector3>();
        //! Create an empty formation map
        formation.Add(1, new Vector3());
        formation.Add(2, new Vector3());
        formation.Add(3, new Vector3());
        formation.Add(4, new Vector3());
        formation.Add(5, new Vector3());
        formation.Add(6, new Vector3());
        formation.Add(7, new Vector3());
        formation.Add(8, new Vector3());

        _cachedTargetReticles = new List<GameObject>();
        for(int i = 0; i < 8; i++)
        {
            _cachedTargetReticles.Add(Instantiate(Resources.Load<GameObject>("Prefabs/GFX/targetreticle"), transform));
            _cachedTargetReticles[i].SetActive(false);
        }

    }

    public static void SetFormation(FormationType formationType)
    {
        //if(_currFormationType == formationType)
        //{
        //    return;
        //}

        _currFormationType = formationType;
        ConstuctFormation(formationType);
    }

    static void ConstuctFormation(FormationType formationType)
    {
        formation.Clear();

        switch(formationType)
        {
            case FormationType.SQUARELONG:
                //_formationApplicationPoint = new Vector3(0, 0, 0);
                formation.Add(1, new Vector3(0.5f, 0, 0));
                formation.Add(2, new Vector3(-0.5f, 0, 0));
                formation.Add(3, new Vector3(0.5f, 0, -1f));
                formation.Add(4, new Vector3(-0.5f, 0, -1f));
                formation.Add(5, new Vector3(0.5f, 0, -2f));
                formation.Add(6, new Vector3(-0.5f, 0, -2f));
                break;
            case FormationType.SQUARELONGSHIFTED:
                //_formationApplicationPoint = new Vector3(0, 0, 0);
                formation.Add(1, new Vector3(-0.5f, 0, -1.5f));
                formation.Add(2, new Vector3(0.5f, 0, 0));
                formation.Add(3, new Vector3(-0.5f, 0, -0.5f));
                formation.Add(4, new Vector3(0.5f, 0, -1f));
                formation.Add(5, new Vector3(0.5f, 0, -2f));
                formation.Add(6, new Vector3(-0.5f, 0, -2.5f));
                break;
            case FormationType.SQUAREWIDE:
                formation.Add(1, new Vector3(0, 0, 0));
                formation.Add(2, new Vector3(1f, 0, 0));
                formation.Add(3, new Vector3(-1f, 0, 0));
                formation.Add(4, new Vector3(1f, 0, -1f));
                formation.Add(5, new Vector3(0, 0, -1f));
                formation.Add(6, new Vector3(-1f, 0, -1f));
                break;
            case FormationType.SPEAR:
                formation.Add(1, new Vector3(0, 0, 0));
                formation.Add(2, new Vector3(0, 0, 2));
                formation.Add(3, new Vector3(0.5f, 0, 1f));
                formation.Add(4, new Vector3(-0.5f, 0, 1f));
                formation.Add(5, new Vector3(1f, 0, 0));
                formation.Add(6, new Vector3(-1f, 0, 0));
                break;
            case FormationType.SPEARHEAD:
                formation.Add(1, new Vector3(0, 0, 0));
                formation.Add(2, new Vector3(0.5f, 0, -1f));
                formation.Add(3, new Vector3(-0.5f, 0, -1f));
                formation.Add(4, new Vector3(1f, 0, -2f));
                formation.Add(5, new Vector3(0, 0, -2));
                formation.Add(6, new Vector3(-1f, 0, -2f));
                break;
            case FormationType.CIRCLE:
                formation.Add(1, new Vector3(0, 0, 1.2f));
                formation.Add(2, new Vector3(1.2f, 0, 1.2f));
                formation.Add(3, new Vector3(-1.2f, 0, 1.2f));
                formation.Add(4, new Vector3(1.2f, 0, -1.2f));
                formation.Add(5, new Vector3(-1.2f, 0, -1.2f));
                formation.Add(6, new Vector3(0, 0, -1.2f));
                break;
            case FormationType.TEE:
                formation.Add(1, new Vector3(0, 0, 0));
                formation.Add(2, new Vector3(1f, 0, 0));
                formation.Add(3, new Vector3(-1f, 0, 0));
                formation.Add(4, new Vector3(0, 0, -1f));
                formation.Add(5, new Vector3(0, 0, -2f));
                formation.Add(6, new Vector3(0, 0, -3f));
                break;
            case FormationType.LINE:
                formation.Add(1, new Vector3(0, 0, 0));
                formation.Add(2, new Vector3(0, 0, -1f));
                formation.Add(3, new Vector3(0, 0, -2f));
                formation.Add(4, new Vector3(0, 0, -3f));
                formation.Add(5, new Vector3(0, 0, -4f));
                formation.Add(6, new Vector3(0, 0, -5f));
                break;
            case FormationType.WALL:
                formation.Add(1, new Vector3(-0.5f, 0, 0));
                formation.Add(2, new Vector3(0.5f, 0, 0));
                formation.Add(3, new Vector3(-1.5f, 0, 0));
                formation.Add(4, new Vector3(1.5f, 0, 0));
                formation.Add(5, new Vector3(-2.5f, 0, 0));
                formation.Add(6, new Vector3(2.5f, 0, 0));
                break;
            case FormationType.PENTAGON:
                formation.Add(1, new Vector3(0, 0, 0));
                formation.Add(2, new Vector3(0, 0, 1f));
                formation.Add(3, new Vector3(-1f, 0, 0));
                formation.Add(4, new Vector3(1f, 0, 0));
                formation.Add(5, new Vector3(-1.2f, 0, -1.2f));
                formation.Add(6, new Vector3(1.2f, 0, -1.2f));
                break;
        }
    }
}