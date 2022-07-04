using System.Collections.Generic;
using System.Linq;
using GenericFunctions;
using UnityEngine;
using UnityEngine.AI;

public class CoverGrid : MonoBehaviour
{
    public static CoverGrid current;

    public static Dictionary<Transform, List<Vector3>> map;
    public static Vector3 mapCenter;
    private Collider[] _coverColliders;
    public Color borderColor;
    public float gridScale = 1;

    public Color lineColor;
    public int numColumns = 5;

    public int numRows = 5;
    public LayerMask obstacleLayerMask;
    public Transform testObject;
    public float testRadius = 10;

    private void Start()
    {
        current = this;

        _coverColliders = new Collider[5];

        GenerateCoverMap();
    }

    private static bool MultiLineCastToThreat(Vector3 start, Transform threat)
    {
        if (threat == null) return false;

        RaycastHit hit;

        if (RaycastUtility.MultiLineCast(3, 3, start, new Vector2(0.28f, 0.2f), threat.position,
            out hit, /*~(1 << LayerMask.NameToLayer("Agents")) | */1 << LayerMask.NameToLayer("Obstacles")))
        {
            //if(hit.collider == threat.GetComponent<Collider>()) {

            DebugExtension.DebugWireSphere(start, Color.red, 0.2f);
            //Debug.DrawLine(start, threat.position, Color.green);
            //Debug.Log("See target!");
            return true;
            //}
        }

        DebugExtension.DebugWireSphere(start, Color.green, 0.2f);
        //Debug.Log("No target!");
        return false;
    }

    public static List<Vector3> GetPointsHiddenFrom(Transform threat, Vector3 origin, float searchNodeRadius)
    {
        if (threat == null) return null;

        var listOfPoints = map.Values.ToList();
        var hiddenPointsList = new List<Vector3>();

        foreach (var list in listOfPoints)
        foreach (var v in list)
            if (hiddenPointsList.Contains(v) == false)
                hiddenPointsList.Add(v);

        return hiddenPointsList.Where(p =>
            Vector3.Distance(p, origin) <= searchNodeRadius && MultiLineCastToThreat(p, threat) &&
            CloseToPath(p, threat.root.GetComponent<NavMeshAgent>().path, 2) == false).ToList();
    }

    public Vector3 GetClosestCoverPositionAroundObject(Transform coverObject, Transform self, Transform threat)
    {
        var possCovPoints = new List<Vector3>();

        foreach (var coverPoint in map[coverObject])
            if (Physics.Linecast(coverPoint, threat.position) == false)
            {
                Debug.Log("### Found good cover spot");
                possCovPoints.Add(coverPoint);
            }

        return Get.ClosestPositionFromList(self.position, possCovPoints);
    }

    private static bool CloseToPath(Vector3 point, NavMeshPath path, float distance)
    {
        for (var i = 0; i < path.corners.Length; i++)
        {
            var corner = path.corners[i];
            if (Vector3.Distance(corner, point) <= distance) return true;
        }

        return false;
    }

    public Transform GetClosestCoverObject(Transform origin)
    {
        return Get.ClosestObjectFromArray(origin.position, map.Keys.ToArray());
    }

    private Dictionary<Transform, List<Vector3>> GenerateCoverMap()
    {
        map = new Dictionary<Transform, List<Vector3>>();

        for (var r = 0; r < numRows; r++)
        for (var c = 0; c < numColumns; c++)
        {
            var rowResultX = new Vector3(
                transform.position.x + c * gridScale,
                transform.position.y,
                transform.position.z + r * gridScale);
            var rowResultZ = new Vector3(
                transform.position.x + (c + 1) * gridScale,
                transform.position.y,
                transform.position.z + r * gridScale);

            var colResultX = new Vector3(
                transform.position.x + c * gridScale,
                transform.position.y,
                transform.position.z + r * gridScale);
            var colResultZ = new Vector3(
                transform.position.x + c * gridScale,
                transform.position.y,
                transform.position.z + (r + 1) * gridScale);

            RaycastHit hit;
            NavMeshHit navHit;

            if (Physics.Raycast(rowResultX, rowResultZ - rowResultX, out hit, Vector3.Distance(rowResultX, rowResultZ),
                obstacleLayerMask))
                if (NavMesh.FindClosestEdge(rowResultX, out navHit, NavMesh.AllAreas))
                {
                    rowResultX = navHit.position;
                    if (map.ContainsKey(hit.transform))
                        map[hit.transform].Add(rowResultX);
                    else
                        map.Add(hit.transform, new List<Vector3> {rowResultX});
                }

            if (Physics.Raycast(rowResultZ, rowResultX - rowResultZ, out hit, Vector3.Distance(rowResultX, rowResultZ),
                obstacleLayerMask))
                if (NavMesh.FindClosestEdge(rowResultZ, out navHit, NavMesh.AllAreas))
                {
                    rowResultZ = navHit.position;
                    if (map.ContainsKey(hit.transform))
                        map[hit.transform].Add(rowResultZ);
                    else
                        map.Add(hit.transform, new List<Vector3> {rowResultZ});
                }

            if (Physics.Raycast(colResultX, colResultZ - colResultX, out hit, Vector3.Distance(colResultX, colResultZ),
                obstacleLayerMask))
                if (NavMesh.FindClosestEdge(colResultX, out navHit, NavMesh.AllAreas))
                {
                    colResultX = navHit.position;
                    if (map.ContainsKey(hit.transform))
                        map[hit.transform].Add(colResultX);
                    else
                        map.Add(hit.transform, new List<Vector3> {colResultX});
                }

            if (Physics.Raycast(colResultZ, colResultX - colResultZ, out hit, Vector3.Distance(colResultX, colResultZ),
                obstacleLayerMask))
                if (NavMesh.FindClosestEdge(colResultZ, out navHit, NavMesh.AllAreas))
                {
                    colResultZ = navHit.position;
                    if (map.ContainsKey(hit.transform))
                        map[hit.transform].Add(colResultZ);
                    else
                        map.Add(hit.transform, new List<Vector3> {colResultZ});
                }
        }

        mapCenter = transform.position + new Vector3(numColumns / 2 * gridScale, 0, numRows / 2 * gridScale);

        return map;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = lineColor;

        if ((Application.isPlaying && map != null ? map : GenerateCoverMap()) == null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, 1);
            return;
        }

        Gizmos.DrawWireSphere(transform.position + new Vector3(numColumns / 2 * gridScale, 0, numRows / 2 * gridScale),
            1);
        foreach (var kvp in Application.isPlaying && map != null ? map : GenerateCoverMap()) //NavMeshHit navHit;
            for (var i = 0; i < kvp.Value.Count; i++)
            {
                var pos = kvp.Value[i];

                //if(NavMesh.FindClosestEdge(pos, out navHit, NavMesh.AllAreas)) {

                //    pos = navHit.position;
                Gizmos.DrawWireSphere(pos, 0.05f);
                //}
            }

        Gizmos.color = borderColor;
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(0, 0, numRows) * gridScale);
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(numColumns, 0, 0) * gridScale);
        Gizmos.DrawLine(transform.position + new Vector3(0, 0, numRows) * gridScale,
            transform.position + new Vector3(numColumns, 0, numRows) * gridScale);
        Gizmos.DrawLine(transform.position + new Vector3(numColumns, 0, 0) * gridScale,
            transform.position + new Vector3(numColumns, 0, numRows) * gridScale);
        //Gizmos.color = lineColor;
        //for(int r = 0; r < numRows; r++) {

        //    for(int c = 0; c < numColumns; c++) {


        //        Vector3 rowResultX = new Vector3(
        //               transform.position.x + c * gridScale,
        //               transform.position.y,
        //               transform.position.z + r * gridScale);
        //        Vector3 rowResultZ = new Vector3(
        //               transform.position.x + (c + 1) * gridScale,
        //               transform.position.y,
        //               transform.position.z + r * gridScale);

        //        Vector3 colResultX = new Vector3(
        //               transform.position.x + c * gridScale,
        //               transform.position.y,
        //               transform.position.z + r * gridScale);
        //        Vector3 colResultZ = new Vector3(
        //               transform.position.x + c * gridScale,
        //               transform.position.y,
        //               transform.position.z + (r + 1) * gridScale);

        //        RaycastHit hit;

        //        bool rowXZYes = Physics.Raycast(rowResultX, rowResultZ - rowResultX, out hit, Vector3.Distance(rowResultX, rowResultZ), obstacleLayerMask);
        //        bool rowZXYes = Physics.Raycast(rowResultZ, rowResultX - rowResultZ, out hit, Vector3.Distance(rowResultX, rowResultZ), obstacleLayerMask);

        //        bool colXZYes = Physics.Raycast(colResultX, colResultZ - colResultX, out hit, Vector3.Distance(colResultX, colResultZ), obstacleLayerMask);
        //        bool colZXYes = Physics.Raycast(colResultZ, colResultX - colResultZ, out hit, Vector3.Distance(colResultX, colResultZ), obstacleLayerMask);
        //        if(rowXZYes || rowZXYes) {


        //            Gizmos.color = Color.red;
        //            //Collider[] colls = Physics.OverlapSphere(rowResultX, 0f, obstacleLayerMask);
        //            //if(colls.Length == 0)
        //            Gizmos.DrawWireSphere(rowZXYes ? rowResultZ : rowResultX, 0.05f);
        //            Gizmos.color = gridGizmoColor;
        //        }
        //        else {

        //            Collider[] colls = Physics.OverlapSphere(rowResultX, 0f, obstacleLayerMask);
        //            if(colls.Length == 0)
        //                Gizmos.DrawLine(rowResultX, rowResultZ);
        //        }

        //        if(colXZYes || colZXYes) {


        //            Gizmos.color = Color.red;
        //            //Collider[] colls = Physics.OverlapSphere(colResultZ, 0f, obstacleLayerMask);
        //            //if(colls.Length == 0)
        //            Gizmos.DrawWireSphere(colXZYes ? colResultX : colResultZ, 0.05f);
        //            Gizmos.color = gridGizmoColor;
        //        }
        //        else {

        //            Collider[] colls = Physics.OverlapSphere(colResultX, 0f, obstacleLayerMask);
        //            if(colls.Length == 0)
        //                Gizmos.DrawLine(colResultX, colResultZ);
        //        }
        //    }
        //}
    }

    public static bool IsInside(Collider test, Vector3 point)
    {
        Vector3 center;
        Vector3 direction;
        Ray ray;
        RaycastHit hitInfo;
        bool hit;

        // Use collider bounds to get the center of the collider. May be inaccurate
        // for some colliders (i.e. MeshCollider with a 'plane' mesh)
        center = test.bounds.center;

        // Cast a ray from point to center
        direction = center - point;
        ray = new Ray(point, direction);
        hit = test.Raycast(ray, out hitInfo, direction.magnitude);

        // If we hit the collider, point is outside. So we return !hit
        return !hit;
    }
}