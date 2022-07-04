using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class UISoundManager : MonoBehaviour
{
    public static UISoundManager current;

    private static AudioSource[] audioSources;

    private void Awake()
    {
        if (current != null && current != this)
        {
            Debug.LogError("Only one instance allowed");
            Destroy(this);
        }
        else
        {
            current = this;
        }

        audioSources = GetComponents<AudioSource>();
    }

    //public static void PlaySound(string nameOfSoundFile) {
    //    audioSourcces.clip = (AudioClip)Resources.Load("SFX/UI SFX/" + nameOfSoundFile, typeof(AudioClip));
    //    audioSourcces.Play();
    //}
    //public static void PlaySound(string nameOfSoundFile, float pitch) {
    //    audioSourcces.clip = (AudioClip)Resources.Load("SFX/UI SFX/" + nameOfSoundFile, typeof(AudioClip));
    //    audioSourcces.pitch = pitch;
    //    audioSourcces.Play();
    //}
    public static void PlaySound(AudioClip audioClip, float pitch = 1)
    {
        //Debug.Log("Trying to play sound '" + audioClip.name + "'");

        for (var i = 0; i < audioSources.Length; i++)
            if (audioSources[i].isPlaying == false)
            {
                current.StartCoroutine(current.CR_PlaySound(audioSources[i], audioClip, Random.Range(0.9f, 1.1f)));
                return;
            }
    }

    private IEnumerator CR_PlaySound(AudioSource aSource, AudioClip audioClip, float pitch)
    {
        aSource.pitch = pitch;
        aSource.clip = audioClip;
        aSource.Play();

        yield return new WaitForEndOfFrame();
    }
}