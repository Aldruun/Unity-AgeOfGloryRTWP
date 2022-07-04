using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class SkinnedMeshToMesh : MonoBehaviour
{
    public SkinnedMeshRenderer skinnedMeshRenderer;
    public VisualEffect vfx;
    public float refreshRate;

    // Start is called before the first frame update
    private void Start()
    {
        StartCoroutine(CR_UpdateVFXGraph());
    }

    private IEnumerator CR_UpdateVFXGraph()
    {
        while(gameObject.activeInHierarchy)
        {
            yield return new WaitForSeconds(refreshRate);
            Mesh m = new Mesh();
            skinnedMeshRenderer.BakeMesh(m);
            Vector3[] verts = m.vertices;
            Mesh m2 = new Mesh();
            m2.vertices = verts;
            //vfx.SetMesh();

            //m.vertices = skinnedMeshRenderer.sharedMesh.vertices;



            vfx.SetMesh("Mesh", m2);

        }

        
    }
}
