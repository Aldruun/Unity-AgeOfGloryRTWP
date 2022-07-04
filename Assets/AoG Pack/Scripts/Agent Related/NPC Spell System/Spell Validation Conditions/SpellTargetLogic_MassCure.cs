using System.Linq;
using UnityEngine;

public class SpellTargetLogic_MassCure : SpellTargetLogic
{
    private Collider[] _targetList;
   
    public override SpellTargetLogic Init(ActorInput owner, ActorInput target, Spell spell, Projectile projectile)
    {
        this.owner = owner;
        this.spell = spell;
        spellTarget = target;
        this.targetPosition = targetPosition;
        GameObject vfxObj = PoolSystem.GetPoolObject("vfx_masscure_wave", ObjectPoolingCategory.VFX);
        VFXPlayer.TriggerVFX(vfxObj, owner.transform.position, Quaternion.identity);
        _targetList = new Collider[6];
        return this;
    }

    public override bool Done()
    {
        //projectile.transform.position = Vector3.MoveTowards(projectile.transform.position, targetPosition, Time.deltaTime * 8);

        //projectile.StopAndDisable();
        HandleAOEHeal();

        return true;
    }

    private void HandleAOEHeal()
    {
        
        int numHits = Physics.OverlapSphereNonAlloc(owner.transform.position, spell.magicEffects[0].aoeRadius, _targetList, 1 << LayerMask.NameToLayer("Actors"));

        if(numHits == 0)
        {
            return;
        }

        //Debug.Log("________________SLEEP");

        ActorInput[] targets = new ActorInput[numHits];


        for(int i = 0; i < numHits; i++)
        {
            Collider col = _targetList[i];
            targets[i] = col.GetComponent<ActorInput>();
            //Debug.Log("________________SLEEP TARGET HIT");
        }
        //_targetList.Select(c => (ActorInput)c.GetComponent<ActorInput>()).ToArray();

        if(targets.Length == 0)
        {
            return;
        }

        targets = targets.Where(t => t.PartyIndex > 0).ToArray();

        for(int i = 0; i < targets.Length; i++)
        {
            //int healAmount = DnD.Roll(3 + ((spell.slotLevel - 1) * 2), 8);

            //Debug.Log("________________SLEEP CREATE PROJECTILE");
            //CreateTargetEffect(targets[i], healAmount);
        }

    }

    private void CreateTargetEffect(ActorInput target, int healAmount)
    {
        //GameObject vfxObj = PoolSystem.GetPoolObject("vfx_masscure_healeffect", ObjectPoolingCategory.VFX);
        //Projectile projectile = vfxObj.GetComponent<Projectile>();

        //Debug.Log(spellAnchor.name);
        //! Create the projectile
        //VFXManager.TriggerVFX(vfxObj, from, Quaternion.identity);


        //if(projectile != null)
        //{

        //VFXManager.TriggerVFX(PoolSystem.GetPoolObject("vfx_masscure_healeffect", ObjectPoolingCategory.VFX), target.transform, Quaternion.identity, false);
        //target.Execute_ModifyHealth(healAmount, ModType.ADDITIVE);
        //}
        //else
        //{
        //    Debug.LogError("Projectile null");
        //}
    }
}