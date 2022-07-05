//using System.Collections;
//using UnityEngine;

//public class Skill_GlobeOfProtection : Skill
//{
//    //[Range(0, 1)]
//    //public float healHealthPercSelf = 0.50f;
//    //[Range(0, 1)]
//    //public float healHealthPercAllies = 0.70f;

//    private Collider[] _friends;

//    public float checkFriendsRadius = 20;
//    public float globeLifeTime = 10;
//    public float lifeTimeBonusPerLevel = 0.3f;

//    public override void Init()
//    {
//        _friends = new Collider[3];
//        base.Init();
//    }

//    public override bool ConditionsMetAI(NPCInput agent)
//    {
//        //if (agent.isPlayer)
//        //{
//        //    var friendlyTarget = PlayerInput.selectedTarget;

//        //    if (friendlyTarget != null && agent.IsFriend(friendlyTarget))
//        //    {
//        //        skillTarget = friendlyTarget;
//        //        return true;
//        //    }

//        //    skillTarget = agent;
//        //    return true;
//        //}

//        var friendInDanger = GetFriendsInRangeNonAlloc(agent, checkFriendsRadius, _friends);
//        if (friendInDanger != null)
//        {
//            skillTarget = friendInDanger;
//            return true;
//        }

//        return false;
//    }

//    public override bool ConditionsMetPlayer(Actor actor)
//    {
//        return true;
//    }

//    public override void IndividualSetup(Actor agent)
//    {
//        skillTarget.ActorStats.isBeingBuffed = true;
//        cooldown = globeLifeTime + 0.5f;
//    }

//    public override void SpawnVFX(Actor agent, Actor target, Vector3 targetPosition)
//    {
//        //skillTarget.isBeingBuffed = false;
//        if (skillTarget == null)
//            return;
//        GameObject vfx = PoolSystem.GetPoolObject(vfxIdentifier, ObjectPoolingCategory.VFX);
//        var duration = globeLifeTime + lifeTimeBonusPerLevel * agent.ActorStats.Level;
//        ClampTo(agent, vfx.transform, skillTarget.transform, duration);
//        //skillTarget.Execute_ApplyStatusEffect(new StatusEffect_StatBuff(skillTarget, this, vfx, 0, 0, 0, 0, 0, 0, 3 + agent.level, 0,
//        //    duration));
//    }
//}