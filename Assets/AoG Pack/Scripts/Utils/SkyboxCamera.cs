using UnityEngine;

public class SkyboxCamera : MonoBehaviour
{
    // the additional rotation to add to the skybox
    // can be set during game play or in the inspector
    public Vector3 SkyBoxRotation;

    // Use this for initialization
    private void Start()
    {
    }

    // if you need to rotate the skybox during gameplay
    // rotate the skybox independently of the main camera
    public void SetSkyBoxRotation(Vector3 rotation)
    {
        SkyBoxRotation = rotation;
    }

    // Update is called once per frame
    private void Update()
    {
        transform.rotation = Quaternion.identity * Quaternion.Euler(SkyBoxRotation * Time.deltaTime);
    }
}