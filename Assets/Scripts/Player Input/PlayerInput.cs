//using AoG.Core;
//using Cinemachine;
//using RotaryHeart.Lib.PhysicsExtension;
//using System.Collections;
//using UnityEngine;
//using UnityEngine.Profiling;
//using Physics = UnityEngine.Physics;

//public enum PlayerControlScheme
//{
//    THIRDPERSON
//}

//public class PlayerInput : ActorInput
//{
//    public string key_selectClosestEnemy = "tab";
//    public string key_toggleWeapon = "f";
//    public string key_toggleReadySpell = "r";

//    public static ActorRecord selectedTarget { get; private set; }

//    private Spell[] _quickSlotSpells;
//    //private PlayerCamera _playerCamera;
//    //private CinemachineVirtualCamera _playerCamera;
//    private Camera cam;
//    private bool switchingStance;

//    private float _blockMovementTimer = 1;

//    private bool _jump;
//    private bool _landed;
//    private bool _falling;
//    private bool _stunned;
//    private bool _moving;

//    public float gravity = -9.8f;
//    public float fallingBuffer = 0.2f;
//    private float _jumpHeight = 1.5f;
//    private float _jumpTimer = 1.6f;

//    private bool walkTogglePressed;
//    private bool rightMouseButtonPressed;
//    private bool leftMouseButtonPressed;

//    //private float leftMouseDownTime;
//    private bool isAttacking;

//    private float[] weaponComboTimes;
//    private float nextComboMoveTimer;
//    private float comboFailTimer;
//    private int currentComboStage = -1;

//    private KeyCode[] numberKeyCodes = new KeyCode[] { KeyCode.Alpha0, KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4, KeyCode.Alpha5, KeyCode.Alpha6, KeyCode.Alpha7, KeyCode.Alpha8, KeyCode.Alpha9 };

//    private Vector3 playerGlobalVelocity;
//    private Vector3 playerVelocity;
//    private Vector3 collisionPoint;
//    private Vector3 lockOnPosition;
//    private Vector3 rootMotion;
//    private Vector3 camFrwdDir;
//    private Vector3 inputRaw;
//    private Quaternion flatCameraRotation;

//    private AnimationSet currAnimationSet;
//    private Vector3 groundedCursorPosition;
    
//    public void Setup(Camera camera)
//    {
//        _quickSlotSpells = new Spell[10];
//        //for(int i = 0; i < spellbook.spells.Count; i++)
//        //{
//        //    if(i == 10)
//        //        break;
//        //    Debug.Log("<color=cyan>Adding player spell </color>'" + spellbook.spells[i].Name + "'");
//        //    _quickSlotSpells[i] = spellbook.spells[i];
//        //}

//        //_animator.Play("Idle", 0);

//        cam = camera;

//        //GameEventSystem.OnPlayerCameraModeChanged = AdjustToCameraMode;

//        weaponComboTimes = new float[] { 0.5f, 0.7f, 0.7f };


//    }

//    public void Update()
//    {
//        //if(_player == null)
//        //    return;

//        if(_blockMovementTimer > 0)
//        {
//            _blockMovementTimer -= Time.deltaTime;
//            return;
//        }
//        Profiler.BeginSample("GetActivatable");
//        GetActivatable();
//        Profiler.EndSample();

//        camFrwdDir = cam.transform.forward;
//        camFrwdDir.y = 0;
//        flatCameraRotation = Quaternion.LookRotation(camFrwdDir);
//        //if(_playerCTRL.m_freeze)
//        //{
//        //    return;
//        //}

//        //_groundRay.origin = self.transform.position + _collisionPoint + Vector3.up * 0.05f;
//        //_groundRay.direction = -Vector3.up;

//        leftMouseButtonPressed = Input.GetMouseButton(0);
//        rightMouseButtonPressed = Input.GetMouseButton(1);
//        //bool mouseMoving = Mathf.Abs(Input.GetAxis("Mouse X") + Input.GetAxis("Mouse Y")) != 0;

//        walkTogglePressed = Input.GetKey(KeyCode.C);

//        //if(Input.GetKeyUp(key_toggleReadySpell))
//        //{
//        //    if(Combat.SpellDrawn == false)
//        //    {
//        //        if(Combat.equippedSpell == null && _quickSlotSpells[0] != null)
//        //        {
//        //            Debug.Log("<color=cyan>PlayerInput:Adding quick spell for player </color>'" + _quickSlotSpells[0].Name + "'");
//        //            Combat.SetEquippedSpell(_quickSlotSpells[0]);
//        //        }

//        //        if(Combat.WeaponDrawn)
//        //        {
//        //            Debug.Log("<color=cyan>PlayerInput:Switching from weapon to spell</color>'");
//        //            CoroutineRunner.Instance.StartCoroutine(ChangeStance(() => Combat.Execute_SheathWeapon(), () => { Combat.Execute_ReadySpells(); }));
//        //        }
//        //        else
//        //        {
//        //            Debug.Log("<color=cyan>PlayerInput:Drawing spell</color>'");
//        //            Combat.Execute_ReadySpells();
//        //        }
//        //    }
//        //    else
//        //    {
//        //        Debug.Log("<color=cyan>PlayerInput:Sheathing spell</color>'");
//        //        Combat.Execute_SheathSpells();
//        //    }
//        //}

//        for(int i = 0; i < numberKeyCodes.Length; ++i)
//        {
//            if(Input.GetKeyDown(numberKeyCodes[i]))
//            {
//                PickSpell(i);
//            }
//        }

//        if(Input.GetKeyUp(key_toggleWeapon))
//        {
//            if(Combat.WeaponDrawn)
//            {
//                Debug.Log("<color=cyan>PlayerInput: Sheathing weapon</color>'");
//                Combat.Execute_SheathWeapon();
//            }
//            else
//            {
//                if(Equipment.equippedWeapon.WeaponObject == null)
//                {
//                    Combat.Execute_EquipBestWeapon(Constants.EQUIP_ANY, true, true);
//                }

//                if(Equipment.equippedWeapon.Weapon == null)
//                {
//                    if(Equipment.equippedWeapon.Weapon == null)
//                        Equipment.SetUpFist();
//                }
//                else
//                    Debug.Log("<color=orange>PlayerInput: Weapon id was </color>" + Equipment.equippedWeapon.Weapon.identifier);

//                if(Combat.SpellDrawn)
//                {
//                    Debug.Log("<color=cyan>PlayerInput: Switching from spell to weapon</color>'");
//                    StartCoroutine(ChangeStance(() => Combat.Execute_SheathSpells(), () => Combat.Execute_DrawWeapon()));
//                }
//                else
//                {
//                    Debug.Log("<color=cyan>PlayerInput: Drawing weapon</color>'");
//                    Combat.Execute_DrawWeapon();
//                }

//                CalculateWeaponVariables(Equipment.equippedWeapon.Weapon);
//            }
//        }

//        inputRaw = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));

//        Profiler.BeginSample("AdjustMovementSpeed");
//        if(Input.GetKey(KeyCode.LeftShift))
//        {
//            ChangeMovementSpeed(MovementSpeed.Walk);
//        }
//        else if(inWater == false)
//        {
//            ChangeMovementSpeed(MovementSpeed.Run);
//        }
//        Profiler.EndSample();

//        Profiler.BeginSample("HandleFootMovement");
//        HandleFootMovement();
//        Profiler.EndSample();

//        if(Combat.WeaponDrawn && switchingStance == false)
//        {
//            Profiler.BeginSample("CheckForPlayerAttackInput");
//            CheckForPlayerAttackInput();
//            Profiler.EndSample();
//            //HandleMeleeInput();
//        }
//        else if(isAttacking)
//        {
//            isAttacking = false;
//            ChangeMovementSpeed(MovementSpeed.Run);
//            ResetComboVariables();
//        }
//    }

//    #region Combat Related

//    //public float[] attackTime;
//    // store each length of each attack in order (in the inspector)
//    //public var attackAnimations : AnimationClip[]; // store a reference to the animations, then use the same index 'attackType'

//    public float waitTime = 0.5f; // how long player has to hit LMB after last attack has finished

//    private float attackTimer = 0.0f; // timer variable
//    //private int attackType = -1;
//    // reference to what attack the combo is up to (-1 is none, 0+ is the array index for attackTime )

//    private enum AttackState { Idle, Attacking, Waiting } // different states while the combo inputs are being checked
//    private AttackState attackState = AttackState.Idle; // current attack input state (public for viewing in the Inspector)    

//    private void CheckForPlayerAttackInput()
//    {
//        switch(attackState) // what state is the combo in
//        {
//            case AttackState.Idle:
//                if(leftMouseButtonPressed/* && Animation.animator.GetCurrentAnimatorStateInfo(1).IsName("New State")*/) // start attacking
//                {
//                    isAttacking = true;
//                    currentComboStage = 0;
//                    SetComboStage(0);
//                    attackState = AttackState.Attacking;
//                    attackTimer = weaponComboTimes[0];
//                    Combat.Execute_Attack();
//                    // tell animation to play attack 0 (first attack)
//                }
//                break;

//            case AttackState.Attacking:
//                attackTimer -= Time.deltaTime; // wait for attackTimer

//                if(attackTimer < 0)
//                {
//                    attackTimer = waitTime;
//                    attackState = AttackState.Waiting;

//                }
//                break;

//            case AttackState.Waiting:
//                attackTimer -= Time.deltaTime; // check the Waiting Timer (attackTimer)

//                if(attackTimer < 0) // ran out of time to chain combo
//                {
//                    currentComboStage = -1;
//                    attackState = AttackState.Idle;
//                    return;
//                    // tell animation to play Idle
//                }

//                if(leftMouseButtonPressed) // continue attacking
//                {
//                    currentComboStage++; // go to next attack

//                    if(currentComboStage >= weaponComboTimes.Length) // check if the combo is over, start a new combo
//                    {
//                        currentComboStage = 0;
//                        attackState = AttackState.Attacking;
//                        attackTimer = weaponComboTimes[0];
//                        // tell animation to play attack 0 (first attack)
//                    }
//                    else
//                    {
//                        attackState = AttackState.Attacking;
//                        attackTimer = weaponComboTimes[currentComboStage];
//                        // tell animation to play attack 'attackType' (next attack in array)
//                    }

//                    SetComboStage(currentComboStage);
//                    Combat.Execute_Attack();

//                }
//                break;
//        }

//        if(leftMouseButtonPressed == false)
//        {
//            if(currentComboStage == -1)
//                isAttacking = false;
//        }
//    }

//    private void SetComboStage(int newStage)
//    {
//        Animation.animator.SetInteger("iMeleeComboStage", newStage);
//    }

//    private void ResetComboVariables()
//    {

//        nextComboMoveTimer = 0;
//        comboFailTimer = 0;
//        currentComboStage = -1;
//        Animation.animator.SetInteger("iMeleeComboStage", 0);
//    }

//    private IEnumerator ChangeStance(System.Action from, System.Action to)
//    {
//        switchingStance = true;
//        //TODO ##############################################################################################################
//        //if(ActorRecord.inSpellChargeLoop)
//        //{
//        //    Combat.Exec_HandleSpell(-1, 0);
//        //}

//        from.Invoke();
//        yield return new WaitForSeconds(currAnimationSet == AnimationSet.UNARMED ? 0.2f : 0.8f);

//        to.Invoke();
//        yield return new WaitForSeconds(currAnimationSet == AnimationSet.UNARMED ? 0.2f : 0.8f);

//        switchingStance = false;
//    }

//    //IEnumerator CR_TryHitTargetDelayed(float delay)
//    //{
//    //    yield return new WaitForSeconds(delay);

//    //    if(Combat == null || Stats.dead)
//    //    {
//    //        yield break;
//    //    }

//    //    RaycastHit hit;

//    //    Ray ray = new Ray(transform.position + transform.forward * 0.2f + Vector3.up * 1.5f, transform.forward);
//    //    if(RotaryHeart.Lib.PhysicsExtension.Physics.SphereCast(ray, 0.5f, out hit, 2, 1 << LayerMask.NameToLayer("Actors"), RotaryHeart.Lib.PhysicsExtension.PreviewCondition.Game))
//    //    {
//    //        ActorInput target = hit.collider.GetComponent<ActorInput>();

//    //        if(target != null)
//    //        {
//    //            target.Combat.ApplyDamage(this, Equipment.equippedWeapon.Weapon, null, false, true);
//    //        }
//    //    }
//    //}

//    private void PickSpell(int numPressed)
//    {
//        int spellIndex = numPressed == 0 ? 9 : numPressed - 1;

//        if(_quickSlotSpells[spellIndex] == Combat.equippedSpell)
//        {
//            return;
//        }

//        Spell selectedSpell = _quickSlotSpells[spellIndex];
//        if(selectedSpell != null)
//        {
//            //TODO ##############################################################################################################
//            //if(ActorRecord.inSpellChargeLoop)
//            //{
//            //    Combat.Exec_HandleSpell(-1, 0);
//            //}

//            Debug.Log("<color=cyan>Player selected spell </color>'" + selectedSpell.Name + "'");
//            Combat.SetEquippedSpell(selectedSpell);
//        }
//    }

//    private void CalculateWeaponVariables(Weapon weapon)
//    {
//        currAnimationSet = weapon.animationPack;
//        Animation.animator.SetFloat("fWeaponSpeed", weapon.speed);

//        //switch(weapon.weaponCategory)
//        //{
//        //    case WeaponCategory.Unarmed:
//        //    case WeaponCategory.Dagger:
//        //    case WeaponCategory.LongSword:
//        //    case WeaponCategory.ShortSword:
//        //        _chargeTimer = _chargeTime = 0f;
//        //        _releaseTimer = _releaseTime = 1f;
//        //        break;
//        //    //case WeaponCategory.SwordAndShield:
//        //    //    break;
//        //    //case WeaponCategory.Dual:
//        //    //    break;
//        //    //case WeaponCategory.TwoHanded:
//        //    //    break;
//        //    case WeaponCategory.Longbow:
//        //    case WeaponCategory.Shortbow:
//        //    case WeaponCategory.XBow:
//        //        _chargeTimer = _chargeTime = 0.95f;
//        //        _releaseTimer = _releaseTime = 0.35f;
//        //        //_attackTime = 1.1f;
//        //        break;
//        //    default:
//        //        break;
//        //}
//    }

//    #endregion Combat Related

//    private void HandleFootMovement()
//    {
//        float maxSpeed = 0;
//        float animSpeed = 0;

//        if(isAttacking)
//        {
//            maxSpeed = 1f;
//            animSpeed = 0.5f;
//        }
//        else if(inputRaw != Vector3.zero)
//        {
//            switch(CurrentMovementSpeed)
//            {
//                case MovementSpeed.Walk:
//                    maxSpeed = 1.2f;
//                    animSpeed = 0.5f;
//                    break;

//                case MovementSpeed.Run:
//                    maxSpeed = 3.5f;
//                    animSpeed = 1f;
//                    break;
//            }
//        }

//        if(IsGrounded())
//        {
//            _landed = false;

//            Profiler.BeginSample("Coroutine JumpDelayed");
//            if(Input.GetKeyDown(KeyCode.Space) && _jump == false && isSwimming == false)
//            {
//                StartCoroutine(JumpDelayed());
//                _jump = true;
//            }
//            Profiler.EndSample();

//            if(_stunned)
//            {
//                playerGlobalVelocity = Vector3.zero;
//            }
//            else
//            {
//                playerVelocity = inputRaw;
//                _moving = playerVelocity != Vector3.zero;

//                if(rightMouseButtonPressed == false)
//                {
//                    playerVelocity = Quaternion.Euler(0, cam.transform.eulerAngles.y, 0) * playerVelocity;

//                    //HandleInPlaceRotation();
//                    //! In place rotation
//                }

//                playerGlobalVelocity = flatCameraRotation * inputRaw;
//                playerGlobalVelocity = rightMouseButtonPressed ? transform.TransformDirection(playerVelocity) : playerVelocity;

//                if(playerGlobalVelocity.magnitude > 1)
//                    playerGlobalVelocity = Vector3.Normalize(playerGlobalVelocity);
//                else
//                {
//                    if(_moving == false)
//                    {
//                        playerGlobalVelocity.x = Mathf.MoveTowards(playerGlobalVelocity.x, 0, Time.deltaTime / 0.5f);
//                        playerGlobalVelocity.z = Mathf.MoveTowards(playerGlobalVelocity.z, 0, Time.deltaTime / 0.5f);
//                    }
//                }
//                playerGlobalVelocity *= maxSpeed;
//            }
//            playerGlobalVelocity.y = -fallingBuffer;

//            Profiler.BeginSample("Animation.UpdateMovementAnimations");
//            Animation.UpdateMovementAnimations(transform.InverseTransformDirection(playerGlobalVelocity), animSpeed);
//            Profiler.EndSample();

//            if(_jump)
//            {
//                if(_stunned == false)
//                {
//                    playerGlobalVelocity.y = Mathf.Sqrt(-2 * gravity * _jumpHeight);
//                }
//            }
//        }
//        else
//        {
//            if(_jump && _landed == false)
//            {
//                if(playerGlobalVelocity.y < -0.1f)
//                {
//                    if(_falling == false)
//                    {
//                        _falling = true;
//                        Animation.animator.SetBool("bFall", true);
//                    }
//                    //}
//                    Profiler.BeginSample("Falling CheckSphere");
//                    if(Physics.CheckSphere(transform.position - (Vector3.up * 0.2f), 0.1f, 1 << LayerMask.NameToLayer("Ground") | 1 << LayerMask.NameToLayer("Obstacles")))
//                    {
//                        Animation.animator.SetBool("bFall", false);
//                        _landed = true;
//                        _falling = false;
//                        _jump = false;
//                    }
//                    Profiler.EndSample();
//                }
//            }
//        }
//        playerGlobalVelocity.y += gravity * Time.deltaTime;

//        Quaternion targetRotation = transform.rotation;
//        float rotationSpeed = 10;

//        Profiler.BeginSample("Plane.Raycast");
//        Ray mouseRay = cam.ScreenPointToRay(Input.mousePosition);
//        Plane p = new(Vector3.up, cc.transform.position);
//        if(p.Raycast(mouseRay, out float hitDist))
//        {
//            groundedCursorPosition = mouseRay.GetPoint(hitDist);
//        }
//        else
//        {
//            targetRotation = cc.transform.rotation;
//        }
//        Profiler.EndSample();

//        //! If in combat, look at mouse position
//        if(Combat.WeaponDrawn && Equipment.equippedWeapon.Weapon.animationPack != AnimationSet.DEFAULT)
//        {
//            targetRotation = Quaternion.LookRotation(groundedCursorPosition - cc.transform.position);
//        } //! If not in combat, look at camera direction while moving
//        else if(inputRaw != Vector3.zero)
//        {
//            targetRotation = flatCameraRotation;
//        }

//        Vector3 finVel = Animation.animator.applyRootMotion ? Animation.animator.deltaPosition / Time.deltaTime : playerGlobalVelocity;
//        cc.Move(finVel * Time.deltaTime);
//        cc.transform.rotation = Quaternion.Lerp(cc.transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
//    }

//    private bool IsGrounded()
//    {
//        // DebugExtension.DebugCircle(_cc.transform.position, _playerCamera.transform.forward, Color.cyan, 0.25f, 0.1f);
//        //RaycastHit groundHit;

//        Profiler.BeginSample("ChecksphereDebug");
//        if(_jump == false && RotaryHeart.Lib.PhysicsExtension.Physics.CheckSphere(cc.transform.position, 0.25f, 1 << LayerMask.NameToLayer("Ground") | 1 << LayerMask.NameToLayer("Obstacles"), PreviewCondition.Both))
//        {
//            Profiler.EndSample();
//            return true;
//        }
//        Profiler.EndSample();
//        return false;
//        //return _cc.isGrounded;
//    }

//    // private void RotateCharacterToCamDir()
//    // {
//    //     Quaternion desiredRot = Quaternion.Euler(transform.eulerAngles.x, _playerCamera.transform.eulerAngles.y, transform.eulerAngles.z);
//    //     transform.rotation = Quaternion.Lerp(transform.rotation, desiredRot, Time.deltaTime * 200 * 0.1f);
//    // }
//    //
//    // private void DiveToCamDir()
//    // {
//    //     Quaternion desiredRot = Quaternion.Euler(_playerCamera.transform.eulerAngles.x, _playerCamera.transform.eulerAngles.y, transform.eulerAngles.z);
//    //     transform.rotation = Quaternion.Lerp(transform.rotation, desiredRot, Time.deltaTime * 200 * 0.1f);
//    // }

//    public void OnControllerColliderHit(ControllerColliderHit hit)
//    {
//        if(hit.point.y <= transform.position.y + 0.25f)
//        {
//            collisionPoint = hit.point;
//            collisionPoint = collisionPoint - transform.position;
//        }

//        // We were moving upwards during hit
//        if(playerGlobalVelocity.y > 0 && (cc.collisionFlags & CollisionFlags.Above) != 0)
//        {
//            //if(_jumpingCollision)
//            //{
//            // Got an airborne collision => prevent further soaring
//            playerGlobalVelocity.y = -gravity * Time.deltaTime;
//            // Let this happen only once per collision
//            //_jumpingCollision = false;
//            //}
//            //_jumpingCollision = true;
//        }
//        //else if((_cc.collisionFlags & CollisionFlags.CollidedBelow) != 0)
//        //{
//        //    _animator.SetBool("bFall", false);
//        //    _landed = true;
//        //    _falling = false;
//        //    _jump = false;
//        //}

//        //Set the ground object only if we are really standing on it
//        //if((_cc.collisionFlags & CollisionFlags.Above) != 0
//        //    && Vector3.Distance(transform.TransformPoint(_colliderRoot), hit.point) < 0.3f * _cc.radius)
//        //{
//        //    //_groundObject = hit.gameObject;
//        //}
//    }

//    private IEnumerator JumpDelayed()
//    {
//        //if(_playerVelocity.x != 0 || _playerVelocity.z != 0)
//        Animation.animator.CrossFade("Jump", 0.1f, 7);
//        //_animator.CrossFadeInFixedTime("Jump Single", 1.1f, 1, 0, 1f);
//        //yield return new WaitForSeconds(0.3f);
//        yield return null;
//        //_jump = false;
//    }

//    //private void AdjustToCameraMode(CameraMode cameraMode)
//    //{
//    //    switch(cameraMode)
//    //    {
//    //        case CameraMode.FPS:
//    //            break;

//    //        case CameraMode.TP:
//    //            break;
//    //    }
//    //}

//    public override void UpdateActiveCellBehaviours()
//    {
//        HeadTracking.SetLooAtWeight(1);

//        UpdateLookAtIK();
//    }

//    public override void UpdateLookAtIK()
//    {
//        HeadTracking.lookAtPosition = groundedCursorPosition;
//    }
    
//    public override Vector3 GetLookAtPoint()
//    {
//        return Animation.head.position + Vector3.up * 0.1f;
//    }

//    public override Vector3 GetMovementVelocity()
//    {
//        return cc.velocity;
//    }

//    internal override float GetCharacterRadius()
//    {
//        return 0.3f;
//    }

//    public override void Signal_ProjectileIncoming(Transform projectile, float aoeRadius)
//    {
//    }

//    //! Highlighting
//    private void GetActivatable()
//    {
//        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

//        RaycastHit selectableHit;
//        if(Physics.Raycast(ray, out selectableHit, Game.gameSettings.interactionDistance, 1 << LayerMask.NameToLayer("Actors")))
//        {
//            if(selectableHit.collider.gameObject.CompareTag("Player"))
//            {
//                return;
//            }
//            NPCInput newUnitUnderCursor = selectableHit.collider.GetComponentInParent<NPCInput>();

//            HighlightUnit(newUnitUnderCursor);
//        }
//        else
//        {
//            HighlightUnit(null);
//        }
//    }

//    private void HighlightUnit(IActivatable highlightable)
//    {
//        if(highlightable == null)
//        {
//            //GameStateManager.Instance.uiScript.HideInteractionPopup();
//            //if(actorUnderCursor != null)
//            //    actorUnderCursor.Unhighlight();
//            CurrentActivator = null;
//            return;
//        }

//        if(CurrentActivator != null)
//        {
//            if(CurrentActivator != highlightable)
//            {
//                //UIHandler.SetCursor(0);
//                //actorUnderCursor.Unhighlight();
//                //    actorUnderCursor.Highlight();
//            }
//        }

//        CurrentActivator = highlightable;
//        //Gameevent.ShowInteractionPopup(highlightable.GetName());
//        //newUnitUnderCursor.Highlight();
//    }

//    private void OnGUI()
//    {
//        //GUI.Label(new Rect(200, 2, 150, 30), "Combo count: " + currentComboStage);

//        GUI.Box(new Rect(10, 10, 100, 25), "Hit " + currentComboStage.ToString());
//        GUI.Box(new Rect((Screen.width * 0.5f) - 50, 10, 100, 25), "" + attackState.ToString());
//        //GUI.Box(new Rect(Screen.width - 110, 10, 100, 25), "Total " + totalChainedHits.ToString());
//        GUI.Box(new Rect((Screen.width * 0.5f) - 50, 45, 100, 25), "" + attackTimer.ToString());
//    }
//}