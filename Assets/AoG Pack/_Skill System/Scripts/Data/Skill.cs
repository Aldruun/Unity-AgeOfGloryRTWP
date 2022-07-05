//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;
//using UnityEngine;

////public enum SkillEffect {

////    None,
////    ModifyHealth,
////    ModifyMaxHealth,
////    ModifyMana,
////    ModifyMaxMana,
////    ModifyPhysAttack,
////    ModifyPhysDefense,
////    ModifyMagAttack,
////    ModifyMagDefense,
////    DamagePhysical,
////    DamageHoly,
////    DamageUnholy,
////    DamageFire,
////    DamageIce,
////    DamageLightning
////};

//public enum ActorMeshEffectType
//{
//    None,
//    Fire,
//    Ice,
//    Lightning,
//    Holy
//}

//public abstract class Skill : ScriptableObject
//{
//    private Animator _animator;
//    public float activationRange;
//    public string animStateName;
//    public float cooldown = 2;
//    internal float cooldownTimer;
//    public Action<float> CooldownHook;

//    [TextArea] public string description;
//    public ActorMeshEffectType ActorMeshEffectType;
//    public DamageType DamageType;
//    public SpellTargetType SpellTargetType;
//    public DeliveryType DeliveryType;
//    public float fwdStartOffset;
//    //public List<StatusEffect> statusEffectsTarget;
//    public float effectValue = 10101;
//    public float manaCost;
//    public int animatorLayer;
//    public int motionID;
//    public bool needsWeaponDrawn;

//    public Action OnSkillAnimationFinished;
//    public float recoveryTime;
//    public Sprite skillIcon;

//    public string skillName;
//    public string startVFXIdentifier;
//    public float vfxDelay;
//    public string vfxIdentifier;
//    public string vfx_impact_identifier;
//    public AudioClip sfx_chargeSkill;
//    public AudioClip[] sfxlist_releaseSkill;
//    internal bool hidden;
//    public bool repeatIfNoAI;

//    public int priority { get; set; }
//    public Transform vfxPoint { get; set; }

//    public Actor skillTarget { get; protected set; }
//    public Vector3 skillTargetLocation { get; protected set; }

//    public virtual void Init()
//    {
//        //if(animStateName != "")
//        //{
//        //    _animator = actor.animator;

//        //    AnimationClip[] clips = _animator.runtimeAnimatorController.animationClips;
//        //    foreach(AnimationClip clip in clips)
//        //        if(clip.name == animStateName)
//        //            //Debug.Log(GetType().Name + ": Name found. Length = " + clip.length);
//        //            recoveryTime = clip.length;
//        //}


//        //Debug.Log("_____weapon:" + actor.agentGear.m_currentWeapon.name);
//        //Debug.Log("_____vfx:" + vfxPoint.name);

//        //if(vfxPoint)
//        //{

//        //}
//    }

//    public void SetPlayerSkillTarget(Actor target)
//    {
//        skillTarget = target;
//    }

//    public abstract bool ConditionsMetAI(NPCInput actor);
//    public abstract bool ConditionsMetPlayer(Actor actor);

//    public bool CanActivate(NPCInput actor)
//    {
//        // Test all conditions that are common across all skills + all unique skill conditions
//        //if(actor.manaPoints < manaCost)
//        //{
//        //    if(actor.debugInput)
//        //        Debug.Log(actor.GetNameAndClass() + ":<color=yellow>*</color> [" + skillName + "]Skill conditions not met - actor.manaPoints < manaCost");

//        //    return false;
//        //}
//        if(cooldownTimer > 0)
//        {
//            if(actor.debugInput)
//                Debug.Log(actor.GetName() + ":<color=yellow>*</color> [" + skillName + "]Skill conditions not met - _cooldownTimer > 0");

//            return false;
//        }
//        if(ConditionsMetAI(actor) == false)
//        {
//            if(actor.debugInput)
//                Debug.Log(actor.GetName() + ":<color=yellow>*</color> [" + skillName + "]Skill conditions not met - ConditionsMet(actor) == false");
//            return false;
//        }

//        return true;
//    }

//    public bool PlayerCanActivate(Actor actor)
//    {
//        // Test all conditions that are common across all skills + all unique skill conditions
//        if(/*actor.manaPoints < manaCost ||*/ /*_cooldownTimer > 0 ||*/ ConditionsMetPlayer(actor) == false)
//            return false;

//        return true;
//    }

//    internal virtual float GetActivationRange(Actor self)
//    {
//        return activationRange;
//    }

//    public void Activate(Actor self, Actor target, Vector3 targetPosition)
//    {
//        if(skillTarget == null)
//            Debug.LogError("Target is null");
//        else if(self.debugInput)
//            Debug.Log(self.GetName() + ": Skill activated <color=white>" + skillName + "</color>");

//        //if(self.Equipment.equippedWeapon.weaponObject != null)
//        //{
//        //    vfxPoint = self.Equipment.equippedWeapon.weaponObject.transform.Find("vfx");
//        //}
//        //else
//        //{
//        if(vfxPoint == null)
//        {
//            vfxPoint = self.Equipment.m_weaponHand;

//            if(vfxPoint == null)
//            {
//                vfxPoint = self.transform.Find("vfx");

//                if(vfxPoint == null)
//                {
//                    vfxPoint = new GameObject("vfx").transform;
//                    vfxPoint.SetParent(self.transform);
//                    vfxPoint.position = self.transform.position + Vector3.up * 1.4f + self.transform.forward * 0.5f;
//                }  
//            }
//        }
//        //}

//        ActivateSkill(self, target, targetPosition);
//        self.StartCoroutine(CR_Cooldown());
//    }

//    public virtual void ActivateSkill(Actor self, Actor target, Vector3 targetPosition)
//    {
//        IndividualSetup(self);

//        //self.Execute_ModifyMana(-manaCost, ModType.ADDITIVE);

//        if(motionID > -1)
//            self.Animation.PlaySpellAnimation(motionID);

//        if(startVFXIdentifier != "")
//        {
//            GameObject startVFX = PoolSystem.GetPoolObject(startVFXIdentifier, ObjectPoolingCategory.VFX);
//            VFXPlayer.TriggerVFX(startVFX, vfxPoint, Vector3.zero, self.transform.rotation, false, 3);
//        }

//        if(sfx_chargeSkill != null)
//            SFXPlayer.TriggerSFX(sfx_chargeSkill, self.transform.position);

//        self.Combat.OnReleaseSpell = () =>
//        {
//            if(sfxlist_releaseSkill.Length > 0)
//                SFXPlayer.TriggerSFX(sfxlist_releaseSkill[UnityEngine.Random.Range(0, sfxlist_releaseSkill.Length)], self.transform.position);
//            SpawnVFX(self, target, targetPosition);
//        };
//    }

//    /// <summary>
//    /// Optional skill setup 
//    /// </summary>
//    /// <param name="actor"></param>
//    public abstract void IndividualSetup(Actor actor);
//    public abstract void SpawnVFX(Actor self, Actor target, Vector3 targetPosition);

//    private IEnumerator CR_Cooldown()
//    {
//        cooldownTimer = cooldown;

//        while(cooldownTimer > 0)
//        {
//            CooldownHook?.Invoke(cooldownTimer);
//            cooldownTimer -= Time.deltaTime;
//            yield return null;
//        }

//        cooldownTimer = 0;
//    }

//    public Actor GetFriendsInRangeNonAlloc(Actor caller, float range,
//        Collider[] populatedArray)
//    {
//        int numHit = Physics.OverlapSphereNonAlloc(caller.transform.position, range, populatedArray,
//            1 << LayerMask.NameToLayer("Actors"));

//        if(numHit == 0)
//            return null;

//        List<Actor> agentList = new List<Actor>();

//        for(int i = 0; i < numHit; i++)
//        {
//            Actor actor = populatedArray[i].GetComponent<Actor>();

//            if( /*actor != _agent &&*/
//                actor.dead == false && actor.ActorStats.IsFriend(caller.ActorStats) && actor.Combat.GetHostileTarget() != null &&
//                actor.ActorStats.isBeingBuffed == false)
//                agentList.Add(actor);
//        }

//        return agentList.OrderBy(a => (a.Combat.GetHostileTarget().transform.position - a.transform.position).sqrMagnitude)
//            .FirstOrDefault();
//    }

//    protected void ClampTo(Actor self, Transform obj, Transform target, float duration)
//    {
//        self.StartCoroutine(CR_ClampToAndFade(obj, target, duration));
//        //SceneManagment.FadeTo(obj.gameObject, 0, 0, 1, SceneManagment.OnFadeDoneAction.Disable);
//    }

//    private IEnumerator CR_ClampToAndFade(Transform obj, Transform target, float duration)
//    {
//        while(duration > 0 && target != null)
//        {
//            obj.position = target.position;
//            duration -= Time.deltaTime;
//            yield return null;
//        }

//        obj.gameObject.SetActive(false);
//    }
//}