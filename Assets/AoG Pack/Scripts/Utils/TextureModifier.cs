using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TextureModifier : MonoBehaviour
{
    private int indexOfDefaultTexture;

    public List<TextureAttributes> listTextures = new List<TextureAttributes>();
    private Terrain terrain;
    private TerrainData terrainData;

    private void Start()
    {
        // Get the attached terrain component
        terrain = GetComponent<Terrain>();

        // Get a reference to the terrain data
        terrainData = terrain.terrainData;

        //This is the # of textures you have added in the terrain editor
        var nbTextures = terrainData.alphamapLayers;

        //See below for the definition of GetMaxHeight
        var maxHeight = GetMaxHeight(terrainData, terrainData.heightmapResolution);

        // Your texture data (i.e. Splatmap) is stored internally as a 3d array of floats with x and y location as the first 2 dimensions of the array and the index of the texture to be used as the 3rd dimension
        var splatmapData = new float[terrainData.alphamapWidth, terrainData.alphamapHeight, terrainData.alphamapLayers];

        //This is just in case someone mixed up min and max when completing the inspector for this script
        for (var i = 0; i < listTextures.Count; i++)
        {
            if (listTextures[i].minAltitude > listTextures[i].maxAltitude)
            {
                var temp = listTextures[i].minAltitude;
                listTextures[i].minAltitude = listTextures[i].maxAltitude;
                listTextures[i].maxAltitude = temp;
            }

            if (listTextures[i].minSteepness > listTextures[i].maxSteepness)
            {
                var temp2 = listTextures[i].minSteepness;
                listTextures[i].minSteepness = listTextures[i].maxSteepness;
                listTextures[i].maxSteepness = temp2;
            }
        }

        //For some reason you need a default texture in Unity
        for (var i = 0; i < listTextures.Count; i++)
            if (listTextures[i].defaultTexture)
                indexOfDefaultTexture = listTextures[i].index;


        for (var y = 0; y < terrainData.alphamapHeight; y++)
        for (var x = 0; x < terrainData.alphamapWidth; x++)
        {
            // Normalise x/y coordinates to range 0-1
            var y_01 = y / (float) terrainData.alphamapHeight;
            var x_01 = x / (float) terrainData.alphamapWidth;

            // Sample the height at this location (note GetHeight expects int coordinates corresponding to locations in the heightmap array)
            var height = terrainData.GetHeight(Mathf.RoundToInt(y_01 * terrainData.heightmapResolution),
                Mathf.RoundToInt(x_01 * terrainData.heightmapResolution));

            //Normalise the height by dividing it by maxHeight
            var normHeight = height / maxHeight;

            // Calculate the steepness of the terrain at this location
            var steepness = terrainData.GetSteepness(y_01, x_01);

            // Normalise the steepness by dividing it by the maximum steepness: 90 degrees
            var normSteepness = steepness / 90.0f;

            //Erase existing splatmap at this point
            for (var i = 0; i < terrainData.alphamapLayers; i++) splatmapData[x, y, i] = 0.0f;

            // Setup an array to record the mix of texture weights at this point
            var splatWeights = new float[terrainData.alphamapLayers];

            for (var i = 0; i < listTextures.Count; i++)
                //The rules you defined in the inspector are being applied for each texture
                if (normHeight >= listTextures[i].minAltitude && normHeight <= listTextures[i].maxAltitude &&
                    normSteepness >= listTextures[i].minSteepness && normSteepness <= listTextures[i].maxSteepness)
                    splatWeights[listTextures[i].index] = 1.0f;

            // Sum of all textures weights must add to 1, so calculate normalization factor from sum of weights
            var z = splatWeights.Sum();

            //If z is 0 at this location (i.e. no texture was applied), put default texture
            if (Mathf.Approximately(z, 0.0f)) splatWeights[indexOfDefaultTexture] = 1.0f;

            // Loop through each terrain texture
            for (var i = 0; i < terrainData.alphamapLayers; i++)
            {
                // Normalize so that sum of all texture weights = 1
                splatWeights[i] /= z;

                // Assign this point to the splatmap array
                splatmapData[x, y, i] = splatWeights[i];
            }
        }

        // Finally assign the new splatmap to the terrainData:
        terrainData.SetAlphamaps(0, 0, splatmapData);
    }

    //This is to get the maximum altitude of your terrain. For some reason TerrainData.
    private float GetMaxHeight(TerrainData tData, int heightmapWidth)
    {
        var maxHeight = 0f;

        for (var x = 0; x < heightmapWidth; x++)
        for (var y = 0; y < heightmapWidth; y++)
            if (tData.GetHeight(x, y) > maxHeight)
                maxHeight = tData.GetHeight(x, y);
        return maxHeight;
    }

    // Using Serializable allows us to embed a class with sub properties in the inspector.
    [Serializable]
    public class TextureAttributes
    {
        public bool defaultTexture;
        public int index;

        [Range(0.0f, 1.0f)] public float maxAltitude;

        [Range(0.0f, 1.0f)] public float maxSteepness;

        [Range(0.0f, 1.0f)] public float minAltitude;

        [Range(0.0f, 1.0f)] public float minSteepness;

        public string name;
    }
}