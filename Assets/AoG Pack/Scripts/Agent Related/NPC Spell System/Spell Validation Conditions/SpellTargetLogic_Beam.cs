using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpellTargetLogic_Beam : SpellTargetLogic
{
    public float beamThickness = 1;
    public bool firstTickAt0 = true;
    public int numTicks = 1;
    private int _ticksAvailable;
    public float tickInterval = 1;
    private float _damageTick01Timer;

    private float _lifeTime;

    private RaycastHit[] _beamEnemyList;
    private ParticleSystem _tempParticleSystem;
    private Transform _vfxBeamStart;

    public override SpellTargetLogic Init(ActorInput owner, ActorInput target, Spell spell, Projectile projectile)
    {
        GameObject vfxObj = PoolSystem.GetPoolObject("vfx_agannazarsscorcher_beam", ObjectPoolingCategory.VFX);
        VFXPlayer.TriggerVFX(vfxObj, owner.Equipment.spellAnchor.position, Quaternion.identity);
        vfxObj.transform.LookAt(target.Combat.GetAttackPoint());
        _vfxBeamStart = vfxObj.transform;
        this.owner = owner;
        spellTarget = target;
        this.spell = spell;
        //this.projectile = projectile;
        _beamEnemyList = new RaycastHit[5];
        _lifeTime = spell.magicEffectsData[0].Rounds * 6;
        _damageTick01Timer = firstTickAt0 ? 0 : tickInterval;
        _ticksAvailable = numTicks;
        _tempParticleSystem = vfxObj.GetComponentInChildren<ParticleSystem>();
        return this;
    }

    public override bool Done()
    {
        _lifeTime -= Time.deltaTime;
        if(_lifeTime <= 0)
        {
            _tempParticleSystem.Stop();
            return true; 
        }
        
        HandleBeamEffect(beamThickness);
        _vfxBeamStart.position = owner.Equipment.spellAnchor.position;
        _vfxBeamStart.LookAt(spellTarget.transform.position);
        return false;
    }

    private void HandleBeamEffect(float diameter)
    {
        Vector3 dir = spellTarget.transform.position - owner.Equipment.spellAnchor.position;
        ParticleSystem.ShapeModule shape = _tempParticleSystem.shape;
        shape.length = dir.magnitude;

        //Debug.Log(owner.ActorRecord.Name + ": Handling line effect");
        int numHits = Physics.SphereCastNonAlloc(_vfxBeamStart.position, diameter, dir.normalized, _beamEnemyList, dir.magnitude, 1 << LayerMask.NameToLayer("Actors"));

        _damageTick01Timer -= Time.deltaTime;
        if(_damageTick01Timer <= 0 && _ticksAvailable > 0)
        {
            _ticksAvailable--;
            _damageTick01Timer = tickInterval;
            for(int i = 0; i < numHits; i++)
            {
                ActorInput beamTarget = _beamEnemyList[i].collider.GetComponent<ActorInput>();

                if(beamTarget == owner) // Don't ignite the caster
                    continue;

                //if(beamTarget == target)
                //{
                //    continue;
                //}

                beamTarget.Combat.ApplyDamage(owner, null, null, false);
                //var statuseffect = spell.magicEffects[0]._statusEffect;
                //if(statuseffect.rounds > 0)
                //{
                //    beamTarget.Execute_ApplyStatusEffect(statuseffect.statusType, statuseffect.rounds);
                //}
            } 
        }
    }

}