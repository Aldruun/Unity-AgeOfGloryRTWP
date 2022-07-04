using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public int border = 5;
    public int wallRemoveSize = 50;
    public int roomRemoveSize = 50;
    public int width = 100;
    public int height = 100;

    public string seed;
    public bool useRandomSeed;

    [Range(0, 100)]
    public int randomFillPercent = 50;

    private int[,] _map;

    // Start is called before the first frame update
    private void Start()
    {
        GenerateMap();
    }

    // Update is called once per frame
    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            GenerateMap();
        }
    }

    private void GenerateMap()
    {
        _map = new int[width, height];
        RandomFillMap();

        for(int i = 0; i < 5; i++)
        {
            SmoothMap();
        }

        ProcessMap();

        int[,] borderedMap = new int[width + border * 2, height + border * 2];

        for(int x = 0; x < borderedMap.GetLength(0); x++)
        {
            for(int y = 0; y < borderedMap.GetLength(1); y++)
            {
                if(x >= border && x < width + border && y >= border && y < height + border)
                {
                    borderedMap[x, y] = _map[x - border, y - border];
                }
                else
                {
                    borderedMap[x, y] = 1;
                }
            }
        }

        CavernMeshGenerator meshGen = GetComponent<CavernMeshGenerator>();
        meshGen.GenerateMesh(borderedMap, 1);
    }

    private void ProcessMap()
    {
        List<List<Coord>> wallRegions = GetRegions(1);

        foreach(List<Coord> wallRegion in wallRegions)
        {
            if(wallRegion.Count < wallRemoveSize)
            {
                foreach(Coord coord in wallRegion)
                {
                    _map[coord.tileX, coord.tileY] = 0;
                }
            }
        }

        List<List<Coord>> roomRegions = GetRegions(0);
        List<Room> survivingRooms = new List<Room>();

        foreach(List<Coord> roomRegion in roomRegions)
        {
            if(roomRegion.Count < roomRemoveSize)
            {
                foreach(Coord coord in roomRegion)
                {
                    _map[coord.tileX, coord.tileY] = 1;
                }
            }
            else
            {
                survivingRooms.Add(new Room(roomRegion, _map));
            }
        }
        survivingRooms.Sort();
        survivingRooms[0].isMainRoom = true;
        survivingRooms[0].isAccessibleFromMainRoom = true;
        ConnectClosestRooms(survivingRooms);
    }

    private void ConnectClosestRooms(List<Room> allRooms, bool forceAccessFromMainRoom = false)
    {
        List<Room> roomListA = new List<Room>();
        List<Room> roomListB = new List<Room>();

        if(forceAccessFromMainRoom)
        {
            foreach(Room room in allRooms)
            {
                if(room.isAccessibleFromMainRoom)
                {
                    roomListB.Add(room);
                }
                else
                {
                    roomListA.Add(room);
                }
            }
        }
        else
        {
            roomListA = allRooms;
            roomListB = allRooms;
        }

        int bestDist = 0;
        Coord bestTileA = new Coord();
        Coord bestTileB = new Coord();
        Room bestRoomA = new Room();
        Room bestRoomB = new Room();
        bool possibleConnectionFound = false;

        foreach(Room roomA in roomListA)
        {
            if(forceAccessFromMainRoom == false)
            {
                possibleConnectionFound = false;
                if(roomA.connectedRooms.Count > 0)
                {
                    continue;
                }
            }

            foreach(Room roomB in roomListB)
            {
                if(roomA == roomB || roomA.IsConnected(roomB))
                {
                    continue;
                }
               
                for(int tileIndexA = 0; tileIndexA < roomA.edgeTiles.Count; tileIndexA++)
                {
                    for(int tileIndexB = 0; tileIndexB < roomB.edgeTiles.Count; tileIndexB++)
                    {
                        Coord tileA = roomA.edgeTiles[tileIndexA];
                        Coord tileB = roomB.edgeTiles[tileIndexB];

                        int distBetweenRooms = (int)(Mathf.Pow(tileA.tileX - tileB.tileX, 2) + Mathf.Pow(tileA.tileY - tileB.tileY, 2));

                        if(distBetweenRooms < bestDist || possibleConnectionFound == false)
                        {
                            bestDist = distBetweenRooms;
                            possibleConnectionFound = true;
                            bestTileA = tileA;
                            bestTileB = tileB;
                            bestRoomA = roomA;
                            bestRoomB = roomB;
                        }
                    }
                }
            }

            if(possibleConnectionFound && forceAccessFromMainRoom == false)
            {
                CreatePassage(bestRoomA, bestRoomB, bestTileA, bestTileB);
            }
        }

        if(possibleConnectionFound && forceAccessFromMainRoom)
        {
            CreatePassage(bestRoomA, bestRoomB, bestTileA, bestTileB);
            ConnectClosestRooms(allRooms, true);
        }

        if(forceAccessFromMainRoom == false)
        {
            ConnectClosestRooms(allRooms, true);
        }
    }

    private void CreatePassage(Room roomA, Room roomB, Coord tileA, Coord tileB)
    {
        Room.ConnectRooms(roomA, roomB);
        Debug.DrawLine(CoordToWorldPoint(tileA), CoordToWorldPoint(tileB), Color.green, 100);

        List<Coord> line = GetLine(tileA, tileB);
        foreach(Coord c in line)
        {
            DrawCircle(c, 1);
        }
    }

    private void DrawCircle(Coord coord, int radius)
    {
        for(int x = -radius; x <= radius; x++)
        {
            for(int y = -radius; y <= radius; y++)
            {
                if(x*x + y*y <= radius*radius)
                {
                    int drawX = coord.tileX + x;
                    int drawY = coord.tileY + y;
                    if(IsInMapRange(drawX, drawY))
                    {
                        _map[drawX, drawY] = 0;
                    }
                }
            }
        }
    }

    private List<Coord> GetLine(Coord from, Coord to)
    {
        List<Coord> line = new List<Coord>();

        int x = from.tileX;
        int y = from.tileY;

        int dx = to.tileX - from.tileX;
        int dy = to.tileY - from.tileY;

        bool inverted = false;
        int step = System.Math.Sign(dx);
        int gradientStep = System.Math.Sign(dy);

        int longest = Mathf.Abs(dx);
        int shortest = Mathf.Abs(dy);

        if(longest < shortest)
        {
            inverted = true;
            longest = Mathf.Abs(dy);
            shortest = Mathf.Abs(dx);

            step = System.Math.Sign(dy);
            gradientStep = System.Math.Sign(dx);
        }

        int gradientAccumulation = longest / 2;
        for(int i = 0; i < longest; i++)
        {
            line.Add(new Coord(x, y));

            if(inverted)
            {
                y += step;
            }
            else
            {
                x += step;
            }

            gradientAccumulation += shortest;
            if(gradientAccumulation >= longest)
            {
                if(inverted)
                {
                    x += gradientStep;
                }
                else
                {
                    y += gradientStep;
                }
                gradientAccumulation -= longest;
            }
        }

        return line;
    }

    private Vector3 CoordToWorldPoint(Coord tile)
    {
        return new Vector3(-width/2+0.5f+tile.tileX, 2, -height/2+0.5f+tile.tileY);
    }

    private List<List<Coord>> GetRegions(int tileType)
    {
        List<List<Coord>> regions = new List<List<Coord>>();
        int[,] mapFlags = new int[width, height];

        for(int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                if(mapFlags[x, y] == 0 && _map[x, y] == tileType)
                {
                    List<Coord> newRegion = GetRegionTiles(x, y);
                    regions.Add(newRegion);

                    foreach(Coord coord in newRegion)
                    {
                        mapFlags[coord.tileX, coord.tileY] = 1;
                    }
                }
            }
        }

        return regions;
    }

    private List<Coord> GetRegionTiles(int startX, int startY)
    {
        List<Coord> tiles = new List<Coord>();
        int[,] mapFlags = new int[width, height];
        int tileType = _map[startX, startY];

        Queue<Coord> queue = new Queue<Coord>();
        queue.Enqueue(new Coord(startX, startY));
        mapFlags[startX, startY] = 1;

        while(queue.Count > 0)
        {
            Coord tile = queue.Dequeue();
            tiles.Add(tile);

            for(int x = tile.tileX - 1; x <= tile.tileX + 1; x++)
            {
                for(int y = tile.tileY - 1; y <= tile.tileY + 1; y++)
                {
                    if(IsInMapRange(x, y) && (y == tile.tileY || x == tile.tileX))
                    {
                        if(mapFlags[x, y] == 0 && _map[x, y] == tileType)
                        {
                            mapFlags[x, y] = 1;
                            queue.Enqueue(new Coord(x, y));
                        }
                    }
                }
            }
        }
        return tiles;
    }

    private bool IsInMapRange(int x, int y)
    {
        return x >= 0 && x < width && y >= 0 && y < height;
    }

    private void RandomFillMap()
    {
        if(useRandomSeed)
        {
            seed = Time.time.ToString();
        }

        System.Random pseudoRandom = new System.Random(seed.GetHashCode());

        for(int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                if(x == 0 || x == width - 1 || y == 0 || y == height -1)
                {
                    _map[x, y] = 1;
                }
                else
                {
                    _map[x, y] = (pseudoRandom.Next(0, 100) < randomFillPercent) ? 1 : 0;
                }
            }
        }
    }

    private void SmoothMap()
    {
        for(int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                int neighbourWallTiles = GetSurroundingWallCount(x, y);

                if(neighbourWallTiles > 4)
                {
                    _map[x, y] = 1;
                }
                else if(neighbourWallTiles < 4)
                {
                    _map[x, y] = 0;
                }
            }
        }
    }

    private int GetSurroundingWallCount(int gridX, int gridY)
    {
        int wallCount = 0;
        for(int neighbourX = gridX - 1; neighbourX <= gridX + 1; neighbourX++)
        {
            for(int neighbourY = gridY - 1; neighbourY <= gridY + 1; neighbourY++)
            {
                if(IsInMapRange(neighbourX, neighbourY))
                {
                    if(neighbourX != gridX || neighbourY != gridY)
                    {
                        wallCount += _map[neighbourX, neighbourY];
                    } 
                }
                else
                {
                    wallCount++;
                }
            }
        }

        return wallCount;
    }

    private struct Coord
    {
        public int tileX;
        public int tileY;

        public Coord(int tileX, int tileY)
        {
            this.tileX = tileX;
            this.tileY = tileY;
        }
    }


    private class Room : System.IComparable<Room>
    {
        public List<Coord> tiles;
        public List<Coord> edgeTiles;
        public List<Room>  connectedRooms;
        public int roomSize;
        public bool isAccessibleFromMainRoom;
        public bool isMainRoom;

        public Room()
        {

        }

        public Room(List<Coord> roomTiles, int[,] map)
        {
            tiles = roomTiles;
            roomSize = tiles.Count;
            edgeTiles = new List<Coord>();
            connectedRooms = new List<Room>();

            foreach(Coord tile in tiles)
            {
                for(int x = tile.tileX - 1; x <= tile.tileX + 1; x++)
                {
                    for(int y = tile.tileY - 1; y <= tile.tileY + 1; y++)
                    {
                        if(x == tile.tileX || y == tile.tileY)
                        {
                            if(map[x, y] == 1)
                            {
                                edgeTiles.Add(tile);
                            }
                        }
                    }
                }
            }
        }

        public void SetAccessibleFromMainRoom()
        {
            if(isAccessibleFromMainRoom == false)
            {
                isAccessibleFromMainRoom = true;

                foreach(Room connectedRoom in connectedRooms)
                {
                    connectedRoom.SetAccessibleFromMainRoom();
                }
            }
        }

        public static void ConnectRooms(Room roomA, Room roomB)
        {
            if(roomA.isAccessibleFromMainRoom)
            {
                roomB.SetAccessibleFromMainRoom();
            }
            else if(roomB.isAccessibleFromMainRoom)
            {
                roomA.SetAccessibleFromMainRoom();
            }

            roomA.connectedRooms.Add(roomB);
            roomB.connectedRooms.Add(roomA);
        }

        public bool IsConnected(Room otherRoom)
        {
            return connectedRooms.Contains(otherRoom);
        }

        public int CompareTo(Room otherRoom)
        {
            return otherRoom.roomSize.CompareTo(roomSize);
        }
    }

}
