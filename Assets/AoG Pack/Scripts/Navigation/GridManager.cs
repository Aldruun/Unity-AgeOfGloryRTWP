using UnityEngine;

public class GridManager : MonoBehaviour
{
    public float cellSize = 1;
    public float groundCheckHeight = 20;

    public LayerMask groundLayer;

    //GenericGrid<GridCell> grid;
    public int sizeX = 10;
    public int sizeZ = 10;

    private void Start()
    {
        //grid = new GenericGrid<GridCell>(sizeX, sizeZ, cellSize, transform.position, 200, groundLayer, () => new GridCell());
    }

    private void Update()
    {
    }

    private void OnDrawGizmos()
    {
        var mx = 0;
        var mz = 0;
        if (Application.isPlaying)
            GetGridCoordinatesUnderCursor(ref mx, ref mz);
        //Vector3 cursorPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //mx = Mathf.FloorToInt((cursorPos - transform.position).x / cellSize);
        //mz = Mathf.FloorToInt((cursorPos - transform.position).z / cellSize);

        for (var x = 0; x < sizeX; x++)
        for (var z = 0; z < sizeZ; z++)
        {
            if (mx == x && mz == z)
                Gizmos.color = Color.green;
            else
                Gizmos.color = Color.white;

            var nodePos = HelperFunctions.GetGridObjectWorldPosition(x, z, cellSize, transform.position) +
                          Vector3.one * cellSize * 0.5f;
            Gizmos.DrawWireCube(GetGroundPosition(nodePos, groundCheckHeight, groundLayer),
                new Vector3(1 * cellSize, 0.1f, 1 * cellSize));
            Gizmos.DrawLine(GetGridObjectWorldPositionGrounded(x, z), GetGridObjectWorldPositionGrounded(x, z + 1));
            Gizmos.DrawLine(GetGridObjectWorldPositionGrounded(x, z), GetGridObjectWorldPositionGrounded(x + 1, z));
        }

        Gizmos.DrawLine(GetGridObjectWorldPositionGrounded(0, sizeZ), GetGridObjectWorldPositionGrounded(sizeX, sizeZ));
        Gizmos.DrawLine(GetGridObjectWorldPositionGrounded(sizeX, 0), GetGridObjectWorldPositionGrounded(sizeX, sizeZ));
    }

    private void GetGridCoordinatesUnderCursor(ref int mx, ref int mz)
    {
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 200))
        {
            mx = Mathf.FloorToInt((hit.point - transform.position).x / cellSize);
            mz = Mathf.FloorToInt((hit.point - transform.position).z / cellSize);
        }
    }

    public static Vector3 GetGroundPosition(Vector3 coords, float groundCheckHeight, LayerMask groundLayer)
    {
        RaycastHit hit;
        if (Physics.Raycast(coords + Vector3.up * groundCheckHeight, -Vector3.up, out hit, groundCheckHeight * 2,
            groundLayer)) return hit.point;

        return coords;
    }

    private Vector3 GetGridObjectWorldPositionGrounded(int x, int z)
    {
        return GetGroundPosition(new Vector3(x, 0, z) * cellSize + transform.position, groundCheckHeight, groundLayer);
    }
}