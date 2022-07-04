using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshConverter : MonoBehaviour
{
    [ContextMenu("Convert To Regular Mesh")]
    private void Convert()
    {
        SkinnedMeshRenderer skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();
        MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
        MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();

        meshFilter.sharedMesh = skinnedMeshRenderer.sharedMesh;
        meshRenderer.sharedMaterials = skinnedMeshRenderer.sharedMaterials;

        DestroyImmediate(skinnedMeshRenderer);
        DestroyImmediate(this);
    }

   


//    To use it:

////SkinnedMeshRenderer originRenderer;
////Texture2D[] cullingTextures;

//Texture2D mainTexture = BodyTextureOcclusion.RebuildTexture((Texture2D)originRenderer.material.mainTexture, cullingTextures, 10);

//    originRenderer.material.mainTexture = mainTexture;
}
