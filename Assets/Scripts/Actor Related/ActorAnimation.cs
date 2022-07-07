using GenericFunctions;
using System;
using System.Collections;
using UnityEngine;

public enum MovementSpeed
{

    Walk,
    Run
}

public class ActorAnimation : MonoBehaviour
{
    public Actor self { get; protected set; }
    public Animator Animator { get; protected set; }
    public MovementSpeed m_movementSpeed;
    protected AnimationSet currentAnimSet;
    public bool m_grounded = true;
    private Rigidbody[] _rigidbodies;

    internal bool isAttacking;
    internal bool isStaggered;
    private readonly bool isDowned;

    private Vector3 velocity;
    private float movementSpeed;

    public Transform head;
    public Transform chest;
    public bool inSpellChargeLoop { get; set; }

    private int hash_float_velocityx;
    private int hash_float_velocityz;
    private int hash_float_speed;

    private int animatorHash_AimLoopBow;

    private int hash_int_animationsetindex;

    private int hash_int_attackvariant;
    private int hash_state_attack;
    private int hash_state_attackunarmed;

    private int hash_int_castmotionindex;
    private int hash_state_chargepell;
    private int hash_state_chargepellfast;
    private int hash_state_castspell;
    private int hash_state_idle;
    private int hash_state_idleunarmed;
    private int hash_state_idle1h;
    private int hash_state_idlebow;
    private int hash_state_idlemagic;
    private int hash_state_cancelspell;
    private int hash_state_cancelattack;
    private int hash_state_cancelbow;

    public virtual void Initialize(Actor actor, Animator animator)
    {
        if(actor.debugInitialization)
        {
            Debug.Log($"<color=grey>{actor.GetName()}: Initializing Animation Component</color>");
        }

        //_rigidbodies = GetComponentsInChildren<Rigidbody>();
        self = actor;
        this.Animator = animator;

        hash_float_velocityx = Animator.StringToHash("VelocityX");
        hash_float_velocityz = Animator.StringToHash("VelocityZ");
        hash_float_speed = Animator.StringToHash("Speed");

        animatorHash_AimLoopBow = Animator.StringToHash("Bow Layer.Bow_IdleDrawn");

        hash_int_animationsetindex = Animator.StringToHash("iAnimationSetIndex");

        hash_int_attackvariant = Animator.StringToHash("iAttackVariant");
        hash_state_attack = Animator.StringToHash("Attack Layer.Attack 1H.Attack");
        hash_state_attackunarmed = Animator.StringToHash("Attack Layer.Attack 1H.Attack Unarmed");

        hash_int_castmotionindex = Animator.StringToHash("iCastMotionIndex");
        hash_state_chargepell = Animator.StringToHash("Spellcasting Layer.ChargeSpell");
        hash_state_chargepellfast = Animator.StringToHash("Spellcasting Layer.ChargeSpellFast");
        hash_state_castspell = Animator.StringToHash("Spellcasting Layer.CastSpell");

        hash_state_cancelspell = Animator.StringToHash("Spellcasting Layer.Cancel");
        hash_state_cancelattack = Animator.StringToHash("Attack Layer.Cancel");
        hash_state_cancelbow = Animator.StringToHash("Bow Layer.Cancel");

        hash_state_idle = Animator.StringToHash("Base Layer.Default.Idle");
        hash_state_idleunarmed = Animator.StringToHash("Base Layer.Unarmed Combat.UC Idle");
        hash_state_idle1h = Animator.StringToHash("Base Layer.1H.1H Idle");
        hash_state_idlebow = Animator.StringToHash("Base Layer.Bow.Bow Idle");
        hash_state_idlemagic = Animator.StringToHash("Base Layer.Spellcasting.Magic Idle");

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
                Animator.Play(hash_state_cancelbow, 3);
                break;
            case AnimationSet.MAGIC:
                Animator.Play(hash_state_cancelspell, 2);
                break;
        }

        switch(formID)
        {
            case AnimationSet.DEFAULT:
                Animator.CrossFade(hash_state_idle, 0.1f, 0);
                break;
            case AnimationSet.UNARMED:
                Animator.CrossFade(hash_state_idleunarmed, 0.1f, 0);
                break;
            case AnimationSet.ONEHANDED:
                Animator.CrossFade(hash_state_idle1h, 0.1f, 0);
                break;
            case AnimationSet.TWOHANDED:
                Debug.LogError("ChangeForm(): 2H AnimationPackage not implemented yet");
                break;
            case AnimationSet.BOW:
                Animator.CrossFade(hash_state_idlebow, 0.1f, 0);
                break;
            case AnimationSet.XBOW:
                Debug.LogError("ChangeForm(): XBow AnimationPackage not implemented yet");
                break;
            case AnimationSet.MAGIC:
                Animator.CrossFade(hash_state_idlemagic, 0.1f, 0);
                break;
            default:
                Debug.LogError("Invalid formID");
                break;
        }
    }

    //public void UpdatePlayerMovementAnimation()
    private void OnAnimatorMove()
    {
        if(self == null || self.dead)
        {
            return;
        }

        //HandleRootmotion();

        ApplyTranslationFromCurve();

        Animator.SetFloat(hash_float_velocityx, velocity.x, 0.04f, Time.deltaTime);
        Animator.SetFloat(hash_float_velocityz, velocity.z, 0.04f, Time.deltaTime);
        Animator.SetFloat(hash_float_speed, movementSpeed, 0.1f, Time.deltaTime);
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

    /// <summary>
    /// Stage 0: Start charging, Stage 2: cast
    /// </summary>
    /// <param name="spell"></param>
    /// <param name="stage"></param>
    /// <param name="motionIndex"></param>
    public void PlayMotion_HandleSpell(int stage, int castMotionIndex)
    {
        // handIndex: 0 = left, 1 = right
        if(castMotionIndex > -1)
        {
            Animator.SetInteger(hash_int_castmotionindex, castMotionIndex);
        }

        if(self.debug)
        {
            Debug.Log(self.GetName() + ": ### Motion start -> " + (stage == 0 ? "charging" : stage == 1 ? "releasing" : "cancelling") +
                     " right" + " hand spell");
        }

        if(stage == 0)
        {
            Animator.CrossFade(castMotionIndex == 0 ? hash_state_chargepellfast : hash_state_chargepell, 0.2f, 2);
        }
        else if(stage == 1)
        {
            Animator.Play(hash_state_castspell);
        }
        else
        {
            Animator.Play(hash_state_cancelspell);
        }
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

        Animator.SetInteger(hash_int_animationsetindex, animIndex);

        if(animIndex > -1)
        {
            Animator.Play("Draw", 4);
        }
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
                Animator.Play(hash_state_cancelbow, 3);
                animIndex = 2;
                break;
        }

        Animator.SetInteger(hash_int_animationsetindex, animIndex);

        if(animIndex > -1)
        {
            Animator.Play("Sheath", 4);
        }
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
        {
            Debug.Log("3. (Animation) " + self.GetName() + ": Playing attack motion for weapon category '" + animationSet + "'");
        }
   
        if(self.ActorStats.isBeast)
        {
            Animator.CrossFade(hash_state_attack, 0.2f, 1);
            return;
        }

        switch(animationSet)
        {
            case AnimationSet.UNARMED:
                Animator.SetInteger(hash_int_attackvariant, UnityEngine.Random.Range(0, 6));
                Animator.CrossFade(hash_state_attackunarmed, 0.1f, 1);
                break;
            case AnimationSet.ONEHANDED:
            case AnimationSet.DAGGER:
                Animator.SetInteger(hash_int_attackvariant, UnityEngine.Random.Range(0, 7));
                Animator.CrossFade(hash_state_attack, 0.1f, 1);
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
        {
            Animator.Play(hash_state_cancelbow);
        }
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
        {
            Animator.SetTrigger("tAttack");
        }
    }

    public void PlayMotion_Stagger()
    {
        isStaggered = true;

        CancelAllAttackAnimations();

        Animator.Play("Stagger", 11);
        
        StartCoroutine(CR_StaggerDone());
    }

    private IEnumerator CR_StaggerDone()
    {
        yield return new WaitForSeconds(1);
        isStaggered = false;
    }

    internal void PlaySpellCastAnimation(Spell spell, int releaseMotionIndex)
    {
        throw new NotImplementedException();
    }

    public bool IsIncapacitated()
    {
        return isDowned;
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
        Animator.Play(hash_state_cancelattack);
        Animator.Play(hash_state_cancelspell);
        Animator.Play(hash_state_cancelbow);
    }

    //public override void Collapse(Agent source) {

    //base.Collapse(source);
    //ChangeAgentState(DeadState.Instance);
    //}
    public virtual void KnockDown(Vector3 force, float duration, bool markDead)
    {
        if(self.NavAgent != null)
        {
            self.NavAgent.enabled = false;
        }

        Animator.applyRootMotion = true;
        Animator.enabled = false;
        //animator.StopPlayback();
        EnableRagdollPhysics(force);


        if(markDead)
        {
            self.transform.gameObject.layer = LayerMask.NameToLayer("Corpses");
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
        {
            self.NavAgent.enabled = false;
        }

        EnableRagdollPhysics(force);
        self.StartCoroutine(CR_LingerOnGround(1000));
        //animator.enabled = false;
    }

    public void CollapseDead(Actor source, Vector3 force)
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
        {
            Animator.Play("Stand Up From Back", 6);
        }
        else
        {
            Animator.Play("Stand Up From Belly", 6);
        }

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
        {
            Animator.Play("Stand Up From Back", 6);
        }
        else
        {
            Animator.Play("Stand Up From Belly", 6);
        }

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
            {
                c.isTrigger = false;
            }

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
            {
                c.isTrigger = true;
            }

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
        {
            head = transform;
        }

        if(chest == null)
        {
            chest = transform;
        }
    }
}
