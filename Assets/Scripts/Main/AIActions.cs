using GenericFunctions;
using System;
using System.Collections;
using UnityEngine;

public enum CastSate
{
    Preparation, // Move to and adjust facing
    Incantation,
    Release,
    Cooldown
}

public static class AIActions
{
    public class Action_DrawWeapon : GameAction
    {
        private float _actTimer = 1;
        private readonly bool done;

        public Action_DrawWeapon(Actor self) : base(self)
        {
            this.self = self;
        }

        public Action_DrawWeapon Set(bool draw)
        {
            //if(draw)
            //{
            //    _self.Execute_DrawWeapon();
            //}
            //else
            //{
            //    _self.Execute_SheathWeapon(); 
            //}
            return this;
        }

        public override bool Done(Actor self)
        {
            _actTimer -= Time.deltaTime;
            if(_actTimer <= 0)
            {
                return done;
            }
            return done;
        }

        public override bool InRange()
        {
            return true;
        }

        public override void Release()
        {

        }
    }

    public class Action_Attack : GameAction
    {
        private bool done;

        public Action_Attack(Actor self) : base(self)
        {
            this.self = self;
        }

        public override bool Done(Actor self)
        {
            if(target == null || target.dead)
            {
                return true;
            }
            if(self == null || self.dead)
            {
                return true;
            }


            if(Actions.MoveIntoRange(self, target.transform.position, requiredRange, requiresFacing))
            {
                if(self.RoundSystem.CanPerformAttack())
                {
                    self.HoldPosition();
                    self.Combat.Execute_BlockAggro(2);
                    self.Combat.Execute_Attack();
                }

            }

            return done;
        }

        internal GameAction Set(Actor target)
        {
            if(self.debug)
            {
                Debug.Log(self.GetName() + ": Attack Action");
            }

            if(self.Equipment.equippedWeapon.Weapon == null)
            {
                self.Equipment.SetUpFist();
                //_self.Combat.Execute_EquipBestWeapon(Constants.EQUIP_ANY, true, true);
            }

            Debug.Assert(self.Equipment != null);
            Debug.Assert(self.Equipment.equippedWeapon != null);
            Debug.Assert(self.Equipment.equippedWeapon.Weapon != null);
            requiredRange = self.Equipment.equippedWeapon.Weapon.Range;
            done = false;
            this.target = target;
            return this;
        }

        public override Vector3 GetActionTargetPosition()
        {
            return GetActionTargetPosition();

        }

        public override bool InRange()
        {
            return true;
        }

        public override void Release()
        {
            //_self.Animation.Animator.SetTrigger("tCancelAttack");
            done = true;
        }
    }
    public class Action_Attack_Once : GameAction
    {
        private bool attacked;
        private bool done;

        public Action_Attack_Once(Actor self) : base(self)
        {
            this.self = self;
        }

        public override bool Done(Actor self)
        {
            if(target == null || target.dead)
            {
                return true;
            }
            if(self == null || self.dead)
            {
                return true;
            }

            if(attacked == false && Actions.MoveIntoRange(self, target.transform.position, requiredRange, requiresFacing))
            {
                if(self.RoundSystem.CanPerformAttack())
                {
                    attacked = true;
                    self.StartCoroutine(CR_Attack(self));
                }

            }

            return done;
        }

        private IEnumerator CR_Attack(Actor self)
        {
            self.HoldPosition();
            self.Combat.Execute_BlockAggro(1.5f);
            self.Combat.Execute_Attack();
            yield return new WaitForSeconds(1.5f);
            done = true;
        }

        internal GameAction Set(Actor target)
        {
            if(self.debug)
            {
                Debug.Log("Attack Action");
            }

            if(self.Combat.GetEquippedWeapon() == null)
            {
                self.Equipment.SetUpFist();
            }
            else if(self.Combat.GetEquippedWeapon().weaponCategory == WeaponCategory.Unarmed)
            {

            }

            requiredRange = self.Equipment.equippedWeapon.Weapon.Range;
            attacked = false;
            done = false;
            this.target = target;
            return this;
        }

        public override Vector3 GetActionTargetPosition()
        {
            return GetActionTargetPosition();
        }

        public override bool InRange()
        {
            return true;
        }

        public override void Release()
        {
            //_self.Animation.Animator.SetTrigger("tCancelAttack");
            done = true;
            //_self.Animation.Animator.SetTrigger("tCancelAttack");
        }
    }

    public class Action_CastSpellAtActor : GameAction
    {
        public CastSate castState = CastSate.Preparation;
        private float _preCastTimer = 2f;
        private float _postCastTimer = 3f;
        private float _castingTime;
        private float _duration;

        internal Spell spell;

        public Action_CastSpellAtActor(Actor self) : base(self)
        {
            this.self = self;
        }

        internal GameAction Set(Actor target, Spell spell)
        {
            this.spell = self.Combat.equippedSpell = spell;
            requiredRange = spell.activationRange;
            this.target = target;
            castState = CastSate.Preparation;
            _preCastTimer = 1f;
            _postCastTimer = 3f;
            self.Combat.noStagger = false;
            _duration = spell.duration * 6;
            return this;
        }

        public override bool Done(Actor self)
        {
            if(target == null || target.dead)
            {
                return true;
            }

            if(self.debugSpellCastStates)
            {
                Debug.Log(self.GetName() + ": Updating castspellatactor");
            }

            switch(castState)
            {
                case CastSate.Preparation:

                    if(self.debugSpellCastStates)
                    {
                        Debug.Log(self.GetName() + ": <color=cyan>CastSpellAtActor: Preparation</color>");
                    }

                    if(spell == null)
                    {
                        if(self.debugSpellCastStates)
                        {
                            Debug.Log(self.GetName() + ": <color=red>Spell null</color>");
                        }
                    }

                    if(target == null)
                    {
                        if(self.debugSpellCastStates)
                        {
                            Debug.Log(self.GetName() + ": <color=red>Target null</color>");
                        }
                    }

                    if(self.isCasting)
                    {
                        return false;
                    }

                    if(Actions.MoveIntoRange(self, target.transform.position, spell.activationRange, requiresFacing))
                    {
                        //_preCastTimer -= Time.deltaTime;
                        //if(_preCastTimer > 0)
                        //{
                        self.Combat.Execute_SheathWeapon();
                        //    return false;
                        //}

                        if(self.RoundSystem.CanCast())
                        {
                            self.HoldPosition();
                            self.Combat.Execute_BlockAggro(2);
                            _castingTime = spell.castingTime * 0.6f;
                            _castingTime += 0.4f;
                            self.Animation.PlayMotion_HandleSpell(0, _castingTime <= 1 ? 0 : 1);
                            castState = CastSate.Incantation;
                            if(self.debugSpellCastStates)
                            {
                                Debug.Log(self.GetName() + ": <color=green>CastSpellAtActor: Preparation done</color>");
                            }
                        }
                    }

                    break;

                case CastSate.Incantation:

                    if(self.debugSpellCastStates)
                    {
                        Debug.Log(self.GetName() + ": <color=cyan>CastSpellAtActor: Incantation</color>");
                    }

                    self.isCasting = true;
                    _castingTime -= Time.deltaTime;

                    if(_castingTime <= 0)
                    {
                        if(self.debugSpellCastStates)
                        {
                            Debug.Log(self.GetName() + ": <color=green>CastSpellAtActor: Incantation done</color>");
                        }

                        castState = CastSate.Release;
                    }

                    break;

                case CastSate.Release:

                    self.Animation.PlayMotion_HandleSpell(1, spell.releaseMotionIndex);
                    if(spell != null)
                    {
                        self.Combat.OnReleaseSpell = () =>
                        {

                            if(self.IsPlayer)
                            {
                                GameEventSystem.SetPortraitActionIcon?.Invoke(self.PartySlot, null);
                            }

                            Activate(self, self.Equipment.spellAnchor, target, null);
                        };
                    }

                    spell = null;
                    self.Combat.noStagger = true;

                    if(self.debugSpellCastStates)
                    {
                        Debug.Log(self.GetName() + ": <color=green>CastSpellAtActor: Releasing done</color>");
                    }

                    castState = CastSate.Cooldown;

                    break;

                case CastSate.Cooldown:

                    if(_postCastTimer > 0)
                    {
                        if(self.debugSpellCastStates)
                        {
                            Debug.Log(self.GetName() + ": <color=cyan>CastSpellAtActor: Cooldown</color>");
                        }

                        _postCastTimer -= Time.deltaTime;
                    }
                    else
                    {
                        if(self.debugSpellCastStates)
                        {
                            Debug.Log(self.GetName() + ": <color=green>CastSpellAtActor: Cooldown done</color>");
                        }

                        return true;
                    }
                    break;
            }

            return false;
        }

        private void Activate(Actor self, Transform spellAnchor, Actor spellTarget, Vector3? location)
        {
            foreach(MagicEffect magicEffect in self.Combat.equippedSpell.magicEffects)
            {
                Transform anchor = spellAnchor;
                if(spellTarget != null)
                {
                    magicEffect.SpawnEffect(self, spellTarget, anchor);
                }
                else
                {
                    if(location == null)
                    {
                        Debug.LogError("Spell target location = null");
                    }

                    magicEffect.SpawnEffectLocation(self, location, anchor);
                }
            }

            self.Combat.equippedSpell.StartCooldown();
        }

        public override bool InRange()
        {
            return true;
        }

        public override void Release()
        {
            if(self.isCasting)
            {
                castState = CastSate.Cooldown;
                self.Animation.Animator.Play("Cancel", 2);
                if(self.debugSpellCastStates)
                {
                    Debug.Log(self.GetName() + ": <color=green>CastSpellAtPoint: Cancelling action</color>");
                }

                self.Unroot();
                self.Combat.Execute_BlockAggro(0);
                self.RoundSystem.SetSpellPause();
                self.Combat.noStagger = false;
                self.isCasting = false;
            }
        }
    }

    public class Action_CastSpellAtLocation : GameAction
    {
        public CastSate castState = CastSate.Preparation;
        private float _preCastTimer;
        private float _postCastTimer = 3f;
        private float _castingTime;

        internal Spell spell;

        public Action_CastSpellAtLocation(Actor self) : base(self)
        {
            this.self = self;
        }

        internal GameAction Set(Vector3 targetLocation, Spell spell)
        {
            this.spell = self.Combat.equippedSpell = spell;
            requiredRange = spell.activationRange;
            _targetPosition = targetLocation;
            castState = CastSate.Preparation;
            _preCastTimer = 1f;
            _postCastTimer = 3f;

            self.Combat.noStagger = false;
            return this;
        }

        //public override void Cancel(ActorMonoController self)
        //{
        //    throw new NotImplementedException();
        //}

        public override bool Done(Actor self)
        {
            switch(castState)
            {
                case CastSate.Preparation:

                    if(spell == null)
                    {
                        if(self.debugSpellCastStates)
                        {
                            Debug.Log(self.GetName() + ": <color=red>Spell null</color>");
                        }
                    }

                    if(self.isCasting)
                    {
                        return false;
                    }
                    if(Actions.MoveIntoRange(self, _targetPosition, spell.activationRange, requiresFacing))
                    {
                        self.Combat.Execute_SheathWeapon();

                        if(self.RoundSystem.CanCast())
                        {
                            self.HoldPosition();

                            self.Combat.Execute_BlockAggro(2);
                            _castingTime = spell.castingTime * 0.6f;
                            _castingTime += 0.4f;
                            self.Animation.PlayMotion_HandleSpell(0, _castingTime <= 1 ? 0 : 1);
                            castState = CastSate.Incantation;
                            if(self.debugSpellCastStates)
                            {
                                Debug.Log(self.GetName() + ": <color=green>CastSpellAtPoint: Preparation done</color>");
                            }
                        }
                    }

                    break;

                case CastSate.Incantation:

                    if(self.debugSpellCastStates)
                    {
                        Debug.Log(self.GetName() + ": <color=cyan>CastSpellAtPoint: Incantation</color>");
                    }

                    self.isCasting = true;
                    _castingTime -= Time.deltaTime;

                    if(_castingTime <= 0)
                    {
                        if(self.debugSpellCastStates)
                        {
                            Debug.Log(self.GetName() + ": <color=green>CastSpellAtPoint: Incantation done</color>");
                        }

                        castState = CastSate.Release;
                    }

                    break;

                case CastSate.Release:

                    self.Animation.PlayMotion_HandleSpell(1, spell.releaseMotionIndex);
                    if(spell != null)
                    {
                        self.Combat.OnReleaseSpell = () =>
                        {
                            if(self.IsPlayer)
                            {
                                GameEventSystem.SetPortraitActionIcon?.Invoke(self.PartySlot, null);
                            }

                            Activate(self, self.Equipment.spellAnchor, null, _targetPosition);
                        };
                    }

                    spell = null;
                    self.Combat.noStagger = true;
                    if(self.debugSpellCastStates)
                    {
                        Debug.Log(self.GetName() + ": <color=green>CastSpellAtPoint: Releasing done</color>");
                    }

                    castState = CastSate.Cooldown;
                    break;

                case CastSate.Cooldown:
                    if(_postCastTimer > 0)
                    {
                        if(self.debugSpellCastStates)
                        {
                            Debug.Log(self.GetName() + ": <color=cyan>CastSpellAtPoint: Cooldown</color>");
                        }

                        _postCastTimer -= Time.deltaTime;
                    }
                    else
                    {
                        if(self.debugSpellCastStates)
                        {
                            Debug.Log(self.GetName() + ": <color=green>CastSpellAtPoint: Cooldown done</color>");
                        }

                        return true;
                    }
                    break;
            }

            return false;
        }

        private void Activate(Actor self, Transform spellAnchor, Actor spellTarget, Vector3? location)
        {
            foreach(MagicEffect magicEffect in self.Combat.equippedSpell.magicEffects)
            {
                Transform anchor = spellAnchor;
                if(spellTarget != null)
                {
                    magicEffect.SpawnEffect(self, spellTarget, anchor);
                }
                else
                {
                    if(location == null)
                    {
                        Debug.LogError("Spell target location = null");
                    }

                    magicEffect.SpawnEffectLocation(self, location, anchor);
                }
            }

            self.Combat.equippedSpell.StartCooldown();
        }

        public override bool InRange()
        {
            ////! Move into range
            //_self.Unroot();
            //if(Vector3.Distance(_self.transform.position, _targetPosition) > _spell.activationRange)
            //{
            //    ActorMotion.Rotate(_self, _self.navAgent.steeringTarget, 300, "");
            //    _self.SetDestination(_targetPosition, 0.1f);
            //    return false;
            //}
            //_self.Hold();

            //Vector3 desiredDir = _targetPosition - _self.transform.position;
            //desiredDir.y = 0;

            ////! Rotate to target
            //if(Mathf.Abs(Vector3.SignedAngle(_self.transform.forward, desiredDir, Vector3.up)) > 2f)
            //{
            //    ActorMotion.Rotate(_self, _self.transform.position + desiredDir, 300, "");
            //    return false;
            //}
            return true;
        }

        public override void Release()
        {

            if(self.isCasting)
            {
                castState = CastSate.Cooldown;
                self.Animation.Animator.Play("Cancel", 2);
                if(self.debugSpellCastStates)
                {
                    Debug.Log(self.GetName() + ": <color=green>CastSpellAtPoint: Cancelling action</color>");
                }

                self.Unroot();
                self.Combat.Execute_BlockAggro(0);
                self.RoundSystem.SetSpellPause();
                self.Combat.noStagger = false;
                self.isCasting = false;
            }
        }
    }

    public class Action_UseItem : GameAction
    {
        public Action_UseItem(Actor self) : base(self)
        {
            this.self = self;
        }

        public Action_UseItem Init()
        {
            requiredRange = -1;
            return this;
        }

        public override bool InRange()
        {
            return true;
        }

        public override bool Done(Actor self)
        {
            throw new NotImplementedException();
        }

        public override void Release()
        {
        }
    }

    public class Action_NPCTalkTo : GameAction
    {
        private readonly Actor _targetPC;
        public Action_NPCTalkTo(Actor dialogOwner, Actor targetPC) : base(dialogOwner)
        {
            Debug.Assert(targetPC.conversationData != null, "Conversation null");
            _targetPosition = targetPC.transform.position;
            _targetPC = targetPC;
        }

        public Action_NPCTalkTo Init()
        {
            requiredRange = -1;
            return this;
        }

        public override bool InRange()
        {
            return true;
        }

        public override bool Done(Actor self)
        {
            if(Actions.MoveIntoRange(self, _targetPosition, 1f, false))
            {
                AoG.Core.GameInterface.Instance.GetUIScript().ShowDialogbox(_targetPC, self.conversationData);
                return true;
            }
            return false;
        }

        public override void Release()
        {
        }
    }

    public class Action_PCTalkTo : GameAction
    {
        private readonly Actor _dialogNPC;
        public Action_PCTalkTo(Actor PC, Actor targetNPC) : base(PC)
        {
            Debug.Assert(targetNPC.conversationData != null, "Conversation null");
            _dialogNPC = targetNPC;
        }

        public Action_PCTalkTo Init()
        {
            requiredRange = -1;
            return this;
        }

        public override bool InRange()
        {
            return true;
        }

        public override bool Done(Actor self)
        {
            if(Get.IsInFOV(_dialogNPC.transform, self.transform.position, 5))
            {
            }
            else
            {
                HelperFunctions.RotateTo(_dialogNPC.transform, self.transform.position, 300);
            }

            if(Actions.MoveIntoRange(self, _dialogNPC.transform.position, 1f, false))
            {
                // Arrived
                //self.hasMovementOrder = false;


                AoG.Core.GameInterface.Instance.GetUIScript().ShowDialogbox(self, _dialogNPC.conversationData);
                return true;
            }
            return false;
        }

        public override void Release()
        {
        }
    }
}

public class MoveAction : GameAction
{
    private Action OnTargetReached;
    private Vector3 _finalFacing;

    public MoveAction(Actor self) : base(self)
    {
        this.self = self;
    }

    public MoveAction Set(Vector3 point, Vector3 finalFacing, float requiredDistance, System.Action OnTargetReached)
    {
        //requiresFacing = true;
        _targetPosition = point;
        this._finalFacing = finalFacing;
        this.requiredRange = requiredDistance;
        this.OnTargetReached = OnTargetReached;
        return this;
    }

    public override bool Done(Actor self)
    {
        if(Actions.MoveIntoRange(self, _targetPosition, requiredRange, false))
        {
            self.hasMovementOrder = false;
            OnTargetReached?.Invoke();
            OnTargetReached = null;
            if(Get.IsInFOV(self.transform, self.transform.position + _finalFacing, 5))
            {
                return true;
            }
            else
            {
                HelperFunctions.RotateTo(self.transform, self.transform.position + _finalFacing, 300);
            }
        }
        return false;
    }

    public override bool InRange()
    {
        return true;
    }

    public override void Release()
    {
        self.HoldPosition();
    }
}

//public enum ActionFailMessage
//{
//    WaitingForSpellPauseEnd,

//}

public abstract class GameAction
{
    //public string debugString;
    protected Actor self;
    protected Actor target;
    protected Vector3 _targetPosition;
    protected float requiredRange;

    internal Action OnDone;

    public int actionID;
    internal bool requiresFacing = true;

    public GameAction(Actor self)
    {
        this.self = self;
    }

    public abstract bool Done(Actor self);
    //public abstract bool Done(ActorMonoController self);

    public abstract void Release();

    public abstract bool InRange();

    //public Scriptable GetActionTarget()
    //{
    //    return _target;
    //}

    public virtual Vector3 GetActionTargetPosition()
    {
        if(target != null)
        {
            _targetPosition = target.transform.position;
        }

        return _targetPosition;
    }

    public float GetRequiredRange()
    {
        return requiredRange;
    }
}
