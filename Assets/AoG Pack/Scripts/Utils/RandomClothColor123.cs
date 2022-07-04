using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomClothColor123 : MonoBehaviour
{
    private SkinnedMeshRenderer rend;
    public Gradient color1Gradient;
    public Gradient color2Gradient;
    // Start is called before the first frame update
    private void Start()
    {
        rend = GetComponent<SkinnedMeshRenderer>();

        rend.materials[1].color = color1Gradient.Evaluate(Random.Range(0, 1f));
        rend.materials[2].color = color2Gradient.Evaluate(Random.Range(0, 1f));
        
    }
}
