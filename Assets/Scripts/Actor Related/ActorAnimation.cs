using GenericFunctions;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Profiling;

public enum MovementSpeed
{

    Walk,
    Run
}

public class ActorAnimation : MonoBehaviour
{
    public ActorInput self { get; protected set; }
    public Animator Animator { get; protected set; }
    //public LocomotionState locomotionState { get; protected set; }
    public MovementSpeed m_movementSpeed;
    protected AnimationSet currentAnimSet;
    public bool m_grounded = true;
    private Rigidbody[] _rigidbodies;

    internal bool isAttacking;
    internal bool isEvading;
    internal bool isBlocking;
    internal bool isStaggered;
    private bool isDowned;
    internal bool inBleedOutState;

    private Vector3 velocity;
    private float movementSpeed;

    public Transform head;
    public Transform chest;
    public bool inSpellChargeLoop { get; set; }

    private int animatorHash_AimLoopBow;

    private float _checkForCloseActorsTimer = 1;
    //private RaycastHit[] _waterHits;

    public virtual void Initialize(ActorInput actor, Animator animator)
    {
        if(actor.debugInitialization)
            Debug.Log($"<color=grey>{actor.GetName()}: Initializing Animation Component</color>");

        self = actor;
        this.Animator = animator;
        animatorHash_AimLoopBow = Animator.StringToHash("Bow Layer.Bow_IdleDrawn");
        _rigidbodies = GetComponentsInChildren<Rigidbody>();

        //_waterHits = new RaycastHit[5];


        FindActorBodyparts();
    }

    public void ChangeForm(AnimationSet formID)
    {
        if(formID == currentAnimSet)
        {
            //if(self.debugAnimation)
            //    Debug.Log(self.GetName() + ": <color=yellow>Already in " + formID + " stance</color>");
            return;
        }
        currentAnimSet = formID;

        //if(self.debugAnimation)
        //    Debug.Log(self.GetName() + ": Changing to motion form '" + formID + "'");
        // Form 0 = Idle
        switch(currentAnimSet)
        {
            case AnimationSet.XBOW:
            case AnimationSet.BOW:
                Animator.Play("Cancel", 3);
                break;
            case AnimationSet.MAGIC:
                Animator.Play("Cancel", 2);
                break;
        }

        switch(formID)
        {
            case AnimationSet.DEFAULT:
                Animator.CrossFade("Idle", 0.1f, 0);
                break;
            case AnimationSet.UNARMED:
                Animator.CrossFade("UC Idle", 0.1f, 0);
                break;
            case AnimationSet.ONEHANDED:
                Animator.CrossFade("1H Idle", 0.1f, 0);
                break;
            case AnimationSet.TWOHANDED:
                Debug.LogError("ChangeForm(): 2H AnimationPackage not implemented yet");
                break;
            case AnimationSet.BOW:
                Animator.CrossFade("Bow Idle", 0.1f, 0);
                break;
            case AnimationSet.XBOW:
                Debug.LogError("ChangeForm(): XBow AnimationPackage not implemented yet");
                break;
            case AnimationSet.MAGIC:
                Animator.CrossFade("Magic Idle", 0.1f, 0);
                break;
            case AnimationSet.SWIM:
                Animator.CrossFade("Swim Idle", 0.1f, 0);
                break;
            default:
                Debug.LogError("Invalid formID");
                break;
        }
    }

    public bool InBowAimingLoop()
    {
        return Animator.GetCurrentAnimatorStateInfo(3).fullPathHash == animatorHash_AimLoopBow;
    }

    //public void UpdatePlayerMovementAnimation()
    private void OnAnimatorMove()
    {
        if(self == null || self.dead)
            return;

        //HandleRootmotion();

        ApplyTranslationFromCurve();

        Animator.SetFloat("VelocityX", velocity.x, 0.04f, Time.deltaTime);
        Animator.SetFloat("VelocityZ", velocity.z, 0.04f, Time.deltaTime);
        Animator.SetFloat("Speed", movementSpeed, 0.1f, Time.deltaTime);
    }

    public void UpdateMovementAnimations(Vector3 velocity, float speed)
    {
        this.velocity = velocity;
        this.movementSpeed = speed;
    }

    /// <summary>
    /// Used for attack animations, i.e. small forward steps during combos.
    /// </summary>
    private void ApplyTranslationFromCurve()
    {
        float attackVelocity = Animator.GetFloat("AttackMovement") * 0.03f;
        if(attackVelocity > 0)
        {
            //animator.applyRootMotion = true;
            if(self.NavAgent != null)
            {
                //self.NavAgent.updatePosition = false;
                self.NavAgent.Move(transform.forward * attackVelocity);

            }
        }
        else
        {
            //animator.applyRootMotion = false;
            if(self.NavAgent != null)
            {
                //self.NavAgent.updatePosition = true;
            }
            else if(self.cc != null)
            {
                self.cc.SimpleMove(new Vector3(0, 0, attackVelocity));
            }
        }
    }

    private void HandleRootmotion()
    {
        if(Animator.GetBool("bApplyRootMotion"))
        {
            
            if(self.NavAgent != null)
            {
                Animator.applyRootMotion = true;
                //self.NavAgent.updatePosition = false;
                self.NavAgent.velocity = Animator.deltaPosition / Time.deltaTime;

            }
        }
        else
        {
            
            if(self.NavAgent != null)
            {
                Animator.applyRootMotion = false;
                //self.NavAgent.updatePosition = true;
            }
            else if(self.cc != null)
            {
                //self.cc.Move();
            }
        }
    }

    internal AnimationSet GetCurrentAnimationSet()
    {
        return currentAnimSet;
    }

    private void PlayMotion_EquipArmor(Armor armor, bool playAnimation, bool placeholder)
    {
        //if(playAnimation)
        //    animator.CrossFade("DropItem", 0.2f, 5);
    }

    //void PlayMotion_PrepareAttack(AnimationPackage animationPackage)
    //{
    //Debug.Log("### playing prepare attack motion for id " + motionIndex);

    //animator.SetInteger("iMotionIndex", motionIndex);

    //    animator.SetTrigger("tPrepareAttack");
    //    ChangeForm(animationPackage);
    //}

    internal void PlaySpellAnimation(int motionIndex)
    {
        // handIndex: 0 = left, 1 = right

        //if(inSpellChargeLoop)
        //{
        //    Debug.LogError(gameObject.name + ": InSpellChargeLoop");
        //    return;
        //}

        Animator.SetInteger("iCastMotionIndex", motionIndex);

        if(self.debugAnimation)
            Debug.Log(self.GetName() + ": ### Motion start -> Motion Index: " + motionIndex);

        Animator.CrossFade("CastSpell", 0.1f, 2);
        if(motionIndex == -1)
        {
            Animator.CrossFade("Cancel", 0.1f, 2);
            //inSpellChargeLoop = false;
        }
        else if(motionIndex >= 0)
        {
            if(currentAnimSet != AnimationSet.MAGIC)
            {
                ChangeForm(AnimationSet.MAGIC);
                //return;
            }

            //inSpellChargeLoop = true;
            Animator.CrossFade("CastSpell", 0.1f, 2);
        }
    }

    internal void PlayMotion_ReadySpells()
    {
        //animator.SetInteger("iMotionIndex", 9);
        //animator.CrossFade("Magic Idle", 0.2f, 0);
        ChangeForm(AnimationSet.MAGIC);
        Animator.CrossFade("SwitchSpell", 0.1f, 2);
    }

    internal void PlayMotion_SwitchSpell()
    {
        //animator.SetInteger("iMotionIndex", 9);
        //animator.CrossFade("Magic Idle", 0.2f, 0);
        Animator.CrossFade("SwitchSpell", 0.1f, 2);
    }
    //void PlayMotion_DrawSpell(int motionIndex, int handIndex)
    //{
    //    animator.SetInteger("iMotionIndex", motionIndex);

    //    if(handIndex == 0)
    //    {
    //        animator.SetTrigger("tDrawSpellLH");
    //    }
    //    else
    //    {
    //        animator.SetTrigger("tDrawSpellRH");
    //    }

    //    ChangeForm(9);
    //}

    internal void PlayMotion_SheathSpell()
    {
        Animator.CrossFade("SwitchSpell", 0.1f, 2);
        //animator.CrossFade("Idle", 0.2f, 0);
        ChangeForm(AnimationSet.DEFAULT);
    }

    internal void PlayMotion_DrawWeapon(AnimationSet animSet)
    {
        ChangeForm(animSet);
        int animIndex = -1;
        switch(animSet)
        {
            //case AnimationSet.UNARMED:

            case AnimationSet.DAGGER:
            case AnimationSet.ONEHANDED:
                animIndex = 0;
                break;
            case AnimationSet.TWOHANDED:
                animIndex = 1;
                break;
            case AnimationSet.BOW:
            case AnimationSet.XBOW:
                animIndex = 2;
                break;
        }

        Animator.SetInteger("iAnimationSetIndex", animIndex);

        if(animIndex > -1)
            Animator.Play("Draw", 4);
    }

    internal void PlayMotion_SheathWeapon(AnimationSet animSet)
    {
        ChangeForm(AnimationSet.DEFAULT);
        int animIndex = -1;
        switch(animSet)
        {
            case AnimationSet.DAGGER:
            case AnimationSet.ONEHANDED:
                animIndex = 0;
                break;
            case AnimationSet.TWOHANDED:
                animIndex = 1;
                break;
            case AnimationSet.BOW:
            case AnimationSet.XBOW:
                Animator.Play("Cancel", 3);
                animIndex = 2;
                break;
        }



        Animator.SetInteger("iAnimationSetIndex", animIndex);

        if(animIndex > -1)
            Animator.Play("Sheath", 4);
    }

    internal void PlayMotion_Attack(AnimationSet animationSet)
    {
        if(currentAnimSet != animationSet)
        {
            ChangeForm(animationSet);
            return;
        }

        if(IsIncapacitated())
        {
            return;
        }

        isAttacking = true;

        if(self.debugAnimation)
            Debug.Log("3. (Animation) " + self.GetName() + ": Playing attack motion for weapon category '" + animationSet + "'");
        //int numAttackMotions = 0; // If num doesn't change -> attack with index of 0
        //if(motionIndex > -1)
        //{
        //    var weaponIndex = motionIndex;
        //    animator.SetInteger("iMotionIndex", weaponIndex);
        //}
        //if(weaponIndex == 1 || weaponIndex == 2) {
        //    numAttackMotions = GetBlendTreeMotionsCount(2);
        //    int wIdx = Random.Range(0, numAttackMotions);

        //    _animator.SetInteger("iAttackIndex_1H", wIdx);
        //}

        if(self.ActorStats.isBeast)
        {
            Animator.CrossFade("Attack", 0.2f, 1);
            return;
        }

        //if(animator.GetFloat("fIdleStance") == 0)
        //{
        //    animator.SetFloat("fIdleStance", 1/*, 0.2f, Time.deltaTime*/);
        //}

        switch(animationSet)
        {
            case AnimationSet.UNARMED:
                Animator.SetInteger("iAttackVariant", UnityEngine.Random.Range(0, 6));
                Animator.CrossFade("Attack Unarmed", 0.1f, 1);
                break;
            case AnimationSet.ONEHANDED:
            case AnimationSet.DAGGER:
                Animator.SetInteger("iAttackVariant", UnityEngine.Random.Range(0, 7));
                Animator.CrossFade("Attack", 0.1f, 1);
                break;
            //case WeaponCategory.Dual:
            //    break;
            //case WeaponCategory.TwoHandedSword:
            //    break;
            //case WeaponCategory.TwoHandedHammer:
            //    break;
            case AnimationSet.BOW:
            case AnimationSet.XBOW:
                Animator.Play("Aim", 3); // Using "Play" here, since releasing arrow should look snappy.
                break;
            default:
                Debug.LogError("Invalid animset in PlayMotion_Attack");
                break;
        }
        StartCoroutine(CR_AttackingDone(1));
        //m_agent.Execute_BlockAggro(2);
        //Debug.Log("3. (Animation) " + gameObject.name + ": Playing attack motion and waiting for animation event to call 'Attack'");
        //int numAttackMotions = 0; // If num doesn't change -> attack with index of 0
        //if(motionIndex > -1)
        //{
        //    int weaponIndex = motionIndex;
        //    animator.SetInteger("iMotionIndex", weaponIndex);
        //}
        //if(weaponIndex == 1 || weaponIndex == 2) {

        //    numAttackMotions = GetBlendTreeMotionsCount(2);
        //    int wIdx = Random.Range(0, numAttackMotions);

        //    _animator.SetInteger("iAttackIndex_1H", wIdx);
        //}

    }

    private IEnumerator CR_AttackingDone(float delay)
    {
        yield return new WaitForSeconds(delay);
        isAttacking = false;
    }

    internal void PlayMotion_HandleBow(int stage)
    {
        // Stage -1: Cancel
        // Stage 0: Draw
        // Stage 1: Release
        if(stage == -1)
            Animator.Play("Cancel", 3);
        else if(stage == 0)
        {
            //if(isAttacking)
            //{
            //    return;
            //}
            //isAttacking = true;
            Animator.CrossFade("Aim", 0.1f, 3);
            //StartCoroutine(CR_AttackingDone(1));
        }
        else if(stage == 1)
            Animator.SetTrigger("tAttack");

    }

    public void PlayMotion_Evade(float height)
    {
        if(isAttacking || IsIncapacitated())
        {
            return;
        }

        Animator.SetFloat("fEvadeHeight", height);
        Animator.Play("Evade", 8);
        isEvading = true;
        StartCoroutine(CR_EvadingDone());
    }

    private IEnumerator CR_EvadingDone()
    {
        yield return new WaitForSeconds(1);
        isEvading = false;
    }

    public void PlayMotion_Block(AnimationSet animationSet)
    {
        if(isAttacking || IsIncapacitated())
        {
            return;
        }
        
        //animator.SetFloat("fEvadeHeight", height);
        Animator.Play("Block", 8);
        isBlocking = true;
    }

    public void PlayMotion_StopBlocking()
    {
        Animator.Play("CancelBlock", 8);
        isBlocking = false;
    }

    public void PlayMotion_BlockAttack(int blockerWeaponTypeIndex)
    {

        Animator.SetTrigger("tBlockAttack");
    }

    public void PlayMotion_BleedOut(int stage)
    {
        if(inBleedOutState)
            return;

        if(self.NavAgent != null)
            self.NavAgent.isStopped = true;

        inBleedOutState = true;

        CancelAllAttackAnimations();

        Animator.CrossFade("BleedOut", 0.2f, 10);

        StartCoroutine(CR_BleedOutDone());
    }

    private IEnumerator CR_BleedOutDone()
    {
        yield return new WaitForSeconds(4);
        Animator.Play("Cancel BleedOut", 10);
        //yield return new WaitForEndOfFrame();
        yield return new WaitUntil(() => Animator.GetCurrentAnimatorStateInfo(10).IsName("New State"));
        inBleedOutState = false;

        if(self.NavAgent != null)
            self.NavAgent.isStopped = false;
    }

    public void PlayMotion_Stagger()
    {
        isStaggered = true;

        CancelAllAttackAnimations();

        if(inBleedOutState)
        {
            if(Animator.GetCurrentAnimatorStateInfo(10).IsName("BleedOut Humanoid 1 Loop") == false)
            {
                isStaggered = false;
                return;
            }
            Animator.Play("BleedOut Hit", 10);
        }
        else
            Animator.Play("Stagger", 11);

        StartCoroutine(CR_StaggerDone());
    }

    private IEnumerator CR_StaggerDone()
    {
        yield return new WaitForSeconds(1);
        isStaggered = false;
    }

    public bool IsIncapacitated()
    {
        return isDowned || isEvading || isBlocking || inBleedOutState;
    }

    public void PlayMotion_Jump(int stage/*Vector3 targetPosition*/)
    {
        switch(stage)
        {
            case 0:
                Animator.CrossFade("Jump", 0.1f, 7);
                break;
            case 1:
                Animator.SetBool("bFall", true);
                //animator.CrossFade("Fall Loop", 0.1f, 7);
                break;
            case 2:
                Animator.SetBool("bFall", false);
                //animator.CrossFade("Land", 0.1f, 7);
                break;
        }


    }

    private void CancelAllAttackAnimations()
    {
        Animator.Play("Cancel", 1);
        Animator.Play("Cancel", 2);
    }

    //public override void Collapse(Agent source) {

    //base.Collapse(source);
    //ChangeAgentState(DeadState.Instance);
    //}
    public virtual void KnockDown(Vector3 force, float duration, bool markDead)
    {
        if(self.NavAgent != null)
            self.NavAgent.enabled = false;
        Animator.applyRootMotion = true;
        Animator.enabled = false;
        //animator.StopPlayback();
        EnableRagdollPhysics(force);


        if(markDead)
        {
            self.transform.gameObject.layer = LayerMask.NameToLayer("Corpse");
            //self.transform.gameObject.layer
            self.StartCoroutine(CR_LingerOnGroundEssential(duration));
        }
        else
        {
            self.StartCoroutine(CR_LingerOnGround(duration));
        }
        //animator.enabled = false;
    }

    public virtual void Collapse(Vector3 force)
    {

        //animator.enabled = false;
        if(self.NavAgent != null)
            self.NavAgent.enabled = false;
        EnableRagdollPhysics(force);
        self.StartCoroutine(CR_LingerOnGround(1000));
        //animator.enabled = false;
    }

    public void CollapseDead(ActorInput source, Vector3 force)
    {
        self.isDowned = true;
        //animator.SetTrigger("tDie");

        //if(self.debug)
        Debug.Log("<color=grey>" + self.GetName() + " Falling dead</color>");
        //animator.enabled = false;
        Animator.enabled = false;
        if(self.NavAgent != null)
        {
            //self.navAgent.isStopped = true;
            self.NavAgent.enabled = false;
        }

        //if(animator.isHuman)
        EnableRagdollPhysics(transform.position);
        //else
        //{
        //}
        //animator.StopPlayback();
        //animator.Rebind();

        //animator.SetBool("bMirrorDeath", Random.value > 0.5f);
        //animator.Play("Die", 0);
        self.transform.gameObject.layer = LayerMask.NameToLayer("Corpses");
        if(self.ActorStats.HasActorFlag(ActorFlags.ESSENTIAL))
        {
            self.StartCoroutine(CR_LingerOnGroundEssential(10f));
        }
    }

    private IEnumerator CR_LingerOnGround(float duration)
    {
        yield return new WaitForSeconds(duration);
        //yield return new WaitWhile(() => animator.GetCurrentAnimatorStateInfo(6).IsName("New State") == false);

        DisableRagdollPhysics();
        bool onBack = Vector3.Dot(chest.forward, Vector3.up) > 0f;
        Animator.enabled = true;
        if(onBack)
            Animator.Play("Stand Up From Back", 6);
        else
            Animator.Play("Stand Up From Belly", 6);

        yield return new WaitForSeconds(Animator.GetCurrentAnimatorStateInfo(6).length);

        //yield return new WaitForSeconds(2);
        //yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(6).IsName("New State"));
        Animator.applyRootMotion = false;

        self.isDowned = false;
        if(self.NavAgent != null)
        {
            self.transform.position = HelperFunctions.GetSampledNavMeshPosition(self.transform.position);
            self.NavAgent.enabled = true;
            self.NavAgent.destination = self.transform.position;
            self.NavAgent.isStopped = false;
        }
    }

    private IEnumerator CR_LingerOnGroundEssential(float duration)
    {
        yield return new WaitForSeconds(duration);
        //yield return new WaitWhile(() => animator.GetCurrentAnimatorStateInfo(6).IsName("New State") == false);

        DisableRagdollPhysics();
        bool onBack = Vector3.Dot(chest.forward, Vector3.up) > 0f;
        Animator.enabled = true;
        if(onBack)
            Animator.Play("Stand Up From Back", 6);
        else
            Animator.Play("Stand Up From Belly", 6);

        yield return new WaitForEndOfFrame();
        self.transform.gameObject.layer = LayerMask.NameToLayer("Actors");
        yield return new WaitForSeconds(Animator.GetCurrentAnimatorStateInfo(6).length);

        //yield return new WaitForSeconds(2);
        //yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(6).IsName("New State"));
        Animator.applyRootMotion = false;

        self.isDowned = false;
        if(self.NavAgent != null)
        {
            self.transform.position = HelperFunctions.GetSampledNavMeshPosition(self.transform.position);
            self.NavAgent.enabled = true;
            self.NavAgent.destination = self.transform.position;
            self.NavAgent.isStopped = false;
        }
    }

    private void EnableRagdollPhysics(Vector3 sourceForce)
    {

        foreach(Rigidbody rb in _rigidbodies)
        {

            Collider c = rb.GetComponent<Collider>();

            //if(rb.gameObject.GetInstanceID() == agent.gameObject.GetInstanceID())
            //{

            //Destroy(rb);
            //Destroy(c);
            //continue;
            //}

            if(c != null)
                c.isTrigger = false;

            rb.isKinematic = false;
            rb.useGravity = true;
            rb.mass = 15f;
            rb.angularDrag = 5f;

        }
        _rigidbodies[0].AddForce(sourceForce, ForceMode.Impulse);
    }

    private void DisableRagdollPhysics()
    {

        foreach(Rigidbody rb in _rigidbodies)
        {
            Collider c = rb.GetComponent<Collider>();
            if(c != null)
                c.isTrigger = true;

            rb.isKinematic = true;
            //rb.useGravity = false;
        }
    }

    public void SetAnimator(Animator animator)
    {
        Animator = animator;
    }

    //protected float GetWaterDepthAtMe()
    //{
    //    Ray ray = new Ray(new Vector3(self.transform.position.x, _waterSurfaceHeight, self.transform.position.z), Vector3.down);
    //    RaycastHit hit;
    //    if(Physics.Raycast(ray, out hit, 100, 1 << LayerMask.NameToLayer("Ground") | 1 << LayerMask.NameToLayer("Obstacles")))
    //    {
    //        return hit.distance;
    //    }

    //    return 0;
    //}

    public void FindActorBodyparts()
    {
        if(Animator.isHuman)
        {
            chest = Animator.GetBoneTransform(HumanBodyBones.Chest);
            head = Animator.GetBoneTransform(HumanBodyBones.Head);
        }
        else //! For generic character models
        {
            head = transform.FindDeepChild("head");
            chest = transform.FindDeepChild("chest");
        }

        if(head == null)
            head = transform;

        if(chest == null)
            chest = transform;
    }
}
