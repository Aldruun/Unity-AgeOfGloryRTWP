//using System.Collections;
//using System.Linq;
//using UnityEngine;

//public class Skill_Teleport : Skill
//{
//    private Material[] _cachedMaterials;

//    private Material[] _materials;
//    public float minLeapRange = 3;
//    public float rangeBonusPerLevel = 0.3f;

//    public override void Init()
//    {
//        //Debug.Log("<color=green>Initiating chameleon skill</color>");
//        //_cachedMaterials = _materials = agent.GetComponentsInChildren<Renderer>()
//        //    .Where(r => r.GetType() != typeof(SpriteRenderer)).Select(r => r.material).ToArray();
//        base.Init();
//    }

//    public override bool ConditionsMetAI(NPCInput agent)
//    {
//        skillTarget = agent.Combat.GetHostileTarget();

//        if (skillTarget == null) return false;

//        //Debug.Log("<color=green>Chameleon skill cond met? -> </color>" + (met == true));
//        return Vector3.Distance(agent.transform.position, skillTarget.transform.position) > minLeapRange;
//    }

//    public override bool ConditionsMetPlayer(Actor actor)
//    {
//        return true;
//    }

//    public override void IndividualSetup(Actor agent)
//    {
//    }

//    public override void SpawnVFX(Actor agent, Actor target, Vector3 targetPosition)
//    {
//        //Debug.Log("<color=green>Starting chameleon vfx</color>");
//        if (skillTarget == null)
//            return;

//        VFXPlayer.TriggerVFX(PoolSystem.GetPoolObject(startVFXIdentifier, ObjectPoolingCategory.VFX),
//            agent.transform.position, Quaternion.identity);

//        agent.StartCoroutine(CR_Reappear(agent));
//    }

//    private IEnumerator CR_Reappear(Actor agent)
//    {
//        var range = activationRange + rangeBonusPerLevel * agent.ActorStats.Level;
//        //Debug.Log("<color=green>switching to chameleon shader</color>");
//        foreach (var material in _materials)
//        {
//            material.shader = Shader.Find("Custom/Invisibility");
//            yield return null;
//        }

//        yield return new WaitForSeconds(0.5f);
//        //var xz = Random.insideUnitCircle.normalized;
//        //var newPosition = new Vector3(xz.x, 0, xz.y) + skillTarget.transform.position;
//        var newPosition = skillTarget.transform.position + agent.transform.forward;
//        var telPos = HelperFunctions.GetSampledNavMeshPosition(newPosition);
//        agent.NavAgent.Warp(telPos);
//        agent.transform.rotation =
//            Quaternion.LookRotation(skillTarget.transform.position - agent.transform.position, Vector3.up);
//        VFXPlayer.TriggerVFX(PoolSystem.GetPoolObject(vfxIdentifier, ObjectPoolingCategory.VFX),
//            agent.transform.position, Quaternion.identity);
//        for (var i = 0; i < _materials.Length; i++)
//        {
//            _materials[i] = _cachedMaterials[i];
//            yield return null;
//        }

//        agent.Animation.PlaySpellAnimation(motionID);
//    }
//}