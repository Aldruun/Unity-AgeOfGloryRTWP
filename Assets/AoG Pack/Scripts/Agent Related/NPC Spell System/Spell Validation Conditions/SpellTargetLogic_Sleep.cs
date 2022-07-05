using System.Linq;
using UnityEngine;

public class SpellTargetLogic_Sleep : SpellTargetLogic
{
    private Collider[] _targetList;

    public override SpellTargetLogic Init(Actor owner, Vector3 targetPosition, Spell spell, Projectile projectile)
    {
        this.owner = owner;
        this.spell = spell;
        this.targetPosition = targetPosition + Vector3.up * 0.2f;

        GameObject vfxObj = PoolSystem.GetPoolObject("vfx_projectile_sleep", ObjectPoolingCategory.VFX);
        VFXPlayer.TriggerVFX(vfxObj, owner.Equipment.spellAnchor.position, Quaternion.identity);
        this.projectile = vfxObj.GetComponent<Projectile>();
        _targetList = new Collider[10];
        return this;
    }

    public override bool Done()
    {
        if(projectile == null)
        {
            return true;
        }
        //projectile.transform.position = Vector3.MoveTowards(projectile.transform.position, targetPosition, Time.deltaTime * 8);
        //if(projectile.ReachedTarget(Vector3.zero, targetPosition, 8))
        //{
        //    projectile.StopAndDisable();
        //    AddAOESleepEffect();
        //    return true;
        //}
        return false;
    }

    private void AddAOESleepEffect()
    {
        //int totalSleepHitpoints = DnD.Roll(5 + ((spell.slotLevel - 1) * 2), 8);
        int numHits = Physics.OverlapSphereNonAlloc(projectile.transform.position, spell.magicEffects[0].aoeRadius, _targetList, 1 << LayerMask.NameToLayer("Actors"));

        if(numHits == 0)
        {
            return;
        }

        //Debug.Log("________________SLEEP");

        Actor[] targets = new Actor[numHits];


        for(int i = 0; i < numHits; i++)
        {
            Collider col = _targetList[i];
            targets[i] = col.GetComponent<Actor>();
            //Debug.Log("________________SLEEP TARGET HIT");
        }
        //_targetList.Select(c => (ActorInput)c.GetComponent<ActorInput>()).ToArray();

        if(targets.Length == 0)
        {
            return;
        }

        targets = targets.Where(t => t.PartySlot == 0).OrderBy(t => ActorUtility.GetStatBase(t.ActorStats, ActorStat.HITPOINTS)).ToArray();

        //for(int i = 0; i < targets.Length; i++)
        //{
        //    bool hit = true;
        //    if(totalSleepHitpoints <= 0)
        //        hit = false;
        //    if(targets[i].isDowned || targets[i].CheckStatusEffect(Status.SLEEP) > 0)
        //        hit = false;
        //    if(totalSleepHitpoints - targets[i].GetBaseStat(Stat.HITPOINTS) < 0)
        //        hit = false;

        //    totalSleepHitpoints -= targets[i].GetBaseStat(Stat.HITPOINTS);
        //    //Debug.Log("________________SLEEP CREATE PROJECTILE");
        //    CreateProjectile(targetPosition, targets[i], hit);
        //}

    }

    private void CreateProjectile(Vector3 from, Actor target, bool hit)
    {
        GameObject vfxObj = PoolSystem.GetPoolObject("vfx_projectile_sleep", ObjectPoolingCategory.VFX);
        Projectile projectile = vfxObj.GetComponent<Projectile>();

        //Debug.Log(spellAnchor.name);
        //! Create the projectile
        //VFXManager.TriggerVFX(vfxObj, from, Quaternion.identity);


        if(projectile != null)
        {
            //projectile.LaunchStraightHoming(owner, target, DamageType.SLEEP, ImpactType.Blade, true, 6, 0,0,0);
            //projectile.OnImpact = () =>
            //{
            //    //if(sfxOnHit != null)
            //    //    SFXPlayer.TriggerSFX(sfxOnHit[Random.Range(0, sfxOnHit.Length)],
            //    //        projectile.transform.position);
            //    //! Create this effect when the projectile arrives at its target position
            //    if(hit)
            //    {
            //        //VFXManager.TriggerVFX(PoolSystem.GetPoolObject("vfx_hit_sleep", ObjectPoolingCategory.VFX), projectile.transform.position, Quaternion.identity);
            //        //spellTarget.Execute_ApplyStatusEffect(Status.SLEEP, 10);
            //    }                //if(duration > 1)
                
            //};
            //Debug.LogError("No projectile component on '" + vfxObj.name + "'");
        }
        else
        {
            Debug.LogError("Projectile null");
        }
    }
}