//using System.Collections;
//using UnityEngine;

//public class Skill_DrinkManaPotion : Skill
//{
//    [Range(0, 1)] public float manaLowThreshold = 0.25f;

//    public override void Init()
//    {
//    }

//    public override bool ConditionsMetAI(NPCInput agent)
//    {
//        //if(agent.mpPercentage > manaLowThreshold)
//        //    return false;
//        if(agent.Equipment.manaPotions == 0)
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
//        //agent.Equipment.manaPotions--;
//        //agent.Execute_ModifyMana((int)effectValue, ModType.PERCENT);

//        VFXPlayer.TriggerVFX(PoolSystem.GetPoolObject("vfx_notify_drinkmanapotion", ObjectPoolingCategory.VFX),
//            agent.transform, Vector3.zero, Quaternion.identity, false, 3);
//    }
//}