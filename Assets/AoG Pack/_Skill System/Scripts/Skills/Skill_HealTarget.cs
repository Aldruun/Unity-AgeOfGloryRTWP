using UnityEngine;

public class Skill_HealTarget : Skill
{
    private Collider[] _wounded;

    public float checkWoundedRadius = 20;

    [Range(0, 1)] public float healHealthPercAllies = 0.60f;

    [Range(0, 1)] public float healHealthPercSelf = 0.50f;

    public override void Init()
    {
        _wounded = new Collider[3];
        base.Init();
    }

    public override bool ConditionsMetAI(NPCInput agent)
    {

        if(ActorUtility.GetHealthPercentage(agent.ActorStats) <= healHealthPercSelf * 100)
        {
            skillTarget = agent;
            return true;
        }

        ActorInput mostWounded = HelperFunctions.GetMostWoundedInRangeNonAlloc(agent, checkWoundedRadius, healHealthPercAllies, _wounded);
        if(mostWounded != null)
        {
            skillTarget = mostWounded;
            return true;
        }


        return false;
    }

    public override bool ConditionsMetPlayer(ActorInput actor)
    {
        return true;
    }

    public override void IndividualSetup(ActorInput self)
    {
        skillTarget.ActorStats.isBeingHealed = true;

    }

    public override void SpawnVFX(ActorInput self, ActorInput target, Vector3 targetPosition)
    {
        if(self == null || self.dead || skillTarget == null || skillTarget.dead)
            return;

        ActorUtility.ModifyActorHealth(target.ActorStats, (int)effectValue + self.ActorStats.Level * 2, ModType.ADDITIVE);

        //VFXManager.TriggerVFX(
        //    PoolSystem.GetPoolObject("vfx_skillactivation_healsingletarget", ObjectPoolingCategory.VFX), vfxPoint,
        //    Quaternion.identity, false);

        skillTarget.ActorStats.isBeingHealed = false;

        VFXPlayer.TriggerVFX(PoolSystem.GetPoolObject(vfxIdentifier, ObjectPoolingCategory.VFX),
            skillTarget.transform, Vector3.zero, Quaternion.identity, false);
    }
}