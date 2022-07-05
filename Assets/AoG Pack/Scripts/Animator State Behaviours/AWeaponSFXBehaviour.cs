using System.Collections;
using UnityEngine;

public class AWeaponSFXBehaviour : StateMachineBehaviour
{
    private IEnumerator CR_PlaySound_Instance;

    private AudioSource _audioSource;
    //public Vector2 randomPitchRange = new Vector2(1, 1);
    public AudioClip[] clipsToPlay;
    public float normalizedDelay;
    
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(CR_PlaySound_Instance == null)
        {
            CR_PlaySound_Instance = CR_PlaySound(animator, stateInfo.length * normalizedDelay);
        }
        AoG.Core.CoroutineRunner.Instance.StartCoroutine(CR_PlaySound_Instance);
    }

    private IEnumerator CR_PlaySound(Animator animator, float delay)
    {
        yield return new WaitForSeconds(delay);
        //Debug.Log("TRIGGER SFX");
        _audioSource = SFXPlayer.TriggerSFX(clipsToPlay[Random.Range(0, clipsToPlay.Length)],
            animator.transform.position);
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (_audioSource != null)
        {
            AoG.Core.CoroutineRunner.Instance.StopCoroutine(CR_PlaySound_Instance);
            CR_PlaySound_Instance = null;
            _audioSource.Stop();
        }
    }
}