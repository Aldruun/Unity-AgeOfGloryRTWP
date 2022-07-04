using UnityEngine;

public class DisableOnStart : MonoBehaviour
{
    // Start is called before the first frame update
    private void OnEnable()
    {
        GetComponent<MeshRenderer>().enabled = false;
    }
}