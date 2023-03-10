//using cakeslice;
using GenericFunctions;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class MobInput : NPCInput
{
    protected Dictionary<Actor, float> _attackers; // attacker, aggro
    Actor _killer;
    bool _hasGem;
    public Class nemesisClass;
    public System.Func<float, List<Actor>> OnGetEnemiesInRange; // checkRange, return Enemy

    public override void FinalizeActor(ActorConfiguration config)
    {
        base.FinalizeActor(config);
       
        StartCoroutine(CR_FindTarget());
    }

    internal void Update()
    {
        if(dead)
        {
            return;
        }

        if(Combat.GetHostileTarget() == null || Vector3.Distance(transform.position, startPosition) > 10)
        {
            //behaviours.combatController.ChangeState(0);
            //MoveTowards(GameMaster.Instance.playerBaseCenter.position, 2);
            SetDestination(startPosition, 2/*, "Mob to playerbase"*/);
        }
        else
        {
            Debug.Log(GetName() + ": Attacking enemy - Updating");
            //behaviours.combatController.ChangeState(1);
            //behaviours.aiBehaviourController.Update();
        }
    }

    public void ApplyDamage(Actor source, SavingThrowType savingThrowType, DamageType damageType, SpellAttackRollType attackRollType, int damageRoll, bool percentage)
    {
        float finalAmount = damageRoll;
       
        if(_attackers.ContainsKey(source) == false)
        {
            Debug.Log("Adding aggro candidat");
            _attackers.Add(source, finalAmount / 100);
        }
        else
        {
            _attackers[source] += finalAmount / 100;
        }

        Combat.ApplyDamage(source, damageType, damageRoll, true);
        
        if(_attackers.ContainsKey(source))
            _attackers[source] += finalAmount;

        if(dead)
        {
            _killer = source;
            Kill();
            //GameEventSystem.OnMobDefeated?.Invoke(this, source);
        }
    }

    public void Kill()
    {
        foreach(Actor attacker in _attackers.Keys)
        {
            if(attacker != null && attacker.ActorStats.isPlayer)
            {
                int levelDifference = ActorStats.Level - attacker.ActorStats.Level;

                float calcExp = Mathf.Clamp(levelDifference, 0, 100) * 20;
                //attacker.AddXP(calcExp);
            }
        }

        Combat.Kill();
    }

    //public override Actor Execute_GetAttackTarget(float range, bool needsLOS)
    //{
    //    return null;
    //}

    //public override Transform Execute_EquipWeapon(Weapon weapon, bool playAnimation, bool playSound)
    //{
    //    equippedWeapon = weapon;

    //    return null;
    //}

    IEnumerator CR_FindTarget()
    {
        while(dead == false)
        {
            Debug.Log(GetName() + ": Aggro loop - Updating");
            yield return new WaitForSeconds(1);

            if(Animation.Animator.GetCurrentAnimatorStateInfo(1).IsName("New State") == false)
            {
                //Debug.Log(actorData.Name + ": Aggro loop - Pause");
                continue;
            }

            if(_attackers.Count > 0)
            {
                KeyValuePair<Actor, float> mostHated = new KeyValuePair<Actor, float>(null, 0);
                foreach(KeyValuePair<Actor, float> kvp in _attackers.ToArray())
                {
                    if(kvp.Key == null || kvp.Key.dead)
                    {
                        _attackers.Remove(kvp.Key);
                        continue;
                    }

                    if(mostHated.Key == null || kvp.Value > mostHated.Value)
                        mostHated = kvp;
                }

                if(mostHated.Key != null)
                {
                    Debug.Log(GetName() + ": Aggro loop - Setting most hated");
                    Combat.SetHostileTarget(mostHated.Key);
                }
            }
            else
            {
                Actor[] targets = HelperFunctions.GetEnemiesInRangeNonAlloc(this, 20, false);
                Actor newTarget = targets.Length > 0 ? targets[0] : null;//.Count > 0 ? targets.MinBy(t => t.actorData.maxHealth) : null;

                if(newTarget != null && _attackers.ContainsKey(newTarget) == false)
                {
                    Debug.Log(GetName() + ": Aggro loop - Adding attacker");
                    _attackers.Add(newTarget, 0);
                    Combat.SetHostileTarget(newTarget);
                }
            }

            //OnEnemySet?.Invoke(attackTarget);
        }

        //Debug.Log(actorData.Name + ": AGGRO LOOP TERMINATED");
    }

    public override void UpdateLookAtIK()
    {
        
    }

    internal override float GetCharacterRadius()
    {
        return 2f;
    }

    //public override ActorMonoController Execute_GetClosestAlly(float range)
    //{
    //    return OnAllyInRangeCheck?.Invoke(range);
    //}
}
