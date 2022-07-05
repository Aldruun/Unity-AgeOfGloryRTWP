using AoG.Core;
using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

public enum ActorMeshEffectType
{
    None,
    Fire,
    Ice,
    Lightning,
    Holy
}

public static class VFXPlayer
{
    public static void PlayVFX_OnHit(Vector3 position, Vector3 targetDir, DamageType damageType)
    {
        //TriggerVFX(PoolSystem.GetPoolObject("vfx_hit_bloodsplatter", ObjectPoolingCategory.VFX), victim.GetAttackPoint().position, Quaternion.LookRotation(dir));
        //TriggerVFX(PoolSystem.GetPoolObject("vfx_bloodsplatter", ObjectPoolingCategory.VFX), victim.GetAttackPoint().position, Quaternion.LookRotation(dir));
        TriggerVFX(PoolSystem.GetPoolObject("vfx_blood", ObjectPoolingCategory.VFX), position, Quaternion.LookRotation(targetDir));

        switch(damageType)
        {

            case DamageType.CRUSHING:
            case DamageType.PIERCING:
            case DamageType.SLASHING:
            case DamageType.MISSILE:
                TriggerVFX(PoolSystem.GetPoolObject("vfx_hit_default", ObjectPoolingCategory.VFX), position, Quaternion.identity);
                //if(successfulHit)
                //    TriggerVFX(PoolSystem.GetPoolObject("vfx_hit_physical", ObjectPoolingCategory.VFX), victim.monoObject.GetComponentInChildren<Collider>().bounds.center + Vector3.up * 0.3f, Quaternion.identity);
                break;
            case DamageType.MAGICFIRE:
            case DamageType.FIRE:
                TriggerVFX(PoolSystem.GetPoolObject("vfx_hit_fire", ObjectPoolingCategory.VFX), position, Quaternion.identity);
                break;
            case DamageType.MAGICCOLD:
                //case DamageType.COLD:
                //    TriggerVFX(PoolSystem.GetPoolObject("vfx_hit_frost", ObjectPoolingCategory.VFX), victim.monoObject.GetComponentInChildren<Collider>().bounds.center, Quaternion.identity);
                break;
            case DamageType.ELECTRICITY:
                TriggerVFX(PoolSystem.GetPoolObject("vfx_hit_lightning", ObjectPoolingCategory.VFX), position, Quaternion.identity);
                break;
            //case DamageType.HOLY:
            //    TriggerVFX(PoolSystem.GetPoolObject("vfx_hit_holy", ObjectPoolingCategory.VFX), position, Quaternion.identity);
            //    break;
            //case DamageType.UNHOLY:
            //    TriggerVFX(PoolSystem.GetPoolObject("vfx_hit_unholy", ObjectPoolingCategory.VFX), position, Quaternion.identity);
                break;
            case DamageType.POISON:
                TriggerVFX(PoolSystem.GetPoolObject("vfx_hit_poison", ObjectPoolingCategory.VFX), position, Quaternion.identity);
                break;
        }

    }

    public static void PlayVFX_OnLevelUp(Actor self, int level)
    {
        TriggerVFX(PoolSystem.GetPoolObject("vfx_notify_levelup", ObjectPoolingCategory.VFX), self.transform, Vector3.zero, Quaternion.identity, false);
    }

    public static GameObject SetSpellHandVFX(Transform spellAnchor, string vfxID)
    {
        GameObject _handVFX = PoolSystem.GetPoolObject(vfxID, ObjectPoolingCategory.VFX);
        _handVFX.transform.SetParent(spellAnchor);
        _handVFX.transform.localPosition = Vector3.zero;
        _handVFX.transform.localRotation = Quaternion.identity;
        return _handVFX;
    }

    public static void RemoveHandVFX(GameObject handVFX)
    {
        if(handVFX != null) // Reason: Could be called while NPC had no spell
        {
            PoolSystem.StorePoolObject(handVFX.transform, ObjectPoolingCategory.VFX);
        }
    }

    //public static void TriggerVFX(GameObject vfxObject, Vector3 position, Quaternion rotation, float duration = 0)
    //{

    //    if(duration < 0)
    //    {

    //        duration = 0;
    //    }
    //    //vfxObject.StopCoroutine("CR_TriggerVFX");
    //    CoroutineRunner.Instance.StartCoroutine(CR_TriggerVFX(vfxObject, position, rotation, duration));
    //}
    //public static void TriggerVFX(GameObject vfxObject, Transform target, Quaternion rotation, bool rotateWithTarget, float duration = 0)
    //{

    //    if(duration < 0)
    //    {

    //        duration = 0;
    //    }

    //    CoroutineRunner.Instance.StartCoroutine(CR_TriggerVFX(vfxObject, target, rotation, rotateWithTarget, duration));
    //}
    //static IEnumerator CR_TriggerVFX(GameObject vfxObject, Vector3 position, Quaternion rotation, float duration = 0)
    //{

    //    vfxObject.transform.position = position;
    //    vfxObject.transform.rotation = rotation;
    //    ParticleSystem[] pe = vfxObject.GetComponentsInChildren<ParticleSystem>();

    //    float longestDuration = 0;
    //    int longestIndex = 0;

    //    for(int i = 0; i < pe.Length; i++)
    //    {

    //        if(longestDuration < pe[i].main.startLifetime.constant)
    //        {

    //            longestIndex = i;
    //            longestDuration = pe[i].main.startLifetime.constant;
    //        }

    //        pe[i].Emit(1);
    //        yield return null;
    //    }

    //    if(pe.Length > 0)
    //        yield return new WaitUntil(() => pe[longestIndex].IsAlive(true) == false);
    //    else
    //        yield return new WaitUntil(() => (duration -= Time.deltaTime) <= 0);

    //    //for(int i = 0; i < pe.Length; i++) {

    //    //    pe[i].Stop(false, ParticleSystemStopBehavior.StopEmitting);
    //    //    yield return null;
    //    //}

    //    //Debug.Log("<color=orange>>></color> Disabled object '" + vfxObject.name + "'");
    //    vfxObject.SetActive(false);
    //}
    //static IEnumerator CR_TriggerVFX(GameObject vfxObject, Transform target, Quaternion rotation, bool copyTargetRotation, float duration = 0)
    //{

    //    ParticleSystem[] pe = vfxObject.GetComponentsInChildren<ParticleSystem>();
    //    float longestDuration = 0;
    //    int longestIndex = 0;
    //    vfxObject.transform.position = target.position;
    //    vfxObject.transform.rotation = copyTargetRotation ? target.rotation : rotation;
    //    for(int i = 0; i < pe.Length; i++)
    //    {


    //        if(longestDuration < pe[i].main.startLifetime.constant)
    //        {

    //            longestIndex = i;
    //            longestDuration = pe[i].main.startLifetime.constant;
    //        }
    //        //pe[i].emission.enabled = true;
    //        pe[i].Simulate(1, false, true);
    //        pe[i].Play();
    //        //pe[i].Emit(1);
    //        yield return null;
    //    }

    //    if(duration > 0)
    //    {

    //        while((duration -= Time.deltaTime) > 0)
    //        {

    //            if(target == null)
    //                break;

    //            vfxObject.transform.position = target.position;
    //            vfxObject.transform.rotation = copyTargetRotation ? target.rotation : rotation;
    //            yield return null;
    //        }
    //    }
    //    else
    //    {

    //        if(pe[longestIndex].main.loop == true)
    //        {

    //            while(vfxObject.activeInHierarchy)
    //            {
    //                if(target == null)
    //                    yield break;

    //                vfxObject.transform.position = target.position;
    //                vfxObject.transform.rotation = copyTargetRotation ? target.rotation : rotation;
    //                yield return null;
    //            }
    //            Debug.Log("<color=pink>#</color>");
    //            yield break;
    //        }
    //        else
    //        {
    //            while(pe[longestIndex].IsAlive(true))
    //            {

    //                if(target == null)
    //                    break;


    //                //Debug.Log("<color=yellow>Assertion: " + (vfxObject.transform.position == target.position) + "</color>");
    //                yield return null;
    //            }
    //        }
    //    }

    //    vfxObject.SetActive(false);
    //}





    public static ParticleSystem[] TriggerVFX(GameObject vfxObject, Vector3 position, Quaternion rotation, float duration = 0)
    {
        if(duration < 0)
            duration = 0;
        ParticleSystem[] pe = vfxObject.GetComponentsInChildren<ParticleSystem>();
        //vfxObject.StopCoroutine("CR_TriggerVFX");
        CoroutineRunner.Instance.StartCoroutine(CR_TriggerVFX(vfxObject, pe, position, rotation, duration));

        return pe;
    }

    public static ParticleSystem[] TriggerVFX(GameObject vfxObject, Transform target, Vector3 offset, Quaternion rotation, bool rotateWithTarget,
        float duration = 0)
    {
        if(duration < 0)
            duration = 0;
        ParticleSystem[] pe = vfxObject.GetComponentsInChildren<ParticleSystem>();
        CoroutineRunner.Instance.StartCoroutine(CR_TriggerVFX(vfxObject, pe, target, offset, rotation, rotateWithTarget, duration));

        return pe;
    }

    private static IEnumerator CR_TriggerVFX(GameObject vfxObject, ParticleSystem[] pe, Vector3 position, Quaternion rotation,
        float duration = 0)
    {
        vfxObject.transform.position = position;
        vfxObject.transform.rotation = rotation;
        VisualEffect ve = vfxObject.GetComponentInChildren<VisualEffect>();
        if(ve != null)
        {
            //ve.Play();
            if(duration <= 0)
                duration = 1.5f;
            ve.Reinit();
            //ve.Play();
            //ve.playRate = 2;
            yield return new WaitForSeconds(duration);
            ve.Stop();
            yield return new WaitUntil(() => ve.aliveParticleCount <= 0);

            vfxObject.SetActive(false);
            //Debug.Log("___ ____ _____ Triggering VFX Routine 1");
            yield break;
        }




        float longestDuration = 0;
        var longestIndex = 0;

        for(var i = 0; i < pe.Length; i++)
        {
            if(duration > 0)
            {
                if(longestDuration < pe[i].main.startLifetime.constant)
                {
                    longestIndex = i;
                    longestDuration = pe[i].main.startLifetime.constant;
                }
            }

            pe[i].Emit(1);
            yield return null;
        }

        //if(pe.Length > 0)
        if(duration > 0)
            yield return new WaitUntil(() => (duration -= Time.deltaTime) <= 0);
        else
        {
            //if(pe[longestIndex] != null)
            yield return new WaitUntil(() => /*pe[longestIndex] == null ||*/ pe[longestIndex].IsAlive(true) == false);
        }

        //for(int i = 0; i < pe.Length; i++) {
        //    pe[i].Stop(false, ParticleSystemStopBehavior.StopEmitting);
        //    yield return null;
        //}

        //Debug.Log("<color=orange>>></color> Disabled object '" + vfxObject.name + "'");
        vfxObject.SetActive(false);
    }

    private static IEnumerator CR_TriggerVFX(GameObject vfxObject, ParticleSystem[] pe, Transform target, Vector3 offset, Quaternion rotation,
        bool copyTargetRotation, float duration = 0)
    {
        float longestDuration = 0;
        var longestIndex = 0;
        vfxObject.transform.position = target.position;
        vfxObject.transform.rotation = copyTargetRotation ? target.rotation : rotation;

        VisualEffect ve = vfxObject.GetComponentInChildren<VisualEffect>();
        if(ve != null)
        {
            if(duration <= 0)
                duration = 1.5f;
            ve.Reinit();
            //ve.playRate = 2;
            //yield return new WaitForSeconds(duration);
            //yield return new WaitUntil(() => ve.aliveParticleCount <= 0);
            while(duration > 0)
            {
                duration -= Time.deltaTime;
                vfxObject.transform.position = target.position;
                vfxObject.transform.rotation = copyTargetRotation ? target.rotation : rotation;

                yield return null;
            }
            ve.Stop();
            yield return new WaitUntil(() => ve.aliveParticleCount <= 0);
            vfxObject.SetActive(false);
            Debug.Log("___ ____ _____ Triggering VFX Routine 2");
            yield break;
        }

        for(var i = 0; i < pe.Length; i++)
        {
            if(duration <= 0)
            {
                if(longestDuration < pe[i].main.startLifetime.constant)
                {
                    longestIndex = i;
                    longestDuration = pe[i].main.startLifetime.constant;
                }
            }

            //pe[i].emission.enabled = true;
            pe[i].Simulate(1, false, true);
            pe[i].Play();
            //pe[i].Emit(1);
            yield return null;
        }

        if(duration > 0)
        {
            while((duration -= Time.deltaTime) > 0)
            {
                if(target == null)
                    break;

                vfxObject.transform.position = target.position;
                vfxObject.transform.rotation = copyTargetRotation ? target.rotation : rotation;
                yield return null;
            }
        }
        else
        {
            if(pe[longestIndex].main.loop)
            {
                while(vfxObject.activeInHierarchy)
                {
                    if(target == null)
                        yield break;

                    vfxObject.transform.position = target.position;
                    vfxObject.transform.rotation = copyTargetRotation ? target.rotation : rotation;
                    yield return null;
                }

                Debug.Log("<color=pink>#</color>");
                yield break;
            }

            while(pe[longestIndex].IsAlive(true))
            {
                if(target == null)
                    break;

                //Debug.Log("<color=yellow>Assertion: " + (vfxObject.transform.position == target.position) + "</color>");
                yield return null;
            }
        }

        vfxObject.SetActive(false);
    }

    //    /// <summary>
    //    /// Trigger VFX Graph that adds particles to a SkinnedMeshRenderer. For effects like a goblin on fire.
    //    /// </summary>
    public static void TriggerActorVFX(Actor target, ActorMeshEffectType actorMeshEffectType, float duration)
    {
        if(duration < 0)
            duration = 0;

        GameObject vfxObject = null;

        switch(actorMeshEffectType)
        {
            case ActorMeshEffectType.None:
                break;
            case ActorMeshEffectType.Fire:
                vfxObject = PoolSystem.GetPoolObject("vfx_actormeshfx_fire", ObjectPoolingCategory.VFX);
                break;
            case ActorMeshEffectType.Ice:
                break;
            case ActorMeshEffectType.Lightning:
                break;
            case ActorMeshEffectType.Holy:
                vfxObject = PoolSystem.GetPoolObject("vfx_actormeshfx_holy", ObjectPoolingCategory.VFX);
                break;
        }

        if(vfxObject != null)
            CoroutineRunner.Instance.StartCoroutine(CR_TriggerActorVFX(vfxObject, target, duration));
        else
            Debug.LogError("VFX object for actor mesh effect '" + actorMeshEffectType + "' not found");
    }

    private static IEnumerator CR_TriggerActorVFX(GameObject vfxObject, Actor target, float duration = 0)
    {
        //float longestDuration = 0;
        //var longestIndex = 0;
        //vfxObject.transform.position = target.transform.position;
        //vfxObject.transform.rotation = target.transform.rotation;
        VisualEffect ve = target.transform.GetComponentInChildren<VisualEffect>();
        if(ve == null)
            yield break;
        ve.Play();
        //ve.SetSkinnedMeshRenderer("SkinnedMeshRenderer", target.transform.GetComponentInChildren<SkinnedMeshRenderer>());
        //UnityEngine.VFX.Utility.VFXPropertyBinder binder = ve.GetComponentInChildren<UnityEngine.VFX.Utility.VFXPropertyBinder>();
        //binder.m_Bindings[0].UpdateBinding(ve);
        if(ve != null)
        {
            if(duration <= 0)
                duration = 1.5f;
            //ve.playRate = 2;
            //yield return new WaitForSeconds(duration);
            //yield return new WaitUntil(() => ve.aliveParticleCount <= 0);
            while(duration > 0)
            {
                duration -= Time.deltaTime;
                //vfxObject.transform.position = target.position;
                //vfxObject.transform.rotation = copyTargetRotation ? target.rotation : rotation;

                yield return null;
            }
            ve.Stop();
            //ve.Stop();
            //vfxObject.SetActive(false);
            Debug.Log("___ ____ _____ Triggering actor mesh VFX routine");
            yield break;
        }

        vfxObject.SetActive(false);
    }
}
