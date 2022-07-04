using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class ActorMeshTranformBinder : MonoBehaviour
{
    [SerializeField] private Transform root;
    private VisualEffect vfx;

    // Start is called before the first frame update
    private void Start()
    {
        vfx = GetComponent<VisualEffect>();
    }

    // Update is called once per frame
    private void Update()
    {
        vfx.SetVector3("Transform_position", root.position);
        vfx.SetVector3("Transform_angles", root.eulerAngles);
        vfx.SetVector3("Transform_scale", root.localScale);
    }
}
