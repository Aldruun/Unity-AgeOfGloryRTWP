using AoG.Core;
using System.Collections;
//using AoG.Core;
//using Cinemachine;
using UnityEngine;

public class CameraShake
{
    //[SerializeField] private bool useCinecam = false;
    private bool shaking;
    private Transform camTransform;
    private Vector3 triggerPosition;
    PlayerCamera cam;
    public CameraShake(PlayerCamera cam, Camera camera)
    {
        this.cam = cam;
        camTransform = camera.transform;
    }

    public void Shake(Vector3 triggerPosition, float shakeAmount, float shakeDuration)
    {
        if(shaking)
            return;
        shaking = true;

        CoroutineRunner.Instance.StartCoroutine(CR_Shake(triggerPosition, shakeAmount, shakeDuration));
    }

    IEnumerator CR_Shake(Vector3 triggerPosition, float shakeAmount, float shakeTimer)
    {
        while(shakeTimer > 0)
        {
            camTransform.position = cam.finalPosition + Random.insideUnitSphere * (shakeAmount * (1 - HelperFunctions.GetLinearDistanceAttenuation(camTransform.position, triggerPosition, 2, 15)));

            shakeTimer -= Time.deltaTime;
            yield return null;
        }
        shaking = false;
        camTransform.position = cam.finalPosition;
    }
}