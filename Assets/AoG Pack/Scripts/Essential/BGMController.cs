using UnityEngine;

public class BGMController : MonoBehaviour
{
    private AudioSource[] _audiosources;

    // Use this for initialization
    private void Start()
    {
        _audiosources = GetComponents<AudioSource>();

        foreach (var aSource in _audiosources)
            if (aSource.clip != null)
            {
                aSource.time = Random.Range(0, aSource.clip.length / 2);
                aSource.Play();
            }
    }
}