//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;
//using UnityEngine;

//public class Skill_DamageOverTime : Skill {

//    [Range(0, 1)]
//    public float damagePerSecond = 0.01f;
//    public float duration = 5f;

//    public override void Init(ActorMonoController agent) {

//        base.Init(agent);
//    }

//    public override bool ConditionsMet(ActorMonoController agent) {

//        skillTarget = agent.attackTarget;
//        return agent.weaponDrawn && skillTarget != null;
//    }

//    public override void ActivateSkill(ActorMonoController agent) {

//        agent.StartCoroutine(CR_Activate(agent));
//    }

//    public IEnumerator CR_Activate(ActorMonoController agent) {

//        agent.GetComponent<Animator>().Play(animStateName, 2);
//        yield return new WaitForSeconds(vfxDelay);

//        if(agent != null && agent.m_destroyed == false)
//            SpawnVFX(agent);
//    }

//    public override void SpawnVFX(ActorMonoController agent) {

//        if(skillTarget == null)
//            return;

//        AgentVFXManager.TriggerVFX(ResourceManager.GetPoolObject(vfxIdentifier, ObjectPoolingCategory.VFX), skillTarget.attackPoint, Quaternion.identity, duration);
//        //agent.StartCoroutine(CR_Poison(agent));

//        StatusEffect statusEffect = new StatusEffect_Damage(DamageType.POISON, effectValue + (agent.m_level * 0.001f), true, 1, 4);
//        skillTarget.Execute_ApplyStatusEffect(statusEffect);
//    }


//    IEnumerator CR_Poison(ActorMonoController agent) {

//        float elapsed = 0;
//        float interval = 0;

//        while(elapsed < duration) {

//            elapsed += Time.deltaTime;
//            interval += Time.deltaTime;

//            if(agent == null || agent.m_destroyed || skillTarget == null) {

//                yield break;
//            }

//            if(interval >= 1) {

//                interval = 0;
//                skillTarget.ApplyDamage(agent, DamageType.POISON, false, true, effectValue + (agent.m_level * 0.001f), true, false);
//            }

//            yield return null;
//        }
//    }

//    //public override bool InRange(Agent agent) {

//    //    if(agent.m_destroyed || agent.m_attackTarget == null) {

//    //        agent.SetDestination(agent.m_transform.position, 1);
//    //        return false;
//    //    }

//    //    if(Vector3.Distance(agent.m_transform.position, agent.m_attackTarget.m_transform.position) > activationRange) {

//    //        agent.SetDestination(agent.m_attackTarget.m_transform.position, 1);
//    //        return false;
//    //    }

//    //    return true;
//    //}
//}

