using UnityEngine;

public class RaycastUtility : MonoBehaviour
{
    public float height = 1.5f;
    public int numColumns = 5;

    public int numRows = 5;

    public Transform origin;
    public Transform target;
    public float width = 0.5f;
    public float xSpacing = 0.2f;
    public float ySpacing = 0.2f;

    public static bool MultiLineCast(int numRaysX, int numRaysY, Vector3 origin, Vector2 widthHeight, Vector3 target,
        out RaycastHit hitInfo, int ignoreLayerMaskInfo)
    {
        var dir = (target - origin).normalized;

        var crossVector = (-Vector3.Cross(Vector3.up, dir)).normalized;

        var crossVector2 = (-Vector3.Cross(crossVector, dir)).normalized;

        ExceptionHandling(origin, target, ref crossVector, ref crossVector2);

        for (var r = 0; r < numRaysY; r++)
        for (var c = 0; c < numRaysX; c++)
        {
            var startPoint = -(crossVector * (numRaysX - 1) * (widthHeight.x / 2.0f) +
                               crossVector2 * (numRaysY - 1) * widthHeight.y) +
                             origin + crossVector * c * widthHeight.x + crossVector2 * r * widthHeight.y;

            if (Physics.Linecast(startPoint, target, out hitInfo, ignoreLayerMaskInfo)
                ) //DebugExtension.DebugWireSphere(hitInfo.point, Color.red, 0.2f);
                //Debug.DrawRay(startPoint, target - origin, Color.green);
                //Debug.Log("Found target!");
                return true;

            //Debug.DrawRay(startPoint, target - origin, Color.red);
        }

        hitInfo = new RaycastHit();

        return false;
    }

    //private void OnDrawGizmos() {

    //    if(origin != null && target != null) {

    //        Vector3 dir = (target.position - origin.position).normalized;

    //        Vector3 crossVector = (-Vector3.Cross(Vector3.up, dir)).normalized;

    //        Vector3 crossVector2 = (-Vector3.Cross(crossVector, dir)).normalized;

    //        ExceptionHandling(origin.position, target.position, ref crossVector, ref crossVector2);

    //        for(int r = 0; r < numRows; r++) {

    //            for(int c = 0; c < numColumns; c++) {

    //                Gizmos.color = Color.green;
    //                Vector3 startPoint = -((crossVector * (numColumns - 1) * (width / 2.0f) + crossVector2 * (numRows - 1) * height) /*/ 2.0f*/) +
    //                                      origin.position + crossVector * c * width + crossVector2 * r * height;
    //                //Vector3 startPoint = -((crossVector * (numColumns - 1) * (width/2.0f) + crossVector2 * (numRows - 1) * height) /*/ 2.0f*/) +
    //                //                      origin.position + crossVector * c * width + crossVector2 * r * height;
    //                Gizmos.DrawRay(startPoint, target.position - origin.position);
    //                //Gizmos.DrawRay(origin.position + crossVector*c*width + crossVector2 * r*height , (target.position - origin.position));

    //            }
    //        }
    //    }
    //}

    private static void ExceptionHandling(Vector3 orgin, Vector3 target, ref Vector3 crossVector,
        ref Vector3 crossVector2)
    {
        // Prüfen ob sich die beiden Vektoren nur über die Y-Achse unterscheiden
        var diff = orgin - target;
        if (diff.x == 0 && diff.z == 0)
        {
            crossVector = Vector3.right;
            crossVector2 = Vector3.forward;
        }
    }
}