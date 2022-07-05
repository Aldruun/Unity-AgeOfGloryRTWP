//using System.Collections;
//using UnityEngine;

//public class Skill_DrinkHealthPotion : Skill
//{
//    [Range(0, 1)] public float healthLowThreshold = 0.35f;

//    public override void Init()
//    {
//    }

//    public override bool ConditionsMetAI(NPCInput agent)
//    {
//        if(agent.hpPercentage > healthLowThreshold)
//            return false;
//        if(agent.Equipment.healthPotions == 0)
//            return false;
//        skillTarget = agent;
//        return true;
//    }

//    public override bool ConditionsMetPlayer(Actor actor)
//    {
//        return true;
//    }

//    public override void IndividualSetup(Actor agent)
//    {
//        agent.Animation.Animator.CrossFade("DrinkPotion", 0.2f, 2);
//    }

//    public override void SpawnVFX(Actor agent, Actor target, Vector3 targetPosition)
//    {
//        agent.Equipment.healthPotions--;
//        ActorUtility.ModifyActorHealth(agent.ActorStats, (int)effectValue, ModType.PERCENT);
        
//        VFXPlayer.TriggerVFX(PoolSystem.GetPoolObject("vfx_notify_drinkhealthpotion", ObjectPoolingCategory.VFX),
//            skillTarget.transform, Vector3.zero, Quaternion.identity, false, 3);
//    }

//}