using UnityEngine;

public class Heatmap
{
    public const int HEATMAP_MINVALUE = 0;
    public const int HEATMAP_MAXVALUE = 100;
    public float cellSize;
    private readonly HeatmapCell[,] gridArray;
    public Vector3 originPosition;

    private readonly int sizeX;
    private readonly int sizeZ;

    public Heatmap(int sizeX, int sizeZ, float cellSize, Transform parent, float checkHeight,
        LayerMask groundLayer)
    {
        this.sizeX = sizeX;
        this.sizeZ = sizeZ;
        this.cellSize = cellSize;
        this.originPosition = parent.position;

        gridArray = new HeatmapCell[sizeX, sizeZ];

        for (var x = 0; x < sizeX; x++)
        for (var z = 0; z < sizeZ; z++)
        {
            var nodePos = HelperFunctions.GetGridObjectWorldPosition(x, z, cellSize, originPosition) +
                          Vector3.one * cellSize * 0.5f;
            var groundedPos = GridManager.GetGroundPosition(nodePos, checkHeight, groundLayer);
            var empty = GameObject.CreatePrimitive(PrimitiveType.Quad);
            empty.transform.eulerAngles += new Vector3(90, 0, 0);
            empty.transform.localScale *= cellSize;
            empty.transform.position = groundedPos;
                empty.transform.SetParent(parent);
            //GameObject tmObj = new GameObject();
            //tmObj.transform.position = groundedPos;
            //TextMesh tm = tmObj.AddComponent<TextMesh>();
            var rend = empty.GetComponent<Renderer>();
            rend.material.shader = Shader.Find("Universal Render Pipeline/Unlit");
            //rend.material.SetFloat("_Glossiness", 0);
            rend.material.SetColor("_BaseColor", Color.black);
            //tm.fontSize = 5;
            //tm.anchor = TextAnchor.LowerCenter;
            //tm.text = 0.ToString();


            var heatmapcell = new HeatmapCell(groundedPos, empty);

            heatmapcell.OnChanged += (oldVal, add) =>
            {
                var color = rend.material.GetColor("_BaseColor");
                float sum = oldVal + add;
                //tm.text = (sum).ToString();
                var step = sum / HEATMAP_MAXVALUE;
                //Debug.Log("sum: " + sum + ", oldVal: " + oldVal + ", add: " + add + ", step: " + step);

                if (step <= 0)
                    color = Color.black;
                else if (step < 0.5f)
                    color = Color.Lerp(Color.red, Color.yellow, step * 2);
                else
                    color = Color.Lerp(Color.yellow, Color.green, (step - 0.5f) * 2);

                rend.material.SetColor("_BaseColor", color);
            };
            gridArray[x, z] = heatmapcell;
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

    public void SetValue(int x, int z, int value)
    {
        if (x >= 0 && z >= 0 && x < sizeX && z < sizeZ)
            gridArray[x, z].SetValue(Mathf.Clamp(value, HEATMAP_MINVALUE, HEATMAP_MAXVALUE));
    }

    public void SetValue(Vector3 worldPosition, int value)
    {
        int x, z;
        GetCellCoordinates(worldPosition, out x, out z);
        SetValue(x, z, value);
    }

    public int GetValue(int x, int z)
    {
        if (x >= 0 && z >= 0 && x < sizeX && z < sizeZ) return gridArray[x, z].GetValue();

        return 0;
    }

    public int GetValue(Vector3 worldPosition)
    {
        int x, z;
        GetCellCoordinates(worldPosition, out x, out z);
        return GetValue(x, z);
    }

    public void AddValue(int x, int z, int value)
    {
        SetValue(x, z, GetValue(x, z) + value);
    }

    public void AddValue(Vector3 worldPosition, int value, int fullValueRange, int totalRange)
    {
        var lowerValueAmount = Mathf.RoundToInt((float) value / (totalRange - fullValueRange));

        GetCellCoordinates(worldPosition, out var origX, out var origZ);
        for (var x = 0; x < totalRange; x++)
        for (var z = 0; z < totalRange - x; z++)
        {
            var radius = x + z;
            var addValueAmount = value;
            if (radius > fullValueRange) addValueAmount -= lowerValueAmount * (radius - fullValueRange);

            AddValue(origX + x, origZ + z, addValueAmount);

            if (x != 0) AddValue(origX - x, origZ + z, addValueAmount);
            if (z != 0)
            {
                AddValue(origX + x, origZ - z, addValueAmount);

                if (x != 0)
                    AddValue(origX - x, origZ - z, addValueAmount);
            }
        }
    }
}