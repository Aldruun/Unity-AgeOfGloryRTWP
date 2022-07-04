using System;
using UnityEngine;

public class GenericGrid<TGridObject> where TGridObject : IGridObject
{
    private readonly float cellSize;
    private readonly TGridObject[,] gridArray;
    private readonly Vector3 originPosition;
    private readonly int sizeX;
    private readonly int sizeZ;

    public GenericGrid(int sizeX, int sizeZ, float cellSize, Vector3 originPosition, float checkHeight,
        LayerMask groundLayer, Func<TGridObject> CreateGridObject)
    {
        this.sizeX = sizeX;
        this.sizeZ = sizeZ;
        this.cellSize = cellSize;
        this.originPosition = originPosition;

        gridArray = new TGridObject[sizeX, sizeZ];

        for (var x = 0; x < sizeX; x++)
        for (var z = 0; z < sizeZ; z++)
        {
            gridArray[x, z] = CreateGridObject();
            IGridObject gridObj = gridArray[x, z];
            gridObj.SetGridValues(x, z);

            var nodePos = HelperFunctions.GetGridObjectWorldPosition(x, z, cellSize, originPosition) +
                          Vector3.one * cellSize * 0.5f;
            var groundedPos = GridManager.GetGroundPosition(nodePos, checkHeight, groundLayer);
            var tmObj = new GameObject();
            tmObj.transform.position = groundedPos;
            var tm = tmObj.AddComponent<TextMesh>();
            tm.fontSize = 5;
            tm.anchor = TextAnchor.LowerCenter;

            tm.text = gridObj.ToString();
            gridObj.OnChanged += () =>
            {
                Debug.Log("Click");
                tm.text = gridObj.ToString();
            };
        }
    }

    public int GetSizeX()
    {
        return sizeX;
    }

    public int GetSizeZ()
    {
        return sizeZ;
    }

    public float GetCellSize()
    {
        return cellSize;
    }

    public void GetCellCoordinates(Vector3 worldPosition, out int x, out int z)
    {
        x = Mathf.FloorToInt((worldPosition - originPosition).x / cellSize);
        z = Mathf.FloorToInt((worldPosition - originPosition).z / cellSize);
    }

    public void SetGridObject(int x, int z, TGridObject value)
    {
        if (x >= 0 && z >= 0 && x < sizeX && z < sizeZ)
            gridArray[x, z] = value;
        //OnGridValueChanged?.Invoke(this, new OnGridValueChangedEventArgs { x = x, z = z });
    }

    public void SetGridObject(Vector3 worldPosition, TGridObject value)
    {
        int x, z;
        GetCellCoordinates(worldPosition, out x, out z);
        SetGridObject(x, z, value);
    }

    public TGridObject GetGridObject(int x, int z)
    {
        if (x >= 0 && z >= 0 && x < sizeX && z < sizeZ) return gridArray[x, z];

        return default;
    }

    public TGridObject GetGridObject(Vector3 worldPosition)
    {
        int x, z;
        GetCellCoordinates(worldPosition, out x, out z);
        return GetGridObject(x, z);
    }
}