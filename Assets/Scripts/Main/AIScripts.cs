using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AIScripts
{
    public class AI_StandardMelee : AICombatScript
    {
        public AI_StandardMelee(Actor actor) : base(actor)
        {
            self = actor;
        }

        public override void OnUpdate()
        {
            if(CanUpdate() == false)
                return;

            Actor target = NearestEnemyOf(self, 10);
            if(target != null)
            {
                EquipWeapon(Constants.EQUIP_MELEE);

                //if(MovedInRange(target, self.Equipment.equippedWeapon.Weapon.Range()))
                Attack(target);
            }

        }

        public override void Release() { }
        public override bool Done() { return false; }
    }

    public class AI_AggroBasedAttack : AICombatScript
    {
        public AI_AggroBasedAttack(Actor actor) : base(actor)
        {
            self = actor;
        }

        public override void OnUpdate()
        {
            if(CanUpdate() == false)
                return;

            Actor target = LastSeenBy(self);
            if(target == null)
                target = NearestEnemyOf(self, 10);

            if(target != null)
            {
                EquipWeapon(Constants.EQUIP_MELEE);

                //if(MovedInRange(target, self.Equipment.equippedWeapon.Weapon.Range()))

                Attack(target);
            }

        }

        public override void Release() { }
        public override bool Done() { return false; }
    }

    public class AI_StandardRanged : AICombatScript
    {
        public AI_StandardRanged(Actor actor) : base(actor)
        {
            self = actor;
        }

        public override void OnUpdate()
        {
            if(CanUpdate() == false)
                return;
            
            Actor target = NearestEnemyOf(self, 10);
            if(target != null)
            {
                EquipWeapon(Constants.EQUIP_RANGED);

                //if(MovedInRange(target, self.Equipment.equippedWeapon.Weapon.Range()))
                Attack(target);
            }

        }

        public override void Release() { }
        public override bool Done() { return false; }
    }

    /// <summary>
    /// - cast memorized attack spells
    /// - if no spells available, use ranged weapon
    /// - if attacked by melee, use melee weapon
    /// </summary>
    public class AI_WizardAggressive : AICombatScript
    {
        Collider[] _enemyColliders;

        DamageType[] prefOffensiveEffects = new[]
        {
            //EffectType.FIRE,
            DamageType.MAGICCOLD,
            //EffectType.RADIANT,
            DamageType.MAGICFIRE,/*,*/
            DamageType.ACID,
            DamageType.MAGIC
        };

        public AI_WizardAggressive(Actor actor) : base(actor)
        {
            self = actor;
            _enemyColliders = new Collider[5];
        }

        public override void OnUpdate()
        {
            if(CanUpdate() == false)
                return;

            Actor target = LastSeenBy(self);
            if(target == null || target.ValidTarget(0) == false)
                target = NearestEnemyOf(self, 10);
            if(target != null)
            {
                if(self.debug)
                    Debug.Log(self.GetName() + ": * Got target");

                if(self.RoundSystem.InSpellPause() == false)
                {
                    if(self.debug)
                        Debug.Log(self.GetName() + ": * Checking spell");

                    float distTargetToAlly = Actions.GetDistanceToNearestActor(self, target.transform.position, _enemyColliders, true);
                    Spell[] availableSpells = AIGetSpellsByEffectType(prefOffensiveEffects, new[] { DeliveryType.InstantActor, DeliveryType.InstantLocation, DeliveryType.SeekActor, DeliveryType.SeekLocation }, 3, distTargetToAlly / 3, 3);
                    _currentSpell = availableSpells[Random.Range(0, availableSpells.Length)];

                    if(_currentSpell != null && self.RoundSystem.CanCast())
                    {
                        CastSpell(target, _currentSpell, Random.Range(5, 15));
                        return;
                    }
                }

                if(InRange(target, Constants.REQUIREDRANGE_MELEE))
                {
                    EquipWeapon(Constants.EQUIP_MELEE);

                    if(_weapon != null && _weapon.CombatType == CombatType.MELEE)
                        Attack(target);
                }
                else
                {
                    EquipWeapon(Constants.EQUIP_RANGED);

                    if(_weapon != null && _weapon.CombatType == CombatType.RANGED)
                        Attack(target);
                }
                //if(MovedInRange(target, self.Equipment.equippedWeapon.Weapon.Range()))                
            }
        }
    

        public override void Release() { }
        public override bool Done() { return false; }
    }

    public class AI_ClericHealer : AICombatScript
    {
        DamageType[] prefOffensiveEffects = new[]
        {
            DamageType.RADIANT,
            DamageType.MAGICCOLD,
            DamageType.MAGICFIRE,
            DamageType.CRUSHING
        };

        Keyword[] prefSupportEffects = new[]
        {
            Keyword.Heal
        };

        public AI_ClericHealer(Actor actor) : base(actor)
        {
            self = actor;
        }

        public override void OnUpdate()
        {
            if(CanUpdate() == false)
                return;

            Actor wounded = GetMostWoundedFriend();
            if(wounded != null /*&& RunCooldown() == false*/)
            {
                _currentSpell = AIGetSpellByEffectType(new[] { DamageType.HEAL }, DeliveryType.InstantActor);
                if(_currentSpell != null/* && MovedInRange(wounded, _currentSpell.activationRange)*/ && self.RoundSystem.CanCast())
                {
                    CastSpell(wounded, _currentSpell, Random.Range(5, 15));
                }
            }
            else
            {
                Actor target = LastSeenBy(self);
                if(target == null)
                    target = NearestEnemyOf(self, 10);

                if(target != null)
                {
                    if(self.RoundSystem.InSpellPause() == false)
                    {
                        _currentSpell = AIGetSpellByEffectType(prefOffensiveEffects, DeliveryType.SeekActor);
                        if(_currentSpell == null)
                        {
                            Spell[] availableSpells = null;
                            if(InRange(target, 2) == false)
                                availableSpells = AIGetSpellsByEffectType(prefOffensiveEffects, new[] { DeliveryType.InstantActor, DeliveryType.InstantLocation, DeliveryType.SeekActor, DeliveryType.SeekLocation }, 3, 0, 3);

                        }
                        //Debug.Log("Found spell");
                        if(_currentSpell != null /*&& MovedInRange(target, _currentSpell.activationRange)*/ && self.RoundSystem.CanCast())
                        {
                            CastSpell(target, _currentSpell, Random.Range(5, 15));
                            return;
                        }

                    }

                    if(InRange(target, Constants.REQUIREDRANGE_MELEE))
                    {
                        EquipWeapon(Constants.EQUIP_MELEE);
                    }
                    else
                    {
                        EquipWeapon(Constants.EQUIP_RANGED);
                    }

                    //if(MovedInRange(target, self.Equipment.equippedWeapon.Weapon.Range()))
                    Attack(target);

                }
            }
        }

        public override void Release() { }
        public override bool Done() { return false; }
    }
}
public abstract class AICombatScript : GameScript
{
    protected new Actor self;
    protected Weapon _weapon;
    protected Spell _currentSpell;
    protected float castCooldown = 2;

    protected Collider[] friendsCapacity = new Collider[7];
    List<Actor> _cachedFriends = new List<Actor>();
    protected Collider[] foesCapacity = new Collider[7];
    List<Actor> _cachedFoes = new List<Actor>();

    float _drawSheathTimer = 2;
    bool _busy;

    protected Class[] targetPriorityClass;

    public AICombatScript(Actor actor) : base(actor)
    {
        self = actor;
    }

    //IEnumerator CR_EquipAndDrawBestWeapon(int WEAPON_TYPE)
    //{
    //    //_busy = true;
       
    //    self.Combat.Execute_EquipBestWeapon(WEAPON_TYPE, true, true);
    //    //_weapon = self.Equipment.equippedWeapon.Weapon;
    //    //AIActions.Action_DrawWeapon act = new AIActions.Action_DrawWeapon(self).Set(true);
    //    //self.CommandActor(act);
    //    yield return new WaitForSeconds(0);
    //    _weapon = self.Equipment.equippedWeapon.Weapon;
    //    //_busy = false;
    //    //Debug.Assert(self.Equipment.equippedWeapon.Weapon != null, "Actor.DrawWeapon::Weapon = null");
    //}

    protected void EquipWeapon(int WEAPON_TYPE)
    {
        if (/*_busy || */_weapon != null)
        {
            return;
        }

        if (self.debugGear)
            Debug.Log(self.GetName() + ":<color=orange>1</color> AIScripts.EquipWeapon");
        self.Combat.Execute_EquipBestWeapon(WEAPON_TYPE, false, true);
        //self.Execute_DrawWeapon();
        _weapon = self.Equipment.equippedWeapon.Weapon;
        //switch (weaponType)
        //{
        //    case 0: // ANY
        //        self.scrMono.StartCoroutine(CR_EquipAndDrawBestWeapon(Constants.EQUIP_MELEE));
        //        break;
        //    case 1: // MELEE
        //        if((CombatType)weaponType == CombatType.MELEE)
        //            self.scrMono.StartCoroutine(CR_EquipAndDrawBestWeapon(Constants.EQUIP_MELEE));
        //        break;
        //    case 2: // RANGED
        //        if((CombatType)weaponType == CombatType.RANGED)
        //            self.scrMono.StartCoroutine(CR_EquipAndDrawBestWeapon(Constants.EQUIP_RANGED));

        //        break;
        //}
    }

    public Spell AIGetSpellByKeyword(Keyword keyword)
    {
        //Spell highestPrioritySpell = null;
        //ActorMonoController spellTarget = null;

        for(int i = 0; i < self.Spellbook.SpellData.Count; i++)
        {
            Spell currSpell = self.Spellbook.SpellData[i].spell;

            if(currSpell.HasKeyWord(keyword))
            {
                return currSpell;
            }

            //if(self.debug)
            //    Debug.Log("<color=cyan>ITERATING SPELLS</color>");
            //if(CanActivateSpell(currSpell, self) == false)
            //    continue;

            //spellTarget = AISelectTarget(self);

            //if(spellTarget == null)
            //{
            //    if(self.debug)
            //        Debug.Log(self.actorData.Name + ": Spell '<color=white>" + self.Combat.equippedSpell.Name +
            //                  "</color>' activation failed -> No valid target");
            //    continue;
            //}

            //if(highestPrioritySpell == null || currSpell.currentPriority > highestPrioritySpell.currentPriority)
            //    //if(_currentSpell == currSpell)
            //    //{
            //    //    continue;
            //    //}

            //    highestPrioritySpell = currSpell;
        }

        return null;
    }

    public Spell AIGetSpellByEffectType(DamageType[] effectTypes, DeliveryType deliveryType)
    {
        //Spell highestPrioritySpell = null;
        //ActorMonoController spellTarget = null;
        int effCount = effectTypes.Length;
        for(int i = 0; i < self.Spellbook.SpellData.Count; i++)
        {
            Spell currSpell = self.Spellbook.SpellData[i].spell;

            for(int effIndex = 0; effIndex < effCount; effIndex++)
            {
                if(currSpell.effectType == effectTypes[effIndex] && currSpell.deliveryType == deliveryType)
                {
                    return currSpell;
                }
            }
        }

        return null;
    }

    public Spell[] AIGetSpellsByEffectType(DamageType[] effectTypes, DeliveryType[] deliveryTypes, int minRange, float maxAOE, int maxReturnedSpells)
    {
        Spell[] spells = new Spell[maxReturnedSpells];
        int numSpellsAdded = 0;
        for(int i = 0; i < self.Spellbook.SpellData.Count; i++)
        {
            Spell currSpell = self.Spellbook.SpellData[i].spell;

            if(currSpell.activationRange < minRange || currSpell.aoeRadius > maxAOE)
            {
                if(self.debug)
                    Debug.Log(self.GetName() + ": * " + currSpell.aoeRadius + " > " + maxAOE);
                continue;
            }

            if(self.debug)
                Debug.Log(self.GetName() + ": * Checking spell");

            for(int j = 0; j < effectTypes.Length; j++)
            {
                if(maxReturnedSpells == numSpellsAdded)
                {
                    break;
                }

                if(currSpell.effectType == effectTypes[j])
                {
                    for(int k = 0; k < deliveryTypes.Length; k++)
                    {
                        if(currSpell.deliveryType == deliveryTypes[k])
                        {
                            spells[numSpellsAdded++] = currSpell;

                            break;
                        }
                    }
                }
            }
        }

        return spells;
    }

    protected Actor GetMostWoundedFriend()
    {
        if(_cachedFriends.Count < 7)
        {
            _cachedFriends = Actions.GetWoundedAlliesInRangeNonAlloc(self, 20, 50, friendsCapacity);
        }

        if(_cachedFriends.Count > 0)
            return _cachedFriends[0];

        return null;
    }

    protected Actor NearestActorOfClass(Class targetClass, SpellTargetType targetType)
    {
        if(_cachedFoes.Count < 7)
        {
            _cachedFoes = Actions.GetActorsOfClassAtLocation(self, self.transform.position, targetClass, 20, targetType, foesCapacity);
        }


        if(_cachedFriends.Count > 0)
        {
            //if(proxOffset > 0)
            //{
            //    if(_cachedFoes.Count < proxOffset + 1)
            //    {
            //        proxOffset = _cachedFoes.Count;
            //        return _cachedFoes[proxOffset];
            //    }
            //}

            return _cachedFoes[0];
        }

        return null;
    }

    protected Actor LastSeenBy(Actor actor)
    {
        return actor.Combat.GetHostileTarget();
    }

    protected Actor LastAttackerOf(Actor actor)
    {
        return actor.Combat.lastAttacker;
    }

    protected Actor NearestEnemyOf(Actor actor, int maxRange)
    {
        if(actor.debug)
        {
            Debug.Log(actor.GetName() + ": Setting attack target");
        }

        actor.Combat.SetHostileTarget(HelperFunctions.GetClosestActor_WithJobs(actor, maxRange, actor.ActorStats.GetEnemyFlags()));

        //if(true)
        //{

        //}

        return actor.Combat.GetHostileTarget();
    }

    protected Actor NearestFriend()
    {
        return HelperFunctions.GetClosestActor_WithJobs(self, 20, ActorFlags.PC | ActorFlags.ALLY);
    }

    protected bool CheckStatGT(Actor actor, ActorStat stat, int threshold)
    {
        return actor.ActorStats.GetBaseStat(stat) > threshold;
    }

    protected bool CheckStatLT(Actor actor, ActorStat stat, int threshold)
    {
        return actor.ActorStats.GetBaseStat( stat) < threshold;
    }

    protected bool HPLT(Actor actor, int hpThreshold)
    {
        return actor.ActorStats.GetBaseStat(ActorStat.HITPOINTS) < hpThreshold;
    }

    protected bool HPGT(Actor actor, int hpThreshold)
    {
        return actor.ActorStats.GetBaseStat(ActorStat.HITPOINTS) > hpThreshold;
    }

    protected bool HPPercentageLT(Actor actor, int hpThreshold)
    {
        return actor.HPPercentage * 100 < hpThreshold;
    }

    protected bool HPPercentageGT(Actor actor, int hpThreshold)
    {
        return actor.HPPercentage * 100 > hpThreshold;
    }

    protected bool MovedInRange(Actor target, float range)
    {
        return Actions.MoveIntoRange(self, target.transform.position, range);
    }

    protected bool InRange(Actor target, float range)
    {
        return Vector3.Distance(self.transform.position, target.transform.position) <= range;
    }

    protected void Attack(Actor target)
    {
        //self.AttackCore(target, _weapon);

        //DebugDraw.Cube(debugPointPrepare, Vector3.one * 0.2f, Quaternion.LookRotation(self.transform.forward), Colors.YellowOrange);
        if(self.debug)
            Debug.Log(self.GetName() + ": Adding attack action");
        if(self.Equipment.equippedWeapon.Weapon != null)
            self.AddAction(new AIActions.Action_Attack_Once(self).Set(target), false);
    }

    protected void CastSpell(Actor target, Spell spell, float cooldown)
    {
        if(self.RoundSystem.InSpellPause())
            return;



        //if(self.attackCoroutine != null)
        //{
        //    self.scrMono.StopCoroutine(self.attackCoroutine);
        //    self.attackCoroutine = null;
        //}

        //self.attackCoroutine = CR_SpellCastRoutine(target, spell);
        //self.scrMono.StartCoroutine(self.attackCoroutine);
        self.AddAction(new AIActions.Action_CastSpellAtActor(self).Set(target, spell), false);
        castCooldown = cooldown;

    }

    //IEnumerator CR_SpellCastRoutine(Actor target, Spell spell)
    //{
    //    if(self.debug)
    //        Debug.Log(self.actorData.Name + ": Spellcast routine start");

    //    Vector3 debugPointLoop = self.transform.position + Vector3.up * 2f;
    //    Vector3 debugPointPrepare = self.transform.position + Vector3.up * 2.3f;
    //    DebugExtension.DebugWireSphere(debugPointLoop, Colors.Green, 0.2f);
    //    //while(self != null && target != null && InRange(target, spell.activationRange) == false)
    //    //{
    //    //    DebugExtension.DebugWireSphere(debugPointPrepare, Colors.Red, 0.2f);
    //    //    if(self.debug)
    //    //        Debug.Log(self.actorData.Name + ": Spellcast routine move into range");
    //    //    yield return null;
    //    //}

    //    if(self.debug)
    //        Debug.Log(self.actorData.Name + ": Spellcast routine in range. Attacking!");

    //    self.AddAction(new AIActions.Action_CastSpellAtActor(self).Set(target, spell), false);
    //    self.attackCoroutine = null;
    //}
}

public abstract class GameScript
{
    protected Scriptable self;
    internal bool running;
    internal bool dead;

    public bool debug_scriptupdating;

    public GameScript(Scriptable self)
    {
        this.self = self;
    }

    public abstract void OnUpdate();
    public abstract bool Done();
    public abstract void Release();

    public bool ActionListEmpty()
    {
        if(self.GetCurrentAction() != null || self.GetNextAction() != null)
        {
            return false;
        }
        //if(self.OverrideAction == null)
        //{
        //    if(self.debug)
        //    {
        //        Debug.Log(self.actorData.Name + ": No action");
        //    }
        //    return true;
        //}

        //if(self.OverrideAction.Done(self) || self.OverrideAction.Cancel())
        //{
        //    self.OverrideAction = null;
        //    return true;
        //}
        //else
        //{
        //    if(self.debug)
        //        Debug.Log(self.actorData.Name + ": Updating action '" + _currentAction.ToString() + "'");
        //}

        return true;
    }

    protected bool CanUpdate()
    {
        if(ActionListEmpty() == false)
        {
            debug_scriptupdating = false;
            return false;
        }

        debug_scriptupdating = true;
        return true;
    }

    internal string GetName()
    {
        throw new System.NotImplementedException();
    }
}
