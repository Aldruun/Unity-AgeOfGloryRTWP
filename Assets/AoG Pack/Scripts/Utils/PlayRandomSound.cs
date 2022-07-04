using UnityEngine;

public class PlayRandomSound : MonoBehaviour
{
    public AudioClip[] audioClips;
    private AudioSource audioSource;

    private void OnEnable()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.pitch = Random.Range(0.95f, 1.05f);
        audioSource.clip = audioClips[Random.Range(0, audioClips.Length)];
        audioSource.Play();
    }
}