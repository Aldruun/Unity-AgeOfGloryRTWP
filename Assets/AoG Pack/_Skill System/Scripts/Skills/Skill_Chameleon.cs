//using System.Collections;
//using System.Linq;
//using UnityEngine;

//public class Skill_Chameleon : Skill
//{
//    private Material[] _cachedMaterials;

//    //[Range(0, 1)]
//    //public float healHealthPercSelf = 0.50f;
//    //[Range(0, 1)]
//    //public float healHealthPercAllies = 0.70f;
//    //Dictionary<Renderer, Material[]> _matMap;
//    //Renderer[] _renderers;
//    private Material[] _materials;

//    public float activationEnemyRange = 10;
//    public float duration = 10;
//    public float lifeTimeBonusPerLevel = 0.3f;

//    public override void Init()
//    {
//        //_matMap = new Dictionary<Renderer, Material[]>();
//        //Debug.Log("<color=green>Initiating chameleon skill</color>");

//        //foreach(var rend in agent.GetComponentsInChildren<Renderer>())
//        //{
//        //    _matMap[rend] = rend.materials;
//        //}

//        //_cachedMaterials = _materials = agent.GetComponentsInChildren<Renderer>()
//        //    .Where(r => r.GetType() != typeof(SpriteRenderer)).Select(r => r.material).ToArray();
//        base.Init();
//    }

//    public override bool ConditionsMetAI(NPCInput agent)
//    {
//        skillTarget = agent;
//        var met = skillTarget != null;

//        //Debug.Log("<color=green>Chameleon skill cond met? -> </color>" + (met == true));
//        return met;
//    }

//    public override void IndividualSetup(Actor agent)
//    {
//        agent.StartCoroutine(CR_Activate(agent));
//    }

//    public IEnumerator CR_Activate(Actor agent)
//    {
//        //Debug.Log("<color=green>Activating chameleon skill</color>");
//        skillTarget.ActorStats.isBeingBuffed = true;

//        cooldown = duration + 0.5f;

//        //agent.GetComponent<Animator>().Play(animStateName, 2);
//        yield return new WaitForSeconds(vfxDelay);

//        //if (agent != null && agent.dead == false)
//        //    //AgentVFXController.TriggerVFX(ResourceManager.GetPoolObject("vfx_skillactivation_wizardbuff", ObjectPoolingCategory.VFX), vfxPoint, Quaternion.identity);
//        //    SpawnVFX(agent);
//        skillTarget.ActorStats.isBeingBuffed = false;
//    }

//    public override void SpawnVFX(Actor agent, Actor target, Vector3 targetPosition)
//    {
//        //Debug.Log("<color=green>Starting chameleon vfx</color>");
//        if (skillTarget == null)
//            return;

//        var dur = duration + lifeTimeBonusPerLevel * agent.ActorStats.Level;
//        //skillTarget.Execute_ApplyStatusEffect(new StatusEffect_StatBuff(skillTarget, this, null, 0, 0, 0, 0, 0, 10 + agent.m_level,
//        //    10 + agent.level, dur));
//        agent.StartCoroutine(CR_Chameleon());
//    }

//    private IEnumerator CR_Chameleon()
//    {
//        //Debug.Log("<color=green>switching to chameleon shader</color>");
//        foreach (var material in _materials)
//        {
//            material.shader = Shader.Find("Custom/Chameleon");
//            yield return null;
//        }

//        yield return new WaitForSeconds(duration);

//        foreach (var material in _materials)
//        {
//            material.shader = Shader.Find("Standard");
//            yield return null;
//        }

//        //for(int i = 0; i < _materials.Length; i++)
//        //{
//        //    _materials[i] = _cachedMaterials[i];
//        //    yield return null;
//        //}
//    }

//    public override bool ConditionsMetPlayer(Actor actor)
//    {
//        return true;
//    }
//}