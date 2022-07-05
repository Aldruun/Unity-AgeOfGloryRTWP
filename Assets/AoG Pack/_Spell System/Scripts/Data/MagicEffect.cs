using System.Collections;
using UnityEngine;

public enum AffectedAttribute
{
    Health,
    Mana
}

[CreateAssetMenu(menuName = "Spell System/Magic Effect")]
public class MagicEffect : ScriptableObject
{
    SpellTargetType _targetType;
    public AffectedAttribute affectedAttribute;
    DamageType _effectType;
    SavingThrowType _savingThrowType;
    CastingType _castingType;
    DeliveryType _deliveryType;
    public StatusEffectData _statusEffect;

    //float _baseCost;
    bool _attackRollRequired;
    SpellAttackRollType _attackRollType;
    int _numAtkDice;
    int _numAtkDiceSides;
    int _numDamageDice;
    int _numDamageDiceSides;
    bool _percentMagnitude;

    public float duration;
    public float aoeRadius;
    public float effectDiameter;
    public float effectRange;
    //public List<StatusEffect> statusEffectsTarget;

    public string id_VFXProjectile;
    public string id_VFXOnHit;
    public string id_VFXHandIdle;
    public string id_VFXHandCharge;
    public string id_VFXHandRelease;
    public ProjectileType projectileType;
    public float projectileSpeed = 10;
    public bool destroyOnImpact = true;
    public bool clampVFXToTarget;

    [Header("Sounds")]
    public WeaponImpactType impactSFXType;
    //public AudioClip sfxDraw;
    //public AudioClip sfxSheath;
    //public AudioClip sfxCharge;
    //public AudioClip sfxReady;
    public AudioClip[] sfxOnRelease;
    //public AudioClip sfxCastLoop;
    public AudioClip[] sfxOnHit;
    public AudioClip sfxChargeSpell;

    //private Collider[] _friends;
    public Projectile projectile { get; protected set; }

    public Actor spellTarget { get; protected set; }

    bool _active;

    SpellTargetLogic _effectLogic;

    Spell _spell;
    public string id_VFXOnInflict;
    public string id_VFXOnImpact;

    public virtual void Init(Spell spell)
    {
        _spell = spell;
        _attackRollRequired = spell.attackRollRequired;
        _attackRollType = spell.attackRollType;
        _effectLogic = spell.targetLogic;

        effectDiameter = spell.effectDiameter;
        effectRange = spell.effectRange;
        aoeRadius = spell.aoeRadius;
        _targetType = spell.spellTargetType;
        _effectType = spell.effectType;
        _savingThrowType = spell.savingThrowType;
        _percentMagnitude = spell.percentMagnitude;
        _castingType = spell.castingType;
        _deliveryType = spell.deliveryType;
        //_baseCost = spell.cost;
        _numDamageDice = spell.damageRollDice;
        _numDamageDiceSides = spell.numDamageRollDieSides;
        //if(spell.keywords.Contains(Keyword.Heal))
        //    _friends = new Collider[5];

        if(sfxOnHit != null && sfxOnHit.Length == 0)
            sfxOnHit = null;
    }

    //protected bool ConditionsMet(AgentMonoController agent)
    //{
    //    switch(_delivery)
    //    {
    //        case DeliveryType.Self:
    //            break;
    //        case DeliveryType.Contact:
    //            break;
    //        case DeliveryType.Aimed:
    //            break;
    //        case DeliveryType.TargetActor:
    //            break;
    //        case DeliveryType.TargetLocation:
    //            break;
    //        default:
    //            break;
    //    }

    //}

    public void SpawnEffect(Actor self, Actor target, Transform spellAnchor)
    {
        if(_effectLogic != null)
        {
            Debug.Assert(_spell != null, "Spell null");
            Debug.Assert(self != null, "owner null");
            Debug.Assert(target != null, "target null");
            _effectLogic.Init(self, target, _spell, projectile);

            self.StartCoroutine(CR_UpdateEffectLogic());
            return;
        }
        //if(_projectile != null)
        //{
        //    return;
        //}
        //int attackResult = DnD.Roll(_numDice, _numDiceSides);

        spellTarget = target;
        if(spellTarget == null)
            return;

        if(id_VFXHandRelease != "")
            VFXPlayer.TriggerVFX(PoolSystem.GetPoolObject(id_VFXHandRelease, ObjectPoolingCategory.VFX), spellAnchor.position, Quaternion.LookRotation(self.transform.forward, Vector3.up));
        if(id_VFXHandIdle != "")
            VFXPlayer.TriggerVFX(PoolSystem.GetPoolObject(id_VFXHandIdle, ObjectPoolingCategory.VFX), spellAnchor, Vector3.zero, Quaternion.identity, false, 5);

        if(sfxOnRelease != null && sfxOnRelease.Length > 0)
        {
            SFXPlayer.TriggerSFX(sfxOnRelease[Random.Range(0, sfxOnRelease.Length)], spellAnchor.position);
        }

        // Special cases
        if(projectileType == ProjectileType.MagicMissile)
        {
            Vector3 dir = target.transform.position - self.transform.position;
            int lvl = self.ActorStats.Level;
            int numProjectiles = lvl < 3 ? 1 : lvl < 5 ? 2 : lvl < 7 ? 3 : lvl < 9 ? 4 : 5;

            Vector3 startPos = self.transform.position; // umm, start position !
            Vector3 targetPos = Vector3.zero; // variable for calculated end position

            float angle = dir.magnitude * 10;
            //float startAngle = -angle * 0.5f; // half the angle to the Left of the forward
            //float finishAngle = angle * 0.5f; // half the angle to the Right of the forward

            // the gap between each ray (increment)
            int inc = Mathf.CeilToInt(angle / numProjectiles);
            angle *= 0.5f;
            for(float i = -angle; i < angle; i += inc) // Angle from forward
            {
                targetPos = (Quaternion.Euler(0, i, 0) * self.transform.forward).normalized;

                GameObject vfxObjSpecial = PoolSystem.GetPoolObject(id_VFXProjectile, ObjectPoolingCategory.VFX);
                Projectile projectileSpecial = vfxObjSpecial.GetComponent<Projectile>();
                projectileSpecial.transform.rotation = Quaternion.LookRotation(targetPos); // UnityEngine.Random.Range(0.5f, -0.5f)
                projectileSpecial.Launch(_effectLogic, self, spellAnchor.position, target, _statusEffect, _targetType, _deliveryType, projectileType, _effectType, SavingThrowType.None, _attackRollType, new Dice(_numDamageDice, _numDamageDiceSides, 1), projectileSpeed, aoeRadius, effectDiameter, effectRange);

                projectileSpecial.OnImpact = () =>
                {
                    if(sfxOnHit != null)
                        SFXPlayer.TriggerSFX(sfxOnHit[Random.Range(0, sfxOnHit.Length)], projectileSpecial.transform.position);
                    if(id_VFXOnHit != "")
                        VFXPlayer.TriggerVFX(PoolSystem.GetPoolObject(id_VFXOnHit, ObjectPoolingCategory.VFX), projectileSpecial.transform.position, Quaternion.identity);
                };
            }


            return;
        }

        //if(_projectile == null)
        //{
        GameObject vfxObj = PoolSystem.GetPoolObject(id_VFXProjectile, ObjectPoolingCategory.VFX);
        //Debug.Log(spellAnchor.name);
        VFXPlayer.TriggerVFX(vfxObj, spellAnchor.position, Quaternion.identity);

        projectile = vfxObj.GetComponent<Projectile>();
        if(projectile != null)
        {



            projectile.Launch(_effectLogic, self, spellAnchor.position, target, _statusEffect, _targetType, _deliveryType, projectileType, _effectType, _savingThrowType, _attackRollType, new Dice(_numDamageDice, _numDamageDiceSides, 0), projectileSpeed, aoeRadius, effectDiameter, effectRange);
            projectile.OnImpact = () =>
            {
                if(sfxOnHit != null)
                    SFXPlayer.TriggerSFX(sfxOnHit[Random.Range(0, sfxOnHit.Length)], projectile.transform.position);
                if(id_VFXOnHit != "")
                    VFXPlayer.TriggerVFX(PoolSystem.GetPoolObject(id_VFXOnHit, ObjectPoolingCategory.VFX), projectile.transform.position, Quaternion.identity);
                //if(duration > 1)
                //    spellTarget.Execute_ApplyStatusEffect(new StatusEffect_Damage(_effectType, _savingThrowType, 1, false, 1, duration, 1));
            };
        }
        else
        {
            Debug.LogError("Projectile null");
        }
    }
    public void SpawnEffectLocation(Actor self, Vector3? target, Transform spellAnchor)
    {
        if(_effectLogic != null)
        {
            _effectLogic.Init(self, target.Value, _spell, projectile);

            self.StartCoroutine(CR_UpdateEffectLogic());
            return;
        }
        //if(_projectile != null)
        //{
        //    return;
        //}
        //int attackResult = DnD.Roll(_numDice, _numDiceSides);

        if(target == null)
            return;

        if(id_VFXHandRelease != "")
            VFXPlayer.TriggerVFX(PoolSystem.GetPoolObject(id_VFXHandRelease, ObjectPoolingCategory.VFX), spellAnchor.position, Quaternion.LookRotation(self.transform.forward, Vector3.up));
        if(id_VFXHandIdle != "")
            VFXPlayer.TriggerVFX(PoolSystem.GetPoolObject(id_VFXHandIdle, ObjectPoolingCategory.VFX), spellAnchor, Vector3.zero, Quaternion.identity, false, 5);

        if(sfxOnRelease.Length > 0)
        {
            SFXPlayer.TriggerSFX(sfxOnRelease[Random.Range(0, sfxOnRelease.Length)], spellAnchor.position);
        }

        //if(_projectile == null)
        //{
        GameObject vfxObj = PoolSystem.GetPoolObject(id_VFXProjectile, ObjectPoolingCategory.VFX);
        projectile = vfxObj.GetComponent<Projectile>();

        //Debug.Log(spellAnchor.name);
        //! Create the projectile
        VFXPlayer.TriggerVFX(vfxObj, spellAnchor.position, Quaternion.LookRotation(self.transform.forward, Vector3.up));


        if(projectile != null)
        {
            if(_effectLogic != null && _spell != null)
            {
                _effectLogic.Init(self, target.Value, _spell, projectile);
            }

            projectile.Launch(_effectLogic, self, spellAnchor.position, target.Value, _statusEffect, _targetType, _deliveryType, projectileType, _effectType, _savingThrowType, _attackRollType, new Dice(_numDamageDice, _numDamageDiceSides, 0), projectileSpeed, aoeRadius, effectDiameter, effectRange);
            projectile.OnImpact = () =>
            {
                if(sfxOnHit != null)
                    SFXPlayer.TriggerSFX(sfxOnHit[Random.Range(0, sfxOnHit.Length)],
                        projectile.transform.position);
                //! Create this effect when the projectile arrives at its target position
                if(id_VFXOnHit != "")
                    VFXPlayer.TriggerVFX(PoolSystem.GetPoolObject(id_VFXOnHit, ObjectPoolingCategory.VFX), projectile.transform.position, Quaternion.identity);
                //if(duration > 1)
                //    spellTarget.Execute_ApplyStatusEffect(new StatusEffect_Damage(1, false, 1, duration, 1));
            };
            //Debug.LogError("No projectile component on '" + vfxObj.name + "'");
        }
        else
        {
            Debug.LogError("Projectile null");
        }
    }


    public void Extinguish()
    {
        _active = false;
        //if(self.debug)
        Debug.Log("<color=orange>Extinguishing magic effect</color>");

        if(projectile != null)
        {
            projectile.StopCoroutine("CR_ClampToAndFade");
            projectile.StopAndDisable("MagicEffect -> Extinguish");
            projectile = null;
        }
    }

    IEnumerator CR_UpdateEffectLogic()
    {
        while(_effectLogic.Done() == false)
        {
            //Debug.Log("___________UPDATING");
            yield return null;
        }
        Debug.Log("___________UPDATING DONE");
    }

    //IEnumerator CR_UpdateConcentrationEffect(Transform spellAnchor, AgentMonoController self)
    //{
    //    _active = true;
    //    while(_active)
    //    {
    //        if(self.debug)
    //            Debug.Log(self.agentData.Name + ": <color=orange>CR_UpdateConcentrationEffect</color>");
    //        self.Execute_ModifyMana(-(baseCost * GameMaster.Instance.gameSettings.globalManaCostMult * Time.deltaTime));
    //        //_projectile.UpdateStatic(spellAnchor.position, spellAnchor.position + self.transform.forward);
    //        yield return null;
    //    }
    //    if(self.debug)
    //        Debug.Log(self.agentData.Name + ": <color=orange>Stopped effect loop</color>");
    //}

    //public AgentMonoController GetMostWoundedInRangeNonAlloc(AgentMonoController caller, float healthThreshold,
    //    float range, Collider[] populatedArray)
    //{
    //    int numHit = Physics.OverlapSphereNonAlloc(caller.transform.position, range, populatedArray,
    //        1 << LayerMask.NameToLayer("Agents"));

    //    if(numHit == 0)
    //        return null;

    //    List<AgentMonoController> agentList = new List<AgentMonoController>();

    //    for(int i = 0; i < numHit; i++)
    //    {
    //        AgentMonoController agent = populatedArray[i].GetComponent<AgentMonoController>();

    //        if(caller != agent && agent.m_healthPercentage <= healthThreshold && agent.isBeingHealed == false)
    //            //if(agent != skillTarget)
    //            //    Debug.Log($"<color=cyan>Found wounded friend ({agent.m_agentData.Name})</color>");

    //            if(agent.IsEnemy(caller) == false)
    //                //if(agent != skillTarget)
    //                //    Debug.Log($"<color=cyan>Found wounded friend ({agent.m_agentData.Name})</color>");
    //                agentList.Add(agent);
    //    }

    //    return agentList.OrderBy(a => a.m_healthPercentage).FirstOrDefault();
    //}

    //public AgentMonoController GetFriendsInRangeNonAlloc(AgentMonoController caller, float range,
    //    Collider[] populatedArray)
    //{
    //    int numHit = Physics.OverlapSphereNonAlloc(caller.transform.position, range, populatedArray,
    //        1 << LayerMask.NameToLayer("Agents"));

    //    if(numHit == 0)
    //        return null;

    //    List<AgentMonoController> agentList = new List<AgentMonoController>();

    //    for(int i = 0; i < numHit; i++)
    //    {
    //        AgentMonoController agent = populatedArray[i].GetComponent<AgentMonoController>();

    //        if( /*agent != _agent &&*/
    //            agent.destroyed == false && agent.IsFriend(caller) && agent.attackTarget != null &&
    //            agent.isBeingBuffed == false)
    //            agentList.Add(agent);
    //    }

    //    return agentList.OrderBy(a => (a.attackTarget.transform.position - a.transform.position).sqrMagnitude)
    //        .FirstOrDefault();
    //}

    protected void ClampTo(Actor self, Transform obj, Transform target, float duration)
    {
        self.StartCoroutine(CR_ClampToAndFade(self, obj, target, duration));
        //SceneManagment.FadeTo(obj.gameObject, 0, 0, 1, SceneManagment.OnFadeDoneAction.Disable);
    }

    private IEnumerator CR_ClampToAndFade(Actor self, Transform obj, Transform target, float duration)
    {
        while(duration > 0 && target != null && self != null && self.dead == false)
        {
            obj.position = target.position;
            obj.rotation = Quaternion.LookRotation(self.transform.forward);
            duration -= Time.deltaTime;
            yield return null;
        }

        obj.gameObject.SetActive(false);
    }
}