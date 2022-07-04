using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AoG.SceneManagement;

public class MapInfo : MonoBehaviour
{
    public AreaTransition[] AreaTransitions;

    public float cameraYRotation = 45f;
    public float cameraXRotation = 45f;
    public float groundLevel;
  
    public FootstepLink[] footstepLinks;

    // Start is called before the first frame update
    private void Start()
    {
        
    }

    // Update is called once per frame
    private void Update()
    {
        
    }

}

[System.Serializable]
public class FootstepLink
{
    public Texture2D texture;
    public FootstepSoundType surface;
}