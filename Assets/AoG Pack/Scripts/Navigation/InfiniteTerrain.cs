using System;
using UnityEngine;


// REFERENCES & CREDITS
// https://github.com/Meowmaritus/Unity_SpaceGame/blob/master/Assets/Scripts/InfiniteTerrain.cs
// https://www.reddit.com/r/Unity3D/comments/2q4ghg/procedural_terrain_generation/
// http://wiki.unity3d.com/index.php/FlyCam_Extended
//
// TEXTURES FROM TERRAIN TOOLKIT
// https://assetstore.unity.com/packages/tools/terrain/terrain-toolkit-2017-83490
//
// ADD LAYER SCRIPT
// https://forum.unity.com/threads/terrain-layers-api-can-you-tell-me-the-starting-point.606019/
//
// CREATE TERRAIN IN SCENE
// CREAT NEW TERRAIN LAYER, ADD GRASS TEXTURE
// ATTACH EXTENDED FLYCAM SCRIPT TO CAMERA
// ATTACH THIS SCRIPT TO TERRAIN, SET PLAYER OBJECT TO CAMERA


public class InfiniteTerrain : MonoBehaviour
{
    private Terrain[,] _terrainGrid = new Terrain[3, 3];
    public GameObject PlayerObject;
    public TerrainLayer TerrainLayer;

    private void Start()
    {
        var linkedTerrain = gameObject.GetComponent<Terrain>();

        _terrainGrid[0, 0] = Terrain.CreateTerrainGameObject(linkedTerrain.terrainData).GetComponent<Terrain>();
        _terrainGrid[0, 1] = Terrain.CreateTerrainGameObject(linkedTerrain.terrainData).GetComponent<Terrain>();
        _terrainGrid[0, 2] = Terrain.CreateTerrainGameObject(linkedTerrain.terrainData).GetComponent<Terrain>();
        _terrainGrid[1, 0] = Terrain.CreateTerrainGameObject(linkedTerrain.terrainData).GetComponent<Terrain>();
        _terrainGrid[1, 1] = linkedTerrain;
        _terrainGrid[1, 2] = Terrain.CreateTerrainGameObject(linkedTerrain.terrainData).GetComponent<Terrain>();
        _terrainGrid[2, 0] = Terrain.CreateTerrainGameObject(linkedTerrain.terrainData).GetComponent<Terrain>();
        _terrainGrid[2, 1] = Terrain.CreateTerrainGameObject(linkedTerrain.terrainData).GetComponent<Terrain>();
        _terrainGrid[2, 2] = Terrain.CreateTerrainGameObject(linkedTerrain.terrainData).GetComponent<Terrain>();

        AddTerrainLayer(_terrainGrid[0, 0], TerrainLayer);
        GenerateHeights(_terrainGrid[0, 0], 0);

        UpdateTerrainPositionsAndNeighbors();
    }

    private void UpdateTerrainPositionsAndNeighbors()
    {
        _terrainGrid[0, 0].transform.position = new Vector3(
            _terrainGrid[1, 1].transform.position.x - _terrainGrid[1, 1].terrainData.size.x,
            _terrainGrid[1, 1].transform.position.y,
            _terrainGrid[1, 1].transform.position.z + _terrainGrid[1, 1].terrainData.size.z);
        _terrainGrid[0, 1].transform.position = new Vector3(
            _terrainGrid[1, 1].transform.position.x - _terrainGrid[1, 1].terrainData.size.x,
            _terrainGrid[1, 1].transform.position.y,
            _terrainGrid[1, 1].transform.position.z);
        _terrainGrid[0, 2].transform.position = new Vector3(
            _terrainGrid[1, 1].transform.position.x - _terrainGrid[1, 1].terrainData.size.x,
            _terrainGrid[1, 1].transform.position.y,
            _terrainGrid[1, 1].transform.position.z - _terrainGrid[1, 1].terrainData.size.z);

        _terrainGrid[1, 0].transform.position = new Vector3(
            _terrainGrid[1, 1].transform.position.x,
            _terrainGrid[1, 1].transform.position.y,
            _terrainGrid[1, 1].transform.position.z + _terrainGrid[1, 1].terrainData.size.z);
        _terrainGrid[1, 2].transform.position = new Vector3(
            _terrainGrid[1, 1].transform.position.x,
            _terrainGrid[1, 1].transform.position.y,
            _terrainGrid[1, 1].transform.position.z - _terrainGrid[1, 1].terrainData.size.z);

        _terrainGrid[2, 0].transform.position = new Vector3(
            _terrainGrid[1, 1].transform.position.x + _terrainGrid[1, 1].terrainData.size.x,
            _terrainGrid[1, 1].transform.position.y,
            _terrainGrid[1, 1].transform.position.z + _terrainGrid[1, 1].terrainData.size.z);
        _terrainGrid[2, 1].transform.position = new Vector3(
            _terrainGrid[1, 1].transform.position.x + _terrainGrid[1, 1].terrainData.size.x,
            _terrainGrid[1, 1].transform.position.y,
            _terrainGrid[1, 1].transform.position.z);
        _terrainGrid[2, 2].transform.position = new Vector3(
            _terrainGrid[1, 1].transform.position.x + _terrainGrid[1, 1].terrainData.size.x,
            _terrainGrid[1, 1].transform.position.y,
            _terrainGrid[1, 1].transform.position.z - _terrainGrid[1, 1].terrainData.size.z);

        _terrainGrid[0, 0].SetNeighbors(null, null, _terrainGrid[1, 0], _terrainGrid[0, 1]);
        _terrainGrid[0, 1].SetNeighbors(null, _terrainGrid[0, 0], _terrainGrid[1, 1], _terrainGrid[0, 2]);
        _terrainGrid[0, 2].SetNeighbors(null, _terrainGrid[0, 1], _terrainGrid[1, 2], null);
        _terrainGrid[1, 0].SetNeighbors(_terrainGrid[0, 0], null, _terrainGrid[2, 0], _terrainGrid[1, 1]);
        _terrainGrid[1, 1].SetNeighbors(_terrainGrid[0, 1], _terrainGrid[1, 0], _terrainGrid[2, 1], _terrainGrid[1, 2]);
        _terrainGrid[1, 2].SetNeighbors(_terrainGrid[0, 2], _terrainGrid[1, 1], _terrainGrid[2, 2], null);
        _terrainGrid[2, 0].SetNeighbors(_terrainGrid[1, 0], null, null, _terrainGrid[2, 1]);
        _terrainGrid[2, 1].SetNeighbors(_terrainGrid[1, 1], _terrainGrid[2, 0], null, _terrainGrid[2, 2]);
        _terrainGrid[2, 2].SetNeighbors(_terrainGrid[1, 2], _terrainGrid[2, 1], null, null);
    }

    private void Update()
    {
        var playerPosition = new Vector3(PlayerObject.transform.position.x, PlayerObject.transform.position.y,
            PlayerObject.transform.position.z);
        Terrain playerTerrain = null;
        var xOffset = 0;
        var yOffset = 0;
        for (var x = 0; x < 3; x++)
        {
            for (var y = 0; y < 3; y++)
                if (playerPosition.x >= _terrainGrid[x, y].transform.position.x &&
                    playerPosition.x <=
                    _terrainGrid[x, y].transform.position.x + _terrainGrid[x, y].terrainData.size.x &&
                    playerPosition.z >= _terrainGrid[x, y].transform.position.z &&
                    playerPosition.z <= _terrainGrid[x, y].transform.position.z + _terrainGrid[x, y].terrainData.size.z)
                {
                    playerTerrain = _terrainGrid[x, y];
                    xOffset = 1 - x;
                    yOffset = 1 - y;
                    break;
                }

            if (playerTerrain != null)
                break;
        }

        if (playerTerrain != _terrainGrid[1, 1])
        {
            var newTerrainGrid = new Terrain[3, 3];
            for (var x = 0; x < 3; x++)
            for (var y = 0; y < 3; y++)
            {
                var newX = x + xOffset;
                if (newX < 0)
                    newX = 2;
                else if (newX > 2)
                    newX = 0;
                var newY = y + yOffset;
                if (newY < 0)
                    newY = 2;
                else if (newY > 2)
                    newY = 0;
                newTerrainGrid[newX, newY] = _terrainGrid[x, y];
            }

            _terrainGrid = newTerrainGrid;
            UpdateTerrainPositionsAndNeighbors();
        }
    }


    public void GenerateHeights(Terrain terrain, float tileSize)
    {
        var heights = new float[terrain.terrainData.heightmapResolution, terrain.terrainData.heightmapResolution];

        for (var i = 0; i < terrain.terrainData.heightmapResolution; i++)
        for (var k = 0; k < terrain.terrainData.heightmapResolution; k++)
        {
            var xCoord = terrain.transform.position.z + terrain.terrainData.size.z *
                (i + i * 1f / (terrain.terrainData.heightmapResolution - 1)) /
                terrain.terrainData.heightmapResolution;
            var zCoord = terrain.transform.position.x + terrain.terrainData.size.x *
                (k + k * 1f / (terrain.terrainData.heightmapResolution - 1)) /
                terrain.terrainData.heightmapResolution;
            heights[i, k] = Mathf.PerlinNoise(xCoord, zCoord) * tileSize / terrain.terrainData.heightmapResolution;
        }

        terrain.terrainData.SetHeights(0, 0, heights);
    }

    public void AddTerrainLayer(Terrain terrain, TerrainLayer terrainLayer)
    {
        // get the current array of TerrainLayers
        var oldLayers = terrain.terrainData.terrainLayers;

        // check to see that you are not adding a duplicate TerrainLayer
        for (var i = 0; i < oldLayers.Length; ++i)
            if (oldLayers[i] == terrainLayer)
                return;

        // NOTE: probably want to track Undo step here before modifying the TerrainData

        var newLayers = new TerrainLayer[oldLayers.Length + 1];

        // copy old array into new array
        Array.Copy(oldLayers, 0, newLayers, 0, oldLayers.Length);

        // add new TerrainLayer to the new array
        newLayers[oldLayers.Length] = terrainLayer;
        terrain.terrainData.terrainLayers = newLayers;
    }
}