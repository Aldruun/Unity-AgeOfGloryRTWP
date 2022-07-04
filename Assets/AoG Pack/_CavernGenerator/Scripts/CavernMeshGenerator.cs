using UnityEngine;
using System.Collections.Generic;

public class CavernMeshGenerator : MonoBehaviour
{
    public int textureTiling = 10;
    public MeshFilter wallMeshFilter;
    public int wallHeight = 5;
    public SquareGrid squareGrid;
    private List<Vector3> _vertices;
    private List<Vector3> _topVertices;
    private List<int> _triangles;

    private Dictionary<int, List<Triangle>> _triangleMap = new Dictionary<int, List<Triangle>>();
    private List<List<int>> _outlines = new List<List<int>>();
    private HashSet<int> _checkedVertices = new HashSet<int>();

    public void GenerateMesh(int[,] map, float squareSize)
    {
        _triangleMap.Clear();
        _outlines.Clear();
        _checkedVertices.Clear();

        squareGrid = new SquareGrid(map, squareSize);

        _vertices = new List<Vector3>();
        _topVertices = new List<Vector3>();
        _triangles = new List<int>();

        for(int x = 0; x < squareGrid.squares.GetLength(0); x++)
        {
            for(int y = 0; y < squareGrid.squares.GetLength(1); y++)
            {
                TriangulateSquare(squareGrid.squares[x, y]);
            }
        }

        Mesh groundMesh = new Mesh();
        MeshFilter mf = transform.Find("void").GetComponent<MeshFilter>();
        mf.mesh = groundMesh;

        groundMesh.vertices = _vertices.ToArray();
     
        groundMesh.triangles = _triangles.ToArray();
        groundMesh.RecalculateNormals();
        //! Top mesh done

        Mesh wallMesh = new Mesh();
        CreateWallMesh(ref wallMesh);

        //Vector2[] uvs = new Vector2[_vertices.Count - groundMesh.vertices.Length];
        //for(int i = 0; i < _vertices.Count - groundMesh.vertices.Length; i++)
        //{
        //    float percentX = Mathf.InverseLerp(-map.GetLength(0) / 2 * squareSize, map.GetLength(0) / 2 * squareSize, _vertices[i].x) * textureTiling;
        //    float percentY = Mathf.InverseLerp(-map.GetLength(1) / 2 * squareSize, map.GetLength(1) / 2 * squareSize, _vertices[i].z) * textureTiling;
        //    uvs[i] = new Vector2(percentX, percentY);
        //}
        //wallMesh.uv = uvs;

        transform.position += new Vector3(0, wallHeight, 0);
    }

    private void CreateWallMesh(ref Mesh wallMesh)
    {
        CalculateMeshOutlines();

        List<Vector3> wallVertices = new List<Vector3>();
        List<int> wallTriangles = new List<int>();
        //Mesh wallMesh = new Mesh();

        foreach(List<int> outline in _outlines)
        {
            for(int i = 0; i < outline.Count - 1; i++)
            {
                int startIndex = wallVertices.Count;
                wallVertices.Add(_vertices[outline[i]]); // left vertex
                wallVertices.Add(_vertices[outline[i+1]]); // right vertex
                wallVertices.Add(_vertices[outline[i]] - Vector3.up * wallHeight); // bottom left vertex
                wallVertices.Add(_vertices[outline[i + 1]] - Vector3.up * wallHeight); // bottom right vertex

                wallTriangles.Add(startIndex + 0);
                wallTriangles.Add(startIndex + 2);
                wallTriangles.Add(startIndex + 3);

                wallTriangles.Add(startIndex + 3);
                wallTriangles.Add(startIndex + 1);
                wallTriangles.Add(startIndex + 0);
            }
        }

        wallMesh.vertices = wallVertices.ToArray();
        wallMesh.triangles = wallTriangles.ToArray();
        wallMeshFilter.mesh = wallMesh;

        MeshCollider wallCollider = wallMeshFilter.GetComponent<MeshCollider>();
        if(wallCollider == null)
        {
            wallCollider = wallMeshFilter.gameObject.AddComponent<MeshCollider>();
        }
        wallCollider.sharedMesh = wallMesh;
    }

    private void TriangulateSquare(Square square)
    {
        switch(square.configuration)
        {
            case 0:
                break;

            // 1 point:
            case 1:
                MeshFromPoints(square.centerLeft, square.centerBottom, square.bottomLeft);
                break;
            case 2:
                MeshFromPoints(square.bottomRight, square.centerBottom, square.centerRight);
                break;
            case 4:
                MeshFromPoints(square.topRight, square.centerRight, square.centerTop);
                break;
            case 8:
                MeshFromPoints(square.topLeft, square.centerTop, square.centerLeft);
                break;

            // 2 points
            case 3:
                MeshFromPoints(square.centerRight, square.bottomRight, square.bottomLeft, square.centerLeft);
                break;
            case 6:
                MeshFromPoints(square.centerTop, square.topRight, square.bottomRight, square.centerBottom);
                break;
            case 9:
                MeshFromPoints(square.topLeft, square.centerTop, square.centerBottom, square.bottomLeft);
                break;
            case 12:
                MeshFromPoints(square.topLeft, square.topRight, square.centerRight, square.centerLeft);
                break;
            case 5:
                MeshFromPoints(square.centerTop, square.topRight, square.centerRight, square.centerBottom, square.bottomLeft, square.centerLeft);
                break;
            case 10:
                MeshFromPoints(square.topLeft, square.centerTop, square.centerRight, square.bottomRight, square.centerBottom, square.centerLeft);
                break;

            // 3 points
            case 7:
                MeshFromPoints(square.centerTop, square.topRight, square.bottomRight, square.bottomLeft, square.centerLeft);
                break;
            case 11:
                MeshFromPoints(square.topLeft, square.centerTop, square.centerRight, square.bottomRight, square.bottomLeft);
                break;
            case 13:
                MeshFromPoints(square.topLeft, square.topRight, square.centerRight, square.centerBottom, square.bottomLeft);
                break;
            case 14:
                MeshFromPoints(square.topLeft, square.topRight, square.bottomRight, square.centerBottom, square.centerLeft);
                break;

            // 4 points
            case 15:
                MeshFromPoints(square.topLeft, square.topRight, square.bottomRight, square.bottomLeft);
                _checkedVertices.Add(square.topLeft.vertexIndex);
                _checkedVertices.Add(square.topRight.vertexIndex);
                _checkedVertices.Add(square.bottomRight.vertexIndex);
                _checkedVertices.Add(square.bottomLeft.vertexIndex);
                break;
        }
    }

    private void MeshFromPoints(params Node[] points)
    {
        AssignVertices(points);

        if(points.Length >= 3)
        {
            CreateTriangle(points[0], points[1], points[2]);
        }
        if(points.Length >= 4)
        {
            CreateTriangle(points[0], points[2], points[3]);
        }
        if(points.Length >= 5)
        {
            CreateTriangle(points[0], points[3], points[4]);
        }
        if(points.Length >= 6)
        {
            CreateTriangle(points[0], points[4], points[5]);
        }
    }

    private void AssignVertices(Node[] points)
    {
        for(int i = 0; i < points.Length; i++)
        {
            if(points[i].vertexIndex == -1)
            {
                points[i].vertexIndex = _vertices.Count;
                _vertices.Add(points[i].position);
            }
        }
    }

    private void CreateTriangle(Node a, Node b, Node c)
    {
        _triangles.Add(a.vertexIndex);
        _triangles.Add(b.vertexIndex);
        _triangles.Add(c.vertexIndex);

        Triangle triangle = new Triangle(a.vertexIndex, b.vertexIndex, c.vertexIndex);
        AddToTriangleMap(triangle.vertexIndexA, triangle);
        AddToTriangleMap(triangle.vertexIndexB, triangle);
        AddToTriangleMap(triangle.vertexIndexC, triangle);
    }

    private void AddToTriangleMap(int vertexIndex, Triangle triangle)
    {
        if(_triangleMap.ContainsKey(vertexIndex) == false)
        {
            _triangleMap.Add(vertexIndex, new List<Triangle>() { triangle });
        }
        else
        {
            _triangleMap[vertexIndex].Add(triangle);
        }
    }

    private void CalculateMeshOutlines()
    {
        for(int vertexIndex = 0; vertexIndex < _vertices.Count; vertexIndex++)
        {
            if(_checkedVertices.Contains(vertexIndex) == false)
            {
                int newOutlineVertex = GetConnectedOutlineVertex(vertexIndex);

                if(newOutlineVertex != -1)
                {
                    _checkedVertices.Add(vertexIndex);

                    List<int> newOutline = new List<int>();
                    newOutline.Add(vertexIndex);
                    _outlines.Add(newOutline);
                    FollowOutline(newOutlineVertex, _outlines.Count - 1);
                    _outlines[_outlines.Count - 1].Add(vertexIndex);
                }
            }
        }
    }

    private void FollowOutline(int vertexIndex, int outlineIndex)
    {
        _outlines[outlineIndex].Add(vertexIndex);
        _checkedVertices.Add(vertexIndex);
        int nextVertexIndex = GetConnectedOutlineVertex(vertexIndex);

        if(nextVertexIndex != -1)
        {
            FollowOutline(nextVertexIndex, outlineIndex);
        }
    }

    private int GetConnectedOutlineVertex(int vertexIndex)
    {
        List<Triangle> trianglesContainingVertex = _triangleMap[vertexIndex];

        for(int i = 0; i < trianglesContainingVertex.Count; i++)
        {
            Triangle triangle = trianglesContainingVertex[i];

            for(int j = 0; j < 3; j++)
            {
                int vertexB = triangle[j];

                if(vertexB != vertexIndex && _checkedVertices.Contains(vertexB) == false)
                {
                    if(IsOutlineEdge(vertexIndex, vertexB))
                    {
                        return vertexB;
                    } 
                }
            }
        }

        return -1;
    }

    private bool IsOutlineEdge(int vertexA, int vertexB)
    {
        List<Triangle> trianglesContainingVertexA = _triangleMap[vertexA];
        int sharedTriangleCount = 0;

        for(int i = 0; i < trianglesContainingVertexA.Count; i++)
        {
            if(trianglesContainingVertexA[i].Contains(vertexB))
            {
                sharedTriangleCount++;
                if(sharedTriangleCount > 1)
                {
                    break;
                }
            }
        }
        return sharedTriangleCount == 1;
    }

    private struct Triangle
    {
        public int vertexIndexA;
        public int vertexIndexB;
        public int vertexIndexC;

        private int[] vertices;

        public Triangle(int vertexIndexA, int vertexIndexB, int vertexIndexC)
        {
            this.vertexIndexA = vertexIndexA;
            this.vertexIndexB = vertexIndexB;
            this.vertexIndexC = vertexIndexC;

            vertices = new int[3];

            vertices[0] = vertexIndexA;
            vertices[1] = vertexIndexB;
            vertices[2] = vertexIndexC;
        }

        public int this[int i]
        {
            get
            {
                return vertices[i];
            }
        }

        public bool Contains(int vertexIndex)
        {
            return vertexIndex == vertexIndexA || vertexIndex == vertexIndexB || vertexIndex == vertexIndexC;
        }
    }

    public class SquareGrid
    {
        public Square[,] squares;

        public SquareGrid(int[,] map, float squareSize)
        {
            int nodeCountX = map.GetLength(0);
            int nodeCountY = map.GetLength(1);
            float mapWidth = nodeCountX * squareSize;
            float mapHeight = nodeCountY * squareSize;

            ControlNode[,] ctrlNodes = new ControlNode[nodeCountX, nodeCountY];

            for(int x = 0; x < nodeCountX; x++)
            {
                for(int y = 0; y < nodeCountY; y++)
                {
                    Vector3 pos = new Vector3(-mapWidth / 2 + x * squareSize + squareSize / 2, 0, -mapHeight / 2 + y * squareSize + squareSize / 2);
                    ctrlNodes[x, y] = new ControlNode(pos, map[x, y] == 1, squareSize);
                }
            }

            squares = new Square[nodeCountX - 1, nodeCountY - 1];

            for(int x = 0; x < nodeCountX - 1; x++)
            {
                for(int y = 0; y < nodeCountY - 1; y++)
                {
                    squares[x, y] = new Square(ctrlNodes[x, y + 1], ctrlNodes[x + 1, y + 1], ctrlNodes[x + 1, y], ctrlNodes[x, y]);
                }
            }
        }
    }

    public class Square
    {
        public ControlNode topLeft, topRight, bottomRight, bottomLeft;
        public Node centerTop, centerRight, centerBottom, centerLeft;

        public int configuration;

        public Square(ControlNode topLeft, ControlNode topRight, ControlNode bottomRight, ControlNode bottomLeft)
        {
            this.topLeft = topLeft;
            this.topRight = topRight;
            this.bottomRight = bottomRight;
            this.bottomLeft = bottomLeft;

            this.centerTop = topLeft.right;
            this.centerRight = bottomRight.above;
            this.centerBottom = bottomLeft.right;
            this.centerLeft = bottomLeft.above;

            if(topLeft.active)
                configuration += 8;
            if(topRight.active)
                configuration += 4;
            if(bottomRight.active)
                configuration += 2;
            if(bottomLeft.active)
                configuration += 1;
        }
    }

    public class Node
    {
        public Vector3 position;
        public int vertexIndex = -1;

        public Node(Vector3 pos)
        {
            position = pos;
        }
    }

    public class ControlNode : Node
    {
        public bool active;
        public Node above, right;

        public ControlNode(Vector3 pos, bool active, float squareSize) : base(pos)
        {
            this.active = active;
            above = new Node(position + Vector3.forward * squareSize / 2);
            right = new Node(position + Vector3.right * squareSize / 2);
        }
    }

    //void OnDrawGizmos()
    //{
    //    if(squareGrid != null)
    //    {
    //        for(int x = 0; x < squareGrid.squares.GetLength(0); x++)
    //        {
    //            for(int y = 0; y < squareGrid.squares.GetLength(1); y++)
    //            {
    //                Gizmos.color = (squareGrid.squares[x, y].topLeft.active) ? Color.black : Color.white;
    //                Gizmos.DrawCube(squareGrid.squares[x, y].topLeft.position, Vector3.one * 0.4f);

    //                Gizmos.color = (squareGrid.squares[x, y].topRight.active) ? Color.black : Color.white;
    //                Gizmos.DrawCube(squareGrid.squares[x, y].topRight.position, Vector3.one * 0.4f);

    //                Gizmos.color = (squareGrid.squares[x, y].bottomRight.active) ? Color.black : Color.white;
    //                Gizmos.DrawCube(squareGrid.squares[x, y].bottomRight.position, Vector3.one * 0.4f);

    //                Gizmos.color = (squareGrid.squares[x, y].bottomLeft.active) ? Color.black : Color.white;
    //                Gizmos.DrawCube(squareGrid.squares[x, y].bottomLeft.position, Vector3.one * 0.4f);

    //                Gizmos.color = Color.grey;
    //                Gizmos.DrawCube(squareGrid.squares[x, y].centerTop.position, Vector3.one * 0.15f);
    //                Gizmos.DrawCube(squareGrid.squares[x, y].centerRight.position, Vector3.one * 0.15f);
    //                Gizmos.DrawCube(squareGrid.squares[x, y].centerBottom.position, Vector3.one * 0.15f);
    //                Gizmos.DrawCube(squareGrid.squares[x, y].centerLeft.position, Vector3.one * 0.15f);
    //            }
    //        }
    //    }
    //}
}
