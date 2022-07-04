using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using GenericFunctions;
using System;
using System.Linq;

public class SwimTrigger : MonoBehaviour
{
    public bool debug;

    [Header("Settings")]
    [SerializeField]
    private ParticleSystem m_ripplesIdleParticleSystem;
    [SerializeField] private ParticleSystem m_ripplesMoveParticleSystem;
    [SerializeField] private float m_startSwimDepth = 0.5f;
    [SerializeField] private float m_swimYOffset;
    [SerializeField] private float m_blendTime = 0.5f;
    //List<MxM_AIController> swimmers;
    public Dictionary<ActorInput, KeyValuePair<SwimmerData, List<ParticleSystem>>> swimmerMap;
    // Start is called before the first frame update

    private CharacterController _cc;

    private void Start()
    {
        swimmerMap = new Dictionary<ActorInput, KeyValuePair<SwimmerData, List<ParticleSystem>>>();
        //GetComponent<Collider>().ignore
    }

    //void OnTriggerEnter(Collider other)
    //{

    //    ActorMonoController agent = other.GetComponent<ActorMonoController>();

    //    if(agent != null && agent.isSwimming == false)
    //    {

    //        List<ParticleSystem> ripplesVFX = null;

    //        if(m_ripplesIdleParticleSystem != null && m_ripplesMoveParticleSystem != null)
    //        {
    //            ripplesVFX = new List<ParticleSystem>();
    //            ParticleSystem p1 = Instantiate(m_ripplesIdleParticleSystem)/*.GetComponent<ParticleSystem>()*/;
    //            ParticleSystem p2 = Instantiate(m_ripplesMoveParticleSystem)/*.GetComponent<ParticleSystem>()*/;
    //            ripplesVFX.Add(p1);
    //            ripplesVFX.Add(p2);
    //        }

    //        //p1.Stop();
    //        //p2.Stop();

    //        if(agent.navAgent != null)
    //        {
    //            agent.navAgent.agentTypeID = 3;
    //        }
    //        else
    //        {
    //            _cc = agent.GetComponent<CharacterController>();
    //        }

    //        //agent.isSwimming = true;
    //        if(swimmerMap.ContainsKey(agent) == false)
    //        {
    //            swimmerMap.Add(agent, new KeyValuePair<SwimmerData, List<ParticleSystem>>(new SwimmerData(), ripplesVFX));
    //        }
    //    }
    //}

    private void OnTriggerStay(Collider col)
    {
        ActorInput collidee = col.GetComponent<ActorInput>();

        if(collidee != null && collidee.inWater == false)
        {
            //agent.isSwimming = true;
            if(swimmerMap.ContainsKey(collidee))
            {
                return;
            }

            collidee.inWater = true;

            List<ParticleSystem> ripplesVFX = null;

            if(m_ripplesIdleParticleSystem != null && m_ripplesMoveParticleSystem != null)
            {
                ripplesVFX = new List<ParticleSystem>();
                ParticleSystem p1 = Instantiate(m_ripplesIdleParticleSystem)/*.GetComponent<ParticleSystem>()*/;
                ParticleSystem p2 = Instantiate(m_ripplesMoveParticleSystem)/*.GetComponent<ParticleSystem>()*/;
                ripplesVFX.Add(p1);
                ripplesVFX.Add(p2);
            }

            swimmerMap.Add(collidee, new KeyValuePair<SwimmerData, List<ParticleSystem>>(new SwimmerData(), ripplesVFX));
            
            //p1.Stop();
            //p2.Stop();

            //if(collidee.navAgent != null)
            //{
            //    collidee.navAgent.agentTypeID = 3;
            //}
            
            if(collidee.ActorStats.isPlayer)
                _cc = collidee.GetComponent<CharacterController>();
            

        }

        //foreach(var kvp in swimmerMap.ToArray())
        //{
        //    ActorMonoController agent = kvp.Key;

        //    Debug.Log(agent.Name + ": Updating swim behaviour");

        //    Vector3 agentPos = agent.transform.position;

        //    // Check if hips are below the water surface

        //    //if(ai.m_hips.position.y <= transform.position.y) {
        //    // 

        //    float constStartSwimHeight = transform.position.y - m_startSwimDepth;

        //    // Water surface - swim start depth
        //    Vector3 startSwimRayOrigin = new Vector3(agentPos.x, transform.position.y/*constStartSwimHeight*/, agentPos.z);

        //    // Play particle systems if existant
        //    if(kvp.Value.Value != null)
        //    {
        //        kvp.Value.Value[0].transform.position = startSwimRayOrigin;
        //        kvp.Value.Value[0].Play();
        //    }

        //    float waterDepthAtAgent = 0;
        //    float swimHeight = 0;
        //    RaycastHit hit;
        //    if(Physics.Raycast(startSwimRayOrigin, -Vector3.up, out hit, 100, 1 << LayerMask.NameToLayer("Ground")))
        //    {
        //        Debug.Log(agent.Name + ": Hitting ground");
        //        waterDepthAtAgent = transform.position.y - hit.point.y;
        //        swimHeight = waterDepthAtAgent + m_swimYOffset;

        //        Debug.DrawLine(startSwimRayOrigin, hit.point, Color.red);
        //        Debug.Log(hit.collider.name);

        //        if(agent.isSwimming == false)
        //        {
        //            Debug.Log(agent.Name + ": Init swim");
        //            // If the water depth at agent position is larger then the startSwimDepth, begin to swim
        //            /*if(agent.transform.position.y < constStartSwimHeight)*/if(waterDepthAtAgent > 0.5f)
        //            {
        //                Debug.Log("Should swim");
        //                agent.isSwimming = true;
        //                agent.Execute_SetLocomotionState(LocomotionState.Swim);

        //                //if(agent.isPlayer)
        //                //{
        //                //    while(agent.transform.position.y < transform.position.y - 0.01f)
        //                //    {
        //                //        agentPos.y = Mathf.MoveTowards(agentPos.y, transform.position.y - 0.01f, Time.deltaTime / m_blendTime);
        //                //    } 
        //                //}
        //            }
        //        }
        //        else if(agent.isSwimming /*waterDepth < m_startSwimDepth*/)
        //        {
        //            Debug.Log(agent.Name + ": Swimming");

        //            kvp.Value.Key.currentWaterDepth = waterDepthAtAgent;

        //            //if(waterDepthAtAgent > 0.5f)
        //            //{
        //            //    agentPos.y = Mathf.MoveTowards(agentPos.y, transform.position.y - 0.01f, Time.deltaTime / m_blendTime);
        //            //agent.transform.position = agentPos;
        //            //}

        //            //if(agent.transform.position.y >= constStartSwimHeight)
        //            //{
        //            //    Debug.Log("Should walk");
        //            //    agent.isSwimming = false;
        //            //    agent.Execute_SetLocomotionState(LocomotionState.Walk);

        //            //    for(int i = 0; i < kvp.Value.Value.Count; i++)
        //            //    {

        //            //        Destroy(kvp.Value.Value[i].gameObject);
        //            //    }
        //            //    swimmerMap.Remove(kvp.Key);

        //            //    if(agent.isPlayer)
        //            //    {
        //            //        //while(agentPos.y != 0)
        //            //        //{
        //            //        //    agentPos.y = Mathf.MoveTowards(agentPos.y, 0, Time.deltaTime / m_blendTime);
        //            //        //}
        //            //    }
        //            //    else
        //            //    {
        //            //        while(agent.navAgent.baseOffset != 0)
        //            //        {
        //            //            agent.navAgent.baseOffset = Mathf.MoveTowards(agent.navAgent.baseOffset, 0, Time.deltaTime / m_blendTime);
        //            //        }
        //            //    }

        //            //    continue;
        //            //}
        //            //else
        //            //{
                   
        //            //}

        //            //if(kvp.Key.isPlayer)
        //            //{
        //            //    Vector3 surfacePos = kvp.Key.transform.position;
        //            //    surfacePos.y = transform.position.y + ;
        //            //    kvp.Key.transform.position = surfacePos;
        //            //    //return;
        //            //}

        //            if(agent.isPlayer)
        //            {
        //                //agentPos.y = Mathf.MoveTowards(agentPos.y, transform.position.y + m_swimYOffset, Time.deltaTime / m_blendTime);
        //                //kvp.Key.swimmer.GetComponent<CharacterController>().stepOffset = Mathf.MoveTowards(kvp.Key.swimmer.GetComponent<CharacterController>().stepOffset, m_swimYOffset, Time.deltaTime / m_blendTime);
        //            }
        //            else
        //                agent.navAgent.baseOffset = Mathf.MoveTowards(agent.navAgent.baseOffset, waterDepthAtAgent + m_swimYOffset, Time.deltaTime / m_blendTime);
                
        //        }
        //    }
        //    //else {
        //    //agent.transform.position = agentPos;
        //    //}
        //    DebugExtension.DebugCircle(new Vector3(agentPos.x, transform.position.y, agentPos.z), Vector3.up, Color.blue, 0.3f);
        //    //DebugExtension.DebugCircle(new Vector3(agentPos.x, swimHeight, agentPos.z), Vector3.up, Color.yellow, 0.3f);
        //    DebugExtension.DebugCircle(startSwimRayOrigin, Vector3.up, Color.white, 0.3f);
            
        //    //}
        //    //else {

        //    //    ai.SwitchMoveType(MovementState.Walk);
        //    //}
        //}
    }

    private void OnTriggerExit(Collider other)
    {
        ActorInput agent = other.GetComponent<ActorInput>();
        
        if(agent != null)
        {
            agent.inWater = false;
            RemoveSwimmer(agent);
        }

    }

    private void RemoveSwimmer(ActorInput agent)
    {
        if(swimmerMap.ContainsKey(agent) == false)
        {
            return;
        }

        var kvp = swimmerMap.Where(k => k.Key == agent).First();
        SwimmerData data = kvp.Value.Key;
   

        //? Destroy particles
        //for(int i = 0; i < kvp.Value.Value.Count; i++)
        //{
        //    Destroy(kvp.Value.Value[i].gameObject);
        //}

        swimmerMap.Remove(agent);
  
    }

    private void OnGUI()
    {
        if(debug)
        {
            float height = 0;
            foreach(var kvp in swimmerMap)
            {
                //GUI.Label(new Rect(500, height, 30, 200), kvp.Key.GetName());
                height += 30;
            }
        }
    }

    private void OnDrawGizmos()
    {
        if(Application.isPlaying)
        {
            foreach(var kvp in swimmerMap)
            {
                UnityEditor.Handles.Label(kvp.Key.transform.position, "swimmer");

                UnityEditor.Handles.Label(kvp.Key.transform.position - Vector3.up * 0.5f, kvp.Value.Key.currentWaterDepth.ToString());
            }
        }
    }
}

[Serializable]
public class SwimmerData
{
    public float currentWaterDepth;

    public SwimmerData()
    {
    }

    public void Update()
    {

    }
}
