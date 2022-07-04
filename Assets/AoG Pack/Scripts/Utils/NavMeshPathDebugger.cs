using UnityEngine;

public class NavMeshPathDebugger : MonoBehaviour
{
    //LineRenderer line; //to hold the line Renderer
    private ActorInput monoAgent; //to hold the agent of this gameObject

    private void Awake()
    {
        //line = GetComponent<LineRenderer>(); //get the line renderer
        //line.startWidth = 0.1f;
        //line.endWidth = 0.1f;
        monoAgent = GetComponentInParent<ActorInput>(); //get the agent
        //monoAgent.OnNavMeshPathRequest += DrawPath;
    }

    private void OnDisable()
    {
        //monoAgent.OnNavMeshPathRequest -= DrawPath;
    }

    private void Update()
    {
        if (monoAgent.NavAgent.hasPath)
            for (var i = 1; i < monoAgent.NavAgent.path.corners.Length; i++)
                Debug.DrawLine(monoAgent.NavAgent.path.corners[i - 1], monoAgent.NavAgent.path.corners[i]);

        //if(monoAgent.navAgent.hasPath && monoAgent.navAgent.path.corners.Length > 1)
        //{
        //    line.positionCount = monoAgent.navAgent.path.corners.Length;
        //    for(int i = 0; i < monoAgent.navAgent.path.corners.Length; i++)
        //    {
        //        line.SetPosition(i, monoAgent.navAgent.path.corners[i]);
        //    }
        //}
    }

    //void DrawPath(NavMeshAgent navAgent, int pathType)
    //{
    //switch(pathType)
    //{
    //    case -1:
    //        line.startColor = line.endColor = Color.white;
    //        break;
    //    case 0:
    //        line.startColor = line.endColor = Color.red;
    //        break;
    //    case 1:
    //        line.startColor = line.endColor = Color.yellow;
    //        break;
    //    case 2:
    //        line.startColor = line.endColor = Color.grey;
    //        break;
    //}

    //StartCoroutine(CR_DrawPath(navAgent));
    //}

    //IEnumerator CR_DrawPath(NavMeshAgent navAgent)
    //{
    //    line.SetPosition(0, transform.position); //set the line's origin

    //    yield return new WaitForEndOfFrame(); //wait for the path to generate

    //    if(navAgent.path == null || navAgent.path.corners.Length == 0) //if the path has 1 or no corners, there is no need
    //    {
    //        line.enabled = false;
    //        yield break;
    //    }
    //    line.enabled = true;
    //    line.positionCount = navAgent.path.corners.Length; //set the array of positions to the amount of corners

    //    for(var i = 0; i < navAgent.path.corners.Length; i++)
    //    {
    //        line.SetPosition(i, navAgent.path.corners[i]); //go through each corner and set that to the line renderer's position
    //    }
    //}
}