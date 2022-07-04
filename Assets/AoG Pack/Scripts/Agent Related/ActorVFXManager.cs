//using System.Collections;
//using UnityEngine;
//using UnityEngine.VFX;

//public class ActorVFXManager : MonoBehaviour
//{
//    public string vfx_defaultHit;

//    public static void RegisterVFXEvents(ActorInput agent)
//    {
//        agent.OnHit += (agressor, damageAmount, damageType, successfulHit) =>
//        {
//            if(agent == null || successfulHit == false)
//                return;

//            if(agent.blocking)
//            {
//                TriggerVFX(PoolSystem.GetPoolObject("vfx_hit_sparks", ObjectPoolingCategory.VFX),
//                         agent.GetAttackPoint().position, Quaternion.identity);
//                return;
//            }

//            switch(damageType)
//            {
//                //case EffectType.Arrow:
//                //    if(successfulHit)
//                //        StartCoroutine(CR_TriggerVFX(
//                //            PoolSystem.GetPoolObject("vfx_hit_physical", ObjectPoolingCategory.VFX),
//                //            agent.GetComponentInChildren<Collider>().bounds.center + Vector3.up * 0.3f,
//                //            Quaternion.identity));
//                //    break;

//                case DamageType.FIRE:
//                    TriggerVFX(PoolSystem.GetPoolObject("vfx_hit_fire", ObjectPoolingCategory.VFX),
//                        agent.GetAttackPoint().position, Quaternion.identity);
//                    break;

//                case DamageType.COLD:
//                    TriggerVFX(PoolSystem.GetPoolObject("vfx_hit_frost", ObjectPoolingCategory.VFX),
//                        agent.GetAttackPoint().position, Quaternion.identity);
//                    break;

//                case DamageType.ELECTRICITY:
//                    TriggerVFX(
//                        PoolSystem.GetPoolObject("vfx_hit_lightning", ObjectPoolingCategory.VFX),
//                        agent.GetAttackPoint().position, Quaternion.identity);
//                    break;

//                case DamageType.HEAL:
//                    TriggerVFX(PoolSystem.GetPoolObject("vfx_hit_holy", ObjectPoolingCategory.VFX),
//                        agent.GetAttackPoint().position, Quaternion.identity);
//                    break;

//                //case DamageType.Unholy:
//                //    StartCoroutine(CR_TriggerVFX(PoolSystem.GetPoolObject("vfx_hit_unholy", ObjectPoolingCategory.VFX),
//                //        agent.GetComponentInChildren<Collider>().bounds.center, Quaternion.identity));
//                //    break;

//                case DamageType.POISON:
//                    TriggerVFX(PoolSystem.GetPoolObject("vfx_hit_poison", ObjectPoolingCategory.VFX),
//                        agent.GetAttackPoint().position, Quaternion.identity);
//                    break;
//            }

//            Vector3 dir = agressor.transform.position - agent.transform.position;
//            dir.y = 0;
//            TriggerVFX(PoolSystem.GetPoolObject("vfx_bloodsplatter", ObjectPoolingCategory.VFX), agent.GetAttackPoint().position, Quaternion.LookRotation(dir));
//            TriggerVFX(PoolSystem.GetPoolObject("vfx_blood", ObjectPoolingCategory.VFX), agent.GetAttackPoint().position, Quaternion.LookRotation(dir));
//        };

//        agent.OnLevelUp += level =>
//        {
//            TriggerVFX(PoolSystem.GetPoolObject("vfx_notify_levelup", ObjectPoolingCategory.VFX),
//                agent.transform, Quaternion.identity, false);
//        };
//    }

//    //internal static GameObject GetSpellChargeVFX(DamageType effectType)
//    //{
//    //    GameObject chargeVFX = null;
//    //    switch(effectType)
//    //    {
//    //        case DamageType.MAGICFIRE:
//    //        case DamageType.FIRE:
//    //            chargeVFX = PoolSystem.GetPoolObject("vfx_chargespell_fire", ObjectPoolingCategory.VFX);
//    //            break;
//    //        case DamageType.MAGICCOLD:
//    //        case DamageType.COLD:
//    //            chargeVFX = PoolSystem.GetPoolObject("vfx_chargespell_frost", ObjectPoolingCategory.VFX);
//    //            break;
//    //        case DamageType.ELECTRICITY:
//    //            chargeVFX = PoolSystem.GetPoolObject("vfx_chargespell_lightning", ObjectPoolingCategory.VFX);
//    //            break;
//    //        case DamageType.RADIANT:
//    //            chargeVFX = PoolSystem.GetPoolObject("vfx_chargespell_radiant", ObjectPoolingCategory.VFX);
//    //            break;
//    //        case DamageType.HEAL:
//    //            chargeVFX = PoolSystem.GetPoolObject("vfx_chargespell_heal", ObjectPoolingCategory.VFX);
//    //            break;
//    //        case DamageType.POISON:
//    //            break;
//    //    }
//    //    return chargeVFX;
//    //}

//    public static ParticleSystem[] TriggerVFX(GameObject vfxObject, Vector3 position, Quaternion rotation, float duration = 0)
//    {
//        if(duration < 0)
//            duration = 0;
//        ParticleSystem[] pe = vfxObject.GetComponentsInChildren<ParticleSystem>();
//        //vfxObject.StopCoroutine("CR_TriggerVFX");
//        CoroutineRunner.Instance.StartCoroutine(CR_TriggerVFX(vfxObject, pe, position, rotation, duration));

//        return pe;
//    }

//    public static ParticleSystem[] TriggerVFX(GameObject vfxObject, Transform target, Quaternion rotation, bool rotateWithTarget,
//        float duration = 0)
//    {
//        if(duration < 0)
//            duration = 0;
//        ParticleSystem[] pe = vfxObject.GetComponentsInChildren<ParticleSystem>();
//        CoroutineRunner.Instance.StartCoroutine(CR_TriggerVFX(vfxObject, pe, target, rotation, rotateWithTarget, duration));

//        return pe;
//    }

//    static IEnumerator CR_TriggerVFX(GameObject vfxObject, ParticleSystem[] pe, Vector3 position, Quaternion rotation, float duration = 0)
//    {
//        vfxObject.transform.position = position;
//        vfxObject.transform.rotation = rotation;
//        VisualEffect ve = vfxObject.GetComponentInChildren<VisualEffect>();
//        if(ve != null)
//        {
//            //ve.Play();
//            if(duration <= 0)
//                duration = 1.5f;
//            //ve.playRate = 2;
//            yield return new WaitForSeconds(duration);
//            ve.Stop();
//            yield return new WaitUntil(() => ve.aliveParticleCount <= 0);
//            vfxObject.SetActive(false);
//            //Debug.Log("___ ____ _____ Triggering VFX Routine 1");
//            yield break;
//        }

        
        

//        float longestDuration = 0;
//        var longestIndex = 0;

//        for(var i = 0; i < pe.Length; i++)
//        {
//            if(longestDuration < pe[i].main.startLifetime.constant)
//            {
//                longestIndex = i;
//                longestDuration = pe[i].main.startLifetime.constant;
//            }

//            pe[i].Emit(1);
//            yield return null;
//        }

//        //if(pe.Length > 0)
//        if(duration > 0)
//            yield return new WaitUntil(() => (duration -= Time.deltaTime) <= 0);
//        else
//            yield return new WaitUntil(() => pe[longestIndex].IsAlive(true) == false);

//        //for(int i = 0; i < pe.Length; i++) {
//        //    pe[i].Stop(false, ParticleSystemStopBehavior.StopEmitting);
//        //    yield return null;
//        //}

//        //Debug.Log("<color=orange>>></color> Disabled object '" + vfxObject.name + "'");
//        vfxObject.SetActive(false);
//    }

//    static IEnumerator CR_TriggerVFX(GameObject vfxObject, ParticleSystem[] pe, Transform target, Quaternion rotation,
//        bool copyTargetRotation, float duration = 0)
//    {
//        float longestDuration = 0;
//        var longestIndex = 0;
//        vfxObject.transform.position = target.position;
//        vfxObject.transform.rotation = copyTargetRotation ? target.rotation : rotation;

//        VisualEffect ve = vfxObject.GetComponentInChildren<VisualEffect>();
//        if(ve != null)
//        {
//            if(duration <= 0)
//                duration = 1.5f;
//            //ve.playRate = 2;
//            //yield return new WaitForSeconds(duration);
//            //yield return new WaitUntil(() => ve.aliveParticleCount <= 0);
//            while(duration > 0)
//            {
//                duration -= Time.deltaTime;
//                vfxObject.transform.position = target.position;
//                vfxObject.transform.rotation = copyTargetRotation ? target.rotation : rotation;

//                yield return null;
//            }
//            //ve.Stop();
//            vfxObject.SetActive(false);
//            Debug.Log("___ ____ _____ Triggering VFX routine at transform");
//            yield break;
//        }

//        for(var i = 0; i < pe.Length; i++)
//        {
//            if(longestDuration < pe[i].main.startLifetime.constant)
//            {
//                longestIndex = i;
//                longestDuration = pe[i].main.startLifetime.constant;
//            }

//            //pe[i].emission.enabled = true;
//            pe[i].Simulate(1, false, true);
//            pe[i].Play();
//            //pe[i].Emit(1);
//            yield return null;
//        }

//        if(duration > 0)
//        {
//            while((duration -= Time.deltaTime) > 0)
//            {
//                if(target == null)
//                    break;

//                vfxObject.transform.position = target.position;
//                vfxObject.transform.rotation = copyTargetRotation ? target.rotation : rotation;
//                yield return null;
//            }
//        }
//        else
//        {
//            if(pe[longestIndex].main.loop)
//            {
//                while(vfxObject.activeInHierarchy)
//                {
//                    if(target == null)
//                        yield break;

//                    vfxObject.transform.position = target.position;
//                    vfxObject.transform.rotation = copyTargetRotation ? target.rotation : rotation;
//                    yield return null;
//                }

//                Debug.Log("<color=pink>#</color>");
//                yield break;
//            }

//            while(pe[longestIndex].IsAlive(true))
//            {
//                if(target == null)
//                    break;

//                //Debug.Log("<color=yellow>Assertion: " + (vfxObject.transform.position == target.position) + "</color>");
//                yield return null;
//            }
//        }

//        vfxObject.SetActive(false);
//    }

//    /// <summary>
//    /// Trigger VFX Graph that adds particles to a SkinnedMeshRenderer. For effects like a goblin on fire.
//    /// </summary>
//    public static void TriggerActorVFX(ActorInput target, ActorMeshEffectType actorMeshEffectType, float duration) 
//    {
//        if(duration < 0)
//            duration = 0;

//        GameObject vfxObject = null;

//        switch(actorMeshEffectType)
//        {
//            case ActorMeshEffectType.None:
//                break;
//            case ActorMeshEffectType.Fire:
//                vfxObject = PoolSystem.GetPoolObject("vfx_actormeshfx_fire", ObjectPoolingCategory.VFX);
//                break;
//            case ActorMeshEffectType.Ice:
//                break;
//            case ActorMeshEffectType.Lightning:
//                break;
//            case ActorMeshEffectType.Holy:
//                vfxObject = PoolSystem.GetPoolObject("vfx_actormeshfx_holy", ObjectPoolingCategory.VFX);
//                break;
//        }

//        if(vfxObject != null)
//            CoroutineRunner.Instance.StartCoroutine(CR_TriggerActorVFX(vfxObject, target, duration));
//        else
//            Debug.LogError("VFX object for actor mesh effect '" + actorMeshEffectType + "' not found");
//    }

//    static IEnumerator CR_TriggerActorVFX(GameObject vfxObject, ActorInput target, float duration = 0)
//    {
//        //float longestDuration = 0;
//        //var longestIndex = 0;
//        //vfxObject.transform.position = target.transform.position;
//        //vfxObject.transform.rotation = target.transform.rotation;
//        VisualEffect ve = target.transform.GetComponentInChildren<VisualEffect>();
//        if(ve == null)
//            yield break;
//        ve.Play();
//        //ve.SetSkinnedMeshRenderer("SkinnedMeshRenderer", target.transform.GetComponentInChildren<SkinnedMeshRenderer>());
//        //UnityEngine.VFX.Utility.VFXPropertyBinder binder = ve.GetComponentInChildren<UnityEngine.VFX.Utility.VFXPropertyBinder>();
//        //binder.m_Bindings[0].UpdateBinding(ve);
//        if(ve != null)
//        {
//            if(duration <= 0)
//                duration = 1.5f;
//            //ve.playRate = 2;
//            //yield return new WaitForSeconds(duration);
//            //yield return new WaitUntil(() => ve.aliveParticleCount <= 0);
//            while(duration > 0)
//            {
//                duration -= Time.deltaTime;
//                //vfxObject.transform.position = target.position;
//                //vfxObject.transform.rotation = copyTargetRotation ? target.rotation : rotation;

//                yield return null;
//            }
//            ve.Stop();
//            //ve.Stop();
//            //vfxObject.SetActive(false);
//            Debug.Log("___ ____ _____ Triggering actor mesh VFX routine");
//            yield break;
//        }

//        vfxObject.SetActive(false);
//    }
//}