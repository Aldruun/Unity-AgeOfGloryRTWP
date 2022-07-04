using System.Collections.Generic;
using UnityEngine;

public class HeatmapManager : MonoBehaviour
{
    public float cellSize = 0.5f;
    public float groundCheckHeight = 100;
    public LayerMask groundLayer;
    private List<Heatmap> heatmaps;

    // Start is called before the first frame update
    private void Start()
    {
        heatmaps = new List<Heatmap>(); // new GenericGrid<HeatmapElement>(20, 20, 0.3f, transform.position + Vector3.left * 10, 200, 1 << LayerMask.NameToLayer("Ground"), () => new HeatmapElement());
        //heatmaps.Add(new Heatmap(40, 40, 0.2f, transform.position + Vector3.right * 10, groundCheckHeight, groundLayer));
        heatmaps.Add(new Heatmap(40, 40, 0.2f, transform, groundCheckHeight, groundLayer));
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetMouseButtonUp(2))
            //int mx = 0;
            //int mz = 0;
            //GetGridCoordinatesUnderCursor(ref mx, ref mz);
            heatmaps[0].AddValue(GetCursorWorldPosition(), Input.GetKey(KeyCode.LeftControl) ? -5 : 5, 5, 5);
    }

    private Vector3 GetCursorWorldPosition()
    {
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 200)) return hit.point;

        return Vector3.zero;
    }

    public void GetCellCoordinates(Heatmap heatmap, Vector3 worldPosition, out int x, out int z)
    {
        x = Mathf.FloorToInt((worldPosition - heatmap.originPosition).x / cellSize);
        z = Mathf.FloorToInt((worldPosition - heatmap.originPosition).z / cellSize);
    }

    private void GetGridCoordinatesUnderCursor(Heatmap heatmap, ref int mx, ref int mz)
    {
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 200))
        {
            mx = Mathf.FloorToInt((hit.point - heatmap.originPosition).x / cellSize);
            mz = Mathf.FloorToInt((hit.point - heatmap.originPosition).z / cellSize);
        }
    }

    public static Vector3 GetGroundPosition(Vector3 coords, float groundCheckHeight, LayerMask groundLayer)
    {
        RaycastHit hit;
        if (Physics.Raycast(coords + Vector3.up * groundCheckHeight, -Vector3.up, out hit, groundCheckHeight * 2,
            groundLayer)) return hit.point;

        return coords;
    }

    public Vector3 GetWorldPosition(int x, int z, Vector3 origin)
    {
        return new Vector3(x, 0, z) * cellSize + origin;
    }
}